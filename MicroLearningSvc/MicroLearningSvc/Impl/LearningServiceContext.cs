using MicroLearningSvc.Db;
using MicroLearningSvc.Util;
using SmartReader;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace MicroLearningSvc.Impl
{
    class LearningServiceContext : IDisposable, ILearningServiceContext
    {
        private const string _sessionCookieName = "_mlsSessionId";

        private readonly DisposableList _disposables = new DisposableList();

        public LearningServiceConfiguration Configuration { get; }

        public string DbConnectionString { get; }
        public IWordNormalizer WordNormalizer { get; }
        public ILearningSessionsManager SessionsManager { get; }
        public ISecureRandom SecureRandom { get; }
        public ILearningContentDeliveryManager DeliveryManager { get; }

        public DateTime UtcNow { get { return DateTime.UtcNow; } }

        public LearningServiceContext(LearningServiceConfiguration cfg)
        {
            this.Configuration = cfg;
            this.DbConnectionString = cfg.MakeDbConnectionString();
         
            this.Initialize();

            this.WordNormalizer = _disposables.Add(new WordNormalizer(cfg.LangModelsDirPath));
            this.SessionsManager = new LearningSessionsManager(cfg.SessionTimeout);
            this.SecureRandom = _disposables.Add(new SecureRandom());

            var deliveryTimer = _disposables.Add(new TimerImpl());
            this.DeliveryManager = _disposables.Add(new LearningContentDeliveryManager(this, deliveryTimer));
        }

        private void Initialize()
        {
            using (var ctx = this.OpenLocalContext())
            {
                if (!ctx.Db.Raw.DatabaseExists())
                    ctx.Db.Raw.CreateDatabase();
            }
        }

        public void SendMail(string targetEmail, string subject, string text)
        {
            // "FromName<FromLogin@host>"
            using (MailMessage mm = new MailMessage(this.Configuration.SmtpLogin, targetEmail))
            {
                mm.Subject = subject;
                mm.Body = text;
                mm.IsBodyHtml = false;

                using (SmtpClient sc = new SmtpClient(this.Configuration.SmtpServerHost, this.Configuration.SmtpServerPort))
                {
                    sc.PickupDirectoryLocation = this.Configuration.SmtpPickupDirectoryLocation;
                    sc.EnableSsl = this.Configuration.SmtpUseSsl;
                    sc.DeliveryMethod = this.Configuration.SmtpDeliveryMethod;
                    sc.UseDefaultCredentials = this.Configuration.SmtpUseDefaultCredentials;
                    sc.Credentials = new NetworkCredential(this.Configuration.SmtpLogin, this.Configuration.SmtpPassword);
                    sc.Send(mm);
                }
            }
        }

        public IBasicOperationContext OpenLocalContext()
        {
            return new BasicOperationContext(this);
        }

        public ILearningRequestContext OpenWebRequestContext()
        {
            return new LearningRequestContext(this, this.ResolveSession());
        }

        private ILearningSessionContext ResolveSession()
        {
            ILearningSessionContext session;
            Guid sessionId;

            if (WcfUtils.GetCookies().TryGetValue(_sessionCookieName, out string encodedSessionIdStr))
            {
                sessionId = Guid.Parse(encodedSessionIdStr.FromBase64());
                if (this.SessionsManager.TryGetSession(sessionId, out session))
                    session.Renew();
                else
                    session = null;
            }
            else
            {
                session = null;
            }

            if (session == null)
            {
                session = this.SessionsManager.CreateSession();

                WcfUtils.AddResponseCookie(_sessionCookieName, session.Id.ToString().ToBase64(), DateTime.UtcNow.AddYears(10), path: "/", httpOnly: true);
            }

            return session;
        }

        public ILearningDbContext OpenDb()
        {
            var cnn = new SqlConnection(this.DbConnectionString);
            cnn.InfoMessage += (sneder, ea) =>
            {
                System.Diagnostics.Debug.Print(ea.Source + ": " + ea.Message);
                ea.Errors.OfType<SqlError>().ToList().ForEach(e => {
                    System.Diagnostics.Debug.Print(e.Source + " (" + e.Procedure + ") : " + e.Message);
                });
            };
            cnn.Open();

            var ctx = new DbContext(cnn);
            ctx.Log = new DebugTextWriter();
            return new LearningDbContext(ctx);
        }

        public void Dispose()
        {
            _disposables.SafeDispose();
        }

        public Article ParseResourceArticle(string url)
        {
            return SmartReader.Reader.ParseArticle(url);
        }

        public byte[] DownloadContent(string resourceUrl)
        {
            var wc = new WebClient();
            wc.Encoding = Encoding.UTF8;
            var data = wc.DownloadData(resourceUrl);
            return data;
        }
    }
    public class DebugTextWriter : StreamWriter
    {
        public DebugTextWriter()
            : base(new DebugOutStream(), Encoding.Unicode, 1024)
        {
            this.AutoFlush = true;
        }

        sealed class DebugOutStream : Stream
        {
            public override void Write(byte[] buffer, int offset, int count)
            {
                System.Diagnostics.Debug.Write(Encoding.Unicode.GetString(buffer, offset, count));
                this.Flush();
            }

            public override bool CanRead => false;
            public override bool CanSeek => false;
            public override bool CanWrite => true;
            public override void Flush() => System.Diagnostics.Debug.Flush();

            public override long Length => throw bad_op;
            public override int Read(byte[] buffer, int offset, int count) => throw bad_op;
            public override long Seek(long offset, SeekOrigin origin) => throw bad_op;
            public override void SetLength(long value) => throw bad_op;
            public override long Position
            {
                get => throw bad_op;
                set => throw bad_op;
            }

            static InvalidOperationException bad_op => new InvalidOperationException();
        };
    }
}
