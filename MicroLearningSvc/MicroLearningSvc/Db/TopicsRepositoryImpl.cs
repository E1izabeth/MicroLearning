using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MicroLearningSvc.Db
{
    class TopicsRepositoryImpl : RepositoryImpl, ITopicsRepository
    {
        public TopicsRepositoryImpl(DbContext db) : base(db) { }

        public DbTagInstanceInfo[] GetTopicTags(long topicId)
        {
            return _db.TopicAssociations.Where(a => a.TopicId == topicId)
                              .Join(_db.TagInstances, a => a.TagInstanceId, i => i.Id, (a, i) => i)
                              .ToArray();
        }

        public DbTopicInfo FindTopicById(long topicId)
        {
            return _db.Topics.FirstOrDefault(t => t.Id == topicId);
        }

        public TopicInfo FindTopicInfoById(long topicId)
        {
            return this.GetTopicsImpl(0, 1, t => t.Id == topicId).FirstOrDefault();
        }

        public int GetAuthorTopicsCount(long userId)
        {
            return _db.Topics.Where(t => t.AuthorUserId == userId).Count();
        }

        public IEnumerable<TopicInfo> GetAuthorTopics(long userId, int skip, int take)
        {
            return this.GetTopicsImpl(skip, take, t => t.AuthorUserId == userId);
        }

        private IEnumerable<TopicInfo> GetTopicsImpl(int skip, int take, Expression<Func<DbTopicInfo,bool>> topicCond)
        {
            var topics = _db.Topics.Where(topicCond)
                            .GroupJoin(_db.UserSubscriptions, t => t.Id, s => s.TopicId, (t, s) => new { t, s = s.FirstOrDefault() })
                            .OrderBy(t => t.t.CreationStamp)
                            .Skip(skip).Take(take)
                            .Select(t => new TopicInfo(
                                t.t,
                                _db.UserSubscriptions.Where(s => s.TopicId == t.t.Id)
                                  .Join(_db.UserSubscriptionContentPortions, s => s.Id, scp => scp.UserSubscriptionId, (s, scp) => scp)
                                  .Where(scp => scp.IsLearned)
                                  .LongCount(),
                                (from rcp in _db.ResourceContentPortions
                                 join ra in _db.ResourceAssociations on rcp.ResourceId equals ra.ResourceId
                                 join rti in _db.TagInstances on ra.TagInstanceId equals rti.Id
                                 join tti in _db.TagInstances on rti.TagId equals tti.TagId
                                 join ta in _db.TopicAssociations on tti.Id equals ta.TagInstanceId
                                 where ta.TopicId == t.t.Id
                                 select rcp).Distinct().LongCount(),
                                _db.TopicAssociations.Where(a => a.TopicId == t.t.Id)
                                                     .Join(_db.TagInstances, a => a.TagInstanceId, i => i.Id, (a, i) => new { a, i })
                                                     .Count(),
                                t.s
                            ));
            return topics;
        }

        public IEnumerable<TopicAssociatedTagInfo> GetAuthorTopicTags(long userId, int skip, int take)
        {
            var tags = _db.Topics.Where(t => t.AuthorUserId == userId)
                          .OrderBy(t => t.CreationStamp)
                          .Skip(skip).Take(take)
                          .Join(_db.TopicAssociations, t => t.Id, a => a.TopicId, (t, a) => new { t, a })
                          .Join(_db.TagInstances, a => a.a.TagInstanceId, i => i.Id, (a, i) => new { a.t, i })
                          .Select(t => new TopicAssociatedTagInfo(t.i, t.t.Id));

            return tags;
        }

        public void AddTopic(DbTopicInfo topic)
        {
            _db.Topics.InsertOnSubmit(topic);
        }

        public void DeleteTopic(DbTopicInfo topic)
        {
            var topicId = topic.Id;

            _db.Topics.DeleteOnSubmit(topic);
            _db.SubmitChanges();

            foreach (var scp in _db.UserSubscriptions.Where(s => s.TopicId == topicId)
                                   .Join(_db.UserSubscriptionContentPortions, s => s.Id, scp => scp.UserSubscriptionId, (s, scp) => scp))
                _db.UserSubscriptionContentPortions.DeleteOnSubmit(scp);

            _db.SubmitChanges();

            foreach (var topicAssociation in _db.TopicAssociations.Where(ta => ta.TopicId == topicId))
                _db.TopicAssociations.DeleteOnSubmit(topicAssociation);

            _db.SubmitChanges();

            foreach (var subscription in _db.UserSubscriptions.Where(s => s.TopicId == topicId))
                _db.UserSubscriptions.DeleteOnSubmit(subscription);

            _db.SubmitChanges();
        }

        public void AssociateTopic(long topicId, IList<DbTagInstanceInfo> tags)
        {
            foreach (var t in tags)
                _db.TopicAssociations.InsertOnSubmit(new DbTopicAssociationInfo() { TopicId = topicId, TagInstanceId = t.Id });
        }
    }

    class TopicAssociatedTagInfo
    {
        public DbTagInstanceInfo TagInstance { get; private set; }
        public long TopicId { get; private set; }

        public TopicAssociatedTagInfo(DbTagInstanceInfo tag, long topicId)
        {
            this.TagInstance = tag;
            this.TopicId = topicId;
        }
    }

    class TopicInfo
    {
        public DbTopicInfo Info { get; private set; }
        public long LearnedPartsCount { get; private set; }
        public long TotalPartsCount { get; private set; }
        public int TagsCount { get; private set; }
        public DbUserSubscriptionInfo Subscription { get; private set; }

        public TopicInfo(DbTopicInfo topic, long learnedPartsCount, long totalPartsCount, int tagsCount, DbUserSubscriptionInfo subscription)
        {
            this.Info = topic;
            this.LearnedPartsCount = learnedPartsCount;
            this.TotalPartsCount = totalPartsCount;
            this.TagsCount = tagsCount;
            this.Subscription = subscription;
        }
    }
}
