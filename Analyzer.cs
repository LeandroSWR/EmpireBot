using OpenQA.Selenium;

namespace CSGOEmpireBot
{
    public class Analyzer
    {
        private readonly IWebDriver _driver;
        // The div data-v might change as the website is updated
        private readonly string selector = "div[data-v-851a155c].font-numeric.text-2xl.font-bold";
        
        public Analyzer(IWebDriver driver)
        {
            _driver = driver;
        }

        public string GetRollingText()
        {
            // Find the div element
            IReadOnlyList<IWebElement> divElements = _driver.FindElements(By.CssSelector(selector));

            try
            {
                if (divElements.Count > 0)
                {
                    return divElements[0].Text;
                }
            }
            catch (StaleElementReferenceException)
            {
                // Handle the StaleElementReferenceException by re-finding the element
                divElements = _driver.FindElements(By.CssSelector(selector));

                if (divElements.Count > 0)
                {
                    return divElements[0].Text;
                }
            }

            return "Uninitialized";
        }

        public List<IWebElement> GetPreviousRolls() =>
            _driver.FindElements(
                    By.CssSelector("div.previous-rolls-item"))
                    .Take(10)
                    .Select(parent => parent.FindElement(By.CssSelector("div:nth-child(1)"))).ToList();
    }
}
