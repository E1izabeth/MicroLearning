using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using MicroLearningSvc.Db;
using MicroLearningSvc.Interaction;
using MicroLearningSvc.Util;

namespace MicroLearningSvc.Impl
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Single, InstanceContextMode = InstanceContextMode.Single)]
    [XmlSerializerFormat]
    class LearningServiceImpl : ILearningSvc
    {
        readonly ILearningServiceContext _ctx;

        public LearningServiceImpl(ILearningServiceContext ctx)
        {
            _ctx = ctx;
        }

        #region profile management

        public void Register(RegisterSpecType registerSpec)
        {
            if (registerSpec.Login.IsEmpty())
                throw new ApplicationException("Login cannot be empty");

            if (registerSpec.Password.IsEmpty() || registerSpec.Password.Length < 10)
                throw new ApplicationException("Password should be of length >10 characters");

            using (var ctx = _ctx.OpenWebRequestContext())
            {
                var loginKey = registerSpec.Login.ToLower();
                if (ctx.Db.Users.FindUserByLoginKey(loginKey) != null)
                {
                    throw new ApplicationException("User " + registerSpec.Login + " already exists");
                }
                else
                {
                    var salt = Convert.ToBase64String(_ctx.SecureRandom.GenerateRandomBytes(64));

                    var user = new DbUserInfo()
                    {
                        Activated = false,
                        HashSalt = salt,
                        Email = registerSpec.Email,
                        IsDeleted = false,
                        RegistrationStamp = DateTime.UtcNow,
                        Login = registerSpec.Login,
                        LoginKey = registerSpec.Login.ToLower(),
                        PasswordHash = registerSpec.Password.ComputeSha256Hash(salt),
                        LastLoginStamp = SqlDateTime.MinValue.Value,
                        LastTokenStamp = SqlDateTime.MinValue.Value
                    };
                    ctx.Db.Users.AddUser(user);
                    ctx.Db.SubmitChanges();

                    this.RequestActivationImpl(user, registerSpec.Email);
                    ctx.Db.SubmitChanges();
                }
            }
        }

        public void RequestActivation(RequestActivationSpecType spec)
        {
            using (var ctx = _ctx.OpenWebRequestContext())
            {
                ctx.ValidateAuthorized(false);
                this.RequestActivationImpl(ctx.Db.Users.GetUserById(ctx.Session.UserId), spec.Email);
                ctx.Db.SubmitChanges();
            }
        }

        private string MakeToken()
        {
            return new[] { "=", "/", "+" }.Aggregate(Convert.ToBase64String(_ctx.SecureRandom.GenerateRandomBytes(64)), (s, c) => s.Replace(c, string.Empty));
        }

        private void RequestActivationImpl(DbUserInfo user, string email)
        {
            if (user.Activated)
                throw new ApplicationException("Already activated");

            var activationToken = this.MakeToken();

            user.LastToken = activationToken;
            user.LastTokenStamp = DateTime.UtcNow;
            user.LastTokenKind = DbUserTokenKind.Activation;

            _ctx.SendMail(
                email, "Registration activation",
                "To confirm your registration follow this link: " + "http://172.16.100.47:8080/#/apply?action=activate&key=" + activationToken
            );
        }

        public void RequestAccess(ResetPasswordSpecType spec)
        {
            using (var ctx = _ctx.OpenWebRequestContext())
            {
                var loginKey = spec.Login.ToLower();

                var user = ctx.Db.Users.FindUserByLoginKey(loginKey);
                if (user != null && user.Email == spec.Email && !user.IsDeleted)
                {
                    var accessRestoreToken = this.MakeToken();

                    user.LastToken = accessRestoreToken;
                    user.LastTokenStamp = DateTime.UtcNow;
                    user.LastTokenKind = DbUserTokenKind.AccessRestore;
                    ctx.Db.SubmitChanges();

                    _ctx.SendMail(
                        spec.Email, "Access restore",
                        "To regain access to your profile follow this link: " + "http://172.16.100.47:8080/#/apply?action=restore&key=" + accessRestoreToken
                    );
                }
                else
                {
                    throw new ApplicationException("User not found or incorrect email");
                }
            }
        }

        public OkType Activate(string key)
        {
            using (var ctx = _ctx.OpenWebRequestContext())
            {
                var user = ctx.Db.Users.FindUserByTokenKey(key);
                if (user != null && user.LastToken != null && user.LastTokenKind == DbUserTokenKind.Activation)
                {
                    if (user.Activated)
                        throw new ApplicationException("Already activated");

                    if (user.LastTokenStamp + _ctx.Configuration.TokenTimeout >= DateTime.UtcNow)
                    {
                        user.LastLoginStamp = DateTime.UtcNow;
                        user.LastToken = null;
                        user.Activated = true;
                        ctx.Db.SubmitChanges();
                        ctx.Session.SetUserContext(user);
                    }
                    else
                    {
                        throw new ApplicationException("Acivation token expired");
                    }
                }
                else
                {
                    throw new ApplicationException("Invalid activation token");
                }
            }

            return new OkType();
        }

        public OkType RestoreAccess(string key)
        {
            using (var ctx = _ctx.OpenWebRequestContext())
            {
                var user = ctx.Db.Users.FindUserByTokenKey(key);
                if (user != null && user.LastToken != null && user.LastTokenKind == DbUserTokenKind.AccessRestore)
                {
                    if (user.LastTokenStamp + _ctx.Configuration.TokenTimeout >= DateTime.UtcNow)
                    {
                        user.LastLoginStamp = DateTime.UtcNow;
                        user.LastToken = null;
                        ctx.Db.SubmitChanges();
                        ctx.Session.SetUserContext(user);
                    }
                    else
                    {
                        throw new ApplicationException("Acivation token expired");
                    }
                }
                else
                {
                    throw new ApplicationException("Invalid activation token");
                }
            }
            
            return new OkType();
        }

        public void SetEmail(ChangeEmailSpecType spec)
        {
            using (var ctx = _ctx.OpenWebRequestContext())
            {
                ctx.ValidateAuthorized(false);

                var user = ctx.Db.Users.GetUserById(ctx.Session.UserId);
                if (user.Email == spec.OldEmail &&
                    user.PasswordHash == spec.Password.ComputeSha256Hash(user.HashSalt))
                {
                    user.Email = spec.NewEmail;
                    ctx.Db.SubmitChanges();
                }
                else
                {
                    throw new ApplicationException("Invalid old email or password");
                }
            }
        }

        public void SetPassword(ChangePasswordSpecType spec)
        {
            using (var ctx = _ctx.OpenWebRequestContext())
            {
                ctx.ValidateAuthorized(false);

                var user = ctx.Db.Users.GetUserById(ctx.Session.UserId);
                if (user.Email == spec.Email)
                // user.PasswordHash == spec.OldPassword.ComputeSha256Hash(user.HashSalt))
                {
                    user.PasswordHash = spec.NewPassword.ComputeSha256Hash(user.HashSalt);
                    ctx.Db.SubmitChanges();

                    _ctx.SendMail(spec.Email, "Password was changed", "Dear " + user.Login + ", your password was changed!");
                }
                else
                {
                    throw new ApplicationException("Invalid old email");
                }
            }
        }

        public void Login(LoginSpecType loginSpec)
        {
            using (var ctx = _ctx.OpenWebRequestContext())
            {
                var loginKey = loginSpec.Login;
                var user = ctx.Db.Users.FindUserByLoginKey(loginKey);
                if (user != null && user.PasswordHash == loginSpec.Password.ComputeSha256Hash(user.HashSalt) && !user.IsDeleted)
                {
                    user.LastLoginStamp = DateTime.UtcNow;
                    ctx.Db.SubmitChanges();

                    ctx.Session.SetUserContext(user);
                }
                else
                {
                    throw new ApplicationException("Invalid credentials");
                }
            }
        }

        public void Logout()
        {
            using (var ctx = _ctx.OpenWebRequestContext())
            {
                ctx.Session.SetUserContext(null);
            }
        }

        public void DeleteProfile()
        {
            using (var ctx = _ctx.OpenWebRequestContext())
            {
                ctx.ValidateAuthorized();

                var user = ctx.Db.Users.GetUserById(ctx.Session.UserId);
                user.IsDeleted = true;
                user.LastToken = null;

                ctx.Db.Subscriptions.DeactivateSubscriptionsByUser(ctx.Session.UserId);

                _ctx.SessionsManager.DropUserSessions(user.Id);
                ctx.Db.SubmitChanges();
            }
        }

        public ProfileFootprintInfoType GetProfileFootprint()
        {
            using (var ctx = _ctx.OpenWebRequestContext())
            {
                ctx.ValidateAuthorized(false);

                var user = ctx.Db.Users.GetUserById(ctx.Session.UserId);

                var parts = user.Email.Split('@');
                var leading = parts[0].Substring(0, Math.Min(2, parts[0].Length));
                var suffixDotPos = parts[1].LastIndexOf('.');
                var ending = suffixDotPos > 0 ? parts[1].Substring(suffixDotPos) : parts[1].Substring(parts[1].Length - Math.Min(2, parts[1].Length));
                var emailFootprint = leading + "***@***" + ending;

                return new ProfileFootprintInfoType()
                {
                    Login = user.Login,
                    EmailFootprint = emailFootprint,
                    IsActivated = user.Activated
                };
            }
        }

        #endregion

        #region resources 

        public CreateResourceSpecType SuggestResourceTags(CreateResourceSpecType spec)
        {
            using (var ctx = _ctx.OpenWebRequestContext())
            {
                ctx.ValidateAuthorized();

                var url = spec.ResourceUrl.NormalizeUrl();
                var data = _ctx.DownloadContent(url);
                
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.Load(new MemoryStream(data), Encoding.UTF8);

                var keywordsTagElement = doc.DocumentNode.SelectSingleNode(@"/html/head/meta[@name='keywords']");
                var docTitle = doc.DocumentNode.SelectSingleNode(@"/html/head/title")?.InnerText;

                var topicPageKeywords = keywordsTagElement?.GetAttributeValue("content", string.Empty)
                                                          ?.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                                          ?? new[] { string.Empty };

                var tags = spec.AssociationTags?.Select(t => t.Word) ?? new string[0];
                if (topicPageKeywords != null && topicPageKeywords.Length > 0)
                    tags = tags.Concat(topicPageKeywords);

                var tagsSet = tags.ToHashSet();

                return new CreateResourceSpecType()
                {
                    ResourceUrl = url,
                    ResourceTitle = string.IsNullOrWhiteSpace(spec.ResourceTitle) ? docTitle : spec.ResourceTitle,
                    AssociationTags = tagsSet.Select(t => new AssociationTagInfoType() { Word = t }).ToArray()
                };
            }
        }

        public ResourceInfoType CreateResource(CreateResourceSpecType spec)
        {
            using (var ctx = _ctx.OpenWebRequestContext())
            {
                ctx.ValidateAuthorized();

                if (string.IsNullOrWhiteSpace(spec.ResourceTitle))
                    throw new WebFaultException(HttpStatusCode.BadRequest);
                if ((spec.AssociationTags?.Length ?? 0) < 1)
                    throw new WebFaultException(HttpStatusCode.BadRequest);

                var url = spec.ResourceUrl.NormalizeUrl();

                var res = ctx.Db.Resources.FindResourceByUrl(url.ToLower());
                if (res == null)
                {
                    var contentParts = this.CollectResourceContent(url);

                    var tags = this.IntroduceTags(ctx, spec.AssociationTags.Select(t => t.Word).ToArray());

                    ctx.Db.SubmitChanges();

                    res = new DbResourceInfo()
                    {
                        AuthorUserId = ctx.Session.UserId,
                        CreationStamp = DateTime.UtcNow,
                        IsValidated = false,
                        Title = spec.ResourceTitle,
                        Url = url.ToLower(),
                        ValidationStamp = SqlDateTime.MinValue.Value,
                        ValidatorUserId = 0
                    };
                    ctx.Db.Resources.AddResource(res, contentParts);

                    ctx.Db.SubmitChanges();

                    ctx.Db.Resources.AssociateResource(res.Id, tags);

                    ctx.Db.SubmitChanges();

                    return res.TranslateResource(ctx.Session.UserId, tags);
                }
                else
                {
                    throw new ApplicationException("Resource already exists");
                }
            }
        }

        List<DbResourceContentPortionInfo> CollectResourceContent(string resourceUrl)
        {
            var article = _ctx.ParseResourceArticle(resourceUrl);
            var partContent = new StringBuilder();
            var contentParts = new LinkedList<DbResourceContentPortionInfo>();

            var reader = new StringReader(article.TextContent);
            string line = null;
            long order = 0;
            while ((line = reader.ReadLine()) != null)
            {
                partContent.AppendLine(line);

                if (partContent.Length > 5000)
                {
                    contentParts.AddLast(new DbResourceContentPortionInfo()
                    {
                        Order = order++,
                        TextContent = partContent.ToString(),
                    });

                    partContent.Clear();
                }
            }

            var tail = Environment.NewLine + partContent.ToString();

            if (contentParts.Count > 0)
                contentParts.Last.Value.TextContent += tail;
            else
                contentParts.AddLast(new DbResourceContentPortionInfo() { Order = 0, TextContent = tail });

            return contentParts.ToList();
        }

        private List<DbTagInstanceInfo> IntroduceTags(ILearningRequestContext ctx, string[] tags)
        {
            tags = tags.Select(w => _ctx.WordNormalizer.NormalizeWord(w).ToLower()).ToArray();

            return ctx.Db.Resources.IntroduceTags(tags);
        }

        public ResourceInfoType GetResource(string idString)
        {
            var id = long.Parse(idString);
            using (var ctx = _ctx.OpenWebRequestContext())
            {
                ctx.ValidateAuthorized();

                var res = ctx.Db.Resources.FindResourceById(id);
                if (res == null)
                    throw new WebFaultException(HttpStatusCode.NotFound);

                var tags = ctx.Db.Resources.GetTagsByResourceId(id);

                return res.TranslateResource(ctx.Session.UserId, tags);
            }
        }

        public ResourcesListType GetResources()
        {
            return this.FilterResourcesRange("0", "10", null);
        }

        public ResourcesListType GetResourcesRange(string skipStr, string takeStr)
        {
            return this.FilterResourcesRange(skipStr, takeStr, null);
        }

        public ResourcesListType FilterResourcesRange(string skipStr, string takeStr, ResourceFilterSpecType filterSpec)
        {
            int skip = int.Parse(skipStr),
                take = int.Parse(takeStr);

            using (var ctx = _ctx.OpenWebRequestContext())
            {
                ctx.ValidateAuthorized();

                var filteredResources = filterSpec?.Item?.Apply(new ResourceFilterSelector(_ctx, ctx.Db, skip, take))
                                                  ?? ctx.Db.Resources.FindResources(skip, take);

                return filteredResources.Resources.TranslateResources(
                    ctx.Session.UserId,
                    filteredResources.TotalResourcesCount,
                    filteredResources.Tags,
                    t => t.ResourceId,
                    t => t.Tag
                );
            }
        }

        enum ResourcesFilterKind
        {
            None,
            ByTopic,
            ByKeywords
        }

        public class ResourceFilterSelector : IResourceFilterSpecParametersBaseTypeVisitor<DbResourcesFilterResullt>
        {
            readonly ILearningServiceContext _svcCtx;
            readonly ILearningDbContext _ctx;
            readonly int _skip, _take;

            public ResourceFilterSelector(ILearningServiceContext svcCtx, ILearningDbContext ctx, int skip, int take)
            {
                _svcCtx = svcCtx;
                _ctx = ctx;
                _skip = skip;
                _take = take;
            }


            DbResourcesFilterResullt IResourceFilterSpecParametersBaseTypeVisitor<DbResourcesFilterResullt>.VisitResourceFilterByKeywordsSpec(ResourceFilterByKeywordsSpec spec)
            {
                var words = spec.AssociationTags.Select(t => _svcCtx.WordNormalizer.NormalizeWord(t.Word).ToLower()).ToArray();
                return _ctx.Resources.FindResourcesByKeywords(words, _skip, _take);
            }

            DbResourcesFilterResullt IResourceFilterSpecParametersBaseTypeVisitor<DbResourcesFilterResullt>.VisitResourceFilterByTopicSpec(ResourceFilterByTopicSpec node)
            {
                return _ctx.Resources.FindResourceByTopic(node.TopicId, _skip, _take);
            }
        }

        public void ValidateResource(string idString)
        {
            var id = long.Parse(idString);
            using (var ctx = _ctx.OpenWebRequestContext())
            {
                ctx.ValidateAuthorized();

                var res = ctx.Db.Resources.FindResourceById(id);
                if (res == null)
                    throw new WebFaultException(HttpStatusCode.NotFound);
                if (res.AuthorUserId == ctx.Session.UserId)
                    throw new WebFaultException(HttpStatusCode.Forbidden);
                if (res.IsValidated)
                    throw new ApplicationException("Already validated");

                res.IsValidated = true;
                res.ValidationStamp = DateTime.UtcNow;
                res.ValidatorUserId = ctx.Session.UserId;
                ctx.Db.SubmitChanges();
            }
        }

        #endregion

        #region topics

        public TopicInfoType GetTopic(string idString)
        {
            long topicId = long.Parse(idString);

            using (var ctx = _ctx.OpenWebRequestContext())
            {
                ctx.ValidateAuthorized();

                var topic = ctx.Db.Topics.FindTopicInfoById(topicId);
                if (topic == null)
                    throw new WebFaultException(HttpStatusCode.NotFound);

                if (topic.Info.AuthorUserId != ctx.Session.UserId)
                    throw new WebFaultException(HttpStatusCode.Forbidden);

                var tags = ctx.Db.Topics.GetTopicTags(topicId);
                
                return topic.TranslateTopic(tags);
            }
        }

        public TopicsListType GetTopics()
        {
            return this.GetTopicsRange("0", "10");
        }

        public TopicsListType GetTopicsRange(string skipStr, string takeStr)
        {
            int skip = int.Parse(skipStr),
                take = int.Parse(takeStr);

            using (var ctx = _ctx.OpenWebRequestContext())
            {
                ctx.ValidateAuthorized();

                var count = ctx.Db.Topics.GetAuthorTopicsCount(ctx.Session.UserId);

                var topics = ctx.Db.Topics.GetAuthorTopics(ctx.Session.UserId, skip, take);

                var tags = ctx.Db.Topics.GetAuthorTopicTags(ctx.Session.UserId, skip, take);

                return topics.TranslateTopics(count, tags, t => t.TopicId, t => t.TagInstance);
            }
        }

        public TopicInfoType CreateTopic(CreateTopicSpecType spec)
        {
            using (var ctx = _ctx.OpenWebRequestContext())
            {
                ctx.ValidateAuthorized();

                if (string.IsNullOrWhiteSpace(spec.TopicName))
                    throw new WebFaultException(HttpStatusCode.BadRequest);

                var topic = new DbTopicInfo()
                {
                    AuthorUserId = ctx.Session.UserId,
                    CreationStamp = DateTime.UtcNow,
                    Title = spec.TopicName
                };

                ctx.Db.Topics.AddTopic(topic);

                ctx.Db.SubmitChanges();

                var tags = this.IntroduceTags(ctx, spec.AssociationTags.Select(t => t.Word).ToArray());

                ctx.Db.Topics.AssociateTopic(topic.Id, tags);

                ctx.Db.SubmitChanges();

                return this.GetTopic(topic.Id.ToString());
            }
        }

        public void DeleteTopic(string idString)
        {
            long topicId = long.Parse(idString);

            using (var ctx = _ctx.OpenWebRequestContext())
            {
                ctx.ValidateAuthorized();

                var topic = ctx.Db.Topics.FindTopicById(topicId);
                if (topic == null)
                    throw new WebFaultException(HttpStatusCode.NotFound);

                if (topic.AuthorUserId != ctx.Session.UserId)
                    throw new WebFaultException(HttpStatusCode.Forbidden);

                ctx.Db.Topics.DeleteTopic(topic);
            }
        }

        #endregion

        #region subscription

        public void ResetTopicLearningProgress(string idStr)
        {
            var topicId = long.Parse(idStr);
            using (var ctx = _ctx.OpenWebRequestContext())
            {
                ctx.ValidateAuthorized();

                var topic = ctx.Db.Topics.FindTopicById(topicId);
                if (topic == null)
                    throw new WebFaultException(HttpStatusCode.NotFound);
                if (topic.AuthorUserId != ctx.Session.UserId)
                    throw new WebFaultException(HttpStatusCode.Forbidden);

                var subscription = ctx.Db.Subscriptions.FindSubscriptionByTopic(topicId);
                if (subscription != null)
                {
                    ctx.Db.Subscriptions.ResetLearningProgress(subscription.Id);

                    ctx.Db.SubmitChanges();
                }
                else
                {
                    throw new WebFaultException(HttpStatusCode.NotFound);
                }
            }
        }

        public void ActivateTopicLearning(string idStr, ActivateTopicSpecType spec)
        {
            var topicId = long.Parse(idStr);
            using (var ctx = _ctx.OpenWebRequestContext())
            {
                var dueTime = TimeSpan.FromSeconds(spec.DueSeconds);
                var interval = TimeSpan.FromSeconds(spec.IntervalSeconds);

                ctx.ValidateAuthorized();

                var topic = ctx.Db.Topics.FindTopicById(topicId);
                if (topic == null)
                    throw new WebFaultException(HttpStatusCode.NotFound);
                if (topic.AuthorUserId != ctx.Session.UserId)
                    throw new WebFaultException(HttpStatusCode.Forbidden);

                var subscription = ctx.Db.Subscriptions.FindSubscriptionByTopic(topic.Id);
                if (subscription == null)
                {
                    subscription = new DbUserSubscriptionInfo()
                    {
                        UserId = ctx.Session.UserId,
                        TopicId = topicId,
                    };
                    ctx.Db.Subscriptions.Add(subscription);
                }

                subscription.Interval = interval;
                subscription.NextPortionTime = DateTime.UtcNow + dueTime;
                subscription.IsActive = true;

                ctx.Db.SubmitChanges();
            }

            _ctx.DeliveryManager.Refresh();
        }

        public void DeactivateTopicLearning(string topicIdStr)
        {
            var topicId = long.Parse(topicIdStr);
            using (var ctx = _ctx.OpenWebRequestContext())
            {
                ctx.ValidateAuthorized();

                var topic = ctx.Db.Topics.FindTopicById(topicId);
                if (topic == null)
                    throw new WebFaultException(HttpStatusCode.NotFound);
                if (topic.AuthorUserId != ctx.Session.UserId)
                    throw new WebFaultException(HttpStatusCode.Forbidden);

                var subscription = ctx.Db.Subscriptions.FindSubscriptionByTopic(topic.Id);
                if (subscription == null)
                    throw new WebFaultException(HttpStatusCode.NotFound);

                subscription.IsActive = false;

                ctx.Db.SubmitChanges();
            }

            _ctx.DeliveryManager.Refresh();
        }

        #endregion

        #region content parts

        public ContentPartsListType GetContentPartsByTopic(string topicIdStr, string skipStr, string takeStr)
        {
            int skip = int.Parse(skipStr),
                take = int.Parse(takeStr);

            var topicId = long.Parse(topicIdStr);
            using (var ctx = _ctx.OpenWebRequestContext())
            {
                ctx.ValidateAuthorized();

                var (count, parts) = ctx.Db.Subscriptions.ContentPartsByTopic(topicId, ctx.Session.UserId, skip, take);

                var results = new List<ContentPartInfoType>();
                foreach (var part in parts)
                {
                    results.Add(new ContentPartInfoType()
                    {
                        ResourceId = part.resourceContentPart.ResourceId,
                        Id = part.resourceContentPart.Id,
                        Order = part.resourceContentPart.Order,
                        Text = part.resourceContentPart.TextContent,
                        IsDelivered = part.subscriptionContentPart?.IsDelivered == true,
                        IsLearned = part.subscriptionContentPart?.IsLearned == true,
                    });
                }

                return new ContentPartsListType() { Items = results.ToArray(), Count = count };
            }
        }

        public void MarkContentPartLearned(string topicIdStr, string partIdStr)
        {
            var topicId = long.Parse(topicIdStr);
            var partId = long.Parse(partIdStr);

            using (var ctx = _ctx.OpenWebRequestContext())
            {
                ctx.ValidateAuthorized();

                var parts = ctx.Db.Subscriptions.ContentPartByTopic(ctx.Session.UserId, topicId, partId);
                if (parts.Count == 0)
                    throw new WebFaultException(HttpStatusCode.NotFound);

                var now = DateTime.UtcNow;
                foreach (var item in parts)
                {
                    if (item.userSubscription == null)
                        continue;

                    var scp = item.subscriptionContentPart;

                    if (scp == null)
                    {
                        ctx.Db.Subscriptions.RegisterPart(scp = new DbUserSubscriptionContentPortionInfo()
                        {
                            ResourceContentPortionId = item.resourceContentPart.Id,
                            UserSubscriptionId = item.userSubscription.Id
                        });
                    }

                    scp.IsLearned = true;
                    scp.LearnedStamp = now;
                }

                ctx.Db.SubmitChanges();
            }

            _ctx.DeliveryManager.Refresh();
        }

        public void UnmarkContentPartLearned(string topicIdStr, string partIdStr)
        {
            var topicId = long.Parse(topicIdStr);
            var partId = long.Parse(partIdStr);

            using (var ctx = _ctx.OpenWebRequestContext())
            {
                ctx.ValidateAuthorized();

                var parts = ctx.Db.Subscriptions.ContentPartByTopic(ctx.Session.UserId, topicId, partId);
                if (parts.Count == 0)
                    throw new WebFaultException(HttpStatusCode.NotFound);

                var now = DateTime.UtcNow;
                foreach (var item in parts)
                {
                    if (item.userSubscription == null)
                        continue;

                    var scp = item.subscriptionContentPart;

                    if (scp != null && scp.IsLearned)
                    {
                        scp.IsLearned = false;
                        scp.LearnedStamp = now;
                        scp.IsMarkedToRepeat = true;
                    }
                }

                ctx.Db.SubmitChanges();
            }

            _ctx.DeliveryManager.Refresh();
        }

        #endregion
    }
}

