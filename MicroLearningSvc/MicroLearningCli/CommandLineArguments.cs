using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroLearningCli
{
    class CommandLineArguments
    {
        [ArgAlias("svc")]
        [ArgDescription("(url) Where the learning service is located")]
        public string ServiceUrl { get; set; }

        [ArgAlias("url")]
        [ArgDescription("(cmd url) Suggest information about resource")]
        public string ResourceUrl { get; set; }

        [ArgAlias("cr")]
        [ArgDescription("(cmd title) Register new resource having given tags and url")]
        public string CreateResource { get; set; }

        [ArgAlias("r")]
        [ArgDescription("(cmd resourceId) Retrieve information about resource")]
        public long GetResource { get; set; }

        [ArgAlias("lr")]
        [ArgDescription("(cmd) Retrieve list of resources having given tags")]
        public bool GetResourcesList { get; set; }

        [ArgAlias("rv")]
        [ArgDescription("(cmd) Mark resource as validated so that it'll show up in topic's content")]
        public bool MarkResourceValidated { get; set; }

        [ArgAlias("ct")]
        [ArgDescription("(cmd title) Create new topic having given tags")]
        public string CreateTopic { get; set; }

        [ArgAlias("t")]
        [ArgDescription("(cmd topicId) Retrieve information about topic")]
        public long GetTopic { get; set; }

        [ArgAlias("lt")]
        [ArgDescription("(cmd) Retrieve list of topics")]
        public bool GetTopicsList { get; set; }

        [ArgAlias("c")]
        [ArgDescription("(cmd) Retrieve content parts of the topic")]
        public bool GetTopicContent { get; set; }

        [ArgAlias("s")]
        [ArgDescription("(cmd interval) Subscribe for a topic having interval of minutes betwee content parts to learn")]
        public long Subscribe { get; set; }

        [ArgAlias("ml")]
        [ArgDescription("(cmd contentPartId) Mark topic content part as learned")]
        public long MarkLearned { get; set; }

        [ArgAlias("mu")]
        [ArgDescription("(cmd contentPartId) Mark topic content part as unlearned")]
        public long MarkUnlearned { get; set; }

        [ArgAlias("u")]
        [ArgDescription("(cmd) Suspend topic subscription")]
        public bool Unsubscribe { get; set; }

        [ArgAlias("d")]
        [ArgDescription("(cmd) Drop topic learning progress")]
        public bool DropTopicLearning { get; set; }

        [ArgAlias("reg")]
        [ArgDescription("(cmd) Regiser new user having given login, password and email")]
        public bool Register { get; set; }

        [ArgAlias("rec")]
        [ArgDescription("(cmd) Request access recovery by given login and email")]
        public bool Recover { get; set; }

        [ArgAlias("a")]
        [ArgDescription("(cmd token) Activate user profile")]
        public string Activate { get; set; }

        [ArgAlias("ra")]
        [ArgDescription("(cmd token) Restore access to user profile")]
        public string RestoreAccess { get; set; }

        [ArgAlias("l")]
        [ArgDescription("(arg) Login to use (optional)")]
        public string Login { get; set; }

        [ArgAlias("del")]
        [ArgDescription("(cmd) Delete current profile")]
        public bool DeleteProfile { get; set; }

        [ArgAlias("p")]
        [ArgDescription("(arg) Password to use (optional)")]
        public string Password { get; set; }

        [ArgAlias("e")]
        [ArgDescription("(arg) Email to use")]
        public string Email { get; set; }

        [ArgAlias("tags")]
        [ArgDescription("(arg) Tags to use for topic or resource")]
        public string AssociationTags { get; set; }

        [ArgDescription("(cmd) Remember service location url")]
        public bool StoreServiceUrl { get; set; }

        [ArgDescription("(cmd) Remember credentials login-password")]
        public bool StoreCreds { get; set; }

        [ArgDescription("(cmd) Remember default login to use")]
        public bool SetDefaultLogin { get; set; }

        [ArgAlias("h")]
        [ArgDescription("(cmd) Show help")]
        public bool Help { get; set; }

        [ArgAlias("dbg")]
        [ArgDescription("(cmd) Attach debugger to the cli process")]
        public bool LaunchDebugger { get; set; }
    }
}
