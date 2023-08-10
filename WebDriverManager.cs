using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace CSGOEmpireBot
{
    internal class WebDriverManager
    {
        public IWebDriver Driver { get; private set; }

        public WebDriverManager()
        {
            ChromeOptions options = new ChromeOptions();
            options.SetLoggingPreference(LogType.Browser, LogLevel.Off);
            options.SetLoggingPreference(LogType.Driver, LogLevel.Off);
            options.AddArgument("--log-level=3");

            ChromeDriverService driverService = ChromeDriverService.CreateDefaultService();
            driverService.HideCommandPromptWindow = true;
            driverService.SuppressInitialDiagnosticInformation = true;

            Driver = new ChromeDriver(driverService, options);
            Driver.Url = "https://csgoempire.com";
        }
    }
}
