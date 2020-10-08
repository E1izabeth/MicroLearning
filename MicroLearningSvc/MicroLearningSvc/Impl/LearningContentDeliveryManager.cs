using MicroLearningSvc.Db;
using MicroLearningSvc.Util;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace MicroLearningSvc.Impl
{
    class LearningContentDeliveryManager : ILearningContentDeliveryManager, IDisposable
    {
        readonly object _lock = new object();
        readonly ILearningServiceContext _ctx;
        readonly ITimer _timer;

        bool _timerIsActive = false;
        DateTime _nearestTimeout = DateTime.MaxValue;

        public LearningContentDeliveryManager(ILearningServiceContext ctx, ITimer timer)
        {
            _ctx = ctx;
            _timer = timer;
            _timer.Elapsed += this.OnDeliveryTimer;

            this.Initialize();
            this.Refresh();
        }

        private void Initialize()
        {
            using (var ctx = _ctx.OpenLocalContext())
            {
                var nearestSubscription = ctx.Db.Subscriptions.DeliveryFindNearestSubscription();

                if (nearestSubscription != null)
                {
                    var timePassedSincePlanned = _ctx.UtcNow - nearestSubscription.NextPortionTime;
                    if (timePassedSincePlanned > TimeSpan.Zero)
                    {
                        ctx.Db.Subscriptions.DeliveryUpgradeSubscriptions(timePassedSincePlanned + TimeSpan.FromMinutes(1));
                    }
                    else
                    {
                        // all deliveries are in future, it's ok
                    }
                }
            }
        }

        private void OnDeliveryTimer(object sender, EventArgs eventArgs)
        {
            lock (_lock)
            {
                _timer.Stop();
                this.PerformDelivery();
                this.Refresh();
            }
        }

        public void Refresh()
        {
            lock (_lock)
            {
                if (this.TryGetTimeToNearestDelivery(out var timeout))
                {
                    var nearestTimeout = DateTime.UtcNow + timeout;
                    if (nearestTimeout < _nearestTimeout)
                    {
                        _timer.Interval = timeout;
                        _timer.Start();
                        _timerIsActive = true;
                        _nearestTimeout = nearestTimeout;
                    }
                }
                else
                {
                    _timer.Stop();
                    _timerIsActive = false;
                }
            }
        }

        private bool TryGetTimeToNearestDelivery(out TimeSpan timeout)
        {
            using (var ctx = _ctx.OpenLocalContext())
            {
                var nearestSubscription = ctx.Db.Subscriptions.DeliveryFindNearestSubscription();

                if (nearestSubscription != null)
                {
                    timeout = nearestSubscription.NextPortionTime - _ctx.UtcNow;
                    if (timeout < TimeSpan.Zero)
                    {
                        if (-timeout < TimeSpan.FromMinutes(5))
                        {
                            timeout = TimeSpan.FromMinutes(1);
                        }
                        else
                        {
                            throw new ApplicationException("should never happen when other logic is correct");
                        }
                    }

                    return true;
                }
                else
                {
                    timeout = default(TimeSpan);
                    return false;
                }
            }
        }

        private void PerformDelivery()
        {
            using (var ctx = _ctx.OpenLocalContext())
            {
                var nearestDeliveryThreshold = _ctx.UtcNow + _ctx.Configuration.DeliveryTimeout;

                var subscriptions = ctx.Db.Subscriptions.DeliveryGetContentToDeliver(nearestDeliveryThreshold);
                var dataToSend = subscriptions.ToList();
                var now = _ctx.UtcNow;

                foreach (var item in dataToSend)
                {
                    try
                    {
                        item.userSubscription.NextPortionTime += item.userSubscription.Interval;

                        var messageText = new StringBuilder();
                        if (item.resourceContentPart.Order > 0)
                            messageText.AppendLine("...");
                        messageText.AppendLine(item.resourceContentPart.TextContent)
                                   .AppendLine("...");

                        _ctx.SendMail(item.user.Email, "Microlearning on " + item.topic.Title, messageText.ToString());

                        DbUserSubscriptionContentPortionInfo contentPortion;
                        if (item.subscriptionContentPart == null)
                        {
                            contentPortion = new DbUserSubscriptionContentPortionInfo()
                            {
                                IsLearned = false,
                                LearnedStamp = SqlDateTime.MinValue.Value,
                                UserSubscriptionId = item.userSubscription.Id,
                                ResourceContentPortionId = item.resourceContentPart.Id,
                            };

                            ctx.Db.Subscriptions.RegisterPart(contentPortion);
                        }
                        else
                        {
                            contentPortion = item.subscriptionContentPart;
                        }

                        contentPortion.DeliveredStamp = now;
                        contentPortion.IsDelivered = true;

                        if (contentPortion.IsMarkedToRepeat)
                            contentPortion.IsMarkedToRepeat = false;

                        ctx.Db.SubmitChanges();
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.Print(ex.FormatExceptionOutputInfo());
                    }
                }

                ctx.Db.SubmitChanges();
            }
        }

        public void Dispose()
        {
            _timer.SafeDispose();
        }
    }
}
