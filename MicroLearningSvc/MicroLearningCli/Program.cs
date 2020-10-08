using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Microsoft.Win32;
using MicroLearningSvc;
using System.Xml.Serialization;
using System.IO;
using System.ServiceModel.Web;
using MicroLearningSvc.Interaction;
using System.Web.Script.Serialization;

namespace MicroLearningCli
{
    static class Program
    {
        static void Main(string[] args)
        {

            var tstypes = XmlVsJsonObjectConverter.CollectTypeScriptMapping(typeof(OkType).Assembly.GetTypes().Where(t => t.HasCustomAttribute<XmlTypeAttribute>()).ToArray());
            var w = new IndentedWriter();
            tstypes.ForEach(t => t.FormatTo(w));
            Console.WriteLine(w.GetContentAsString());
            return;

            var options = new CommandLineArguments();
            var argsParser = new CommandLineAnalyzer<CommandLineArguments>();

            if (argsParser.TryParse(args, options) && !options.Help)
            {
                if (options.LaunchDebugger)
                    System.Diagnostics.Debugger.Launch();

                if (System.Diagnostics.Debugger.IsAttached)
                {
                    DoWork(options);
                }
                else
                {
                    try
                    {
                        DoWork(options);
                    }
                    catch (WebFaultException<WcfRestErrorDetails> ex)
                    {
                        var err = ex.Detail.Info;
                        do
                        {
                            Console.WriteLine(err.Message);
                            err = err.InnerError;
                        }
                        while (err != null);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        Environment.ExitCode = -1;
                    }
                }
            }
            else
            {
                Console.WriteLine(argsParser.MakeHelp());
            }
        }

        static void PrepareCreds(CommandLineArguments options)
        {
            using (var reg = new MyRegistry())
            {
                if (options.Login.IsEmpty())
                    options.Login = Environment.GetEnvironmentVariable("MICRO_LEARNING_LOGIN");

                if (options.Login.IsEmpty())
                    options.Login = reg.FindDefaultLogin();

                if (options.Login.IsEmpty())
                {
                    Console.Write("Login: ");
                    options.Login = Console.ReadLine();
                }

                if (options.Password.IsEmpty())
                    options.Password = reg.FindPassword(options.Login).Unprotect();

                if (options.Password.IsEmpty())
                {
                    Console.Write("Password: ");
                    options.Password = ReadPasswordLine();
                }

                if (options.StoreCreds)
                    reg.SetCreds(options.Login, options.Password.Protect());

                if (options.SetDefaultLogin)
                    reg.SetDefaultLogin(options.Login);
            }

            if (options.Login.IsEmpty())
                throw new ApplicationException("Credentials not specified");
        }

        private static void PrepareServiceUrl(CommandLineArguments options)
        {
            using (var reg = new MyRegistry())
            {
                if (options.ServiceUrl.IsEmpty())
                {
                    options.ServiceUrl = reg.FindServiceUrl();
                }

                if (options.ServiceUrl.IsEmpty())
                {
                    Console.Write("Service Url: ");
                    options.ServiceUrl = Console.ReadLine();
                }

                if (options.StoreServiceUrl)
                {
                    reg.SetServiceUrl(options.ServiceUrl);
                }
            }

            if (options.ServiceUrl.IsEmpty())
                throw new ApplicationException("Service Url not specified");
        }

        static void DoWork(CommandLineArguments options)
        {
            PrepareServiceUrl(options);
            var svc = new LearningSvcRoot(options.ServiceUrl);

            if (options.Register)
            {
                PrepareCreds(options);
                svc.Register(options.Login, options.Password, options.Email);
            }
            else if (options.Activate.IsNotEmpty())
            {
                svc.Activate(options.Activate);
            }
            else if (options.Recover)
            {
                svc.RequestAccess(options.Login, options.Email);
            }
            else if (options.RestoreAccess.IsNotEmpty())
            {
                using (var session = svc.RestoreAccess(options.RestoreAccess))
                {
                    session.ChangePassword(options.Password, options.Email);
                }
            }
            else
            {
                PrepareCreds(options);
                using (var session = svc.Login(options.Login, options.Password))
                {
                    DoOps(session, options);
                }
            }

            Console.WriteLine("Accomplied.");
        }

        static string TrimArg(this string str)
        {
            return str.Trim(' ', '\r', '\n', '\t', '"');
        }

