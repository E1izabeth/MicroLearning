using MicroLearningSvc.Db;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroLearningSvc
{
    interface ILearningServiceContext
    {
        LearningServiceConfiguration Configuration { get; }

        string DbConnectionString { get; }
        IWordNormalizer WordNormalizer { get; }
        ILearningSessionsManager SessionsManager { get; }
        ISecureRandom SecureRandom { get; }
        ILearningContentDeliveryManager DeliveryManager { get; }

        ILearningDbContext OpenDb();
        void SendMail(string targetEmail, string subject, string text);
        SmartReader.Article ParseResourceArticle(string url);
        byte[] DownloadContent(string resourceUrl);

        IBasicOperationContext OpenLocalContext();
        ILearningRequestContext OpenWebRequestContext();

        DateTime UtcNow { get; }
    }

    interface IWordNormalizer : IDisposable
    {
        string NormalizeWord(string word);
    }

    interface ILearningSessionsManager : IDisposable
    {
        TimeSpan SessionCleanupTimeout { get; set; }
        
        ILearningSessionContext CreateSession();
        
        ILearningSessionContext GetSession(Guid id);
        bool TryGetSession(Guid sessionId, out ILearningSessionContext session);
     
        void DeleteSession(Guid sessionId);
        void DropUserSessions(long userId);
        void CleanupSessions();
    }

    interface ISecureRandom : IDisposable
    {
        int Next(int minValue, int maxExclusiveValue);
        byte[] GenerateRandomBytes(int bytesNumber);
    }

    interface ILearningContentDeliveryManager : IDisposable
    {
        void Refresh();
    }

    interface IBasicOperationContext : IDisposable
    {
        ILearningDbContext Db { get; }
    }

    interface ILearningRequestContext : IBasicOperationContext
    {
        ILearningSessionContext Session { get; }
        void ValidateAuthorized();
        void ValidateAuthorized(bool requireActivated = true);
    }

    interface ILearningSessionContext
    {
        Guid Id { get; }
        DateTime LastActivity { get; }
        long UserId { get; }
        bool IsActivated { get; }

        event Action<long> OnUserContextChanging;

        void Renew();
        void SetUserContext(DbUserInfo user);
    }
}
