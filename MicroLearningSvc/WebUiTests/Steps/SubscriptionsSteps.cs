using MicroLearningSvc;
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
    public class SubscriptionsSteps : TechTalk.SpecFlow.Steps
    {
        [When(@"List item with '(.*)' title presented")]
        public void WhenResourceWithTitlePresented(string text)
        {
            var xpath = $"//div[@id='list']/div[contains(./@class,'list-group-item') and contains(.//div[@class='row'][1]/div[@class='col'][1]/h5,'{text}')]";
            var item = Web.Wait.UntilNotObscured(Some.ElementIsVisible(By.XPath(xpath)));

            // bi-chevron-compact-up
            var shortText = item.FindElement(By.XPath(".//div/small/em")).Text;
            item.FindElement(By.XPath(".//button[contains(.//*[local-name()='svg']/@class,'bi-chevron-double-down')]")).Click();
            var longText = item.FindElement(By.XPath(".//div/small/em")).Text;
            Assert.IsTrue(shortText.Length < longText.Length);

        }


        //  Choose date 'Start date and time:' in future
        [Then(@"Choose date '(.*)' in future")]
        public void ChooseDateInFuture(string fieldsetLegendText)
        {
            var fieldset = Web.Driver.FindElement(By.XPath($"//fieldset[contains(./legend,'{fieldsetLegendText}')]"));
            fieldset.FindElement(By.XPath(".//button[contains(.//*[local-name()='svg']/@class,'bi-calendar')]")).Click();

            var now = DateTime.Now.AddDays(2).ToString("yyyy-MM-dd");
            Web.Wait.UntilNotObscured(Some.ElementToBeClickable(By.XPath($"//div[@data-date='{now}']"))).Click();

            fieldset.FindElement(By.XPath(".//button[contains(.//*[local-name()='svg']/@class,'bi-clock')]")).Click();
            var timeCloseBtn = Web.Wait.UntilNotObscured(Some.ElementToBeClickable(By.XPath("//footer[@class='b-time-footer']//button[contains(.,'Close')]")));
            var timeIncButtons = Web.Driver.FindElements(By.XPath("//button[@aria-label='Increment']"));
            timeIncButtons.ForEach(btn => btn.Click());
            timeCloseBtn.Click();

        }

        [Then(@"Choose interval '(.*)'")]
        public void ThenChooseIntervalStep(string interval)
        {
            Web.Driver.ExecuteJavaScript($"arguments[0].value = '{interval}'", Web.Driver.FindElement(By.XPath($"//input[@placeholder='Days:Hours:Minutes']")));
        }

        //  Choose date 'Start date and time:' in past
        [Then(@"Choose date '(.*)' in past")]
        public void ChooseDateInPast(string fieldsetLegendText)
        {
            var fieldset = Web.Driver.FindElement(By.XPath($"//fieldset[contains(./legend,'{fieldsetLegendText}')]"));
            fieldset.FindElement(By.XPath(".//button[contains(.//*[local-name()='svg']/@class,'bi-calendar')]")).Click();

            var now = DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd");
            Web.Wait.UntilNotObscured(Some.ElementToBeClickable(By.XPath($"//div[@data-date='{now}']"))).Click();

            fieldset.FindElement(By.XPath(".//button[contains(.//*[local-name()='svg']/@class,'bi-clock')]")).Click();
            var timeCloseBtn = Web.Wait.UntilNotObscured(Some.ElementToBeClickable(By.XPath("//footer[@class='b-time-footer']//button[contains(.,'Close')]")));
            var timeIncButtons = Web.Driver.FindElements(By.XPath("//button[@aria-label='Increment']"));
            timeIncButtons.ForEach(btn => btn.Click());
            timeCloseBtn.Click();

        }

        [When(@"Button '(.*)' disabled")]
        public void WhenButtonDisabledStep(string btnName)
        {
            Assert.IsFalse(Web.Driver.FindElement(By.XPath($"//button[contains(., '{btnName}')]")).Enabled);
        }

        [When(@"Button '(.*)' enabled")]
        public void WhenButtonEnabledStep(string btnName)
        {
            Assert.IsTrue(Web.Driver.FindElement(By.XPath($"//button[contains(., '{btnName}')]")).Enabled);
        }

    }
}
