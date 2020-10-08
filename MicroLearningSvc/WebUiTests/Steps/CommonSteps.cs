#pragma warning disable CS0618 // Type or member is obsolete
#pragma warning disable IDE0009 // Type or member is obsolete
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
    public class CommonSteps : TechTalk.SpecFlow.Steps
    {
        [When(@"Tab '(.*)' available")]
        public void WhenTabAvailableStep(string tabName)
        {
            Web.Wait.UntilNotObscured(Some.ElementIsVisible(By.LinkText(tabName)));
        }

        [When(@"Button '(.*)' available")]
        public void WhenButtonAvailableStep(string btnName)
        {
            Web.Wait.UntilNotObscured(Some.ElementToBeClickable(By.XPath($"//button[contains(., '{btnName}')]")));
        }

        //[When(@"Button '(.*)' unavailable")]
        //public void WhenButtonUnavailableStep(string btnName)
        //{
        //    Web.Wait.un(Some.ElementToBeClickable(By.XPath($"//button[contains(., '{btnName}')]")));
        //}

        [When(@"Input having placeholder '(.*)' recieved typing '(.*)'")]
        public void WhenFillInputStep(string placeholderText, string typing = "")
        {
            Web.Driver.FindElement(By.XPath($"//input[@placeholder='{placeholderText}']")).SendKeys(typing);
        }

        [When(@"Input having placeholder '(.*)' contains value '(.*)'")]
        public void ThenCheckInputStep(string placeholderText, string typing = "")
        {
            Assert.AreEqual(Web.Driver.FindElement(By.XPath($"//input[@placeholder='{placeholderText}']")).GetProperty("value"), typing);
        }

        [When(@"Input having placeholder '(.*)' recieved empty typing")]
        public void WhenFillInputStep(string placeholderText)
        {
            Web.Driver.FindElement(By.XPath($"//input[@placeholder='{placeholderText}']")).SendKeys(string.Empty);
        }

        [When(@"Input having placeholder '(.*)' contains empty value")]
        public void ThenCheckInputStep(string placeholderText)
        {
            Assert.IsTrue(Web.Driver.FindElement(By.XPath($"//input[@placeholder='{placeholderText}']")).GetProperty("value").Trim().Length == 0);
        }

        [When(@"'(.*)' button clicked")]
        public void WhenClickButtonStep(string btnName)
        {
            Web.Driver.FindElement(By.XPath($"//button[contains(.,'{btnName}')]")).Click();
        }

        [Then(@"Input having placeholder '(.*)' is not valid")]
        public void ThenTextInputIsNotValidStep(string placeholderText)
        {
            var input = Web.Driver.FindElement(By.XPath($"//input[@placeholder='{placeholderText}']"));
            Assert.IsNotNull(input);
            var isInvalid = !Web.Driver.ExecuteJavaScript<bool>("return arguments[0].validity.valid", input)
                                || input.GetAttribute("class").Contains("is-invalid");
            Assert.IsTrue(isInvalid);
        }

        [Then(@"Alert '(.*)' appeared")]
        public void ThenAlertAppearedStep(string errorMessage)
        {
            Web.Wait.UntilNotObscured(Some.ElementIsVisible(By.CssSelector(".alert")));
            Assert.AreEqual(Web.Driver.FindElement(By.CssSelector(".alert")).GetProperty("innerText"), errorMessage);
        }

        [Then(@"Alert disappeared")]
        public void ThenAlertDisappeareddStep()
        {
            Web.Wait.Until(Some.InvisibilityOfElementLocated(By.CssSelector(".alert")));
        }

        [Then(@"Toast with text '(.*)' appeared")]
        public void ThenToastAppearedStep(string desiredText)
        {
            var xpath = $"//div[@class='toast']/div[@class='toast-body']";
            var toastBodyElement = Web.Wait.UntilNotObscured(Some.ElementIsVisible(By.XPath(xpath)));
            var toastText = toastBodyElement.Text;
            Assert.IsTrue(toastText.Contains(desiredText));
        }

        [When(@"Toast with text '(.*)' closed")]
        public void WhenToastClosedStep(string toastText)
        {
            var xpath = $"//div[@class='toast']/div[@class='toast-body' and contains(.,'{toastText}')]/../header/button[contains(./@class, 'close')]";
            Web.Driver.FindElement(By.XPath(xpath)).Click();
        }

        [Then(@"Toast with text '(.*)' disappeared")]
        public void ThenToastDisappearedStep(string toastText)
        {
            var xpath = $"//div[@class='toast']/div[@class='toast-body' and contains(.,'{toastText}')]";
            Web.Wait.Until(Some.InvisibilityOfElementLocated(By.XPath(xpath)));
        }

        [When(@"Link '(.*)' clicked")]
        public void WhenClickLinkStep(string linkText)
        {
            Web.Driver.FindElement(By.LinkText(linkText)).Click();
        }

        [When(@"Text '(.*)' presented on page")]
        public void WhenTextPresentedOnPageStep(string text)
        {
            Web.Wait.Until(Some.ElementIsVisible(By.XPath($"//*[contains(., '{text}')]")));
            Assert.IsTrue(Web.Driver.FindElements(By.XPath($"//*[contains(., '{text}')]")).Count > 0);
        }

        [When(@"Text '(.*)' not presented on page")]
        public void WhenTextNotPresentedOnPageStep(string text)
        {
            Assert.IsFalse(Web.Driver.FindElements(By.XPath($"//*[contains(., '{text}')]")).Count > 0);
        }

        [Then(@"Page refreshed")]
        public void ThenPageRefreshedStep()
        {
            Web.Driver.Navigate().Refresh();
        }

        [When(@"'(.*)' tab opened")]
        public void WhenTabOpenedStep(string tabName)
        {
            When($"Tab '{tabName}' available");
            When($"Link '{tabName}' clicked");
            When($"Tab '{tabName}' available");
        }

        [When(@"Input having id '(.*)' recieved typing '(.*)'")]
        public void WhenInputHavingIdRecievedTyping(string id, string typing)
        {
            Web.Driver.FindElement(By.Id(id)).SendKeys(typing);
        }

        [When(@"Input having id '(.*)' recieved empty typing")]
        public void WhenInputHavingIdRecievedTyping(string id)
        {
            Web.Driver.FindElement(By.Id(id)).SendKeys(string.Empty);
        }

        [Then(@"Input having id '(.*)' is not valid")]
        public void ThenTextInputWithIdIsNotValidStep(string id)
        {
            var input = Web.Driver.FindElement(By.XPath($"//input[@id='{id}']"));
            Assert.IsNotNull(input);
            var isInvalid = !Web.Driver.ExecuteJavaScript<bool>("return arguments[0].validity.valid", input) || input.GetAttribute("class").Contains("is-invalid");
            Assert.IsTrue(isInvalid);
        }
    }
}
