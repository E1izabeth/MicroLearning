using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Runtime.InteropServices;
using MicroLearningCli;
using MicroLearningSvc;
using MicroLearningSvc.Db;
using MicroLearningSvc.Impl;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace UnitTestProject
{
    [TestClass]
    public class ProfileTests : TestBase
    {

        [TestMethod]
        public void RegistrationTest()
        {
            using (var ctx = TestContext.CreateInstance())
            {
                DbUserInfo dbUser = null;

                ctx.SecureRandom.Setup(x => x.GenerateRandomBytes(64)).Returns(new byte[64]);
                ctx.DbContext.Setup(x => x.SubmitChanges());
                ctx.ServiceContext.Setup(x => x.SendMail(testUserEmail, It.IsAny<string>(), It.IsAny<string>()));
                ctx.DbUsersRepo.Setup(x => x.FindUserByLoginKey(testUserLogin)).Returns((DbUserInfo)null);
                ctx.DbUsersRepo.Setup(x => x.AddUser(It.Is<DbUserInfo>(u => u.Email == testUserEmail && u.Login == testUserLogin && u.LoginKey == testUserLogin && u.LastTokenKind == DbUserTokenKind.Activation)))
                               .Callback((DbUserInfo u) => dbUser = u);

                var testController = new LearningServiceImpl(ctx.ServiceContext.Object);

                testController.Register(new RegisterSpecType() { Login = testUserLogin, Email = testUserEmail, Password = "pwdpwdpwdpwd" });

                ctx.ServiceContext.Verify(x => x.SendMail(testUserEmail, It.IsAny<string>(), It.Is<string>(s => s.Contains(dbUser.LastToken))));
            }
        }

        [TestMethod]
        public void RegistrationEmptyLoginTest()
        {
            using (var ctx = TestContext.CreateInstance())
            {
                var testController = new LearningServiceImpl(ctx.ServiceContext.Object);

                var ex = Assert.ThrowsException<ApplicationException>(
                    () => testController.Register(new RegisterSpecType() { Login = "", Email = testUserEmail, Password = "pwdpwdpwdpwd" })
                );
                Assert.IsTrue(ex.Message.Contains("Login cannot be empty"));
            }
        }

        [TestMethod]
        public void RegistrationEmptyPasswordTest()
        {
            using (var ctx = TestContext.CreateInstance())
            {
                var testController = new LearningServiceImpl(ctx.ServiceContext.Object);

                var ex = Assert.ThrowsException<ApplicationException>(
                    () => testController.Register(new RegisterSpecType() { Login = testUserLogin, Email = testUserEmail, Password = "" })
                );
                Assert.IsTrue(ex.Message.Contains("Password should be of length >10 characters"));
            }
        }

        [TestMethod]
        public void SecondRegisterFailsTest()
        {
            var existingUser = this.MakeTestUser();

            using (var ctx = TestContext.CreateInstance())
            {
                ctx.DbUsersRepo.Setup(x => x.FindUserByLoginKey(testUserLogin)).Returns(existingUser);

                var testController = new LearningServiceImpl(ctx.ServiceContext.Object);

                var ex = Assert.ThrowsException<ApplicationException>(
                    () => testController.Register(new RegisterSpecType() { Login = "tester", Email = "test@local", Password = "pwdpwdpwdpwd" })
                );
                Assert.AreEqual(ex.Message, "User tester already exists");
            }
        }

        [TestMethod]
        public void LoginTest()
        {
            var existingUser = this.MakeTestUser();

            using (var ctx = TestContext.CreateInstance())
            {
                ctx.DbContext.Setup(x => x.SubmitChanges());
                ctx.DbUsersRepo.Setup(x => x.FindUserByLoginKey(testUserLogin)).Returns(existingUser);
                ctx.SessionContext.Setup(x => x.SetUserContext(It.Is<DbUserInfo>(o => o == existingUser)));

                var testController = new LearningServiceImpl(ctx.ServiceContext.Object);

                testController.Login(new LoginSpecType() { Login = testUserLogin, Password = testPassword });
            }
        }
        
        [TestMethod]
        public void LoginFailsDueToWrongPasswordTest()
        {
            var existingUser = this.MakeTestUser();

            using (var ctx = TestContext.CreateInstance())
            {
                ctx.DbUsersRepo.Setup(x => x.FindUserByLoginKey(testUserLogin)).Returns(existingUser);

                var testController = new LearningServiceImpl(ctx.ServiceContext.Object);

                var ex = Assert.ThrowsException<ApplicationException>(
                    () => testController.Login(new LoginSpecType() { Login = testUserLogin, Password = testPassword2 })
                );
                Assert.AreEqual(ex.Message, "Invalid credentials");
            }
        }

        [TestMethod]
        public void ActivationRequestTest()
        {
            var existingUser = this.MakeTestUser(false);

            using (var ctx = TestContext.CreateInstance())
            {
                ctx.DbContext.Setup(x => x.SubmitChanges());
                ctx.DbUsersRepo.Setup(x => x.GetUserById(existingUser.Id)).Returns(existingUser);
                ctx.RequestContext.Setup(x => x.ValidateAuthorized(false));
                ctx.SessionContext.Setup(x => x.UserId).Returns(existingUser.Id);
                ctx.SecureRandom.Setup(x => x.GenerateRandomBytes(64)).Returns(new byte[64]);
                ctx.ServiceContext.Setup(x => x.SendMail(testUserEmail, It.IsAny<string>(), It.IsAny<string>()));

                var testController = new LearningServiceImpl(ctx.ServiceContext.Object);

                testController.RequestActivation(new RequestActivationSpecType() { Email = existingUser.Email });

                ctx.ServiceContext.Verify(x => x.SendMail(testUserEmail, It.IsAny<string>(), It.Is<string>(s => s.Contains(existingUser.LastToken))));
            }
        }

        [TestMethod]
        public void ActivationRequestAlreadyActivetedTest()
        {
            var existingUser = this.MakeTestUser(false);
            existingUser.Activated = true;

            using (var ctx = TestContext.CreateInstance())
            {
                ctx.DbUsersRepo.Setup(x => x.GetUserById(existingUser.Id)).Returns(existingUser);
                ctx.RequestContext.Setup(x => x.ValidateAuthorized(false));
                ctx.SessionContext.Setup(x => x.UserId).Returns(existingUser.Id);

                var testController = new LearningServiceImpl(ctx.ServiceContext.Object);
                var ex = Assert.ThrowsException<ApplicationException>(
                    () => testController.RequestActivation(new RequestActivationSpecType() { Email = existingUser.Email })
                );
                Assert.IsTrue(ex.Message.Contains("Already activated"));
            }
        }


        [TestMethod]
        public void ActivateTest()
        {
            var existingUser = this.MakeTestUser(false);

            existingUser.LastTokenKind = DbUserTokenKind.Activation;
            existingUser.Activated = false;

            using (var ctx = TestContext.CreateInstance())
            {
                ctx.DbContext.Setup(x => x.SubmitChanges());
                ctx.DbUsersRepo.Setup(x => x.FindUserByTokenKey(activationToken)).Returns(existingUser);
                ctx.SessionContext.Setup(x => x.SetUserContext(It.Is<DbUserInfo>(o => o == existingUser)));
                ctx.ServiceContext.Setup(x => x.Configuration).Returns(new LearningServiceConfiguration() { TokenTimeout = TimeSpan.FromDays(7) });

                var testController = new LearningServiceImpl(ctx.ServiceContext.Object);

                testController.Activate(activationToken);
            }
        }

        [TestMethod]
        public void ActivateAlreadyActivatedTest()
        {
            var existingUser = this.MakeTestUser(true);
            existingUser.LastTokenKind = DbUserTokenKind.Activation;

            using (var ctx = TestContext.CreateInstance())
            {
                ctx.DbUsersRepo.Setup(x => x.FindUserByTokenKey(activationToken)).Returns(existingUser);
                var testController = new LearningServiceImpl(ctx.ServiceContext.Object);

                var ex = Assert.ThrowsException<ApplicationException>(() => testController.Activate(activationToken));
                Assert.IsTrue(ex.Message.Contains("Already activated"));
            }
        }

        [TestMethod]
        public void ActivateTokenExpiredTest()
        {
            var existingUser = this.MakeTestUser();
            existingUser.LastTokenKind = DbUserTokenKind.Activation;
            existingUser.Activated = false;

            existingUser.LastTokenStamp -= TimeSpan.FromDays(14);

            using (var ctx = TestContext.CreateInstance())
            {
                ctx.DbUsersRepo.Setup(x => x.FindUserByTokenKey(activationToken)).Returns(existingUser);
                ctx.ServiceContext.Setup(x => x.Configuration).Returns(new LearningServiceConfiguration() { TokenTimeout = TimeSpan.FromDays(7) });

                var testController = new LearningServiceImpl(ctx.ServiceContext.Object);
                var ex = Assert.ThrowsException<ApplicationException>(() => testController.Activate(activationToken));
                Assert.IsTrue(ex.Message.Contains("Acivation token expired"));
            }
        }


        [TestMethod]
        public void ActivateInvalidTokenTest()
        {
            var existingUser = this.MakeTestUser();
            existingUser.LastToken = null;

            existingUser.LastTokenKind = DbUserTokenKind.Activation;
            existingUser.Activated = false;

            using (var ctx = TestContext.CreateInstance())
            {
                ctx.DbUsersRepo.Setup(x => x.FindUserByTokenKey(activationToken)).Returns(existingUser);

                var testController = new LearningServiceImpl(ctx.ServiceContext.Object);

                var ex = Assert.ThrowsException<ApplicationException>(() => testController.Activate(activationToken));
                Assert.IsTrue(ex.Message.Contains("Invalid activation token"));
            }
        }

        [TestMethod]
        public void RequestAccessTest()
        {
            var existingUser = this.MakeTestUser();

            using (var ctx = TestContext.CreateInstance())
            {
                ctx.DbContext.Setup(x => x.SubmitChanges());
                ctx.DbUsersRepo.Setup(x => x.FindUserByLoginKey(testUserLogin)).Returns(existingUser);
                ctx.SecureRandom.Setup(x => x.GenerateRandomBytes(64)).Returns(new byte[64]);
                ctx.ServiceContext.Setup(x => x.SendMail(testUserEmail, It.IsAny<string>(), It.IsAny<string>()));

                var testController = new LearningServiceImpl(ctx.ServiceContext.Object);

                testController.RequestAccess(new ResetPasswordSpecType() { Email = testUserEmail, Login = testUserLogin });

                ctx.ServiceContext.Verify(x => x.SendMail(testUserEmail, It.IsAny<string>(), It.Is<string>(s => s.Contains(existingUser.LastToken))));
            }
        }

        [TestMethod]
        public void RequestAccessUserNotFoundTest()
        {
            using (var ctx = TestContext.CreateInstance())
            {
                ctx.DbUsersRepo.Setup(x => x.FindUserByLoginKey(testUserLogin)).Returns<DbUserInfo>(null);

                var testController = new LearningServiceImpl(ctx.ServiceContext.Object);

                var ex = Assert.ThrowsException<ApplicationException>(
                    () => testController.RequestAccess(new ResetPasswordSpecType() { Email = testUserEmail, Login = testUserLogin })
                );
                Assert.IsTrue(ex.Message.Contains("User not found or incorrect email"));
            }
        }

        [TestMethod]
        public void RestoreAccessTest()
        {
            var existingUser = this.MakeTestUser();
            existingUser.LastTokenKind = DbUserTokenKind.AccessRestore;

            using (var ctx = TestContext.CreateInstance())
            {
                ctx.DbContext.Setup(x => x.SubmitChanges());
                ctx.DbUsersRepo.Setup(x => x.FindUserByTokenKey(activationToken)).Returns(existingUser);
                ctx.SessionContext.Setup(x => x.SetUserContext(It.Is<DbUserInfo>(o => o == existingUser)));
                ctx.ServiceContext.Setup(x => x.Configuration).Returns(new LearningServiceConfiguration() { TokenTimeout = TimeSpan.FromDays(7) });

                var testController = new LearningServiceImpl(ctx.ServiceContext.Object);

                testController.RestoreAccess(activationToken);
            }
        }

        [TestMethod]
        public void RestoreAccessTokenExpiredTest()
        {
            var existingUser = this.MakeTestUser();
            existingUser.LastTokenStamp -= TimeSpan.FromDays(10);
            existingUser.LastTokenKind = DbUserTokenKind.AccessRestore;

            using (var ctx = TestContext.CreateInstance())
            {
                ctx.DbUsersRepo.Setup(x => x.FindUserByTokenKey(activationToken)).Returns(existingUser);
                ctx.ServiceContext.Setup(x => x.Configuration).Returns(new LearningServiceConfiguration() { TokenTimeout = TimeSpan.FromDays(7) });

                var testController = new LearningServiceImpl(ctx.ServiceContext.Object);
                var ex = Assert.ThrowsException<ApplicationException>(() => testController.RestoreAccess(activationToken));
                Assert.IsTrue(ex.Message.Contains("Acivation token expired"));
            }
        }


        [TestMethod]
        public void RestoreAccessInvalidTokenTest()
        {
            var existingUser = this.MakeTestUser();
            existingUser.LastToken = null;

            using (var ctx = TestContext.CreateInstance())
            {
                ctx.DbUsersRepo.Setup(x => x.FindUserByTokenKey(activationToken)).Returns(existingUser);

                var testController = new LearningServiceImpl(ctx.ServiceContext.Object);

                var ex = Assert.ThrowsException<ApplicationException>(() => testController.RestoreAccess(activationToken));
                Assert.IsTrue(ex.Message.Contains("Invalid activation token"));
            }
        }

        [TestMethod]
        public void SetEmailTest()
        {
            var existingUser = this.MakeTestUser();

            using (var ctx = TestContext.CreateInstance())
            {
                ctx.SessionContext.Setup(x => x.UserId).Returns(testUserId);
                ctx.RequestContext.Setup(x => x.ValidateAuthorized(false));
                ctx.DbUsersRepo.Setup(x => x.GetUserById(testUserId)).Returns(existingUser);
                ctx.DbContext.Setup(x => x.SubmitChanges());

                var testController = new LearningServiceImpl(ctx.ServiceContext.Object);

                testController.SetEmail(new ChangeEmailSpecType() { NewEmail = testUserEmail2, OldEmail = testUserEmail, Password = testPassword });

                Assert.AreEqual(existingUser.Email, testUserEmail2);
            }
        }

        [TestMethod]
        public void SetEmailInvalidOldEmailTest()
        {
            var existingUser = this.MakeTestUser();
            existingUser.Email = existingUser.Email + "fault";

            using (var ctx = TestContext.CreateInstance())
            {
                ctx.SessionContext.Setup(x => x.UserId).Returns(testUserId);
                ctx.RequestContext.Setup(x => x.ValidateAuthorized(false));
                ctx.DbUsersRepo.Setup(x => x.GetUserById(testUserId)).Returns(existingUser);

                var testController = new LearningServiceImpl(ctx.ServiceContext.Object);
                var ex = Assert.ThrowsException<ApplicationException>(
                    () => testController.SetEmail(new ChangeEmailSpecType() { NewEmail = testUserEmail2, OldEmail = testUserEmail, Password = testPassword })
                );
                Assert.IsTrue(ex.Message.Contains("Invalid old email"));
            }
        }

        [TestMethod]
        public void SetPasswordTest()
        {
            var existingUser = this.MakeTestUser();

            using (var ctx = TestContext.CreateInstance())
            {
                ctx.SessionContext.Setup(x => x.UserId).Returns(testUserId);
                ctx.RequestContext.Setup(x => x.ValidateAuthorized(false));
                ctx.DbUsersRepo.Setup(x => x.GetUserById(testUserId)).Returns(existingUser);
                ctx.DbContext.Setup(x => x.SubmitChanges());
                ctx.ServiceContext.Setup(x => x.SendMail(testUserEmail, It.IsAny<string>(), It.IsAny<string>()));

                var testController = new LearningServiceImpl(ctx.ServiceContext.Object);

                testController.SetPassword(new ChangePasswordSpecType() { Email = testUserEmail, /*OldPassword = testPassword,*/ NewPassword = testPassword2 });

                Assert.AreEqual(existingUser.PasswordHash, testPassword2.ComputeSha256Hash(existingUser.HashSalt));

                ctx.ServiceContext.Verify(x => x.SendMail(testUserEmail, It.IsAny<string>(), It.Is<string>(s => s.Contains("password was changed"))));
            }
        }

        [TestMethod]
        public void SetPasswordInvalidEmailTest()
        {
            var existingUser = this.MakeTestUser();
            existingUser.Email = existingUser.Email + "fault";

            using (var ctx = TestContext.CreateInstance())
            {
                ctx.SessionContext.Setup(x => x.UserId).Returns(testUserId);
                ctx.RequestContext.Setup(x => x.ValidateAuthorized(false));
                ctx.DbUsersRepo.Setup(x => x.GetUserById(testUserId)).Returns(existingUser);

                var testController = new LearningServiceImpl(ctx.ServiceContext.Object);

                var ex = Assert.ThrowsException<ApplicationException>(
                    () => testController.SetPassword(new ChangePasswordSpecType() { Email = testUserEmail, /* OldPassword = testPassword,*/ NewPassword = testPassword2 })
                );

                Assert.IsTrue(ex.Message.Contains("Invalid old email"));
            }
        }



        [TestMethod]
        public void Logout()
        {
            var existingUser = this.MakeTestUser();

            using (var ctx = TestContext.CreateInstance())
            {
                ctx.SessionContext.Setup(x => x.SetUserContext(null));

                var testController = new LearningServiceImpl(ctx.ServiceContext.Object);

                testController.Logout();
            }
        }

        [TestMethod]
        public void DeleteProfileTest()
        {
            var existingUser = this.MakeTestUser();

            using (var ctx = TestContext.CreateInstance())
            {
                ctx.DbContext.Setup(x => x.SubmitChanges());
                ctx.DbUsersRepo.Setup(x => x.GetUserById(testUserId)).Returns(existingUser);
                ctx.DbSubscriptionsRepo.Setup(x => x.DeactivateSubscriptionsByUser(existingUser.Id));
                ctx.SessionManager.Setup(x => x.DropUserSessions(existingUser.Id));
                ctx.SessionContext.Setup(x => x.UserId).Returns(existingUser.Id);


                var testController = new LearningServiceImpl(ctx.ServiceContext.Object);

                testController.DeleteProfile();
            }
        }
    }
}
