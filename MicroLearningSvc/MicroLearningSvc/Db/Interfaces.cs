using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroLearningSvc.Db
{
    interface ILearningDbContext
    {
        IUsersRepository Users { get; }         
        IResourcesRepository Resources{ get; }         
        ITopicsRepository Topics { get; }
        ISubscriptionsRepository Subscriptions { get; }

        IDbContext Raw { get; }

        void SubmitChanges();
    }

    interface IUsersRepository
    {
        void AddUser(DbUserInfo user);
        DbUserInfo GetUserById(long userId);
        DbUserInfo FindUserByLoginKey(string loginKey);
        DbUserInfo FindUserByTokenKey(string key);
    }

    interface IResourcesRepository
    {
        DbResourcesFilterResullt FindResources(int skip, int take);
        DbResourcesFilterResullt FindResourceByTopic(long topicId, int skip, int take);
        DbResourcesFilterResullt FindResourcesByKeywords(string[] words, int skip, int take);

        DbResourceInfo FindResourceByUrl(string url);
        void AddResource(DbResourceInfo res, IList<DbResourceContentPortionInfo> parts);
        List<DbTagInstanceInfo> IntroduceTags(string[] tags);
        DbResourceInfo FindResourceById(long id);
        IList<DbTagInstanceInfo> GetTagsByResourceId(long id);
        void AssociateResource(long id, IList<DbTagInstanceInfo> tags);
    }

    interface ITopicsRepository
    {
        TopicInfo FindTopicInfoById(long topicId);
        DbTopicInfo FindTopicById(long topicId);
        DbTagInstanceInfo[] GetTopicTags(long topicId);
        int GetAuthorTopicsCount(long userId);
        IEnumerable<TopicInfo> GetAuthorTopics(long userId, int skip, int take);
        IEnumerable<TopicAssociatedTagInfo> GetAuthorTopicTags(long userId, int skip, int take);
        void AddTopic(DbTopicInfo topic);
        void DeleteTopic(DbTopicInfo topicId);
        void AssociateTopic(long id, IList<DbTagInstanceInfo> tags);
    }

    interface ISubscriptionsRepository
    {
        void DeactivateSubscriptionsByUser  (long userId);
        DbUserSubscriptionInfo FindSubscriptionByTopic(long topicId);
        void ResetLearningProgress(long subscriptionId);
        void Add(DbUserSubscriptionInfo subscription);
        (int, IQueryable<SubscriptionContentPart>) ContentPartsByTopic(long topicId, long userId, int skip, int take);
        IList<SubscriptionResourceContentPart> ContentPartByTopic(long userId, long topicId, long partId);
        void RegisterPart(DbUserSubscriptionContentPortionInfo contentPortion);
        
        DbUserSubscriptionInfo DeliveryFindNearestSubscription();
        void DeliveryUpgradeSubscriptions(TimeSpan delta);
        IEnumerable<DeliveryContentPart> DeliveryGetContentToDeliver(DateTime nearestDeliveryThreshold);
    }

}
