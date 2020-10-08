using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.ServiceModel;
using System.ServiceModel.Activation;
using HtmlAgilityPack;
using MicroLearningSvc.Impl;
using MicroLearningSvc.Interaction;
using MicroLearningSvc.Util;

namespace MicroLearningSvc
{


    class Program
    {
        //public IEnumerable<DocumentDto> GetDocumentsRange(int from, int count)
        //{
        //    var Db = new DbContext();
        //    return Db.Docs
        //             .Where(x => x.CreatedOn >= DateTime.Today && x.CreatedOn < DateTime.Today.AddDays(1))
        //             .Skip(from).Take(count)
        //             .Select(x => new DocumentDto { ...})
        //             .ToList();
        //}

        // --= LocalDb Controls =--
        // list the instancies
        //   sqllocaldb i
        // stop selected instance
        //   sqllocaldb p "selected instance"
        // delete
        //   sqllocaldb d "selected instance"
        // recreate or create new one 
        //   sqllocaldb c "new instance"

        static void Main(string[] args)
        {
            var cfg = new LearningServiceConfiguration()
            {
                // DbName = "appdata",
                // DbFileName = "mydbfilename4.mdf",
                DbConnectionString = @"Data Source=localhost\SQLEXPRESS;Initial Catalog=learningdb1;Integrated Security=True;MultipleActiveResultSets=True;TransparentNetworkIPResolution=False",
                //DbConnectionString = @"Data Source=localhost\SQLEXPRESS;Initial Catalog=learningapp-testdb1;Integrated Security=True;MultipleActiveResultSets=True;TransparentNetworkIPResolution=False",
                ServiceHostUrl = "http://0.0.0.0:8181/mysvc/",
                SessionTimeout = TimeSpan.FromHours(1),
                LangModelsDirPath = @"D:\Temp\udpipe-ud-2.5-191206",
                TokenTimeout = TimeSpan.FromDays(7),
                DeliveryTimeout = TimeSpan.FromMinutes(15),

                //SmtpServerHost = "smtp.live.com",
                //SmtpServerPort = 587,
                //SmtpLogin = "ipm.local@hotmail.com",
                //SmtpPassword = "ifmo2020",
                //SmtpUseSsl = true,
                //SmtpUseDefaultCredentials = false,
                //SmtpDeliveryMethod = SmtpDeliveryMethod.Network
                SmtpLogin = "learningsvc@localhost",
                SmtpServerHost = "127.0.0.1",
                SmtpServerPort = 25,
                SmtpUseSsl = false,
                SmtpUseDefaultCredentials = true,
                SmtpDeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory,
                SmtpPickupDirectoryLocation = @"D:\Temp\"
            };

            using (var svc = new MicroLearningService(cfg))
            { 
                Console.WriteLine();
                Console.WriteLine("Press Q to stop");
                while (Console.ReadKey().Key != ConsoleKey.Q) ;
            }
        }
    }
}
