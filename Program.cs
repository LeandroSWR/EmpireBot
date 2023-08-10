using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

class Program
{
    static void Main(string[] args)
    {
        Console.SetWindowSize(105, 58);
        Console.CursorVisible = false;

        // Set up Chrome options and create the WebDriver
        ChromeOptions options = new ChromeOptions();
        options.SetLoggingPreference(LogType.Browser, LogLevel.Off);
        options.SetLoggingPreference(LogType.Driver, LogLevel.Off);
        options.AddArgument("--log-level=3");
        //options.AddArgument("--headless"); // Run Chrome in headless mode (without GUI)

        ChromeDriverService driverService = ChromeDriverService.CreateDefaultService();
        driverService.HideCommandPromptWindow = true; // This hides the command prompt window
        driverService.SuppressInitialDiagnosticInformation = true; // This will suppress initial diagnostic information

        IWebDriver driver = new ChromeDriver(driverService, options);

        // Navigate to the website
        driver.Url = "https://csgoempire.com";

        // Wait for the page to load (you may need to adjust the time as needed)
        Thread.Sleep(2500);

        // Define the CSS selector for the div element
        string divSelector = "div[data-v-851a155c].font-numeric.text-2xl.font-bold";

        bool rollGoing = false;

        List<string> previousCoins = new List<string>();

        int ctTrain = 0;
        int tTrain = 0;
        int dTrain = 0;
        int ctDTrain = 0;
        int tDTrain = 0;
        int tCount = 0;
        int dCount = 0;
        int ctCount = 0;
        int trainCount = 0;

        Stopwatch noRollWatch = Stopwatch.StartNew();
        Stopwatch midRollWatch = Stopwatch.StartNew();
        // Run the loop until the application is closed
        while (true)
        {
            // Wait for a short period before checking again (you can adjust the time as needed)
            Thread.Sleep(10);

            // Update Roll info
            Console.SetCursorPosition(13, 41);
            Console.ForegroundColor = rollGoing ? ConsoleColor.Green : ConsoleColor.Red;
            Console.Write(rollGoing + " ");

            // Find the div element
            IReadOnlyList<IWebElement> divElements = driver.FindElements(By.CssSelector(divSelector));

            string? innerText = null;
            if (divElements.Count > 0)
            {
                try
                {
                    innerText = divElements[0].Text;
                }
                catch (StaleElementReferenceException)
                {
                    // Handle the StaleElementReferenceException by re-finding the element
                    divElements = driver.FindElements(By.CssSelector(divSelector));

                    if (divElements.Count > 0)
                    {
                        innerText = divElements[0].Text;
                    }
                }
            }

            if (innerText != null && (innerText == "0.00" || innerText == ""))//!backgroundPosition.Contains("451px"))
            {
                rollGoing = true;
            }

            // *** Handle Crashes *** (Website goes offline for a few seconds sometimes but the rolls don't stop)
            // Define the CSS selector for the SVG element
            string svgSelector = "svg[data-v-56be08fe].loader-spinner";

            // Find the SVG element
            IReadOnlyList<IWebElement> svgElements = driver.FindElements(By.CssSelector(svgSelector));

            bool hasElement = svgElements.Count > 0;

            if (rollGoing)
            {
                noRollWatch.Restart();
            }
            else if (!rollGoing)
            {
                midRollWatch.Restart();
            }

            // Check if the page source has changed, indicating a dice roll
            if (rollGoing && innerText != null && (innerText == "0.00" || innerText == ""))
            {
                List<IWebElement> previousRolls = driver.FindElements(
                    By.CssSelector("div.previous-rolls-item"))
                    .Take(10)
                    .Select(parent => parent.FindElement(By.CssSelector("div:nth-child(1)"))).ToList();

                // Create a list to store the child classes as strings
                if (previousCoins.Count == 0)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        previousCoins.Add(previousRolls[i].GetAttribute("class").Split(' ')[0]);
                    }
                }
                else
                {
                    previousCoins.Add(previousRolls[9].GetAttribute("class").Split(' ')[0]);
                }

                tCount = 0;
                ctCount = 0;
                dCount = 0;

                foreach (string coin in previousCoins)
                {
                    if (coin == "coin-t")
                    {
                        tCount++;
                    }
                    else if (coin == "coin-ct")
                    {
                        ctCount++;
                    }
                    else
                    {
                        dCount++;
                    }
                }

                // Check if the list contains the certain string
                IEnumerable<string> last10 = previousCoins.Skip(Math.Max(0, previousCoins.Count - 10));

                if (!last10.Contains("coin-ct") || !last10.Contains("coin-t"))
                {
                    // Perform the sound notification here (e.g., play a beep sound)
                    Console.Beep();
                }

                rollGoing = false;

                Console.Clear();

                // Draw the Menu
                Console.ForegroundColor = ConsoleColor.White;
                foreach (string row in menuSprite)
                {
                    Console.WriteLine(row);
                }

                // Update Last 30
                if (previousCoins.Count > 0)
                {
                    int xVal = 93;
                    int yVal = 13;
                    for (int i = previousCoins.Count - 1, l = 1; i >= previousCoins.Count - 90 && i >= 0; i--, l++)
                    {
                        Console.SetCursorPosition(xVal, yVal);
                        Console.ForegroundColor = previousCoins[i] == "coin-t" ? ConsoleColor.DarkYellow : previousCoins[i] == "coin-ct" ? ConsoleColor.Blue : ConsoleColor.White;
                        for (int j = 0; j < 3; j++)
                        {
                            Console.Write(coinSprite);
                            Console.SetCursorPosition(xVal, yVal + j);
                        }

                        xVal -= 3;

                        if (l == 30)
                        {
                            yVal = 17;
                            xVal = 93;
                        }
                        if (l == 60)
                        {
                            yVal = 21;
                            xVal = 93;
                        }
                    }
                }

                // Update Last 100
                if (previousCoins.Count > 100)
                {
                    List<string> last100Coins = previousCoins.Skip(Math.Max(0, previousCoins.Count - 100)).ToList();
                    for (int i = 0; i < 3; i++)
                    {
                        int currentX = 32 + i * 14;
                        Console.SetCursorPosition(currentX, 27);
                        Console.ForegroundColor = i == 0 ? ConsoleColor.Blue : i == 1 ? ConsoleColor.White : ConsoleColor.DarkYellow;
                        for (int j = 0; j < 3; j++)
                        {
                            Console.Write(coinSprite);
                            Console.SetCursorPosition(currentX, 27 + j);
                        }

                        // Update Text
                        Console.SetCursorPosition(currentX + 4, 28);
                        Console.Write(last100Coins.Count(s => s == (i == 0 ? "coin-ct" : i == 1 ? "coin-bonus" : "coin-t")));
                    }
                }
                else
                {
                    Console.SetCursorPosition(33, 28);
                    Console.Write("Don't have 100 coins registered yet");
                }

                // Update Last 1000
                if (previousCoins.Count > 1000)
                {
                    List<string> last100Coins = previousCoins.Skip(Math.Max(0, previousCoins.Count - 1000)).ToList();
                    for (int i = 0; i < 3; i++)
                    {
                        int currentX = 32 + i * 14;
                        Console.SetCursorPosition(currentX, 34);
                        Console.ForegroundColor = i == 0 ? ConsoleColor.Blue : i == 1 ? ConsoleColor.White : ConsoleColor.DarkYellow;
                        for (int j = 0; j < 3; j++)
                        {
                            Console.Write(coinSprite);
                            Console.SetCursorPosition(currentX, 34 + j);
                        }

                        // Update Text
                        Console.SetCursorPosition(currentX + 4, 35);
                        Console.Write(last100Coins.Count(s => s == (i == 0 ? "coin-ct" : i == 1 ? "coin-bonus" : "coin-t")));
                    }
                }
                else
                {
                    Console.SetCursorPosition(33, 34);
                    Console.Write("Don't have 1000 coins registered yet");
                }

                // *** Update Longest Trains ***
                int currentCTTrain = 0;
                int currentTTrain = 0;
                int currentDTrain = 0;
                int currentCTDTrain = 0;
                int currentTDTrain = 0;
                trainCount = 0;

                for (int i = 0; i < previousCoins.Count; i++)
                {
                    if (previousCoins[i] == "coin-ct")
                    {
                        currentCTTrain++;
                        currentCTDTrain++;

                        if (currentCTTrain > ctTrain)
                        {
                            ctTrain = currentCTTrain;
                        }
                        if (currentCTDTrain > ctDTrain)
                        {
                            ctDTrain = currentCTDTrain;
                        }
                        if (currentTTrain >= 10)
                        {
                            trainCount++;
                        }
                        if (currentTDTrain >= 10)
                        {
                            trainCount++;
                        }

                        currentTTrain = 0;
                        currentTDTrain = 0;
                        currentDTrain = 0;
                    }
                    else if (previousCoins[i] == "coin-bonus")
                    {
                        currentDTrain++;
                        currentCTDTrain++;
                        currentTDTrain++;

                        if (currentDTrain > dTrain)
                        {
                            dTrain = currentDTrain;
                        }
                        if (currentCTDTrain > ctDTrain)
                        {
                            ctDTrain = currentCTDTrain;
                        }
                        if (currentTDTrain > tDTrain)
                        {
                            tDTrain = currentTDTrain;
                        }

                        currentCTTrain = 0;
                        currentTTrain = 0;
                    }
                    else if (previousCoins[i] == "coin-t")
                    {
                        currentTTrain++;
                        currentTDTrain++;

                        if (currentTTrain > tTrain)
                        {
                            tTrain = currentTTrain;
                        }
                        if (currentTDTrain > tDTrain)
                        {
                            tDTrain = currentTDTrain;
                        }
                        if (currentCTTrain >= 10)
                        {
                            trainCount++;
                        }
                        if (currentCTDTrain >= 10)
                        {
                            trainCount++;
                        }

                        currentCTTrain = 0;
                        currentCTDTrain = 0;
                        currentDTrain = 0;
                    }
                }

                // Update Longest Trains text
                Console.ForegroundColor = ConsoleColor.White;
                double expectedBonus = previousCoins.Count * (1.0 / 15);
                double expectedOrange = previousCoins.Count * (7.0 / 15);
                double expectedBlack = previousCoins.Count * (7.0 / 15);

                double underrepresentedBonus = Math.Max(expectedBonus - dCount, 0);
                double underrepresentedOrange = Math.Max(expectedOrange - tCount, 0);
                double underrepresentedBlack = Math.Max(expectedBlack - ctCount, 0);
                
                double totalUnderrepresented = underrepresentedBonus + underrepresentedOrange + underrepresentedBlack;


                double convergenceProbabilityBonus = underrepresentedBonus / totalUnderrepresented;
                double convergenceProbabilityOrange = underrepresentedOrange / totalUnderrepresented;
                double convergenceProbabilityBlack = underrepresentedBlack / totalUnderrepresented;

                Console.SetCursorPosition(23, 43);
                Console.Write($"{convergenceProbabilityBlack * 100:0.00}%");
                Console.SetCursorPosition(22, 45);
                Console.Write($"{convergenceProbabilityOrange * 100:0.00}%");
                Console.SetCursorPosition(25, 47);
                Console.Write($"{convergenceProbabilityBonus * 100:0.00}%");
                Console.SetCursorPosition(24, 49);
                Console.Write(ctDTrain);
                Console.SetCursorPosition(23, 51);
                Console.Write(tDTrain);
                Console.SetCursorPosition(22, 53);
                Console.Write(ctTrain);
                Console.SetCursorPosition(21, 55);
                Console.Write(tTrain);
                Console.SetCursorPosition(24, 57);
                Console.Write(dTrain);
                Console.SetCursorPosition(60, 43);
                Console.Write("Total Number of 10+ Trains: " + trainCount);
            }
        }
    }

    public static string[] menuSprite = {
        "*****************************************************************************************************",
        @"*                  _____                                   __________           __                  *",
        @"*                 /     \    ____    ____    ____   ___.__.\______   \  ____  _/  |_                *",
        @"*                /  \ /  \  /  _ \  /    \ _/ __ \ <   |  | |    |  _/ /  _ \ \   __\               *",
        @"*               /    Y    \(  <_> )|   |  \\  ___/  \___  | |    |   \(  <_> ) |  |                 *",
        @"*               \____|__  / \____/ |___|  / \___  > / ____| |______  / \____/  |__|                 *",
        @"*                       \/              \/      \/  \/             \/                               *",
        "*                                                                                                   *",
        "*****************************************************************************************************",
        "*                                                                                                   *",
        "*                                                                                                   *",
        "*                                            Last 30                                                *",
        "*                                                                                                   *",
        "*                                                                                                   *",
        "*                                                                                                   *",
        "*                                                                                                   *",
        "*                                                                                                   *",
        "*                                                                                                   *",
        "*                                                                                                   *",
        "*                                                                                                   *",
        "*                                                                                                   *",
        "*                                                                                                   *",
        "*                                                                                                   *",
        "*                                                                                                   *",
        "*                                                                                                   *",
        "*                                            Last 100                                               *",
        "*                                                                                                   *",
        "*                                                                                                   *",
        "*                                                                                                   *",
        "*                                                                                                   *",
        "*                                                                                                   *",
        "*                                                                                                   *",
        "*                                            Last 1000                                              *",
        "*                                                                                                   *",
        "*                                                                                                   *",
        "*                                                                                                   *",
        "*                                                                                                   *",
        "*                                                                                                   *",
        "*                                                                                                   *",
        "*****************************************************************************************************",
        "*                                               *",
        "*   Rolling:                                    *",
        "*                                               *",
        "*   Current CT Chance:                          *",
        "*                                               *",
        "*   Current T Chance:                           *",
        "*                                               *",
        "*   Current Dice Chance:                        *",
        "*                                               *",
        "*   Longest CT/D Train:                         *",
        "*                                               *",
        "*   Longest T/D Train:                          *",
        "*                                               *",
        "*   Longest CT Train:                           *",
        "*                                               *",
        "*   Longest T Train:                            *",
        "*                                               *",
        "*   Longest Dice Train:                         *",
        "*                                               *",
        "*************************************************",
    };

    public static string coinSprite = "██";
}