using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel.Web;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MicroLearningSvc.Db;
using MicroLearningSvc.Impl;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using UnitTestProject.Util;

namespace UnitTestProject
{
    [TestClass]
    public class TopicTest : TestBase
    {

        [TestMethod]
        public void GetTopicTest()
        {
            var topic1 = this.MakeTestTopic1();
            var tagInst1 = this.MakeTestTagInst1();

            using (var ctx = TestContext.CreateInstance())
            {
                ctx.RequestContext.Setup(x => x.ValidateAuthorized());
                ctx.SessionContext.Setup(x => x.UserId).Returns(topic1.AuthorUserId);
                ctx.DbTopicsRepo.Setup(x => x.FindTopicById(topic1.Id)).Returns(topic1);
                ctx.DbTopicsRepo.Setup(x => x.GetTopicTags(topic1.Id)).Returns(new[] { tagInst1 });
                ctx.DbTopicsRepo.Setup(x => x.GetTopicContentCounters(topic1.Id)).Returns((topicLearnedCounter, topicTotalCounter));
                var testController = new LearningServiceImpl(ctx.ServiceContext.Object);

                var result = testController.GetTopic(topic1.Id.ToString());

                Assert.AreEqual(topic1.Id, result.Id);
            }
        }

        [TestMethod]
        public void GetTopicForbiddenTest()
        {
            var topic1 = this.MakeTestTopic1();
            
            using (var ctx = TestContext.CreateInstance())
            {
                ctx.RequestContext.Setup(x => x.ValidateAuthorized());
                ctx.SessionContext.Setup(x => x.UserId).Returns(otherUserId);

                ctx.DbTopicsRepo.Setup(x => x.FindTopicById(topic1.Id)).Returns(topic1);

                var testController = new LearningServiceImpl(ctx.ServiceContext.Object);

                var ex = Assert.ThrowsException<WebFaultException>(() => testController.GetTopic(topic1.Id.ToString()));
                Assert.IsTrue(ex.StatusCode == HttpStatusCode.Forbidden);
            }
        }

        [TestMethod]
        public void GetTopicNullTest()
        {
            var topic1 = this.MakeTestTopic1();

            DbTopicInfo topic = null;
            using (var ctx = TestContext.CreateInstance())
            {
                ctx.RequestContext.Setup(x => x.ValidateAuthorized());
                ctx.DbTopicsRepo.Setup(x => x.FindTopicById(topic1.Id)).Returns(topic);

                var testController = new LearningServiceImpl(ctx.ServiceContext.Object);

                var ex = Assert.ThrowsException<WebFaultException>(() => testController.GetTopic(topic1.Id.ToString()));
                Assert.IsTrue(ex.StatusCode == HttpStatusCode.NotFound);
            }
        }

        [TestMethod]
        public void GetTopicsRangeTest()
        {
            var (topics, associatedTag1, associatedTag2) = this.MakeTestTopics();

            var skipCount = 0;
            var takeCount = 5;
            var topicAssociatedTagInfo = new [] { associatedTag1, associatedTag2 };
            
            var resultCollection = topics.Where(t => t.AuthorUserId == testUserId).Skip(skipCount).Take(takeCount).ToArray();
            
            using (var ctx = TestContext.CreateInstance())
            {
                ctx.RequestContext.Setup(x => x.ValidateAuthorized());
                ctx.SessionContext.Setup(x => x.UserId).Returns(testUserId);
                ctx.DbTopicsRepo.Setup(x => x.GetAuthorTopicsCount(testUserId)).Returns(topics.Select(t => t.AuthorUserId == testUserId).Count);
                ctx.DbTopicsRepo.Setup(x => x.GetAuthorTopics(testUserId, skipCount, takeCount)).Returns(topics.Where(t => t.AuthorUserId == testUserId).Skip(skipCount).Take(takeCount).ToList());
                ctx.DbTopicsRepo.Setup(x => x.GetAuthorTopicTags(testUserId, skipCount, takeCount)).Returns(topicAssociatedTagInfo);

                var testController = new LearningServiceImpl(ctx.ServiceContext.Object);

                var resList = testController.GetTopicsRange(skipCount.ToString(), takeCount.ToString());

                CollectionAssert.AreEqual(resList.Items, resultCollection, MyComparer.EquilityFor<TopicInfoType, DbTopicInfo>((a, b) => a.Id == b.Id));
            }
        }

