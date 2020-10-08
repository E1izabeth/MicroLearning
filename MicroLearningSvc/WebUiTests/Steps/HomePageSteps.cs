#pragma warning disable CS0618 // Type or member is obsolete
#pragma warning disable IDE0009 // Type or member is obsolete
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;
using System;
using TechTalk.SpecFlow;
using WebUiTests.Util;
using Some = OpenQA.Selenium.Support.UI.ExpectedConditions;

namespace WebUiTests.Steps
{
    [Binding]
    public class HomePageSteps : TechTalk.SpecFlow.Steps
    {
        [When(@"Recover form filled with login '(.*)', email '(.*)' and email repeat '(.*)'")]
        public void WhenRecoverFormFilledStep(string login, string email1, string email2)
        {
            When($"Input having placeholder 'Enter login' recieved typing '{login}'");
            When($"Input having placeholder 'Enter email' recieved typing '{email1}'");
            When($"Input having placeholder 'Repeat email' recieved typing '{email2}'");
        }

        [When(@"Register form filled with login '(.*)', email '(.*)', password '(.*)' and password repeat '(.*)'")]
        public void WhenRegisterFormFilledStep(string login, string email, string pwd1, string pwd2)
        {
            When($"Input having placeholder 'Enter login' recieved typing '{login}'");
            When($"Input having placeholder 'Enter email' recieved typing '{email}'");
            When($"Input having placeholder 'Enter password' recieved typing '{pwd1}'");
            When($"Input having placeholder 'Repeat password' recieved typing '{pwd2}'");
        }

        [Then(@"Profile login should be equal to '(.*)'")]
        public void ThenShouldBeEqualToStep(string loginToCheck)
        {
            Assert.AreEqual(Web.Driver.FindElement(By.CssSelector("#data-login")).GetProperty("value"), loginToCheck);
        }

        [Then(@"'(.*)' mode opened")]
        public void ThenModeWithNameOpenedStep(string cardHeaderName)
        {
            var cardHeader = Web.Driver.FindElement(By.CssSelector(".card-header > div:nth-child(1)"));
            Web.Wait.Until(Some.TextToBePresentInElement(cardHeader, cardHeaderName));
            Assert.AreEqual(cardHeader.GetProperty("innerText"), cardHeaderName);
        }

        [When(@"'(.*)' mode opened")]
        public void WhenModeWithNameOpenedStep(string cardHeaderName)
        {
            Assert.AreEqual(Web.Driver.FindElement(By.CssSelector(".card-header > div:nth-child(1)")).GetProperty("innerText"), cardHeaderName);
        }
    }
}
