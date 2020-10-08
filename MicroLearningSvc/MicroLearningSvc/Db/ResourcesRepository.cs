using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroLearningSvc.Db
{
    class ResourcesRepository : RepositoryImpl, IResourcesRepository
    {
        public ResourcesRepository(DbContext db) : base(db) { }

        public void AddResource(DbResourceInfo res, IList<DbResourceContentPortionInfo> contentParts)
        {
            _db.Resources.InsertOnSubmit(res);
            _db.SubmitChanges();

            foreach (var c in contentParts)
            {
                c.ResourceId = res.Id;
                _db.ResourceContentPortions.InsertOnSubmit(c);
            }
            _db.SubmitChanges();
        }

        public DbResourceInfo FindResourceById(long id)
        {
            return _db.Resources.FirstOrDefault(r => r.Id == id);
        }

        public DbResourceInfo FindResourceByUrl(string url)
        {
            return _db.Resources.FirstOrDefault(r => r.Url == url);
        }

        public DbResourcesFilterResullt FindResourcesByKeywords(string[] words, int skip, int take)
        {
            var knownTagIds = _db.TagInstances.Where(t => words.Contains(t.Word)).Select(t => t.TagId).ToList();

            var ress = _db.Resources.Join(_db.ResourceAssociations, r => r.Id, a => a.ResourceId, (r, a) => new { r, a })
                                    .Join(_db.TagInstances, a => a.a.TagInstanceId, i => i.Id, (a, i) => new { a.r, i })
                                    .Where(r => knownTagIds.Contains(r.i.TagId))
                                    .Select(r => r.r);

            return this.FindResourceImpl(ress, skip, take);
        }

        public DbResourcesFilterResullt FindResourceByTopic(long topicId, int skip, int take)
        {
            var ress = _db.Resources.Join(_db.ResourceAssociations, r => r.Id, a => a.ResourceId, (r, a) => new { r, a })
            .Join(_db.TagInstances, a => a.a.TagInstanceId, i => i.Id, (a, i) => new { a.r, i })
            .Join(_db.TagInstances, a => a.i.TagId, i2 => i2.TagId, (a, i2) => new { a.r, i2 })
            .Join(_db.TopicAssociations, a => a.i2.Id, a2 => a2.TagInstanceId, (a, a2) => new { a.r, a2 })
            .Where(r => r.a2.TopicId == topicId)
            .Select(r => r.r);

            return this.FindResourceImpl(ress, skip, take);
        }

        public DbResourcesFilterResullt FindResources(int skip, int take)
        {
            return this.FindResourceImpl(_db.Resources, skip, take);
        }

        private DbResourcesFilterResullt FindResourceImpl(IQueryable<DbResourceInfo> filteredResources, int skip, int take)
        {

            var count = filteredResources.Count();

            var resources = filteredResources.OrderBy(t => t.CreationStamp)
                            .DistinctBy(t => t.Id)
                            .Skip(skip).Take(take)
                            .Select(r => Tuple.Create(r, _db.ResourceAssociations.Where(a => a.ResourceId == r.Id)
                                                            .Join(_db.TagInstances, a => a.TagInstanceId, i => i.Id, (a, i) => new { a, i })
                                                            .Count()));

            var tags = filteredResources.OrderBy(t => t.CreationStamp)
                          .DistinctBy(t => t.Id)
                          .Skip(skip).Take(take)
                          .Join(_db.ResourceAssociations, t => t.Id, a => a.ResourceId, (t, a) => new { t, a })
                          .Join(_db.TagInstances, a => a.a.TagInstanceId, i => i.Id, (a, i) => new { a.t, i })
                          .Select(t => new DbResourceWrappedTag(t.t.Id, t.i));

            return new DbResourcesFilterResullt(count, resources, tags);
        }

        public IList<DbTagInstanceInfo> GetTagsByResourceId(long resId)
        {
            return _db.ResourceAssociations.Where(a => a.ResourceId == resId)
                      .Join(_db.TagInstances, a => a.TagInstanceId, t => t.Id, (a, t) => t)
                      .ToList();
        }

        public List<DbTagInstanceInfo> IntroduceTags(string[] tags)
        {
            var knownTags = _db.TagInstances.Where(t => tags.Contains(t.Word)).ToList();
            var newTagWords = tags.Where(w => knownTags.All(t => t.Word != w)).ToList();

            if (newTagWords.Count > 0)
            {
                var genTags = newTagWords.Select(w => new DbTagInfo()).ToList();
                genTags.ForEach(t => _db.Tags.InsertOnSubmit(t));
                _db.SubmitChanges();

                var newTags = newTagWords.Select((w, n) => new DbTagInstanceInfo() { Word = w, TagId = genTags[n].Id }).ToList();
                newTags.ForEach(t => _db.TagInstances.InsertOnSubmit(t));
                _db.SubmitChanges();

                return knownTags.Concat(newTags).ToList();
            }
            else
            {
                return knownTags;
            }
        }

        public void AssociateResource(long resId, IList<DbTagInstanceInfo> tags)
        {
            foreach (var t in tags)
                _db.ResourceAssociations.InsertOnSubmit(new DbResourceAssociationInfo() { ResourceId = resId, TagInstanceId = t.Id });
        }
    }

    class DbResourcesFilterResullt
    {
        public int TotalResourcesCount { get; }
        public IEnumerable<Tuple<DbResourceInfo, int>> Resources { get; }
        public IEnumerable<DbResourceWrappedTag> Tags { get; }

        public DbResourcesFilterResullt(int totalResourcesCount, IEnumerable<Tuple<DbResourceInfo, int>> resources, IEnumerable<DbResourceWrappedTag> tags)
        {
            this.TotalResourcesCount = totalResourcesCount;
            this.Resources = resources;
            this.Tags = tags;
        }
    }

    class DbResourceWrappedTag
    {
        public long ResourceId { get; }
        public DbTagInstanceInfo Tag { get; }

        public DbResourceWrappedTag(long resourceId, DbTagInstanceInfo tag)
        {
            this.ResourceId = resourceId;
            this.Tag = tag;
        }
    }

}
