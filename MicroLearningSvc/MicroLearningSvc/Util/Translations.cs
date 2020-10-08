using MicroLearningSvc.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroLearningSvc.Util
{
    static class Translations
    {
        public static TopicInfoType TranslateTopic(this TopicInfo topic, IList<DbTagInstanceInfo> tags)
        {
            return new TopicInfoType()
            {
                Id = topic.Info.Id,
                Title = topic.Info.Title,
                AssociationTags = tags.Select(t => t.TranslateTagInfo()).ToArray(),
                LearnedContentParts = topic.LearnedPartsCount,
                TotalContentParts = topic.TotalPartsCount,
                IsActive = topic.Subscription?.IsActive ?? false,
                DeliveryIntervalSeconds = topic.Subscription == null ? 0 : (long)topic.Subscription.Interval.TotalSeconds,
                DeliveryIntervalSecondsSpecified = topic.Subscription != null,
                LearnedContentPartsSpecified = true,
                TotalContentPartsSpecified = true
            };
        }

        public static AssociationTagInfoType TranslateTagInfo(this DbTagInstanceInfo tag)
        {
            return new AssociationTagInfoType() { Word = tag.Word, Id = tag.TagId, IdSpecified = true };
        }

        public static TopicsListType TranslateTopics<TTag, TKey>(this IEnumerable<TopicInfo> topics, int topicsCount, IEnumerable<TTag> wrappedTags, Func<TTag, TKey> keyGetter, Func<TTag, DbTagInstanceInfo> tagUnwrapper)
            where TKey : IEquatable<TKey>
        {
            return new TopicsListType()
            {
                Items = topics.FillSubitems(
                    wrappedTags, 
                    t => new TopicInfoType() { 
                        Id = t.Info.Id, 
                        Title = t.Info.Title, 
                        IsActive = t.Subscription?.IsActive ?? false,
                        DeliveryIntervalSeconds = t.Subscription == null ? 0 : (long)t.Subscription.Interval.TotalSeconds,
                        DeliveryIntervalSecondsSpecified = t.Subscription != null,
                        LearnedContentParts = t.LearnedPartsCount, 
                        TotalContentParts = t.TotalPartsCount,
                        LearnedContentPartsSpecified = true,
                        TotalContentPartsSpecified = true
                    },
                    t => t.TagsCount,
                    (t, tags) => t.AssociationTags = tags.Select(a => tagUnwrapper(a).TranslateTagInfo()).ToArray()).ToArray(),
                Count = topicsCount
            };
        }

        public static ResourcesListType TranslateResources<TTag, TKey>(this IEnumerable<Tuple<DbResourceInfo, int>> topics, long currUserId, int topicsCount, IEnumerable<TTag> wrappedTags, Func<TTag, TKey> keyGetter, Func<TTag, DbTagInstanceInfo> tagUnwrapper)
            where TKey : IEquatable<TKey>
        {
            return new ResourcesListType()
            {
                Items = topics.FillSubitems(wrappedTags, r => new ResourceInfoType() { Id = r.Item1.Id, Title = r.Item1.Title, Url = r.Item1.Url, IsValidated = r.Item1.IsValidated, IsMyResource = r.Item1.AuthorUserId == currUserId }, r => r.Item2,
                                            (t, tags) => t.AssociationTags = tags.Select(a => tagUnwrapper(a).TranslateTagInfo()).ToArray()).ToArray(),
                Count = topicsCount
            };
        }

        //public static IEnumerable<TOut> FillSubitems<TOut, TKey, TSubitem>(
        //                                            this IEnumerable<TOut> seq, IEnumerable<TSubitem> subSeq,
        //                                            Func<TSubitem, TKey> keyGetter, Action<TOut, TSubitem[]> setter)
        //    where TKey : IEquatable<TKey>
        //{
        //    var items = seq.GetEnumerator();
        //    var subitems = subSeq.GetEnumerator();
        //    try
        //    {
        //        if (items.MoveNext())
        //        {
        //            if (subitems.MoveNext())
        //            {
        //                var subitem = subitems.Current;
        //                var key = keyGetter(subitem);

        //                var subitemsToSet = new List<TSubitem>();
        //                subitemsToSet.Add(subitem);
        //                while (subitems.MoveNext())
        //                {
        //                    subitem = subitems.Current;
        //                    var newKey = keyGetter(subitem);
        //                    if (!newKey.Equals(key))
        //                    {
        //                        setter(items.Current, subitemsToSet.ToArray());
        //                        subitemsToSet.Clear();
        //                        yield return items.Current;
        //                        if (!items.MoveNext())
        //                            yield break;
        //                    }
        //                    subitemsToSet.Add(subitem);
        //                }

        //            }

        //            do { yield return items.Current; }
        //            while (items.MoveNext());
        //        }
        //    }
        //    finally
        //    {
        //        subitems.SafeDispose();
        //        items.SafeDispose();
        //    }
        //}

        public static ResourceInfoType TranslateResource(this DbResourceInfo res, long currUserId, IList<DbTagInstanceInfo> tags)
        {
            return new ResourceInfoType()
            {
                Id = res.Id,
                Title = res.Title,
                Url = res.Url,
                IsValidated = res.IsValidated,
                IsMyResource = res.AuthorUserId == currUserId,
                AssociationTags = tags.Select(t => t.TranslateTagInfo()).ToArray()
            };
        }



        public static IEnumerable<TOut> FillSubitems<TIn, TOut, TSubitem>(this IEnumerable<TIn> seq, IEnumerable<TSubitem> subSeq,
                                            Func<TIn, TOut> map, Func<TIn, int> countGetter, Action<TOut, TSubitem[]> setter)
        {
            var subitems = subSeq.GetEnumerator();
            try
            {
                foreach (var item in seq)
                {
                    var result = map(item);

                    var subitemsCount = countGetter(item);
                    var subitemsToSet = new TSubitem[subitemsCount];
                    for (int i = 0; i < subitemsCount && subitems.MoveNext(); i++)
                        subitemsToSet[i] = subitems.Current;

                    setter(result, subitemsToSet);

                    yield return result;
                }
            }
            finally
            {
                subitems.SafeDispose();
            }
        }
    }
}