        static void DoOps(LerningSvcSession session, CommandLineArguments options)
        {
            if (options.DeleteProfile)
            {
                session.DeleteProfile();
            }
            else if (options.CreateResource.IsNotEmpty())
            {
                var tags = options.AssociationTags.Split(',').Select(TrimArg).ToArray();
                var resource = session.CreateResource(options.CreateResource.TrimArg(), options.ResourceUrl.TrimArg(), tags);
                Console.WriteLine(resource.Info.ToTextForm());
            }
            else if (options.CreateTopic.IsNotEmpty())
            {
                var tags = options.AssociationTags.Split(',').Select(TrimArg).ToArray();
                var topic = session.CreateTopic(options.CreateTopic.TrimArg(), tags);
                Console.WriteLine(topic.Info.ToTextForm());
            }
            else if (options.GetResource > 0)
            {
                var resource = session.GetResource(options.GetResource);

                if (options.MarkResourceValidated)
                {
                    resource.Validate();
                }
                else
                {
                    Console.WriteLine(resource.Info.ToTextForm());
                }
            }
            else if (options.GetTopic > 0)
            {
                var topic = session.GetTopic(options.GetTopic);

                if (options.Subscribe > 0)
                {
                    topic.Subscribe(options.Subscribe);
                }
                else if (options.GetTopicContent)
                {
                    foreach (var item in topic.GetContentParts())
                        Console.WriteLine(item.ToTextForm());
                }
                else if (options.Unsubscribe)
                {
                    topic.Unsubscribe();
                }
                else if (options.DropTopicLearning)
                {
                    topic.ResetLearning();
                }
                else if (options.MarkLearned > 0)
                {
                    topic.MarkContentLearned(options.MarkLearned);
                }
                else if (options.MarkUnlearned > 0)
                {
                    topic.MarkContentLearned(options.MarkUnlearned);
                }
                else
                {
                    Console.WriteLine(topic.Info.ToTextForm());
                }
            }
            else if (options.GetTopicsList)
            {
                foreach (var item in session.GetTopics())
                    Console.WriteLine(item.Info.ToTextForm());
            }
            else if (options.GetResourcesList)
            {
                var tags = options.AssociationTags.Split(',').Select(TrimArg).ToArray();
                foreach (var item in session.GetResourcesByTags(tags))
                    Console.WriteLine(item.Info.ToTextForm());
            }
            else if (options.ResourceUrl.IsNotEmpty())
            {
                var spec = session.SuggestResourse(new CreateResourceSpecType() { ResourceUrl = options.ResourceUrl, AssociationTags = new AssociationTagInfoType[0] });
                Console.WriteLine(spec.ToTextForm());
                Console.WriteLine();

                if (spec.AssociationTags.Any())
                    Console.WriteLine("\t-tags " + string.Join(",", spec.AssociationTags.Select(t => t.Word)));

                Console.WriteLine("\t-url \"" + spec.ResourceUrl + "\"");
                Console.WriteLine("\t-cr \"" + spec.ResourceTitle + "\"");
            }
            else
            {
                Console.WriteLine("No command specified");
            }
        }

        static string ToTextForm(this object obj)
        {
            var xs = new XmlSerializer(obj.GetType());
            var sw = new StringWriter();
            xs.Serialize(sw, obj);
            return sw.GetStringBuilder().ToString();

            //var converter = new XmlVsJsonObjectConverter(obj.GetType());
            //var formatter = new JavaScriptSerializer();
            //var tree = converter.ToTree(obj, true);
            //var text = formatter.Serialize(tree);
            //return text;
        }

        static string Protect(this string value)
        {
            byte[] plaintext = Encoding.UTF8.GetBytes(value);

            // Generate additional entropy (will be used as the Initialization vector)
            byte[] entropy = new byte[20];
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
                rng.GetBytes(entropy);

            byte[] ciphertext = ProtectedData.Protect(plaintext, entropy, DataProtectionScope.CurrentUser);

            return Convert.ToBase64String(Encoding.UTF8.GetBytes(Convert.ToBase64String(entropy) + "|" + Convert.ToBase64String(ciphertext)));
        }

        static string Unprotect(this string value)
        {
            var parts = Encoding.UTF8.GetString(Convert.FromBase64String(value)).Split('|');
            var entropy = Convert.FromBase64String(parts[0]);
            var ciphertext = Convert.FromBase64String(parts[1]);

            var plaintext = ProtectedData.Unprotect(ciphertext, entropy, DataProtectionScope.CurrentUser);
            return Encoding.UTF8.GetString(plaintext);
        }

        class MyRegistry : IDisposable
        {
            private readonly DisposableList _disposables = new DisposableList();
            private readonly RegistryKey _appKey;
            private readonly RegistryKey _credsKey;

            const string _serviceUrlValueName = "ServiceUrl";

