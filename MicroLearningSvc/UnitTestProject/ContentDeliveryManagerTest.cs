using MicroLearningSvc;
using MicroLearningSvc.Db;
using MicroLearningSvc.Impl;
using MicroLearningSvc.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitTestProject.Util;

namespace UnitTestProject
{
    [TestClass]
    public class ContentDeliveryManagerTest : TestBase
    {

        [TestMethod]
        public void InitialOldSubscriptionsToTheFutureUpgradeTest()
        {
            using (var ctx = TestContext.CreateInstance())
            {
                // ctx.ServiceContext.Setup(x => x.Configuration).Returns(new LearningServiceConfiguration() { DeliveryTimeout = TimeSpan.FromHours(1) });

                var now = DateTime.UtcNow;
                var dt = TimeSpan.FromMinutes(5);

                var nearestSubscription = new DbUserSubscriptionInfo()
                {
                    Id = 24,
                    IsActive = true,
                    TopicId = 22,
                    UserId = 22,
                    Interval = TimeSpan.FromHours(1),
                    NextPortionTime = now - dt
                };

                TimeSpan off = TimeSpan.Zero;

                ctx.ServiceContext.Setup(x => x.UtcNow).Returns(now);
                ctx.LocalContext.Setup(x => x.Db.Subscriptions.DeliveryFindNearestSubscription()).Returns(nearestSubscription);
                ctx.LocalContext.Setup(x => x.Db.Subscriptions.DeliveryUpgradeSubscriptions(It.IsAny<TimeSpan>()))
                                .Callback((TimeSpan backToTheFuture) => nearestSubscription.NextPortionTime += (off = backToTheFuture));

                var timer = ctx.MakeTimerMock(true, false);

                using (var man = new LearningContentDeliveryManager(ctx.ServiceContext.Object, timer.Instance))
                {
                }

                Assert.IsTrue(timer.Interval == nearestSubscription.NextPortionTime - now);
                Assert.IsTrue(off > dt);
            }
        }

        [TestMethod]
        public void InitialNoDeliveryTest()
        {
            using (var ctx = TestContext.CreateInstance())
            {
                var now = DateTime.UtcNow;

                ctx.LocalContext.Setup(x => x.Db.Subscriptions.DeliveryFindNearestSubscription()).Returns<DbUserSubscriptionInfo>(null);

                var timer = ctx.MakeTimerMock(false, true);

                using (var man = new LearningContentDeliveryManager(ctx.ServiceContext.Object, timer.Instance))
                {
                }
            }
        }

        [TestMethod]
        public void InitialFirstFutureDeliveryTest()
        {
            using (var ctx = TestContext.CreateInstance())
            {
                var now = DateTime.UtcNow;
                var deliveryTimeout = TimeSpan.FromHours(1);

                var nearestSubscription = new DbUserSubscriptionInfo()
                {
                    Id = 24,
                    IsActive = true,
                    TopicId = 22,
                    UserId = 22,
                    Interval = TimeSpan.FromHours(1),
                    NextPortionTime = now + TimeSpan.FromMinutes(5)
                };

                var (contentPart, contentPortion, subscr, subscrContentParts, topic1) = this.MakeContentPartsForTest(true);

                var user = base.MakeTestUser();
                var contentPartsToDeliver = new[]
                {
                    new DeliveryContentPart(topic1, subscr,base.MakeTestResources()[0], contentPart, null, user)
                };

                ctx.ServiceContext.Setup(x => x.UtcNow).Returns(() => now);
                ctx.ServiceContext.Setup(x => x.Configuration).Returns(new LearningServiceConfiguration() { DeliveryTimeout = deliveryTimeout });
                ctx.ServiceContext.Setup(x => x.SendMail(user.Email, It.IsAny<string>(), It.Is<string>(s => s.Contains(contentPart.TextContent))));

                DbUserSubscriptionContentPortionInfo learnedPart = null;
                ctx.LocalContext.Setup(x => x.Db.Subscriptions.DeliveryFindNearestSubscription()).Returns(() => nearestSubscription);
                ctx.LocalContext.Setup(x => x.Db.Subscriptions.DeliveryGetContentToDeliver(It.IsAny<DateTime>())).Returns(contentPartsToDeliver);
                ctx.LocalContext.Setup(x => x.Db.Subscriptions.RegisterPart(It.IsNotNull<DbUserSubscriptionContentPortionInfo>()))
                                .Callback((DbUserSubscriptionContentPortionInfo scp) => learnedPart = scp);
                ctx.LocalContext.Setup(x => x.Db.SubmitChanges()).Callback(() => nearestSubscription = null);

                var timer = ctx.MakeTimerMock(true, true);

                using (var man = new LearningContentDeliveryManager(ctx.ServiceContext.Object, timer.Instance))
                {
                    now += timer.Interval;
                    timer.Raise();
                }

                ctx.LocalContext.Verify(x => x.Db.Subscriptions.DeliveryGetContentToDeliver(now + deliveryTimeout));
                timer.Mock.Verify(x => x.Start(), Times.Exactly(1));
                timer.Mock.Verify(x => x.Stop(), Times.Exactly(1));
                Assert.IsTrue(learnedPart.ResourceContentPortionId == contentPartsToDeliver[0].rcp.Id);
                Assert.IsTrue(learnedPart.UserSubscriptionId == subscr.Id);
                Assert.IsTrue(learnedPart.IsDelivered);
                Assert.IsFalse(learnedPart.IsLearned);
                Assert.IsFalse(learnedPart.IsMarkedToRepeat);
            }
        }

