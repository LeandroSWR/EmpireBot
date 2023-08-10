using OpenQA.Selenium;
using System.Collections.Generic;

namespace CSGOEmpireBot
{
    internal class Analyzer
    {
        private readonly IWebDriver _driver;
        // The div data-v might change as the website is updated
        private readonly string selector = "div[data-v-851a155c].font-numeric.text-2xl.font-bold";
        
        public Analyzer(IWebDriver driver)
        {
            _driver = driver;
        }

        public IReadOnlyList<IWebElement> GetDivElements() =>
            _driver.FindElements(By.CssSelector(selector));

        public List<IWebElement> GetPreviousRolls() =>
            _driver.FindElements(
                    By.CssSelector("div.previous-rolls-item"))
                    .Take(10)
                    .Select(parent => parent.FindElement(By.CssSelector("div:nth-child(1)"))).ToList();
    }
}
