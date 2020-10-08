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
    public class ListItemsSteps : TechTalk.SpecFlow.Steps
    {

        IWebElement _resourceItem;

        [When(@"Item with '(.*)' title presented")]
        public void WhenItemWithTitlePresentedStep(string text)
        {
            var xpath = $"//div[@id='list']/div[contains(./@class,'list-group-item') and contains(.//div[@class='row'][1]/div[@class='col'][1]/h5,'{text}')]";
            _resourceItem = Web.Wait.UntilNotObscured(Some.ElementIsVisible(By.XPath(xpath)));
        }

        [When(@"Clicked on this item")]
        public void WhenClickedOnThisItemStep()
        {
            _resourceItem.Click();
        }

        [When(@"Shevron-down clicked on this item for full text")]
        public void WhenShevronDownTextFullStep()
        {
            _resourceItem.FindElement(By.XPath("//button[contains(.//*[local-name()='svg']/@class,'bi-chevron-double-down')]")).Click();
            var shevronLength1 = _resourceItem.Text.Length;
            _resourceItem.FindElement(By.XPath("//button[contains(.//*[local-name()='svg']/@class,'bi-chevron-compact-up')]")).Click();
            var shevronLength2 = _resourceItem.Text.Length;
            Assert.IsTrue(shevronLength1 > shevronLength2);
        }

        [When(@"Only items with '(.*)' title presented")]
        public void WhenOnlyItemWithTitlePresentedStep(string text)
        {
            var listItems = Web.Driver.FindElements(By.XPath($"//div[@id='list']/div[contains(./@class,'list-group-item')]"));
            Assert.IsTrue(listItems.All(item => item.FindElements(By.XPath($".[contains(.//div[@class='row'][1]/div[@class='col'][1]/h5,'{text}')]")) != null));
        }

        [When(@"Only items with '(.*)' tags presented")]
        public void WhenOnlyResourceWithTagsPresentedStep(string wantedTag)
        {
            var listItems = Web.Driver.FindElements(By.XPath($"//div[@id='list']/div[contains(./@class,'list-group-item')]"));
            Assert.IsTrue(listItems.All(item => item.FindElement(By.XPath($".//small/em[contains(.,'{wantedTag}')]")) != null));
        }

        [When(@"Items with '(.*)' tags presented")]
        public void WhenResourceWithTagsPresentedStep(string wantedTag)
        {
            var listItems = Web.Driver.FindElements(By.XPath($"//div[@id='list']/div[contains(./@class,'list-group-item')]"));
            Assert.IsTrue(listItems.Any(item => item.FindElement(By.XPath($"//small/em[contains(.,'{wantedTag}')]")) != null));
        }


        [When(@"Items with '(.*)' title not presented")]
        public void WhenResourceWithTitleNotPresentedStep(string text)
        {
            var listitems = Web.Driver.FindElements(By.XPath($"//div[@id='list']/div[contains(./@class,'list-group-item')]"));
            Assert.IsTrue(listitems.All(item => item.FindElements(By.XPath($"//div[@class='col'][1]/h5[contains(.,'{text}')]")).Count == 0));
        }

        [When(@"This resource has '(.*)' badge")]
        public void WhenThisResourceHasBadgeStep(string text)
        {
            Assert.IsTrue(_resourceItem.FindElements(By.XPath($".//span[contains(./@class,'badge') and contains(.,'{text}')]")).Count > 0);
        }

        [When(@"This resource has no '(.*)' badge")]
        public void WhenThisResourceHasNoBadgeStep(string text)
        {
            Assert.IsFalse(_resourceItem.FindElements(By.XPath($".//span[contains(./@class,'badge') and contains(.,'{text}')]")).Count > 0);
        }

        [When(@"This resource has '(.*)' button")]
        public void WhenThisResourceHasButtonStep(string text)
        {
            Web.Wait.UntilNotObscured(d => _resourceItem.FindElement(By.XPath($".//button[contains(.,'{text}')]")));
        }

        [When(@"This resource has no '(.*)' button")]
        public void WhenThisResourceHasNoButtonStep(string text)
        {
            Web.Wait.Until(d => _resourceItem.FindElements(By.XPath($".//button[contains(.,'{text}')]")).Count == 0);
        }
    }
}