        [TestMethod]
        public void GetTopicsTest()
        {
            var (topics, associatedTag1, associatedTag2) = this.MakeTestTopics();
            
            var skipCount = 0;
            var takeCount = 10;
            var topicAssociatedTagInfo = new TopicAssociatedTagInfo[] { associatedTag1, associatedTag2 };

            var resultCollection = topics.Where(t => t.AuthorUserId == testUserId).Skip(skipCount).Take(takeCount).ToArray();

            using (var ctx = TestContext.CreateInstance())
            {
                ctx.RequestContext.Setup(x => x.ValidateAuthorized());
                ctx.SessionContext.Setup(x => x.UserId).Returns(testUserId);
                ctx.DbTopicsRepo.Setup(x => x.GetAuthorTopicsCount(testUserId)).Returns(topics.Select(t => t.AuthorUserId == testUserId).Count);
                ctx.DbTopicsRepo.Setup(x => x.GetAuthorTopics(testUserId, skipCount, takeCount)).Returns(topics.Where(t => t.AuthorUserId == testUserId).Skip(skipCount).Take(takeCount).ToList());
                ctx.DbTopicsRepo.Setup(x => x.GetAuthorTopicTags(testUserId, skipCount, takeCount)).Returns(topicAssociatedTagInfo);

                var testController = new LearningServiceImpl(ctx.ServiceContext.Object);

                var resList = testController.GetTopics();

                CollectionAssert.AreEqual(resList.Items, resultCollection, MyComparer.EquilityFor<TopicInfoType, DbTopicInfo>((a, b) => a.Id == b.Id));
            }
        }

        [TestMethod]
        public void CreateTopicTest()
        {
            var tagInst1 = this.MakeTestTagInst1();

            DbTopicInfo newTopic = null;

            var tags = new[] { tagInst1.Word };

            using (var ctx = TestContext.CreateInstance())
            {
                ctx.RequestContext.Setup(x => x.ValidateAuthorized());
                ctx.SessionContext.Setup(x => x.UserId).Returns(testUserId);
                ctx.DbContext.Setup(x => x.SubmitChanges());
                ctx.DbTopicsRepo.Setup(x => x.AddTopic(It.IsNotNull<DbTopicInfo>())).Callback((DbTopicInfo top) => { newTopic = top; });
                ctx.DbTopicsRepo.Setup(x => x.AssociateTopic(It.IsAny<long>(), It.IsNotNull<IList<DbTagInstanceInfo>>()));
                ctx.DbResourcesRepo.Setup(x => x.IntroduceTags(tags)).Returns(new List<DbTagInstanceInfo>() { tagInst1 });
                ctx.WordNormalizer.Setup(x => x.NormalizeWord(tagInst1.Word)).Returns(tagInst1.Word);
                ctx.DbTopicsRepo.Setup(x => x.FindTopicById(It.IsAny<long>())).Returns((long topicId) => newTopic);
                ctx.DbTopicsRepo.Setup(x => x.GetTopicTags(It.IsAny<long>())).Returns((long topicId) => new[] { tagInst1});
                ctx.DbTopicsRepo.Setup(x => x.GetTopicContentCounters(It.IsAny<long>())).Returns((long topicId) => (0, 0));

                var testController = new LearningServiceImpl(ctx.ServiceContext.Object);

                var createdTopic = testController.CreateTopic(new CreateTopicSpecType() { 
                    AssociationTags = new[] { new AssociationTagInfoType() { Word = tagInst1.Word } }, 
                    TopicName = topicTitle1
                });

                Assert.AreEqual(tagInst1.Word, createdTopic.AssociationTags.First().Word);
                Assert.AreEqual(topicTitle1, createdTopic.Title);
            }
        }


        [TestMethod]
        public void DeleteTopicTest()
        {
            var topic1 = this.MakeTestTopic1();

            using (var ctx = TestContext.CreateInstance())
            {
                ctx.RequestContext.Setup(x => x.ValidateAuthorized());
                ctx.SessionContext.Setup(x => x.UserId).Returns(testUserId);
                ctx.DbTopicsRepo.Setup(x => x.DeleteTopic(topic1));

                ctx.DbTopicsRepo.Setup(x => x.FindTopicById(topic1.Id)).Returns(topic1);

                var testController = new LearningServiceImpl(ctx.ServiceContext.Object);

                testController.DeleteTopic(topic1.Id.ToString());
            }
        }

        [TestMethod]
        public void DeleteTopicNullTest()
        {
            var topic1 = this.MakeTestTopic1();
            
            using (var ctx = TestContext.CreateInstance())
            {
                ctx.RequestContext.Setup(x => x.ValidateAuthorized());
                ctx.DbTopicsRepo.Setup(x => x.FindTopicById(topic1.Id)).Returns<DbTopicInfo>(null);

                var testController = new LearningServiceImpl(ctx.ServiceContext.Object);
                var ex = Assert.ThrowsException<WebFaultException>(() => testController.DeleteTopic(topic1.Id.ToString()));
                Assert.IsTrue(ex.StatusCode == HttpStatusCode.NotFound);
            }
        }

        [TestMethod]
        public void DeleteTopicForbiddenTest()
        {
            var topic1 = this.MakeTestTopic1();

            using (var ctx = TestContext.CreateInstance())
            {
                ctx.RequestContext.Setup(x => x.ValidateAuthorized());
                ctx.SessionContext.Setup(x => x.UserId).Returns(otherUserId);
                ctx.DbTopicsRepo.Setup(x => x.FindTopicById(topic1.Id)).Returns(topic1);

                var testController = new LearningServiceImpl(ctx.ServiceContext.Object);
                var ex = Assert.ThrowsException<WebFaultException>(() => testController.DeleteTopic(topic1.Id.ToString()));
                Assert.IsTrue(ex.StatusCode == HttpStatusCode.Forbidden);
            }
        }
    }
}