            public MyRegistry()
            {
                var softwareKey = _disposables.Add(Registry.CurrentUser.OpenSubKey("Software", true));

                _appKey = _disposables.Add(softwareKey.CreateSubKey("MicroLearningCli", true));
                _credsKey = _disposables.Add(_appKey.CreateSubKey("Credentials", true));
            }

            public void SetServiceUrl(string url)
            {
                _appKey.SetValue(_serviceUrlValueName, url);
            }

            public string FindServiceUrl()
            {
                return _appKey.GetValue(_serviceUrlValueName) as string;
            }

            public void SetCreds(string name, string pwd)
            {
                _credsKey.SetValue(name, pwd);
            }

            public string FindPassword(string name)
            {
                return _credsKey.GetValue(name) as string;
            }

            public void SetDefaultLogin(string name)
            {
                _credsKey.SetValue(string.Empty, name);
            }

            public string FindDefaultLogin()
            {
                return _credsKey.GetValue(string.Empty) as string;
            }

            public void Dispose()
            {
                _disposables.Dispose();
            }
        }

        static string ReadPasswordLine()
        {
            var key = Console.ReadKey(true);
            var str = new List<char>();
            var pos = 0;

            while (key.Key != ConsoleKey.Enter)
            {
                switch (key.Key)
                {
                    case ConsoleKey.LeftArrow:
                        {
                            if (pos > 0)
                            {
                                Console.Write("\b");
                                pos--;
                            }
                        }
                        break;
                    case ConsoleKey.RightArrow:
                        {
                            if (pos < str.Count)
                            {
                                Console.Write("*");
                                pos++;
                            }
                        }
                        break;
                    case ConsoleKey.Backspace:
                        {
                            if (pos > 0)
                            {
                                str.RemoveAt(pos - 1);
                                Console.Write("\b \b");
                                pos--;
                            }
                        }
                        break;
                    case ConsoleKey.Delete:
                        {
                            if (pos < str.Count)
                            {
                                str.RemoveAt(pos);
                                Console.Write(new string('*', str.Count - pos) + " ");
                                Console.Write(new string('\b', str.Count - pos + 1));
                            }
                        }
                        break;
                    case ConsoleKey.Home:
                        {
                            Console.Write(new string('\b', pos));
                            pos = 0;
                        }
                        break;
                    case ConsoleKey.End:
                        {
                            Console.Write(new string('*', str.Count - pos));
                            pos = str.Count;
                        }
                        break;
                    default:
                        {
                            str.Insert(pos, key.KeyChar);
                            Console.Write(new string('*', str.Count - pos));
                            Console.Write(new string('\b', str.Count - pos - 1));
                            pos++;
                        }
                        break;
                }

                key = Console.ReadKey(true);
            }

            Console.Write(new string('*', str.Count - pos));
            Console.WriteLine();

            return new string(str.ToArray());
        }

        //static void OldMain(string[] args)
        //{
        //    var env = Environment.GetEnvironmentVariables();
        //    env.OfType<System.Collections.DictionaryEntry>().ToDictionary(kv => kv.Key);
        //    var svcClient = new LearningSvcRoot("http://127.0.0.1:8181/mysvc/");
        //    //svcClient.Activate("i6QPoqXeXA4tbpkpvo4PLlFhAuVk4NC0Sb2QOhDbImFQmw7RiP5Kd4s8i34ElWttMsejm1vpMndrGhBXRA");

        //    //svcClient.Register("test", "tester", "test@localhost");
        //    //svcClient.Register("test", "tester", "ged.yuko@hotmail.com");
        //    //svcClient.Activate("PhG21NZSQ7kzeidAtFb9kvnB7G5vRLLMUSPefszr4gsXApivdkePwA2BPzd6MaBC2oZ3kwWfHLpP3EV9lHhpA");


        //    using (var session = svcClient.Login("test", "tester"))
        //    {
        //        // session.RequestActivation("test@localhost");

        //        var spec = session.SuggestResourse(new CreateResourceSpecType() { ResourceUrl = "https://en.wikipedia.org/wiki/Compiler" });
        //        spec.AssociationTags = new[]
        //        {
        //            new AssociationTagInfoType() { Word = "compiler" },
        //            new AssociationTagInfoType() { Word = "translator" }
        //        };
        //        var res = session.CreateResource(spec);


        //        // var topics = session.GetTopics().ToArray();

        //        // var topic = session.CreateTopic("mytopic", new[] { "compilers", "parsers" });

        //        // topic.Subscribe();


        //        // do work
        //    }
        //}

    }
}



