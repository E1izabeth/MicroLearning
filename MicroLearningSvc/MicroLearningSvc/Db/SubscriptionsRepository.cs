using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroLearningSvc.Db
{
    class SubscriptionsRepository : RepositoryImpl, ISubscriptionsRepository
    {
        public SubscriptionsRepository(DbContext dbContext) : base(dbContext) { }

        public void Add(DbUserSubscriptionInfo subscription)
        {
            _db.UserSubscriptions.InsertOnSubmit(subscription);
        }

        public void DeactivateSubscriptionsByUser(long userId)
        {
            foreach (var subscription in _db.UserSubscriptions.Where(s => s.UserId == userId))
                subscription.IsActive = false;
        }

        public DbUserSubscriptionInfo FindSubscriptionByTopic(long topicId)
        {
            return _db.UserSubscriptions.FirstOrDefault(s => s.TopicId == topicId);
        }

        public void ResetLearningProgress(long subscriptionId)
        {
            _db.ExecuteCommand("DELETE FROM UserSubscriptionContentPortions WHERE UserSubscriptionId = {0}", subscriptionId);
        }

        public (int, IQueryable<SubscriptionContentPart>) ContentPartsByTopic(long topicId, long userId, int skip, int take)
        {
            var q = from t in _db.Topics
                    where t.Id == topicId && t.AuthorUserId == userId
                    join ta in _db.TopicAssociations on t.Id equals ta.TopicId
                    join tti in _db.TagInstances on ta.TagInstanceId equals tti.Id
                    join rti in _db.TagInstances on tti.TagId equals rti.TagId
                    join ra in _db.ResourceAssociations on rti.Id equals ra.TagInstanceId
                    join r in _db.Resources on ra.ResourceId equals r.Id
                    join rcp in _db.ResourceContentPortions on r.Id equals rcp.ResourceId
                    join s in _db.UserSubscriptions
                               on new { tId = ta.TopicId, uId = t.AuthorUserId }
                               equals new { tId = s.TopicId, uId = s.UserId }
                               into presentedUserSubscriptions
                    from s in presentedUserSubscriptions.DefaultIfEmpty()
                    join scp in _db.UserSubscriptionContentPortions
                                    on new { cpId = rcp.Id, sId = s.Id }
                                    equals new { cpId = scp.ResourceContentPortionId, sId = scp.UserSubscriptionId }
                                    into presentedUserSubscriptionContentPortions
                    from scp in presentedUserSubscriptionContentPortions.DefaultIfEmpty()
                    orderby r.CreationStamp, rcp.Order
                    select new { rcp, scp };

            var qq = q.Distinct();

            var count = qq.Count();
            var parts = qq.Skip(skip).Take(take).Select(p => new SubscriptionContentPart(p.rcp, p.scp));

            return (count, parts);
        }

        public IList<SubscriptionResourceContentPart> ContentPartByTopic(long userId, long topicId, long partId)
        {
            var q = from t in _db.Topics
                    where t.Id == topicId && t.AuthorUserId == userId
                    join ta in _db.TopicAssociations on t.Id equals ta.TopicId
                    join tti in _db.TagInstances on ta.TagInstanceId equals tti.Id
                    join rti in _db.TagInstances on tti.TagId equals rti.TagId
                    join ra in _db.ResourceAssociations on rti.Id equals ra.TagInstanceId
                    join r in _db.Resources on ra.ResourceId equals r.Id
                    join rcp in _db.ResourceContentPortions on r.Id equals rcp.ResourceId
                    where rcp.Id == partId
                    join s in _db.UserSubscriptions
                               on new { tId = ta.TopicId, uId = t.AuthorUserId }
                               equals new { tId = s.TopicId, uId = s.UserId }
                               into presentedUserSubscriptions
                    from s in presentedUserSubscriptions.DefaultIfEmpty()
                    join scp in _db.UserSubscriptionContentPortions
                                    on new { cpId = rcp.Id, sId = s.Id }
                                    equals new { cpId = scp.ResourceContentPortionId, sId = scp.UserSubscriptionId }
                                    into presentedUserSubscriptionContentPortions
                    from scp in presentedUserSubscriptionContentPortions.DefaultIfEmpty()
                    orderby r.CreationStamp, rcp.Order
                    select new { s, rcp, scp };

            var qq = q.Distinct();

            var parts = qq.Select(x => new SubscriptionResourceContentPart(x.s, x.rcp, x.scp)).ToList();

            return parts;
        }

        public void RegisterPart(DbUserSubscriptionContentPortionInfo contentPortion)
        {
            _db.UserSubscriptionContentPortions.InsertOnSubmit(contentPortion);
        }

        public DbUserSubscriptionInfo DeliveryFindNearestSubscription()
        {
            return _db.UserSubscriptions.Where(s => s.IsActive).OrderByDescending(s => s.NextPortionTime).FirstOrDefault();
        }

        public void DeliveryUpgradeSubscriptions(TimeSpan delta)
        {
            //_db.ExecuteCommand("UPDATE DbUserSubscriptionInfo SET NextPortionTime = NextPortionTime + {0} WHERE IsActive = 1", delta);
            _db.ExecuteCommand(@"UPDATE DbUserSubscriptionInfo " +
                "SET NextPortionTime = DATEADD(minute, {0}, NextPortionTime)" +
                "WHERE IsActive = 1", delta.TotalMinutes);
        }

        public IEnumerable<DeliveryContentPart> DeliveryGetContentToDeliver(DateTime nearestDeliveryThreshold)
        {

            //var subscriptions = from s in _db.UserSubscriptions where s.IsActive && s.NextPortionTime < nearestDeliveryThreshold
            //                    join t in _db.Topics on s.TopicId equals t.Id
            //                    join ta in _db.TopicAssociations on s.Id equals ta.TopicId
            //                    join tti in _db.TagInstances on ta.TagInstanceId equals tti.Id
            //                    join rti in _db.TagInstances on tti.TagId equals rti.TagId
            //                    join ra in _db.ResourceAssociations on rti.Id equals ra.TagInstanceId
            //                    join r in _db.Resources on ra.ResourceId equals r.Id
            //                    join rcp in _db.ResourceContentPortions on r.Id equals rcp.ResourceId
            //                    join scp in _db.UserSubscriptionContentPortions
            //                                    on new { cpId = rcp.Id, sId = s.Id }
            //                                    equals new { cpId = scp.ResourceContentPortionId, sId = scp.UserSubscriptionId }
            //                                    into presentedUserSubscriptionContentPortions
            //                    from scp in presentedUserSubscriptionContentPortions.DefaultIfEmpty()
            //                    where s.IsActive && (scp == null || (scp.IsMarkedToRepeat || !scp.IsDelivered)) && (r.IsValidated || r.AuthorUserId == s.UserId)
            //                    orderby r.CreationStamp, rcp.Order
            //                    group new { t, s, rcp, scp } by s.Id into g
            //                    select g.First();

            var subscriptions = from s in _db.UserSubscriptions
                                where s.IsActive && s.NextPortionTime < nearestDeliveryThreshold
                                join u in _db.Users on s.UserId equals u.Id
                                join t in _db.Topics on s.TopicId equals t.Id
                                join ta in _db.TopicAssociations on t.Id equals ta.TopicId
                                join tti in _db.TagInstances on ta.TagInstanceId equals tti.Id
                                join rti in _db.TagInstances on tti.TagId equals rti.TagId
                                join ra in _db.ResourceAssociations on rti.Id equals ra.TagInstanceId
                                join r in _db.Resources on ra.ResourceId equals r.Id
                                join rcp in _db.ResourceContentPortions on r.Id equals rcp.ResourceId
                                join scp in _db.UserSubscriptionContentPortions
                                                on new { cpId = rcp.Id, sId = s.Id }
                                                equals new { cpId = scp.ResourceContentPortionId, sId = scp.UserSubscriptionId }
                                                into presentedUserSubscriptionContentPortions
                                from scp in presentedUserSubscriptionContentPortions.DefaultIfEmpty()
                                where s.IsActive && (scp == null || (scp.IsMarkedToRepeat || !scp.IsDelivered)) && (r.IsValidated || r.AuthorUserId == s.UserId)
                                group new { t, s, r, rcp, scp, u } by s.Id into g
                                select (from e in g orderby e.r.CreationStamp, e.rcp.Order select e).Distinct().First();

            /*

            SELECT * FROM (
                SELECT [s].*, [t].[Title] as [TopicTitle], [r].[Title] as [ResourceTitle], [r].[Url] as [ResourceUrl], [rcp].[TextContent], [rcp].[Order], 
                       ROW_NUMBER() OVER (PARTITION BY [s].[Id] ORDER BY [r].[CreationStamp], [rcp].[Order] ASC) AS [PartDeliveryOrder]
                FROM [DbUserSubscriptionInfo] AS [s]
                INNER JOIN [DbTopicInfo] AS [t] ON [s].[TopicId] = [t].[Id]
                INNER JOIN [DbTopicAssociationInfo] AS [ta] ON [s].[Id] = [ta].[TopicId]
                INNER JOIN [DbTagInstanceInfo] AS [tti] ON [ta].[TagInstanceId] = [tti].[Id]
                INNER JOIN [DbTagInstanceInfo] AS [rti] ON [tti].[TagId] = [rti].[TagId]
                INNER JOIN [DbResourceAssociationInfo] AS [ra] ON [rti].[Id] = [ra].[TagInstanceId]
                INNER JOIN [DbResourceInfo] AS [r] ON [ra].[ResourceId] = [r].[Id]
                INNER JOIN [DbResourceContentPortionInfo] AS [rcp] ON [r].[Id] = [rcp].[ResourceId]
                LEFT OUTER JOIN (SELECT 1 AS [test], * FROM [DbUserSubscriptionContentPortionInfo]) AS [scp] 
                        ON ([rcp].[Id] = [scp].[ResourceContentPortionId]) AND ([s].[Id] = [scp].[UserSubscriptionId])
                WHERE ([s].[IsActive] = 1) AND ([s].[NextPortionTime] < @p0) 
                  AND (([scp].[test] IS NULL) OR ([scp].[IsMarkedToRepeat] = 1) OR (NOT ([scp].[IsDelivered] = 1)))
                  AND (([r].[IsValidated] = 1) OR ([r].[AuthorUserId] = [s].[UserId])) 
            ) AS [result] WHERE [PartDeliveryOrder] = 1

             */

            return subscriptions.Select(x => new DeliveryContentPart(x.t, x.s, x.r, x.rcp, x.scp, x.u));
        }
    }

    class SubscriptionContentPart
    {
        public readonly DbResourceContentPortionInfo resourceContentPart;
        public readonly DbUserSubscriptionContentPortionInfo subscriptionContentPart;

        public SubscriptionContentPart(DbResourceContentPortionInfo rcp, DbUserSubscriptionContentPortionInfo scp)
        {
            this.resourceContentPart = rcp;
            this.subscriptionContentPart = scp;
        }
    }

    class SubscriptionResourceContentPart : SubscriptionContentPart
    {
        public readonly DbUserSubscriptionInfo userSubscription;

        public SubscriptionResourceContentPart(DbUserSubscriptionInfo s, DbResourceContentPortionInfo rcp, DbUserSubscriptionContentPortionInfo scp)
            : base(rcp, scp)
        {
            this.userSubscription = s;
        }
    }

    class DeliveryContentPart : SubscriptionResourceContentPart
    {
        public readonly DbTopicInfo topic;
        public readonly DbResourceInfo resource;
        public readonly DbUserInfo user;

        public DeliveryContentPart(DbTopicInfo t, DbUserSubscriptionInfo s, DbResourceInfo r, DbResourceContentPortionInfo rcp, DbUserSubscriptionContentPortionInfo scp, DbUserInfo u)
            : base(s, rcp, scp)
        {
            this.topic = t;
            this.resource = r;
            this.user = u;
        }
    }
}
