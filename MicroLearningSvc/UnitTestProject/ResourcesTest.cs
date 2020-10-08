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
    public class ResourcesTest : TestBase
    {
        [TestMethod]
        public void SuggestResourceTagsTest()
        {
            var inSpec = new CreateResourceSpecType()
            {
                ResourceUrl = "https://habr.com/ru/company/ua-hosting/blog/342826/",
                ResourceTitle = "Linuxes",
                AssociationTags = new[] { new AssociationTagInfoType() { Word = "tag" } }
            };

            using (var ctx = TestContext.CreateInstance())
            {
                ctx.RequestContext.Setup(x => x.ValidateAuthorized());
                ctx.ServiceContext.Setup(x => x.DownloadContent(inSpec.ResourceUrl)).Returns(Encoding.UTF8.GetBytes(Properties.Resources.Linuxes));

                var testController = new LearningServiceImpl(ctx.ServiceContext.Object);

                var outSpec = testController.SuggestResourceTags(inSpec);

                Assert.IsNotNull(outSpec.AssociationTags);

            }
        }

        [TestMethod]
        public void CreateResourceTest()
        {
            var resUrl = "https://en.wikipedia.org/wiki/linux";
            var resTitle = "Linuxes";

            var smartReader = new SmartReader.Reader(resUrl, Properties.Resources.Linux);
            var article = smartReader.GetArticle();

            DbResourceInfo newRes = null;
            IList<DbResourceContentPortionInfo> newPortions = null;

            var dbTags = new List<DbTagInstanceInfo>() { new DbTagInstanceInfo() { Id = 25, TagId = 2, Word = "tag" } };

            using (var ctx = TestContext.CreateInstance())
            {
                ctx.DbContext.Setup(x => x.SubmitChanges());
                ctx.WordNormalizer.Setup(x => x.NormalizeWord("tag")).Returns("tag");
                ctx.RequestContext.Setup(x => x.ValidateAuthorized());
                ctx.SessionContext.Setup(x => x.UserId).Returns(testUserId);
                ctx.ServiceContext.Setup(x => x.ParseResourceArticle(resUrl)).Returns(article);
                ctx.DbResourcesRepo.Setup(x => x.FindResourceByUrl(resUrl)).Returns<DbResourceInfo>(null);
                ctx.DbResourcesRepo.Setup(x => x.IntroduceTags(It.IsNotNull<string[]>())).Returns(dbTags);
                ctx.DbResourcesRepo.Setup(x => x.AssociateResource(It.IsAny<long>(), It.IsNotNull<IList<DbTagInstanceInfo>>()));
                ctx.DbResourcesRepo.Setup(x => x.AddResource(It.IsNotNull<DbResourceInfo>(), It.IsNotNull<IList<DbResourceContentPortionInfo>>()))
                                   .Callback((DbResourceInfo res, IList<DbResourceContentPortionInfo> portions) => { newRes = res; newPortions = portions; });

                var testController = new LearningServiceImpl(ctx.ServiceContext.Object);

                var result = testController.CreateResource(new CreateResourceSpecType()
                {
                    ResourceUrl = resUrl,
                    ResourceTitle = resTitle,
                    AssociationTags = new[] { new AssociationTagInfoType() { Word = "tag" } }
                });

                Assert.AreEqual(result.Title, resTitle);
                Assert.AreEqual(result.IsMyResource, true);
                Assert.AreEqual(result.IsValidated, false);
                Assert.AreEqual(newRes.Title, resTitle);
                Assert.AreEqual(newRes.AuthorUserId, testUserId);
                Assert.AreEqual(newRes.IsValidated, false);
                Assert.IsTrue(newPortions.Count > 0);
            }
        }

        [TestMethod]
        public void CreateSmallResourceTest()
        {
            var resUrl = "https://en.wikipedia.org/wiki/linux";
            var resTitle = "Linuxes";

            var smartReader = new SmartReader.Reader(resUrl, Properties.Resources.Small);
            var article = smartReader.GetArticle();

            DbResourceInfo newRes = null;
            IList<DbResourceContentPortionInfo> newPortions = null;

            var dbTags = new List<DbTagInstanceInfo>() { new DbTagInstanceInfo() { Id = 25, TagId = 2, Word = "tag" } };

            using (var ctx = TestContext.CreateInstance())
            {
                ctx.DbContext.Setup(x => x.SubmitChanges());
                ctx.WordNormalizer.Setup(x => x.NormalizeWord("tag")).Returns("tag");
                ctx.RequestContext.Setup(x => x.ValidateAuthorized());
                ctx.SessionContext.Setup(x => x.UserId).Returns(testUserId);
                ctx.ServiceContext.Setup(x => x.ParseResourceArticle(resUrl)).Returns(article);
                ctx.DbResourcesRepo.Setup(x => x.FindResourceByUrl(resUrl)).Returns<DbResourceInfo>(null);
                ctx.DbResourcesRepo.Setup(x => x.IntroduceTags(It.IsNotNull<string[]>())).Returns(dbTags);
                ctx.DbResourcesRepo.Setup(x => x.AssociateResource(It.IsAny<long>(), It.IsNotNull<IList<DbTagInstanceInfo>>()));
                ctx.DbResourcesRepo.Setup(x => x.AddResource(It.IsNotNull<DbResourceInfo>(), It.IsNotNull<IList<DbResourceContentPortionInfo>>()))
                                   .Callback((DbResourceInfo res, IList<DbResourceContentPortionInfo> portions) => { newRes = res; newPortions = portions; });

                var testController = new LearningServiceImpl(ctx.ServiceContext.Object);

                var result = testController.CreateResource(new CreateResourceSpecType()
                {
                    ResourceUrl = resUrl,
                    ResourceTitle = resTitle,
                    AssociationTags = new[] { new AssociationTagInfoType() { Word = "tag" } }
                });

                Assert.AreEqual(result.Title, resTitle);
                Assert.AreEqual(result.IsMyResource, true);
                Assert.AreEqual(result.IsValidated, false);
                Assert.AreEqual(newRes.Title, resTitle);
                Assert.AreEqual(newRes.AuthorUserId, testUserId);
                Assert.AreEqual(newRes.IsValidated, false);
                Assert.IsTrue(newPortions.Count > 0);
            }
        }

        [TestMethod]
        public void CreateResourceWithoutTagsTest()
        {
            var resUrl = "https://en.wikipedia.org/wiki/linux";
            var resTitle = "Linuxes";

            using (var ctx = TestContext.CreateInstance())
            {
                ctx.RequestContext.Setup(x => x.ValidateAuthorized());

                var testController = new LearningServiceImpl(ctx.ServiceContext.Object);
                var ex = Assert.ThrowsException<WebFaultException>(() => testController.CreateResource(new CreateResourceSpecType()
                {
                    ResourceUrl = resUrl,
                    ResourceTitle = resTitle
                }));
                Assert.IsTrue(ex.StatusCode == HttpStatusCode.BadRequest);
            }
        }

        [TestMethod]
        public void CreateExistingResourceTest()
        {
            var resUrl = "https://en.wikipedia.org/wiki/linux";
            var resTitle = "Linuxes";
            var exRes = new DbResourceInfo() { Url = resUrl };

            using (var ctx = TestContext.CreateInstance())
            {
                ctx.RequestContext.Setup(x => x.ValidateAuthorized());
                ctx.DbResourcesRepo.Setup(x => x.FindResourceByUrl(resUrl)).Returns(exRes);

                var testController = new LearningServiceImpl(ctx.ServiceContext.Object);
                var ex = Assert.ThrowsException<ApplicationException>(() => testController.CreateResource(new CreateResourceSpecType()
                {
                    ResourceUrl = resUrl,
                    ResourceTitle = resTitle,
                    AssociationTags = new[] { new AssociationTagInfoType() { Word = "tag" } }
                }));
                Assert.IsTrue(ex.Message.Contains("Resource already exists"));
            }
        }

        [TestMethod]
        public void GetResourceTest()
        {
            var (res, tags) = this.MakeTestResource();

            using (var ctx = TestContext.CreateInstance())
            {
                ctx.RequestContext.Setup(x => x.ValidateAuthorized());
                ctx.SessionContext.Setup(x => x.UserId).Returns(res.AuthorUserId);
                ctx.DbResourcesRepo.Setup(x => x.GetTagsByResourceId(res.Id)).Returns(tags);
                ctx.DbResourcesRepo.Setup(x => x.FindResourceById(res.Id)).Returns(res);
                var testController = new LearningServiceImpl(ctx.ServiceContext.Object);

                var result = testController.GetResource(res.Id.ToString());

                Assert.AreEqual(res.Id, result.Id);
                Assert.IsTrue(result.AssociationTags.Any(t => t.Id == tags[0].Id));
            }
        }

        [TestMethod]
        public void GetResourceNullTest()
        {
            var (res, tags) = this.MakeTestResource();

            using (var ctx = TestContext.CreateInstance())
            {
                ctx.RequestContext.Setup(x => x.ValidateAuthorized());
                ctx.DbResourcesRepo.Setup(x => x.FindResourceById(res.Id)).Returns<DbResourceInfo>(null);
                var testController = new LearningServiceImpl(ctx.ServiceContext.Object);

                var ex = Assert.ThrowsException<WebFaultException>(() => testController.GetResource(res.Id.ToString()));
                Assert.IsTrue(ex.StatusCode == HttpStatusCode.NotFound);
            }
        }

        [TestMethod]
        public void FilterResourceRangeTest()
        {
            var skipCount = 1;
            var takeCount = 2;
            var filterSpec = new ResourceFilterSpecType() { Item = null };

            var resources = this.MakeTestResources();
            var resultCollection = resources.Skip(skipCount).Take(takeCount).ToArray();

            var resResults = new DbResourcesFilterResullt(takeCount, resultCollection, new DbResourceWrappedTag[0]);

            using (var ctx = TestContext.CreateInstance())
            {
                ctx.RequestContext.Setup(x => x.ValidateAuthorized());
                ctx.SessionContext.Setup(x => x.UserId).Returns(testUserId);
                ctx.DbContext.Setup(x => x.Resources.FindResources(skipCount, takeCount)).Returns(resResults);

                var testController = new LearningServiceImpl(ctx.ServiceContext.Object);

                var resList = testController.FilterResourcesRange(skipCount.ToString(), takeCount.ToString(), filterSpec);

                CollectionAssert.AreEqual(resList.Items, resultCollection, MyComparer.EquilityFor<ResourceInfoType, DbResourceInfo>((a, b) => a.Id == b.Id));
            }
        }


        [TestMethod]
        public void FilterByTopicResourceRangeTest()
        {
            var skipStr = 1;
            var takeStr = 2;
            var filterTopicId = 2;
            var filterSpec = new ResourceFilterSpecType() { Item = new ResourceFilterByTopicSpec() { TopicId = filterTopicId } };

            var (resResults, tagInst1) = this.MakeFilteredResourcesResult();

            using (var ctx = TestContext.CreateInstance())
            {
                ctx.RequestContext.Setup(x => x.ValidateAuthorized());
                ctx.SessionContext.Setup(x => x.UserId).Returns(testUserId);
                ctx.DbContext.Setup(x => x.Resources.FindResourceByTopic(filterTopicId, skipStr, takeStr)).Returns(resResults);

                var testController = new LearningServiceImpl(ctx.ServiceContext.Object);

                var resList = testController.FilterResourcesRange(skipStr.ToString(), takeStr.ToString(), filterSpec);


                CollectionAssert.AreEqual(resList.Items, resResults.Resources.ToList(), MyComparer.EquilityFor<ResourceInfoType, DbResourceInfo>((a, b) => a.Id == b.Id));
            }
        }

        [TestMethod]
        public void FilterByKeywordsRangeTest()
        {
            var tag1 = this.MakeTestTag1();
            var (resResults, tagInst1) = this.MakeFilteredResourcesResult();

            var skipCount = 1;
            var takeCount = 2;

            var filterSpec = new ResourceFilterSpecType()
            {
                Item = new ResourceFilterByKeywordsSpec()
                {
                    AssociationTags = new[] { new AssociationTagInfoType() { Id = tag1.Id, Word = tagInst1.Word } }
                }
            };

            using (var ctx = TestContext.CreateInstance())
            {
                ctx.RequestContext.Setup(x => x.ValidateAuthorized());
                ctx.SessionContext.Setup(x => x.UserId).Returns(testUserId);
                ctx.DbContext.Setup(x => x.Resources.FindResourcesByKeywords(new string[] { "linux" }, skipCount, takeCount)).Returns(resResults);
                ctx.WordNormalizer.Setup(x => x.NormalizeWord("linux")).Returns("linux");

                var testController = new LearningServiceImpl(ctx.ServiceContext.Object);

                var resList = testController.FilterResourcesRange(skipCount.ToString(), takeCount.ToString(), filterSpec);

                CollectionAssert.AreEqual(resList.Items, resResults.Resources.ToList(), MyComparer.EquilityFor<ResourceInfoType, DbResourceInfo>((a, b) => a.Id == b.Id));
            }
        }


        [TestMethod]
        public void GetResourcesTest()
        {
            var skipCount = 0;
            var takeCount = 10;
            
            var filterSpec = new ResourceFilterSpecType() { Item = null };

            var (resResults, tagInst1) = this.MakeFilteredResourcesResult();

            using (var ctx = TestContext.CreateInstance())
            {
                ctx.RequestContext.Setup(x => x.ValidateAuthorized());
                ctx.SessionContext.Setup(x => x.UserId).Returns(testUserId);
                ctx.DbContext.Setup(x => x.Resources.FindResources(skipCount, takeCount)).Returns(resResults);

                var testController = new LearningServiceImpl(ctx.ServiceContext.Object);

                var resList = testController.GetResources();

                CollectionAssert.AreEqual(resList.Items, resResults.Resources.ToArray(), MyComparer.EquilityFor<ResourceInfoType, DbResourceInfo>((a, b) => a.Id == b.Id));
            }
        }

        [TestMethod]
        public void GetResourcesRangeTest()
        {
            var skipCount = 0;
            var takeCount = 10;
            var filterSpec = new ResourceFilterSpecType() { Item = null };

            var (resResults, tagInst1) = this.MakeFilteredResourcesResult();

            using (var ctx = TestContext.CreateInstance())
            {
                ctx.RequestContext.Setup(x => x.ValidateAuthorized());
                ctx.SessionContext.Setup(x => x.UserId).Returns(testUserId);
                ctx.DbContext.Setup(x => x.Resources.FindResources(skipCount, takeCount)).Returns(resResults);

                var testController = new LearningServiceImpl(ctx.ServiceContext.Object);

                var resList = testController.GetResourcesRange(skipCount.ToString(), takeCount.ToString());

                CollectionAssert.AreEqual(resList.Items, resResults.Resources.ToArray(), MyComparer.EquilityFor<ResourceInfoType, DbResourceInfo>((b, a) => a.Id == b.Id));
            }
        }

        [TestMethod]
        public void ValidateResourceTest()
        {
            var res = this.MakeTestResource().Item1;
            res.IsValidated = false;

            using (var ctx = TestContext.CreateInstance())
            {
                ctx.DbResourcesRepo.Setup(x => x.FindResourceById(res.Id)).Returns(res);
                ctx.RequestContext.Setup(x => x.ValidateAuthorized());
                ctx.SessionContext.Setup(x => x.UserId).Returns(testUserId);
                ctx.DbContext.Setup(x => x.SubmitChanges());

                var testController = new LearningServiceImpl(ctx.ServiceContext.Object);

                testController.ValidateResource(res.Id.ToString());

                Assert.IsTrue(res.IsValidated);
            }
        }

        [TestMethod]
        public void ValidateResourceAlreadyValidatedTest()
        {
            var res = this.MakeTestResource().Item1;
            res.IsValidated = true;

            using (var ctx = TestContext.CreateInstance())
            {
                ctx.DbResourcesRepo.Setup(x => x.FindResourceById(res.Id)).Returns(res);
                ctx.RequestContext.Setup(x => x.ValidateAuthorized());
                ctx.SessionContext.Setup(x => x.UserId).Returns(testUserId);

                var testController = new LearningServiceImpl(ctx.ServiceContext.Object);

                var ex = Assert.ThrowsException<ApplicationException>(() => testController.ValidateResource(res.Id.ToString()));

                Assert.AreEqual(ex.Message, "Already validated");
            }
        }

        [TestMethod]
        public void ValidateResourceNullTest()
        {
            DbResourceInfo res = null;
            var resId = 42;

            using (var ctx = TestContext.CreateInstance())
            {
                ctx.RequestContext.Setup(x => x.ValidateAuthorized());
                ctx.DbResourcesRepo.Setup(x => x.FindResourceById(resId)).Returns(res);

                var testController = new LearningServiceImpl(ctx.ServiceContext.Object);

                var ex = Assert.ThrowsException<WebFaultException>(() => testController.ValidateResource(resId.ToString()));

                Assert.IsTrue(ex.StatusCode == HttpStatusCode.NotFound);
            }
        }

        [TestMethod]
        public void ValidateResourceForbiddenTest()
        {
            var resources = this.MakeTestResources();

            using (var ctx = TestContext.CreateInstance())
            {
                ctx.RequestContext.Setup(x => x.ValidateAuthorized());
                ctx.DbResourcesRepo.Setup(x => x.FindResourceById(resources[0].Id)).Returns(resources[0]);
                ctx.SessionContext.Setup(x => x.UserId).Returns(resources[0].AuthorUserId);

                var testController = new LearningServiceImpl(ctx.ServiceContext.Object);

                var ex = Assert.ThrowsException<WebFaultException>(() => testController.ValidateResource(resources[0].Id.ToString()));

                Assert.IsTrue(ex.StatusCode == HttpStatusCode.Forbidden);
            }
        }
    }
}
