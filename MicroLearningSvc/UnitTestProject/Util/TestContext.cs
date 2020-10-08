using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MicroLearningSvc;
using MicroLearningSvc.Db;
using MicroLearningSvc.Impl;
using Moq;
using UnitTestProject.Util;

namespace UnitTestProject
{
    internal interface IRequestContextTestContext : IDisposable
    {
        Mock<ILearningServiceContext> ServiceContext { get; }
        Mock<ILearningSessionContext> SessionContext { get; }

        Mock<ILearningDbContext> DbContext { get; }
        Mock<IUsersRepository> DbUsersRepo { get; }
    }

    internal interface ITestContext : IDisposable
    {
        MockRepository Mocks { get; }

        Mock<ILearningServiceContext> ServiceContext { get; }
        Mock<IBasicOperationContext> LocalContext { get; }
        Mock<ILearningRequestContext> RequestContext { get; }

        Mock<ILearningDbContext> DbContext { get; }
        Mock<IUsersRepository> DbUsersRepo { get; }
        Mock<IResourcesRepository> DbResourcesRepo { get; }
        Mock<ITopicsRepository> DbTopicsRepo { get; }
        Mock<ISubscriptionsRepository> DbSubscriptionsRepo { get; }

        Mock<IWordNormalizer> WordNormalizer { get; }
        Mock<ISecureRandom> SecureRandom { get; }
        Mock<ILearningSessionContext> SessionContext { get; }
        Mock<ILearningSessionsManager> SessionManager { get; }
        Mock<ILearningContentDeliveryManager> DeliveryManager { get; }
    }

    internal static class TestContext
    {
        //private class ContextImpl : RealProxy
        //{
        //    private readonly MockRepository _repo = new MockRepository(MockBehavior.Strict);

        //    private readonly Dictionary<Type, Mock> _mocks = new Dictionary<Type, Mock>();

        //    public ContextImpl()
        //        : base(typeof(ITestContext)) { }

        //    public override IMessage Invoke(IMessage msg)
        //    {
        //        if (msg is IMethodCallMessage mcm)
        //        {
        //            if (mcm.MethodName == "Dispose")
        //            {
        //                foreach (var mock in _mocks.Values)
        //                    mock.VerifyAll();

        //                return new ReturnMessage(null, mcm.Args, mcm.ArgCount, mcm.LogicalCallContext, mcm);
        //            }
        //            else if (mcm.MethodBase is MethodInfo method && method.Name.StartsWith("get_"))
        //            {
        //                if (method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(Mock<>))
        //                {
        //                    var mockType = method.ReturnType.GetGenericArguments()[0];
        //                    if (!_mocks.TryGetValue(mockType, out var mockObj))
        //                    {
        //                        var callCreateMock = Expression.Lambda<Func<Mock>>(Expression.Convert(
        //                            Expression.Call(Expression.Constant(_repo), "Create", new[] { mockType }), typeof(Mock)
        //                        ));

        //                        var callCreateMockMethod = callCreateMock.Compile();
        //                        mockObj = callCreateMockMethod();
        //                        _mocks.Add(mockType, mockObj);
        //                    }

        //                    return new ReturnMessage(mockObj, mcm.Args, mcm.ArgCount, mcm.LogicalCallContext, mcm);
        //                }
        //                else if (method.Name == "get_Mocks")
        //                {
        //                    return new ReturnMessage(null, mcm.Args, mcm.ArgCount, mcm.LogicalCallContext, mcm);
        //                }
        //                else
        //                {
        //                    throw new NotImplementedException();
        //                }
        //            }
        //            else
        //            {
        //                throw new NotImplementedException();
        //            }
        //        }
        //        else
        //        {
        //            throw new NotImplementedException();
        //        }
        //    }
        //}

        public static ITestContext CreateInstance()
        {
            //return (ITestContext)new ContextImpl().GetTransparentProxy();

            return new TestContextImpl();
        }

        public static IRequestContextTestContext CreacteRequestContextInstance()
        {
            return new RequestContextTestContextImpl();
        }
    }

    internal class TestContextBase : IDisposable
    {
        public MockRepository Mocks { get { return _repo; } }

        private readonly MockRepository _repo = new MockRepository(MockBehavior.Strict);

        private readonly Dictionary<Type, Mock> _mocks = new Dictionary<Type, Mock>();

        private static Exception _lastException = null;

        static TestContextBase()
        {
            AppDomain.CurrentDomain.FirstChanceException += (sender, ea) => _lastException = ea.Exception;
        }

        protected Mock<T> GetMock<T>(Action<Mock<T>> setup = null)
            where T : class
        {
            var mockType = typeof(T);
            if (!_mocks.TryGetValue(mockType, out var mockObj))
            {
                mockObj = this.Mocks.Create<T>();
                _mocks.Add(mockType, mockObj);

                setup?.Invoke((Mock<T>)mockObj);
            }

            return (Mock<T>)mockObj;
        }


