using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.ServiceModel.Web;
using MicroLearningCli;
using MicroLearningSvc;
using MicroLearningSvc.Db;
using MicroLearningSvc.Impl;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace UnitTestProject
{
    [TestClass]
    public class SubscriptionsTests : TestBase
    {
        [TestMethod]
        public void ResetTopicLearningProgressTest()
        {
            var (subscriptions, topic1) = this.MakeTestSubscriptions();
            var subscr = subscriptions.Where(s => s.TopicId == topic1.Id).First();

            using (var ctx = TestContext.CreateInstance())
            {
                ctx.RequestContext.Setup(x => x.ValidateAuthorized());
                ctx.DbTopicsRepo.Setup(x => x.FindTopicById(topic1.Id)).Returns(topic1);
                ctx.SessionContext.Setup(x => x.UserId).Returns(testUserId);
                ctx.DbSubscriptionsRepo.Setup(x => x.FindSubscriptionByTopic(topic1.Id)).Returns(subscr);
                ctx.DbSubscriptionsRepo.Setup(x => x.ResetLearningProgress(subscriptions[0].Id));
                ctx.DbContext.Setup(x => x.SubmitChanges());

                var testController = new LearningServiceImpl(ctx.ServiceContext.Object);

                testController.ResetTopicLearningProgress(subscriptions[0].Id.ToString());
            }
        }


        [TestMethod]
        public void ResetTopicLearningProgressForbiddenTest()
        {
            var (subscriptions, topic1) = this.MakeTestSubscriptions();
            var subscr = subscriptions.Where(s => s.TopicId == topic1.Id).First();

            using (var ctx = TestContext.CreateInstance())
            {
                ctx.RequestContext.Setup(x => x.ValidateAuthorized());
                ctx.SessionContext.Setup(x => x.UserId).Returns(otherUserId);
                ctx.DbTopicsRepo.Setup(x => x.FindTopicById(topic1.Id)).Returns(topic1);

                var testController = new LearningServiceImpl(ctx.ServiceContext.Object);

                var ex = Assert.ThrowsException<WebFaultException>(() => testController.ResetTopicLearningProgress(subscriptions[0].Id.ToString()));
                Assert.IsTrue(ex.StatusCode == HttpStatusCode.Forbidden);
            }
        }


        [TestMethod]
        public void ResetTopicLearningProgressNotFoundTest()
        {
            var (subscriptions, topic1) = this.MakeTestSubscriptions();
            var subscr = subscriptions.Where(s => s.TopicId == topic1.Id).First();

            using (var ctx = TestContext.CreateInstance())
            {
                ctx.RequestContext.Setup(x => x.ValidateAuthorized());
                ctx.DbTopicsRepo.Setup(x => x.FindTopicById(topic1.Id)).Returns<DbTopicInfo>(null);


                var testController = new LearningServiceImpl(ctx.ServiceContext.Object);

                var ex = Assert.ThrowsException<WebFaultException>(() => testController.ResetTopicLearningProgress(subscriptions[0].Id.ToString()));
                Assert.IsTrue(ex.StatusCode == HttpStatusCode.NotFound);
            }
        }

        [TestMethod]
        public void ResetTopicLearningProgressSubscriptionNullTest()
        {
            var (subscriptions, topic1) = this.MakeTestSubscriptions();
            var subscr = subscriptions.Where(s => s.TopicId == topic1.Id).First();

            using (var ctx = TestContext.CreateInstance())
            {
                ctx.RequestContext.Setup(x => x.ValidateAuthorized());
                ctx.SessionContext.Setup(x => x.UserId).Returns(testUserId);
                ctx.DbTopicsRepo.Setup(x => x.FindTopicById(topic1.Id)).Returns(topic1);
                ctx.DbSubscriptionsRepo.Setup(x => x.FindSubscriptionByTopic(topic1.Id)).Returns<DbUserSubscriptionInfo>(null);


                var testController = new LearningServiceImpl(ctx.ServiceContext.Object);

                var ex = Assert.ThrowsException<WebFaultException>(() => testController.ResetTopicLearningProgress(subscriptions[0].Id.ToString()));
                Assert.IsTrue(ex.StatusCode == HttpStatusCode.NotFound);
            }
        }

        [TestMethod]
        public void ActivateTopicLearningTest()
        {
            DbUserSubscriptionInfo subscr = null;
            var topic1 = this.MakeTestTopic1();

            using (var ctx = TestContext.CreateInstance())
            {
                ctx.RequestContext.Setup(x => x.ValidateAuthorized());
                ctx.DbTopicsRepo.Setup(x => x.FindTopicById(topic1.Id)).Returns(topic1);
                ctx.SessionContext.Setup(x => x.UserId).Returns(testUserId);
                ctx.DbContext.Setup(x => x.SubmitChanges());
                ctx.DeliveryManager.Setup(x => x.Refresh());
                ctx.DbSubscriptionsRepo.Setup(x => x.FindSubscriptionByTopic(topic1.Id)).Returns<DbUserSubscriptionInfo>(null);
                ctx.DbSubscriptionsRepo.Setup(x => x.Add(It.IsNotNull<DbUserSubscriptionInfo>()))
                                       .Callback((DbUserSubscriptionInfo s) => subscr = s);

                var testController = new LearningServiceImpl(ctx.ServiceContext.Object);

                testController.ActivateTopicLearning(topic1.Id.ToString(), new ActivateTopicSpecType() { DueSeconds = 0, IntervalSeconds = (long)TimeSpan.FromDays(1).TotalSeconds });

                Assert.IsTrue(subscr.IsActive);
            }
        }

        [TestMethod]
        public void ActivateTopicLearningNullTest()
        {
            var (subscriptions, topic1) = this.MakeTestSubscriptions();
            var subscr = subscriptions.Where(s => s.TopicId == topic1.Id).First();

            using (var ctx = TestContext.CreateInstance())
            {
                ctx.RequestContext.Setup(x => x.ValidateAuthorized());
                ctx.DbTopicsRepo.Setup(x => x.FindTopicById(topic1.Id)).Returns<DbTopicInfo>(null);

                var testController = new LearningServiceImpl(ctx.ServiceContext.Object);

                var ex = Assert.ThrowsException<WebFaultException>(() => testController.ActivateTopicLearning(topic1.Id.ToString(), new ActivateTopicSpecType() { DueSeconds = 0, IntervalSeconds = (long)TimeSpan.FromDays(1).TotalSeconds }));
                
                Assert.IsTrue(ex.StatusCode == HttpStatusCode.NotFound);
            }
        }

        [TestMethod]
        public void ActivateTopicLearningForbiddenTest()
        {
            var (subscriptions, topic1) = this.MakeTestSubscriptions();

            var subscr = subscriptions.Where(s => s.TopicId == topic1.Id).First();

            using (var ctx = TestContext.CreateInstance())
            {
                ctx.RequestContext.Setup(x => x.ValidateAuthorized());
                ctx.DbTopicsRepo.Setup(x => x.FindTopicById(topic1.Id)).Returns(topic1);
                ctx.SessionContext.Setup(x => x.UserId).Returns(otherUserId);

                var testController = new LearningServiceImpl(ctx.ServiceContext.Object);

                var ex = Assert.ThrowsException<WebFaultException>(() => testController.ActivateTopicLearning(topic1.Id.ToString(), new ActivateTopicSpecType() { DueSeconds = 0, IntervalSeconds = (long)TimeSpan.FromDays(1).TotalSeconds }));

                Assert.IsTrue(ex.StatusCode == HttpStatusCode.Forbidden);
            }
        }


        [TestMethod]
        public void ActivateTopicLearningSubscriptionNotNullTest()
        {
            var (subscriptions, topic1) = this.MakeTestSubscriptions();

            using (var ctx = TestContext.CreateInstance())
            {
                ctx.RequestContext.Setup(x => x.ValidateAuthorized());
                ctx.DbTopicsRepo.Setup(x => x.FindTopicById(topic1.Id)).Returns(topic1);
                ctx.SessionContext.Setup(x => x.UserId).Returns(testUserId);
                ctx.DbContext.Setup(x => x.SubmitChanges());
                ctx.DeliveryManager.Setup(x => x.Refresh());
                ctx.DbSubscriptionsRepo.Setup(x => x.FindSubscriptionByTopic(topic1.Id)).Returns(subscriptions.First());

                var testController = new LearningServiceImpl(ctx.ServiceContext.Object);

                testController.ActivateTopicLearning(topic1.Id.ToString(), new ActivateTopicSpecType() { DueSeconds = 0, IntervalSeconds = (long)TimeSpan.FromDays(1).TotalSeconds });

                Assert.IsTrue(subscriptions.First().IsActive);
            }
        }


        [TestMethod]
        public void DeactivateTopicLearningTest()
        {
            var (subscriptions, topic1) = this.MakeTestSubscriptions();
            var subscr = subscriptions.Where(s => s.TopicId == topic1.Id).First();

            using (var ctx = TestContext.CreateInstance())
            {
                ctx.RequestContext.Setup(x => x.ValidateAuthorized());
                ctx.DbTopicsRepo.Setup(x => x.FindTopicById(topic1.Id)).Returns(topic1);
                ctx.SessionContext.Setup(x => x.UserId).Returns(testUserId);
                ctx.DbSubscriptionsRepo.Setup(x => x.FindSubscriptionByTopic(topic1.Id)).Returns(subscr);
                ctx.DbContext.Setup(x => x.SubmitChanges());
                ctx.DeliveryManager.Setup(x => x.Refresh());

                var testController = new LearningServiceImpl(ctx.ServiceContext.Object);

                testController.DeactivateTopicLearning(topic1.Id.ToString());

                Assert.IsFalse(subscr.IsActive);
            }
        }

        [TestMethod]
        public void DeactivateTopicLearningNullTest()
        {
            var (subscriptions, topic1) = this.MakeTestSubscriptions();
            var subscr = subscriptions.Where(s => s.TopicId == topic1.Id).First();

            using (var ctx = TestContext.CreateInstance())
            {
                ctx.RequestContext.Setup(x => x.ValidateAuthorized());
                ctx.DbTopicsRepo.Setup(x => x.FindTopicById(topic1.Id)).Returns<DbTopicInfo>(null);

                var testController = new LearningServiceImpl(ctx.ServiceContext.Object);

                var ex = Assert.ThrowsException<WebFaultException>(() =>testController.DeactivateTopicLearning(topic1.Id.ToString()));
                Assert.IsTrue(ex.StatusCode == HttpStatusCode.NotFound);
            }
        }

        [TestMethod]
        public void DeactivateTopicLearningForbiddenTest()
        {
            var (subscriptions, topic1) = this.MakeTestSubscriptions();
            var subscr = subscriptions.Where(s => s.TopicId == topic1.Id).First();

            using (var ctx = TestContext.CreateInstance())
            {
                ctx.RequestContext.Setup(x => x.ValidateAuthorized());
                ctx.DbTopicsRepo.Setup(x => x.FindTopicById(topic1.Id)).Returns(topic1);
                ctx.SessionContext.Setup(x => x.UserId).Returns(otherUserId);

                var testController = new LearningServiceImpl(ctx.ServiceContext.Object);

                var ex = Assert.ThrowsException<WebFaultException>(() => testController.DeactivateTopicLearning(topic1.Id.ToString()));
                Assert.IsTrue(ex.StatusCode == HttpStatusCode.Forbidden);
            }
        }

        [TestMethod]
        public void DeactivateTopicLearningSubscriptionNullTest()
        {
            var (subscriptions, topic1) = this.MakeTestSubscriptions();
            var subscr = subscriptions.Where(s => s.TopicId == topic1.Id).First();

            using (var ctx = TestContext.CreateInstance())
            {
                ctx.RequestContext.Setup(x => x.ValidateAuthorized());
                ctx.DbTopicsRepo.Setup(x => x.FindTopicById(topic1.Id)).Returns(topic1);
                ctx.SessionContext.Setup(x => x.UserId).Returns(testUserId);
                ctx.DbSubscriptionsRepo.Setup(x => x.FindSubscriptionByTopic(topic1.Id)).Returns<DbUserSubscriptionInfo>(null);

                var testController = new LearningServiceImpl(ctx.ServiceContext.Object);

                var ex = Assert.ThrowsException<WebFaultException>(() => testController.DeactivateTopicLearning(topic1.Id.ToString()));
                Assert.IsTrue(ex.StatusCode == HttpStatusCode.NotFound);
            }
        }
    }
}
