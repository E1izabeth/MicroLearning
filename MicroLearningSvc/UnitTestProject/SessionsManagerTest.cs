using MicroLearningSvc;
using MicroLearningSvc.Db;
using MicroLearningSvc.Impl;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel.Web;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnitTestProject.Util;

namespace UnitTestProject
{
    [TestClass]
    public class SessionsManagerTest : TestBase
    {
        private static readonly TimeSpan _defaultSessionTimeout = TimeSpan.FromHours(3);

        private ILearningSessionsManager MakeSessionsManager(TimeSpan? sessionTimeout = null)
        {
            return new LearningSessionsManager(sessionTimeout ?? _defaultSessionTimeout);
        }

        [TestMethod]
        public void CreateSessionTest()
        {
            using (var man = this.MakeSessionsManager())
            {
                var sessions = man.CreateFewSessions();

                Assert.IsTrue(sessions.Count() == sessions.DistinctBy(s => s.Id).Count());
            }
        }

        [TestMethod]
        public void GetExistingSessionTest()
        {
            using (var man = this.MakeSessionsManager())
            {
                var sessions = man.CreateFewSessions();

                var retrieved = sessions.Select(s => man.GetSession(s.Id)).ToArray();

                CollectionAssert.AreEqual(sessions, retrieved);
            }
        }

        [TestMethod]
        public void GetMissingSessionTest()
        {
            using (var man = this.MakeSessionsManager())
            {
                var sessions = man.CreateFewSessions();

                var ids = Enumerable.Range(0, int.MaxValue)
                                    .Select(n => Guid.NewGuid())
                                    .Where(id => sessions.All(s => s.Id != id))
                                    .Take(10);

                foreach (var id in ids)
                {
                    var ex = Assert.ThrowsException<ApplicationException>(() => man.GetSession(id));

                    Assert.IsTrue(ex.Message.Contains(id.ToString()) && ex.Message.Contains("does not exists"));
                }
            }
        }

        [TestMethod]
        public void TryGetExistingSessionTest()
        {
            using (var man = this.MakeSessionsManager())
            {
                var sessions = man.CreateFewSessions();

                var retrieved = sessions.Select(s => (ok: man.TryGetSession(s.Id, out var result), sess: result)).ToArray();

                Assert.IsTrue(retrieved.All(s => s.ok));
                CollectionAssert.AreEqual(sessions, retrieved.Select(s => s.sess).ToArray());
            }
        }

        [TestMethod]
        public void TryGetMissingSessionTest()
        {
            using (var man = this.MakeSessionsManager())
            {
                var sessions = man.CreateFewSessions();

                var ids = Enumerable.Range(0, int.MaxValue)
                                    .Select(n => Guid.NewGuid())
                                    .Where(id => sessions.All(s => s.Id != id))
                                    .Take(10);

                var retrieved = ids.Select(id => (ok: man.TryGetSession(id, out var result), sess: result)).ToArray();

                Assert.IsTrue(retrieved.All(s => !s.ok));
            }
        }

        [TestMethod]
        public void DeleteSessionTest()
        {
            using (var man = this.MakeSessionsManager())
            {
                var session = man.CreateSession();
                man.DeleteSession(session.Id);

                var exists = man.TryGetSession(session.Id, out var s);

                Assert.IsFalse(exists);
            }
        }

        [TestMethod]
        public void DeleteMissingSessionTest()
        {
            using (var man = this.MakeSessionsManager())
            {
                man.DeleteSession(Guid.NewGuid());
            }
        }

        [TestMethod]
        public void DropUserSessionsTest()
        {
            var testUser = this.MakeTestUser();

            using (var man = this.MakeSessionsManager())
            {
                var sessions = man.CreateFewSessions();

                var userSessions = Enumerable.Range(0, 5).Select(n => sessions.PickRandomItem()).Distinct().ToArray();
                userSessions.ForEach(r => r.SetUserContext(testUser));

                man.DropUserSessions(testUser.Id);

                Assert.IsTrue(sessions.Except(userSessions).All(s => man.TryGetSession(s.Id, out var s1) == true));
                Assert.IsTrue(userSessions.All(s => man.TryGetSession(s.Id, out var s2) == false));
            }
        }

        [TestMethod]
        public void DropMissingUserSessionsTest()
        {
            var testUser = this.MakeTestUser();

            using (var man = this.MakeSessionsManager())
            {
                man.DropUserSessions(testUserId);
            }
        }

        [TestMethod]
        public void CleanupSessionsTest()
        {
            using (var man = this.MakeSessionsManager(TimeSpan.FromMilliseconds(3)))
            {
                var sessions = man.CreateFewSessions();

                var userSessions = Enumerable.Range(0, 5).Select(n => sessions.PickRandomItem()).Distinct().ToArray();
                userSessions.ForEach(sess => sess.Renew());

                Thread.CurrentThread.Join(5);

                man.CleanupSessions();

                Assert.IsTrue(userSessions.All(s => man.TryGetSession(s.Id, out var s2) == false));
            }
        }

