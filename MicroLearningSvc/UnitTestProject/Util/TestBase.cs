using MicroLearningSvc;
using MicroLearningSvc.Db;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestProject
{
    public class TestBase
    {
        protected const long testUserId = 125;
        protected const long otherUserId = 37;
        protected const string testUserLogin = "tester";
        protected const string testPassword = "pwdpwdpwdpwd";
        protected const string testPassword2 = "newpassword";
        protected const string testUserEmail = "email@mail.ru";
        protected const string testUserEmail2 = "tester@test.com";
        protected readonly string activationToken = Convert.ToBase64String(new byte[64]);
        protected readonly string salt = Convert.ToBase64String(new byte[64]);


        internal const string keyword1 = "linux";
        internal const string topicTitle1 = "Linux";
        internal const string keyword2 = "ubuntu";
        internal const string topicTitle2 = "Ubuntu";

        internal const int topicLearnedCounter = 8;
        internal const int topicTotalCounter = 42;

        internal DbTagInfo MakeTestTag1() { return new DbTagInfo() { Id = 1 }; }

        internal const int topic1Id = 1;
        internal const int topic2Id = 2;

        internal DbTopicInfo MakeTestTopic1() { return new DbTopicInfo() { Id = topic1Id, Title = topicTitle1, AuthorUserId = testUserId }; }
        internal DbTagInstanceInfo MakeTestTagInst1() { return new DbTagInstanceInfo() { Word = keyword1, Id = 1, TagId = 1 }; }

        internal DbTagInstanceInfo MakeTestTagInst2() { return new DbTagInstanceInfo() { Word = keyword2, Id = 2, TagId = 2 }; }

        static TestBase()
        {
            AppDomain.CurrentDomain.FirstChanceException += (sender, ea) => Console.WriteLine(ea.Exception.ToString());
        }

        internal DbUserInfo MakeTestUser(bool activated = true)
        {
            var existingUser = new DbUserInfo()
            {
                Id = testUserId,
                Activated = activated,
                LoginKey = testUserLogin,
                HashSalt = salt,
                Email = testUserEmail,
                PasswordHash = testPassword.ComputeSha256Hash(salt),
                LastToken = activationToken,
                LastTokenKind = activated ? DbUserTokenKind.AccessRestore : DbUserTokenKind.Activation,
                LastTokenStamp = DateTime.UtcNow - TimeSpan.FromMinutes(30)
            };

            return existingUser;
        }

        internal (DbResourceInfo, DbTagInstanceInfo[]) MakeTestResource()
        {
            var resUrl = "https://habr.com/ru/company/ua-hosting/blog/342826/";
            var resTitle = "Linuxes";

            var res = new DbResourceInfo()
            {
                Id = 42,
                Title = resTitle,
                Url = resUrl,
                AuthorUserId = 32
            };

            var tags = new[] { new DbTagInstanceInfo() { Id = 1, TagId = 1, Word = "word" } };
            return (res, tags);
        }

        internal DbResourceInfo[] MakeTestResources()
        {
            var resources = new[]
            {
                    new DbResourceInfo(){Title="Wiki", Url="https://ru.wikipedia.org/wiki/Linux", Id = 1},
                    new DbResourceInfo(){Title="Org", Url="https://www.linux.org/", Id = 2},
                    new DbResourceInfo(){Title="Habr", Url="https://habr.com/en/company/ua-hosting/blog/342826/", Id = 3},
                    new DbResourceInfo(){Title="Lifehacker", Url="https://lifehacker.ru/distributivy-linux/", Id = 4},
                    new DbResourceInfo(){Title="Ubuntu", Url="https://ubuntu.com/", Id = 5},
                    new DbResourceInfo(){Title="Linux.com", Url="https://www.linux.com/", Id = 6},
                    new DbResourceInfo(){Title="1Linux", Url="https://1linux.ru/info/obzor-os-linux.html", Id = 7},
                    new DbResourceInfo(){Title="Linux.ru", Url="https://www.linux.ru/", Id = 8},
                    new DbResourceInfo(){Title="Windows.Microsoft", Url="https://windows.microsoft.com", Id = 9},
                    new DbResourceInfo(){Title="Wiki", Url="https://ru.wikipedia.org/wiki/Windows", Id = 10},
                    new DbResourceInfo(){Title="Builts", Url="https://windowsobraz.com/", Id = 11},
                    new DbResourceInfo(){Title="Fandom", Url="https://windows.fandom.com/ru/wiki/Windows", Id = 12},
            };

            return resources;
        }

        internal (DbResourcesFilterResullt,DbTagInstanceInfo) MakeFilteredResourcesResult()
        {
            var resources = this.MakeTestResources();
            var (associationRes, tagInst1, tagInst2) = this.MakeTestAssociationRes();
            var filterResult = new DbResourcesFilterResullt(
                resources.Length,
                resources.Where(r => associationRes.Where(a => a.TagInstanceId == tagInst2.Id).Select(ra => ra.ResourceId).Any(id => id == r.Id)).ToArray(),
                resources.Select(r => new DbResourceWrappedTag(r.Id, new DbTagInstanceInfo(){Id = 1, TagId = 1, Word = "linux"})).ToArray()
            );
            return (filterResult, tagInst1);
        }

        internal (DbUserSubscriptionInfo[], DbTopicInfo) MakeTestSubscriptions()
        {
            var subscriptions = new[]
            {
                new DbUserSubscriptionInfo(){Id = 1, Interval = new TimeSpan(1,0,0), IsActive = true, TopicId = 1, UserId = testUserId},
                new DbUserSubscriptionInfo(){Id = 2, Interval = new TimeSpan(3,0,0), IsActive = true, TopicId = 2, UserId = testUserId},
                new DbUserSubscriptionInfo(){Id = 3, Interval = new TimeSpan(12,0,0), IsActive = true, TopicId = 3, UserId = testUserId},
                new DbUserSubscriptionInfo(){Id = 4, Interval = new TimeSpan(48,0,0), IsActive = true, TopicId = 6, UserId = testUserId},
                new DbUserSubscriptionInfo(){Id = 5, Interval = new TimeSpan(1,30,0), IsActive = false, TopicId = 8, UserId = testUserId}
            };

            var topic1 = this.MakeTestTopic1();

            return (subscriptions, topic1);
        }

        internal (DbTopicInfo[], TopicAssociatedTagInfo, TopicAssociatedTagInfo) MakeTestTopics()
        {
            var topics = new[]
            {
                new DbTopicInfo(){Id = 1, Title = "Linux", AuthorUserId = testUserId},
                new DbTopicInfo(){Id = 2, Title = "Ubuntu", AuthorUserId = testUserId},
                new DbTopicInfo(){Id = 3, Title = "Windows", AuthorUserId = testUserId},
                new DbTopicInfo(){Id = 4, Title = "Bunnies", AuthorUserId = otherUserId},
                new DbTopicInfo(){Id = 5, Title = "Attractor", AuthorUserId = otherUserId},
                new DbTopicInfo(){Id = 6, Title = "Chaos", AuthorUserId = otherUserId},
                new DbTopicInfo(){Id = 7, Title = "Dynamics", AuthorUserId = otherUserId},
                new DbTopicInfo(){Id = 8, Title = "Linear Systems", AuthorUserId = otherUserId},
                new DbTopicInfo(){Id = 9, Title = "NetLogo", AuthorUserId = otherUserId}
            };

            var tagInst1 = this.MakeTestTagInst1();
            var tagInst2 = this.MakeTestTagInst2();

            var associatedTag1 = new TopicAssociatedTagInfo(tagInst1, topic1Id);
            var associatedTag2 = new TopicAssociatedTagInfo(tagInst2, topic2Id);

            return (topics, associatedTag1, associatedTag2);
        }

        internal (DbResourceAssociationInfo[], DbTagInstanceInfo, DbTagInstanceInfo) MakeTestAssociationRes()
        {
            var tagInst1 = this.MakeTestTagInst1();
            var tagInst2 = this.MakeTestTagInst2();

            var associationRes = new[]{
                new DbResourceAssociationInfo() { Id = 1, ResourceId = 1, TagInstanceId = tagInst1.Id },
                new DbResourceAssociationInfo() { Id = 2, ResourceId = 2, TagInstanceId = tagInst1.Id },
                new DbResourceAssociationInfo() { Id = 3, ResourceId = 6, TagInstanceId = tagInst1.Id },
                new DbResourceAssociationInfo() { Id = 4, ResourceId = 7, TagInstanceId = tagInst1.Id },
                new DbResourceAssociationInfo() { Id = 6, ResourceId = 7, TagInstanceId = tagInst2.Id },
                new DbResourceAssociationInfo() { Id = 5, ResourceId = 8, TagInstanceId = tagInst1.Id },
                new DbResourceAssociationInfo() { Id = 7, ResourceId = 3, TagInstanceId = tagInst2.Id },
                new DbResourceAssociationInfo() { Id = 8, ResourceId = 4, TagInstanceId = tagInst2.Id },
                new DbResourceAssociationInfo() { Id = 9, ResourceId = 5, TagInstanceId = tagInst2.Id },
            };

            return (associationRes, tagInst1, tagInst2);
        }

        internal (DbResourceContentPortionInfo, DbUserSubscriptionContentPortionInfo, DbUserSubscriptionInfo, SubscriptionResourceContentPart[], DbTopicInfo) MakeContentPartsForTest(bool noUserPart = false, bool isLearned = false)
        {
            var (subscriptions, topic1) = this.MakeTestSubscriptions();
            var resources = this.MakeTestResources();

            var contentPart = new DbResourceContentPortionInfo()
            {
                Id = 1,
                Order = 1,
                ResourceId = resources[0].Id,
                TextContent = "textContent"
            };

            var subscr = subscriptions.Where(s => s.TopicId == topic1.Id).First();

            var contentPortion = new DbUserSubscriptionContentPortionInfo()
            {
                Id = 1,
                IsLearned = isLearned,
                IsDelivered = false,
                UserSubscriptionId = subscr.Id,
                ResourceContentPortionId = 1,
            };

            var subscrContentParts = new[] {
                new SubscriptionResourceContentPart(null, contentPart, contentPortion),
                new SubscriptionResourceContentPart(subscr, contentPart, contentPortion)
            };
            var subscrContentPartsNoUserPart = new[] { new SubscriptionResourceContentPart(subscr, contentPart, null) };

            return (contentPart, contentPortion, subscr, noUserPart ? subscrContentPartsNoUserPart : subscrContentParts, topic1);
        }
    }
}