        [TestMethod]
        public void InitialResendDeliveryTest()
        {
            using (var ctx = TestContext.CreateInstance())
            {
                var now = DateTime.UtcNow;
                var deliveryTimeout = TimeSpan.FromHours(1);

                var nearestSubscription = new DbUserSubscriptionInfo()
                {
                    Id = 24,
                    IsActive = true,
                    TopicId = 22,
                    UserId = 22,
                    Interval = TimeSpan.FromHours(1),
                    NextPortionTime = now + TimeSpan.FromMinutes(5)
                };

                var (contentPart, contentPortion, subscr, subscrContentParts, topic1) = this.MakeContentPartsForTest(false);

                var user = base.MakeTestUser();
                var contentPartsToDeliver = new[]
                {
                    new DeliveryContentPart(topic1, subscr, base.MakeTestResources()[0], contentPart, contentPortion, user)
                };

                contentPortion.IsMarkedToRepeat = true;
                contentPart.Order = 0;

                ctx.ServiceContext.Setup(x => x.UtcNow).Returns(() => now);
                ctx.ServiceContext.Setup(x => x.Configuration).Returns(new LearningServiceConfiguration() { DeliveryTimeout = deliveryTimeout });
                ctx.ServiceContext.Setup(x => x.SendMail(user.Email, It.IsAny<string>(), It.Is<string>(s => s.Contains(contentPart.TextContent))));

                ctx.LocalContext.Setup(x => x.Db.Subscriptions.DeliveryFindNearestSubscription()).Returns(() => nearestSubscription);
                ctx.LocalContext.Setup(x => x.Db.Subscriptions.DeliveryGetContentToDeliver(It.IsAny<DateTime>())).Returns(contentPartsToDeliver);
                ctx.LocalContext.Setup(x => x.Db.SubmitChanges()).Callback(() => nearestSubscription = null);

                var timer = ctx.MakeTimerMock(true, true);

                using (var man = new LearningContentDeliveryManager(ctx.ServiceContext.Object, timer.Instance))
                {
                    now += timer.Interval;
                    timer.Raise();
                }

                ctx.LocalContext.Verify(x => x.Db.Subscriptions.DeliveryGetContentToDeliver(now + deliveryTimeout));
                timer.Mock.Verify(x => x.Start(), Times.Exactly(1));
                timer.Mock.Verify(x => x.Stop(), Times.Exactly(1));
                Assert.IsTrue(contentPortion.IsDelivered);
                Assert.IsFalse(contentPortion.IsLearned);
                Assert.IsFalse(contentPortion.IsMarkedToRepeat);
            }
        }
    }

}
