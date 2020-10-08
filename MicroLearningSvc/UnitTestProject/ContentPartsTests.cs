using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
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
    public class ContentPartsTests : TestBase
    {

        [TestMethod]
        public void GetContentPartsByTopicTest()
        {
            var skip = 1;
            var take = 1;
            var contentPartsCount = 1;
            var (contentPart, contentPortion, subscr, subscrContentParts, topic1) = this.MakeContentPartsForTest();

            using (var ctx = TestContext.CreateInstance())
            {
                ctx.RequestContext.Setup(x => x.ValidateAuthorized());
                ctx.SessionContext.Setup(x => x.UserId).Returns(testUserId);
                ctx.DbSubscriptionsRepo.Setup(x => x.ContentPartsByTopic(topic1.Id, testUserId, skip, take)).Returns(
                    (contentPartsCount, new[] { new SubscriptionContentPart(contentPart, contentPortion) }.AsQueryable())
                );

                var testController = new LearningServiceImpl(ctx.ServiceContext.Object);

                var contPartsList = testController.GetContentPartsByTopic(topic1.Id.ToString(), skip.ToString(), take.ToString());

                Assert.AreEqual(contentPartsCount, contPartsList.Count);
                Assert.AreEqual(contentPart.Id, contPartsList.Items.First().Id);
                Assert.AreEqual(contentPart.Order, contPartsList.Items.First().Order);
                Assert.AreEqual(contentPart.ResourceId, contPartsList.Items.First().ResourceId);
                Assert.AreEqual(contentPart.TextContent, contPartsList.Items.First().Text);
            }
        }

        [TestMethod]
        public void MarkContentPartLearnedTest()
        {
            var (contentPart, contentPortion, subscr, subscrContentParts, topic1) = this.MakeContentPartsForTest();

            using (var ctx = TestContext.CreateInstance())
            {
                ctx.RequestContext.Setup(x => x.ValidateAuthorized());
                ctx.SessionContext.Setup(x => x.UserId).Returns(testUserId);
                ctx.DbSubscriptionsRepo.Setup(x => x.ContentPartByTopic(testUserId, topic1.Id, contentPart.Id)).Returns(subscrContentParts);
                ctx.DbContext.Setup(x => x.SubmitChanges());
                ctx.DeliveryManager.Setup(x => x.Refresh());

                var testController = new LearningServiceImpl(ctx.ServiceContext.Object);

                testController.MarkContentPartLearned(topic1.Id.ToString(), contentPart.Id.ToString());

                Assert.IsTrue(contentPortion.IsLearned);
            }
        }

        [TestMethod]
        public void MarkContentPartLearnedPortionNullTest()
        {
            var (contentPart, contentPortion, subscr, subscrContentPartsNoUserPart, topic1) = this.MakeContentPartsForTest(true);

            DbUserSubscriptionContentPortionInfo part = null;

            using (var ctx = TestContext.CreateInstance())
            {
                ctx.RequestContext.Setup(x => x.ValidateAuthorized());
                ctx.SessionContext.Setup(x => x.UserId).Returns(testUserId);
                ctx.DbContext.Setup(x => x.SubmitChanges());
                ctx.DeliveryManager.Setup(x => x.Refresh());
                ctx.DbSubscriptionsRepo.Setup(x => x.ContentPartByTopic(testUserId, topic1.Id, contentPart.Id)).Returns(subscrContentPartsNoUserPart);
                ctx.DbSubscriptionsRepo.Setup(x => x.RegisterPart(It.IsNotNull<DbUserSubscriptionContentPortionInfo>()))
                                       .Callback((DbUserSubscriptionContentPortionInfo p) => part = p);

                var testController = new LearningServiceImpl(ctx.ServiceContext.Object);

                testController.MarkContentPartLearned(topic1.Id.ToString(), contentPart.Id.ToString());

                Assert.IsTrue(part.IsLearned);
            }
        }

        [TestMethod]
        public void UnmarkContentPartLearnedTest()
        {
            var (contentPart, contentPortion, subscr, subscrContentParts, topic1) = this.MakeContentPartsForTest(false, true);

            using (var ctx = TestContext.CreateInstance())
            {
                ctx.RequestContext.Setup(x => x.ValidateAuthorized());
                ctx.SessionContext.Setup(x => x.UserId).Returns(testUserId);
                ctx.DbSubscriptionsRepo.Setup(x => x.ContentPartByTopic(testUserId, topic1.Id, contentPart.Id)).Returns(subscrContentParts);
                ctx.DbContext.Setup(x => x.SubmitChanges());
                ctx.DeliveryManager.Setup(x => x.Refresh());

                var testController = new LearningServiceImpl(ctx.ServiceContext.Object);

                testController.UnmarkContentPartLearned(topic1.Id.ToString(), contentPart.Id.ToString());

                Assert.IsFalse(contentPortion.IsLearned);
            }
        }

        [TestMethod]
        public void UnmarkMissingContentPartLearnedTest()
        {
            var (contentPart, contentPortion, subscr, subscrContentParts, topic1) = this.MakeContentPartsForTest(true);

            using (var ctx = TestContext.CreateInstance())
            {
                ctx.RequestContext.Setup(x => x.ValidateAuthorized());
                ctx.SessionContext.Setup(x => x.UserId).Returns(testUserId);
                ctx.DbSubscriptionsRepo.Setup(x => x.ContentPartByTopic(testUserId, topic1.Id, contentPart.Id)).Returns(subscrContentParts);
                ctx.DbContext.Setup(x => x.SubmitChanges());
                ctx.DeliveryManager.Setup(x => x.Refresh());

                var testController = new LearningServiceImpl(ctx.ServiceContext.Object);

                testController.UnmarkContentPartLearned(topic1.Id.ToString(), contentPart.Id.ToString());

                Assert.IsFalse(contentPortion.IsLearned);
            }
        }

        [TestMethod]
        public void MarkContentPartLearnedNoPartsTest()
        {
            var (contentPart, contentPortion, subscr, subscrContentParts, topic1) = this.MakeContentPartsForTest(false, true);

            using (var ctx = TestContext.CreateInstance())
            {
                ctx.RequestContext.Setup(x => x.ValidateAuthorized());
                ctx.SessionContext.Setup(x => x.UserId).Returns(testUserId);
                ctx.DbSubscriptionsRepo.Setup(x => x.ContentPartByTopic(testUserId, topic1.Id, contentPart.Id)).Returns(new SubscriptionResourceContentPart[] { });

                var testController = new LearningServiceImpl(ctx.ServiceContext.Object);

                var ex = Assert.ThrowsException<WebFaultException>(() => testController.MarkContentPartLearned(topic1.Id.ToString(), contentPart.Id.ToString()));
                Assert.IsTrue(ex.StatusCode == System.Net.HttpStatusCode.NotFound);
            }
        }

        [TestMethod]
        public void UnmarkContentPartLearnedNoPartsTest()
        {
            var (contentPart, contentPortion, subscr, subscrContentParts, topic1) = this.MakeContentPartsForTest(false, true);

            using (var ctx = TestContext.CreateInstance())
            {
                ctx.RequestContext.Setup(x => x.ValidateAuthorized());
                ctx.SessionContext.Setup(x => x.UserId).Returns(testUserId);
                ctx.DbSubscriptionsRepo.Setup(x => x.ContentPartByTopic(testUserId, topic1.Id, contentPart.Id)).Returns(new SubscriptionResourceContentPart[] { });

                var testController = new LearningServiceImpl(ctx.ServiceContext.Object);

                var ex = Assert.ThrowsException<WebFaultException>(() => testController.UnmarkContentPartLearned(topic1.Id.ToString(), contentPart.Id.ToString()));
                Assert.IsTrue(ex.StatusCode == System.Net.HttpStatusCode.NotFound);
            }
        }
    }
}
