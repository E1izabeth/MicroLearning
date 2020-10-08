using MicroLearningSvc.Db;
using MicroLearningSvc.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace MicroLearningSvc.Impl
{
    class BasicOperationContext : IDisposable, IBasicOperationContext
    {
        protected readonly DisposableList _disposables = new DisposableList();

        private readonly ILearningServiceContext _svcCtx;

        ILearningDbContext _dbContext = null;
        public ILearningDbContext Db { get { return _dbContext ?? (_dbContext = this.OpenDb()); } }

        // private TransactionScope _transaction = null;

        public BasicOperationContext(ILearningServiceContext svcCtx)
        {
            _svcCtx = svcCtx;
        }

        private ILearningDbContext OpenDb()
        {
            var ctx = _svcCtx.OpenDb();
            _disposables.Add(ctx.Raw.Connection);
            _disposables.Add(ctx.Raw);
            //_transaction = _disposables.Add(new TransactionScope(
            //    TransactionScopeOption.Required,
            //    new TransactionOptions
            //    {
            //        IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            //    }
            //));
            return ctx;
        }

        public void Dispose()
        {
            //if (_transaction != null)
            //    _transaction.Complete();

            _disposables.SafeDispose();
        }
    }

    class LearningRequestContext : BasicOperationContext, ILearningRequestContext
    {
        public ILearningSessionContext Session { get; private set; }

        public LearningRequestContext(ILearningServiceContext svcCtx, ILearningSessionContext session)
            : base(svcCtx)
        {
            this.Session = session;
        }

        public void ValidateAuthorized()
        {
            this.ValidateAuthorized(true);
        }

        public void ValidateAuthorized(bool requireActivated = true)
        {
            if (this.Session.UserId == 0)
                throw new WebFaultException(HttpStatusCode.Forbidden);

            if (requireActivated && !this.Db.Users.GetUserById(this.Session.UserId).Activated)
                throw new WebFaultException(HttpStatusCode.Forbidden);
        }
    }
}
