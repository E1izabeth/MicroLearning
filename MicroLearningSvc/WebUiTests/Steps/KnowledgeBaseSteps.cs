#pragma warning disable CS0618 // Type or member is obsolete
#pragma warning disable IDE0009 // Type or member is obsolete
using Google.Protobuf.WellKnownTypes;
using MicroLearningSvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;
using System;
using System.Linq;
using TechTalk.SpecFlow;
using WebUiTests.Util;
using Some = OpenQA.Selenium.Support.UI.ExpectedConditions;

namespace WebUiTests.Steps
{
    [Binding]
    public class KnowledgeBaseFeatureSteps : TechTalk.SpecFlow.Steps
    {
        [Given(@"User logged in")]
        public void GivenUserLoggedIn()
        {
            When("Input having placeholder 'Enter login' recieved typing 'panama2'");
            When("Input having placeholder 'Enter password' recieved typing '1234567890-'");
            When("'Sign-in!' button clicked");
        }


        [Then(@"Create resource modal appeared")]
        public void ThenModalAppearedStep()
        {
            Web.Wait.Until(Some.ElementExists(By.CssSelector("#modal-create-resource___BV_modal_content_")));
        }

        [Then(@"Create resource modal disappeared")]
        public void ThenModalDisappearedStep()
        {
            Web.Wait.Until(Some.InvisibilityOfElementLocated(By.CssSelector("#modal-create-resource___BV_modal_content_")));
        }

    }
}
