using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroLearningCli
{

    public class LearningSvcClient
    {
        protected readonly LerningSvcWcfClient _client;

        public LearningSvcClient(LerningSvcWcfClient client)
        {
            _client = client;
        }

        public LearningSvcClient(string svcRootUrl)
        {
            _client = new LerningSvcWcfClient(svcRootUrl);
        }
    }

    public class LearningSvcRoot : LearningSvcClient
    {
        public LearningSvcRoot(string svcRootUrl) : base(svcRootUrl) { }

        public LerningSvcSession Login(string login, string password)
        {
            _client.Proxy.Login(new LoginSpecType() { Login = login, Password = password });
            return new LerningSvcSession(_client);
        }

        public void Register(string login, string password, string email)
        {
            _client.Proxy.Register(new RegisterSpecType() { Login = login, Password = password, Email = email });
        }

        public LerningSvcSession Activate(string key)
        {
            _client.Proxy.Activate(key);
            return new LerningSvcSession(_client);
        }

        public LerningSvcSession RestoreAccess(string key)
        {
            _client.Proxy.RestoreAccess(key);
            return new LerningSvcSession(_client);
        }

        public void RequestAccess(string login, string email)
        {
            _client.Proxy.RequestAccess(new ResetPasswordSpecType() { Email = email, Login = login });
        }
    }

    public class LerningSvcSession : LearningSvcClient, IDisposable
    {
        public LerningSvcSession(LerningSvcWcfClient client) : base(client) { }

        public void RequestActivation(string email)
        {
            _client.Proxy.RequestActivation(new RequestActivationSpecType() { Email = email });
        }

        public LearningTopic CreateTopic(string topicName, string[] tags)
        {
            var topicInfo = _client.Proxy.CreateTopic(new CreateTopicSpecType()
            {
                TopicName = topicName,
                AssociationTags = tags.Select(t => new AssociationTagInfoType() { Word = t }).ToArray()
            });

            return new LearningTopic(_client, topicInfo);
        }

        public void ChangePassword(string newPassword, string email)
        {
            _client.Proxy.SetPassword(new ChangePasswordSpecType() { Email = email, NewPassword = newPassword });
        }

        public LearningTopic GetTopic(long id)
        {
            return new LearningTopic(_client, _client.Proxy.GetTopic(id.ToString()));
        }

        public CreateResourceSpecType SuggestResourse(CreateResourceSpecType spec)
        {
            return _client.Proxy.SuggestResourceTags(spec);
        }

        public LearningResource CreateResource(string title, string url, string[] tags)
        {
            var tagitems = tags.Select(t => new AssociationTagInfoType() { Word = t }).ToArray();
            var spec = new CreateResourceSpecType() { ResourceTitle = title, ResourceUrl = url, AssociationTags = tagitems };
            return new LearningResource(_client, _client.Proxy.CreateResource(spec));
        }

        public LearningResource GetResource(long id)
        {
            return new LearningResource(_client, _client.Proxy.GetResource(id.ToString()));
        }

        public void SetEmail(string oldEmail, string newEmail, string password)
        {
            _client.Proxy.SetEmail(new ChangeEmailSpecType() { OldEmail = oldEmail, NewEmail = newEmail, Password = password });
        }

        //public void SetPassword(string email, string oldPassword, string newPassword)
        //{
        //    _client.Proxy.SetPassword(new ChangePasswordSpecType() { Email = email, OldPassword = oldPassword, NewPassword = newPassword });
        //}

        public void DeleteProfile()
        {
            _client.Proxy.DeleteProfile();
        }

        public void Dispose()
        {
            _client.Proxy.Logout();
        }

        public IEnumerable<LearningTopic> GetTopics()
        {
            var topics = _client.Proxy.GetTopics();
            if (topics?.Items == null)
                yield break;

            var i = 0;
            do
            {
                foreach (var item in topics.Items)
                    yield return new LearningTopic(_client, item);

                i += topics.Items.Length;
            }
            while (i < topics.Count);
        }

        public IEnumerable<LearningTopic> GetTopicsParts()
        {
            TopicsListType parts;

            var start = 0;
            {
                parts = _client.Proxy.GetTopicsRange(start.ToString(), "10");
                if (parts?.Items == null)
                    yield break;

                foreach (var topicInfo in parts.Items)
                    yield return new LearningTopic(_client, topicInfo);

                start += 10;
            } while (start < parts.Count) ;
        }

        public IEnumerable<LearningResource> GetResourcesByTags(string[] tags)
        {
            var spec = new ResourceFilterSpecType()
            {
                Item = new ResourceFilterByKeywordsSpec() { AssociationTags = tags.Select(t => new AssociationTagInfoType() { Word = t }).ToArray() }
            };

            var result = new List<LearningResource>();
            ResourcesListType parts;

            var start = 0;
            {
                parts = _client.Proxy.FilterResourcesRange(start.ToString(), "10", spec);
                if (parts?.Items == null)
                    yield break;

                foreach (var resourceInfo in parts.Items)
                    yield return new LearningResource(_client, resourceInfo);

                start += 10;
            } while (start < parts.Count) ;
        }
    }

    public class LearningResource : LearningSvcClient
    {
        public ResourceInfoType Info { get; private set; }

        public LearningResource(LerningSvcWcfClient client, ResourceInfoType info)
            : base(client)
        {
            this.Info = info;
        }

        public void Validate()
        {
            _client.Proxy.ValidateResource(this.Info.Id.ToString());
        }

        public void Refresh()
        {
            this.Info = _client.Proxy.GetResource(this.Info.Id.ToString());
        }
    }

    public class LearningTopic : LearningSvcClient
    {
        public TopicInfoType Info { get; private set; }

        public LearningTopic(LerningSvcWcfClient client, TopicInfoType info)
            : base(client)
        {
            this.Info = info;
        }

        public void Subscribe(long timeoutMinutes)
        {
            var spec = new ActivateTopicSpecType() { DueSeconds = timeoutMinutes * 60, IntervalSeconds = timeoutMinutes * 60 };
            _client.Proxy.ActivateTopicLearning(this.Info.Id.ToString(), spec);
        }

        public void Unsubscribe()
        {
            _client.Proxy.DeactivateTopicLearning(this.Info.Id.ToString());
        }

        public void ResetLearning()
        {
            _client.Proxy.ResetTopicLearningProgress(this.Info.Id.ToString());
        }

        public IEnumerable<ContentPartInfoType> GetContentParts()
        {
            ContentPartsListType parts;

            var start = 0;
            {
                parts = _client.Proxy.GetContentPartsByTopic(this.Info.Id.ToString(), start.ToString(), "10");
                if (parts?.Items == null)
                    yield break;

                foreach (var contentPart in parts.Items)
                    yield return contentPart;

                start += 10;
            } while (start < parts.Count) ;
        }

        public void MarkContentLearned(long contentPart)
        {
            _client.Proxy.MarkContentPartLearned(this.Info.Id.ToString(), contentPart.ToString());
        }

        public void UnmarkContentLearned(long contentPart)
        {
            _client.Proxy.UnmarkContentPartLearned(this.Info.Id.ToString(), contentPart.ToString());
        }
    }
}
