using MicroLearningSvc;
using MicroLearningSvc.Impl;
using MicroLearningSvc.Interaction;
using MicroLearningSvc.Util;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using TechTalk.SpecFlow;

namespace WebUiTests.Util
{
    [Binding]
    public sealed class ServiceContext
    {
        public const string TestDbConnectionString = @"Data Source=localhost\SQLEXPRESS;Initial Catalog=learningapp-testdb1;Integrated Security=True;MultipleActiveResultSets=True;TransparentNetworkIPResolution=False";
        public const string TestServiceHostUrl = @"http://0.0.0.0:8181/mysvc/";

        private static readonly LearningServiceConfiguration _testServiceConfiguration = new LearningServiceConfiguration()
        {
            DbConnectionString = TestDbConnectionString,
            SessionTimeout = TimeSpan.FromHours(1),
            LangModelsDirPath = @"D:\Temp\udpipe-ud-2.5-191206",
            TokenTimeout = TimeSpan.FromDays(7),
            DeliveryTimeout = TimeSpan.FromMinutes(15),
            ServiceHostUrl = TestServiceHostUrl,

            SmtpLogin = "learningsvc@localhost",
            SmtpServerHost = "127.0.0.1",
            SmtpServerPort = 25,
            SmtpUseSsl = false,
            SmtpUseDefaultCredentials = true,
            SmtpDeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory,
            SmtpPickupDirectoryLocation = @"D:\Temp\"
        };

        private static readonly DisposableList _disposables = new DisposableList();

        [BeforeTestRun(Order = 1000)]
        public static void Setup()
        {
            _disposables.Add(new MicroLearningService(_testServiceConfiguration));
        }

        [AfterTestRun]
        public static void Cleanup()
        {
            _disposables.SafeDispose();
        }
    }
}