        public void Dispose()
        {
            var stack = new StackTrace();
            var currentMethod = MethodInfo.GetCurrentMethod();
            var callerFrame = stack.GetFrames().SkipWhile(f => f.GetMethod() != currentMethod).Skip(1).First();
            var callerOffset = callerFrame.GetILOffset();

            var callerMethod = callerFrame.GetMethod();
            var callerBody = callerMethod.GetMethodBody();

            //if (callerBody.ExceptionHandlingClauses.Any(e => callerOffset >= e.HandlerOffset && callerOffset < e.HandlerOffset + e.HandlerLength))

            // var callerOp = (IlOp<MethodBase>)IlParser.Translate(callerMethod).TakeWhile(o => o.Offset < callerOffset).Last();
            //var callingInstanceTypes = new[] { typeof(Action), typeof(IDisposable), typeof(TestContext) };
            //if (callingInstanceTypes.Any(t => t == callerOp.Arg))

            var exCode = Marshal.GetExceptionCode();
            if (exCode != 0)
            {
                Console.WriteLine("--------- Skipping mock verification due to unhandled exception ------------------------------");
            }
            else
            {
                Console.WriteLine("------------------------ Verifying mock object -----------------------------------------------");
                foreach (var mock in _mocks.Values)
                    mock.VerifyAll();
            }
        }
    }

    internal class RequestContextTestContextImpl : TestContextBase, IRequestContextTestContext
    {
        public Mock<ILearningServiceContext> ServiceContext => this.GetMock<ILearningServiceContext>();

        public Mock<ILearningSessionContext> SessionContext => this.GetMock<ILearningSessionContext>();

        public Mock<ILearningDbContext> DbContext => this.GetMock<ILearningDbContext>(m =>
        {
            this.ServiceContext.Setup(x => x.OpenDb()).Returns(m.Object);

            var dbCnn = this.GetMock<IDbConnection>();
            dbCnn.Setup(x => x.Dispose());

            var dbCtx = this.GetMock<IDbContext>();
            dbCtx.Setup(x => x.Connection).Returns(dbCnn.Object);
            dbCtx.Setup(x => x.Dispose());

            m.Setup(x => x.Raw).Returns(dbCtx.Object);
        });

        public Mock<IUsersRepository> DbUsersRepo => this.GetMock<IUsersRepository>(m =>
        {
            this.DbContext.Setup(x => x.Users).Returns(m.Object);
        });
    }

    internal class TestContextImpl : TestContextBase, ITestContext
    {
        public Mock<ILearningServiceContext> ServiceContext => this.GetMock<ILearningServiceContext>();

        public Mock<ILearningRequestContext> RequestContext => this.GetMock<ILearningRequestContext>(m =>
        {
            this.ServiceContext.Setup(c => c.OpenWebRequestContext()).Returns(m.Object);
            m.Setup(x => x.Dispose());
        });

        public Mock<IBasicOperationContext> LocalContext => this.GetMock<IBasicOperationContext>(m =>
        {
            this.ServiceContext.Setup(c => c.OpenLocalContext()).Returns(m.Object);
            m.Setup(x => x.Dispose());
        });

        public Mock<ILearningDbContext> DbContext => this.GetMock<ILearningDbContext>(m =>
        {
            this.RequestContext.Setup(x => x.Db).Returns(m.Object);
        });

        public Mock<IUsersRepository> DbUsersRepo => this.GetMock<IUsersRepository>(m =>
        {
            this.DbContext.Setup(x => x.Users).Returns(m.Object);
        });

        public Mock<IResourcesRepository> DbResourcesRepo => this.GetMock<IResourcesRepository>(m =>
        {
            this.DbContext.Setup(x => x.Resources).Returns(m.Object);
        });

        public Mock<ITopicsRepository> DbTopicsRepo => this.GetMock<ITopicsRepository>(m =>
        {
            this.DbContext.Setup(x => x.Topics).Returns(m.Object);
        });

        public Mock<ISubscriptionsRepository> DbSubscriptionsRepo => this.GetMock<ISubscriptionsRepository>(m =>
        {
            this.DbContext.Setup(x => x.Subscriptions).Returns(m.Object);
        });

        public Mock<IWordNormalizer> WordNormalizer => this.GetMock<IWordNormalizer>(m =>
        {
            this.ServiceContext.Setup(x => x.WordNormalizer).Returns(m.Object);
        });

        public Mock<ISecureRandom> SecureRandom => this.GetMock<ISecureRandom>(m =>
        {
            this.ServiceContext.Setup(x => x.SecureRandom).Returns(m.Object);
        });

        public Mock<ILearningSessionContext> SessionContext => this.GetMock<ILearningSessionContext>(m =>
        {
            this.RequestContext.Setup(x => x.Session).Returns(m.Object);
        });

        public Mock<ILearningSessionsManager> SessionManager => this.GetMock<ILearningSessionsManager>(m =>
        {
            this.ServiceContext.Setup(x => x.SessionsManager).Returns(m.Object);
        });

        public Mock<ILearningContentDeliveryManager> DeliveryManager => this.GetMock<ILearningContentDeliveryManager>(m =>
        {
            this.ServiceContext.Setup(x => x.DeliveryManager).Returns(m.Object);
        });
    }
}
