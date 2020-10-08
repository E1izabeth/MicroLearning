#pragma warning disable CS0618 // Type or member is obsolete
using MicroLearningSvc;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using TechTalk.SpecFlow;

namespace WebUiTests.Util
{
    [Binding]
    public sealed class WebContext
    {
        private readonly ScenarioContext _context;
        IWebDriver _driver;

        public WebContext(ScenarioContext injectedContext)
        {
            _context = injectedContext;
        }

        [BeforeScenario(Order = 10000)]
        public void Setup()
        {
            _driver = new OpenQA.Selenium.Firefox.FirefoxDriver();
            _context.Add("webDriver", _driver);

            _driver.Navigate().GoToUrl("http://localhost:8080/");
            _driver.Manage().Window.Size = new System.Drawing.Size(1311, 1045);
        }

        [AfterScenario]
        public void Cleanup()
        {
            _context.Remove("webDriver");
            _driver.SafeDispose();
        }
    }

    public static class Web
    {
        /*
        Just because I don't use Seleium RC and my tests running stnchonously.
        (From documentation: "In some cases sharing a state through static fields can be an efficient solution".)
        */
        public static IWebDriver Driver { get { return ScenarioContext.Current.Get<IWebDriver>("webDriver"); } }

        public static WebDriverWait WaitSeconds(int seconds) { return new WebDriverWait(Web.Driver, TimeSpan.FromSeconds(seconds)); }
        public static WebDriverWait WaitFor(TimeSpan timeout) { return new WebDriverWait(Web.Driver, timeout); }
        public static WebDriverWait Wait { get { return new WebDriverWait(Web.Driver, TimeSpan.FromSeconds(60)); } }

        public static IWebElement ForElement(this WebDriverWait wait, By selector)
        {
            return wait.Until(d => d.FindElement(selector));
        }

        public static IWebElement ForElement(this WebDriverWait wait, By selector, Func<IWebElement, bool> cond)
        {
            return wait.Until(d =>
            {
                var element = d.FindElement(selector);
                return cond(element) ? element : null;
            });
        }

        //wait until element is <cond> and not obscured
        public static IWebElement UntilNotObscured(this WebDriverWait wait, Func<IWebDriver, IWebElement> cond)
        {
            return wait.Until(d =>
            {
                var element = cond(d);
                if (element == null)
                    return null;
                
                var s1 = d.GetElementSelectorImpl(element);

                var loc = d.GetElementBoundingBox(element);
                if (!d.TryGetSelectorOfElementAt(loc.X + 1, loc.Y + 2, wait.Timeout, out var s2))
                    return null;

                return s1 == s2 ? element : null;
            });
        }

        //get rectangle, describing the element position
        public static RectangleF GetElementBoundingBox(this IWebDriver driver, IWebElement element)
        {
            var o = driver.ExecuteJavaScript<Dictionary<string, object>>("return arguments[0].getBoundingClientRect()", element);
            var o2 = o.ToDictionary(kv => kv.Key, kv => (float)Convert.ChangeType(kv.Value, TypeCode.Single));
            var rect = new RectangleF(o2["x"], o2["y"], o2["width"], o2["height"]);
            return rect;
        }

        public static bool TryGetSelectorOfElementAt(this IWebDriver driver, float x, float y, TimeSpan timeout, out string selector)
        {
            var start = DateTime.Now;
            do
            {
                try
                {
                    var element = driver.ExecuteJavaScript<IWebElement>(@"return document.elementFromPoint(arguments[0], arguments[1])", x, y);
                    selector = driver.GetElementSelectorImpl(element);
                }
                catch
                {
                    selector = null;
                    System.Threading.Thread.Sleep(100);
                }
            } while (selector == null && start + timeout < DateTime.Now);

            return selector != null;
        }


        //because there is difference between two references for element (that are the same element) taken in two different moments of time in selenium 
        public static string GetElementSelectorImpl(this IWebDriver driver, IWebElement element)
        {
            var script = @"
                return (function(elt) {
                    function getElementIdx(elt)
                    {
                        var count = 1;
                        for (var sib = elt.previousSibling; sib; sib = sib.previousSibling)
                        {
                            if (sib.nodeType == 1 && sib.tagName == elt.tagName) 
                                count++;
                        }

                        return count;
                    }

                    var path = '';
                    for (; elt && elt.nodeType == 1; elt = elt.parentNode)
                    {
                        idx = getElementIdx(elt);
                        xname = elt.tagName;
                        if (idx > 1)
                            xname += '[' + idx + ']';
                        path = '/' + xname + path;
                    }

                    return path;   
                })(arguments[0])
                ";
            return driver.ExecuteJavaScript<string>(script, element);
        }
    }
}
