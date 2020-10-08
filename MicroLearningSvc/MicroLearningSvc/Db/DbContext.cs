using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroLearningSvc.Db
{
    interface IDbContext : IDisposable
    {
        Table<DbUserInfo> Users { get; }
        Table<DbResourceInfo> Resources { get; }
        Table<DbResourceContentPortionInfo> ResourceContentPortions { get; }
        Table<DbResourceAssociationInfo> ResourceAssociations { get; }
        Table<DbTagInfo> Tags { get; }
        Table<DbTagInstanceInfo> TagInstances { get; }
        Table<DbTopicInfo> Topics { get; }
        Table<DbTopicAssociationInfo> TopicAssociations { get; }
        Table<DbUserSubscriptionInfo> UserSubscriptions { get; }
        Table<DbUserSubscriptionContentPortionInfo> UserSubscriptionContentPortions { get; }

        IDbConnection Connection { get; }

        bool DatabaseExists();
        void CreateDatabase();
    }

    class DbContext : DataContext, IDbContext
    {
        public Table<DbUserInfo> Users { get; }
        public Table<DbResourceInfo> Resources { get; }
        public Table<DbResourceContentPortionInfo> ResourceContentPortions { get; }
        public Table<DbResourceAssociationInfo> ResourceAssociations { get; }
        public Table<DbTagInfo> Tags { get; }
        public Table<DbTagInstanceInfo> TagInstances { get; }
        public Table<DbTopicInfo> Topics { get; }
        public Table<DbTopicAssociationInfo> TopicAssociations { get; }
        public Table<DbUserSubscriptionInfo> UserSubscriptions { get; }
        public Table<DbUserSubscriptionContentPortionInfo> UserSubscriptionContentPortions { get; }

        IDbConnection IDbContext.Connection { get { return base.Connection; } }

        public DbContext(IDbConnection cnn)
            : base(cnn)
        {
            this.Users = base.GetTable<DbUserInfo>();
            this.Users = base.GetTable<DbUserInfo>();
            this.Resources = base.GetTable<DbResourceInfo>();
            this.ResourceContentPortions = base.GetTable<DbResourceContentPortionInfo>();
            this.ResourceAssociations = base.GetTable<DbResourceAssociationInfo>();
            this.Tags = base.GetTable<DbTagInfo>();
            this.TagInstances = base.GetTable<DbTagInstanceInfo>();
            this.Topics = base.GetTable<DbTopicInfo>();
            this.TopicAssociations = base.GetTable<DbTopicAssociationInfo>();
            this.UserSubscriptions = base.GetTable<DbUserSubscriptionInfo>();
            this.UserSubscriptionContentPortions = base.GetTable<DbUserSubscriptionContentPortionInfo>();
        }
    }

    [Table]
    class DbUserInfo
    {
        [Column(IsPrimaryKey = true, AutoSync = AutoSync.OnInsert, DbType = "BIGINT NOT NULL IDENTITY", IsDbGenerated = true)]
        public long Id { get; set; }
        [Column(DbType = "nvarchar(150)")]
        public string Login { get; set; }
        [Column(DbType = "nvarchar(150)")]
        public string LoginKey { get; set; }
        [Column]
        public DateTime RegistrationStamp { get; set; }
        [Column]
        public DateTime LastLoginStamp { get; set; }

        [Column(DbType = "nvarchar(150)")]
        public string PasswordHash { get; set; }
        [Column(DbType = "nvarchar(150)")]
        public string HashSalt { get; set; }

        [Column(DbType = "nvarchar(150)")]
        public string Email { get; set; }
        [Column]
        public bool Activated { get; set; }

        [Column(DbType = "nvarchar(150)")]
        public string LastToken { get; set; }
        [Column]
        public DateTime LastTokenStamp { get; set; }
        [Column]
        public DbUserTokenKind LastTokenKind { get; set; }

        [Column]
        public bool IsDeleted { get; set; }
    }

    public enum DbUserTokenKind
    {
        Activation,
        AccessRestore
    }


    [Table]
    class DbResourceInfo
    {
        [Column(IsPrimaryKey = true, AutoSync = AutoSync.OnInsert, DbType = "BIGINT NOT NULL IDENTITY", IsDbGenerated = true)]
        public long Id { get; set; }
        [Column(DbType = "nvarchar(150)")]
        public string Url { get; set; }
        [Column(DbType = "nvarchar(150)")]
        public string Title { get; set; }
        [Column]
        public long AuthorUserId { get; set; }
        [Column]
        public DateTime CreationStamp { get; set; }

        [Column]
        public bool IsValidated { get; set; }
        [Column]
        public long ValidatorUserId { get; set; }
        [Column]
        public DateTime ValidationStamp { get; set; }
    }

    [Table]
    class DbResourceContentPortionInfo
    {
        [Column(IsPrimaryKey = true, AutoSync = AutoSync.OnInsert, DbType = "BIGINT NOT NULL IDENTITY", IsDbGenerated = true)]
        public long Id { get; set; }
        [Column]
        public long ResourceId { get; set; }
        [Column]
        public long Order { get; set; }
        [Column(DbType = "nvarchar(max)")]
        public string TextContent { get; set; }
    }

    [Table]
    class DbResourceAssociationInfo
    {
        [Column(IsPrimaryKey = true, AutoSync = AutoSync.OnInsert, DbType = "BIGINT NOT NULL IDENTITY", IsDbGenerated = true)]
        public long Id { get; set; }
        [Column]
        public long TagInstanceId { get; set; }
        [Column]
        public long ResourceId { get; set; }
    }


    [Table]
    class DbTagInfo
    {
        [Column(IsPrimaryKey = true, AutoSync = AutoSync.OnInsert, DbType = "BIGINT NOT NULL IDENTITY", IsDbGenerated = true)]
        public long Id { get; set; }
    }

    [Table]
    class DbTagInstanceInfo
    {
        [Column(IsPrimaryKey = true, AutoSync = AutoSync.OnInsert, DbType = "BIGINT NOT NULL IDENTITY", IsDbGenerated = true)]
        public long Id { get; set; }

        [Column]
        public long TagId { get; set; }

        [Column(DbType = "nvarchar(150)")]
        public string Word { get; set; }
    }

    [Table]
    class DbTopicInfo
    {
        [Column(IsPrimaryKey = true, AutoSync = AutoSync.OnInsert, DbType = "BIGINT NOT NULL IDENTITY", IsDbGenerated = true)]
        public long Id { get; set; }

        [Column]
        public long AuthorUserId { get; set; }
        [Column]
        public DateTime CreationStamp { get; set; }

        [Column(DbType = "nvarchar(150)")]
        public string Title { get; set; }
    }

    [Table]
    class DbTopicAssociationInfo
    {
        [Column(IsPrimaryKey = true, AutoSync = AutoSync.OnInsert, DbType = "BIGINT NOT NULL IDENTITY", IsDbGenerated = true)]
        public long Id { get; set; }
        [Column]
        public long TagInstanceId { get; set; }
        [Column]
        public long TopicId { get; set; }
    }


    [Table]
    class DbUserSubscriptionInfo
    {
        [Column(IsPrimaryKey = true, AutoSync = AutoSync.OnInsert, DbType = "BIGINT NOT NULL IDENTITY", IsDbGenerated = true)]
        public long Id { get; set; }

        [Column]
        public long UserId { get; set; }
        [Column]
        public long TopicId { get; set; }
        [Column]
        public bool IsActive { get; set; }
        [Column(Name = "Interval")]
        public long DbIntervalTicks { get; set; }
        
        public TimeSpan Interval
        {
            get { return new TimeSpan(this.DbIntervalTicks); }
            set { this.DbIntervalTicks = value.Ticks; }
        }
        
        [Column]
        public DateTime NextPortionTime { get; set; }
    }

    [Table]
    class DbUserSubscriptionContentPortionInfo
    {
        [Column(IsPrimaryKey = true, AutoSync = AutoSync.OnInsert, DbType = "BIGINT NOT NULL IDENTITY", IsDbGenerated = true)]
        public long Id { get; set; }

        [Column]
        public long UserSubscriptionId { get; set; }
        [Column]
        public long ResourceContentPortionId { get; set; }
        [Column]
        public bool IsLearned { get; set; }
        [Column]
        public DateTime LearnedStamp { get; set; }
        [Column]
        public bool IsDelivered { get; set; }
        [Column]
        public DateTime DeliveredStamp { get; set; }
        [Column]
        public bool IsMarkedToRepeat { get; set; }
    }

}