        [TestMethod]
        public void RenewTest()
        {
            using (var man = this.MakeSessionsManager())
            {
                var session = man.CreateSession();
                var last = session.LastActivity;
                Thread.CurrentThread.Join(50);
                session.Renew();

                Assert.IsTrue(last != session.LastActivity);
            }
        }

        [TestMethod]
        public void SetUserContextTest()
        {
            var testUser = this.MakeTestUser();

            using (var man = this.MakeSessionsManager())
            {
                var session = man.CreateSession();
                bool eventRaised = false;
                session.OnUserContextChanging += (smth) => { eventRaised = true; };
                Assert.IsFalse(eventRaised);

                var oldUserId = session.UserId;
                session.SetUserContext(testUser);

                Assert.IsTrue(session.UserId == testUser.Id);
                Assert.IsTrue(session.UserId != oldUserId);
                Assert.IsTrue(eventRaised);
            }
        }

        [TestMethod]
        public void SetUserContextNullTest()
        {
            DbUserInfo testUser = null;

            using (var man = this.MakeSessionsManager())
            {
                var session = man.CreateSession();
                bool eventRaised = false;
                session.OnUserContextChanging += (smth) => { eventRaised = true; };

                session.SetUserContext(testUser);

                Assert.IsTrue(session.UserId == 0);
                Assert.IsTrue(eventRaised);
            }
        }

        [TestMethod]
        public void DefaultSessionTest()
        {
            using (var man = this.MakeSessionsManager())
            {
                var session = man.CreateSession();
                
                Assert.IsTrue(session.UserId == 0);
                Assert.IsFalse(session.IsActivated);
            }
        }

        [TestMethod]
        public void ExplicitDeleteAllUserSessionsTest()
        {
            var testUser = this.MakeTestUser();

            using (var man = this.MakeSessionsManager())
            {
                var sessions = man.CreateFewSessions();

                var userSessions = Enumerable.Range(0, 5).Select(n => sessions.PickRandomItem()).Distinct().ToArray();
                userSessions.ForEach(r => r.SetUserContext(testUser));

                userSessions.ForEach(r => man.DeleteSession(r.Id));
                
                Assert.IsTrue(sessions.Except(userSessions).All(s => man.TryGetSession(s.Id, out var s1) == true));
                Assert.IsTrue(userSessions.All(s => man.TryGetSession(s.Id, out var s2) == false));
            }
        }

        [TestMethod]
        public void RequestContextValidateAuthorizedActivatedTest()
        {
            using (var ctx = TestContext.CreacteRequestContextInstance())
            {
                ctx.SessionContext.Setup(x => x.UserId).Returns(testUserId);
                ctx.DbUsersRepo.Setup(x => x.GetUserById(testUserId)).Returns(this.MakeTestUser());

                using (var requestContext = new LearningRequestContext(ctx.ServiceContext.Object, ctx.SessionContext.Object))
                {
                    requestContext.ValidateAuthorized();
                }
            }
        }

        [TestMethod]
        public void RequestContextValidateAuthorizedNotActivatedTest()
        {
            using (var ctx = TestContext.CreacteRequestContextInstance())
            {
                ctx.SessionContext.Setup(x => x.UserId).Returns(testUserId);

                using (var requestContext = new LearningRequestContext(ctx.ServiceContext.Object, ctx.SessionContext.Object))
                {
                    requestContext.ValidateAuthorized(false);
                }
            }
        }
        
        [TestMethod]
        public void RequestContextValidateAuthorizedNoUserFailTest()
        {
            using (var ctx = TestContext.CreacteRequestContextInstance())
            {
                ctx.SessionContext.Setup(x => x.UserId).Returns(0);

                using (var requestContext = new LearningRequestContext(ctx.ServiceContext.Object, ctx.SessionContext.Object))
                {
                    var ex = Assert.ThrowsException<WebFaultException>(() => requestContext.ValidateAuthorized());
                    
                    Assert.IsTrue(ex.StatusCode == HttpStatusCode.Forbidden);
                }
            }
        }
        
        [TestMethod]
        public void RequestContextValidateAuthorizedNoActivationFailTest()
        {
            using (var ctx = TestContext.CreacteRequestContextInstance())
            {
                ctx.SessionContext.Setup(x => x.UserId).Returns(testUserId);
                ctx.DbUsersRepo.Setup(x => x.GetUserById(testUserId)).Returns(this.MakeTestUser(false));

                using (var requestContext = new LearningRequestContext(ctx.ServiceContext.Object, ctx.SessionContext.Object))
                {
                    var ex = Assert.ThrowsException<WebFaultException>(() => requestContext.ValidateAuthorized());

                    Assert.IsTrue(ex.StatusCode == HttpStatusCode.Forbidden);
                }
            }
        }
    }

    static class SessionsManagerExtensions
    {
        static readonly object _rndLock = new object();
        static readonly Random _rnd = new Random();

        public static ILearningSessionContext[] CreateFewSessions(this ILearningSessionsManager sessionsManager)
        {
            return Enumerable.Range(0, 10).Select(n => sessionsManager.CreateSession()).ToArray();
        }

        public static T PickRandomItem<T>(this IList<T> items)
        {
            lock (_rndLock)
            {
                return items[_rnd.Next(0, items.Count)];
            }
        }
    }
}
