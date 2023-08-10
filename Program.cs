using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using CSGOEmpireBot;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

class Program
{
    static void Main(string[] args)
    {
        
        WebDriverManager webDriverManager = new WebDriverManager();
        Analyzer analyzer = new Analyzer(webDriverManager.Driver);
        ConsoleUI consoleUI = new ConsoleUI();

        // Wait for the page to load (adjust as needed)
        Thread.Sleep(2500);

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

        // Run the loop until the application is closed
        while (true)
        {
            // Wait for a short period before checking again (you can adjust the time as needed)
            Thread.Sleep(10);

            // Find the div element
            IReadOnlyList<IWebElement> divElements = analyzer.GetDivElements();

            string? innerText = null;
            try
            {
                if (divElements.Count > 0)
                {
                    innerText = divElements[0].Text;
                }
            }
            catch (StaleElementReferenceException)
            {
                // Handle the StaleElementReferenceException by re-finding the element
                divElements = analyzer.GetDivElements();

                if (divElements.Count > 0)
                {
                    innerText = divElements[0].Text;
                }
            }

            if (innerText != null && (innerText == "0.00" || innerText == ""))
            {
                rollGoing = true;
            }

            // Update Roll info
            consoleUI.DrawStat(13, 41, innerText/*rollGoing + " "*/, rollGoing ? ConsoleColor.Green : ConsoleColor.Red);

            // Check if the page source has changed, indicating a dice roll
            if (rollGoing && innerText != null && innerText != "0.00" && innerText != "")
            {
                List<IWebElement> previousRolls = analyzer.GetPreviousRolls();

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
                consoleUI.DrawMenu();

                // Update Last 30
                if (previousCoins.Count > 0)
                {
                    int xVal = 93;
                    int yVal = 13;
                    for (int i = previousCoins.Count - 1, coinsDrew = 1; i >= previousCoins.Count - 90 && i >= 0; i--, coinsDrew++)
                    {
                        ConsoleColor color = previousCoins[i] == "coin-t" ? ConsoleColor.DarkYellow : previousCoins[i] == "coin-ct" ? ConsoleColor.Blue : ConsoleColor.White;
                        consoleUI.DrawCoin(xVal, yVal, color);

                        xVal -= 3;

                        if (coinsDrew == 30)
                        {
                            yVal = 17;
                            xVal = 93;
                        }
                        if (coinsDrew == 60)
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
                        ConsoleColor color = i == 0 ? ConsoleColor.Blue : i == 1 ? ConsoleColor.White : ConsoleColor.DarkYellow;
                        consoleUI.DrawCoin(currentX, 27, color);

                        // Update Text
                        string numOfCoins = last100Coins.Count(s => s == (i == 0 ? "coin-ct" : i == 1 ? "coin-bonus" : "coin-t")).ToString();
                        consoleUI.DrawStat(currentX + 4, 28, numOfCoins);
                    }
                }
                else
                {
                    consoleUI.DrawStat(33, 28, "Don't have 100 coins registered yet");
                }

                // Update Last 1000
                if (previousCoins.Count > 1000)
                {
                    List<string> last100Coins = previousCoins.Skip(Math.Max(0, previousCoins.Count - 1000)).ToList();
                    for (int i = 0; i < 3; i++)
                    {
                        int currentX = 32 + i * 14;
                        ConsoleColor color = i == 0 ? ConsoleColor.Blue : i == 1 ? ConsoleColor.White : ConsoleColor.DarkYellow;
                        consoleUI.DrawCoin(currentX, 34, color);

                        // Update Text
                        string numOfCoins = last100Coins.Count(s => s == (i == 0 ? "coin-ct" : i == 1 ? "coin-bonus" : "coin-t")).ToString();
                        consoleUI.DrawStat(currentX + 4, 35, numOfCoins);
                    }
                }
                else
                {
                    consoleUI.DrawStat(33, 35, "Don't have 1000 coins registered yet");
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

                // Update Longest Trains text
                consoleUI.DrawStat(23, 43, $"{convergenceProbabilityBlack * 100:0.00}%");
                consoleUI.DrawStat(22, 45, $"{convergenceProbabilityOrange * 100:0.00}%");
                consoleUI.DrawStat(25, 47, $"{convergenceProbabilityBonus * 100:0.00}%");
                consoleUI.DrawStat(24, 49, ctDTrain.ToString());
                consoleUI.DrawStat(23, 51, tDTrain.ToString());
                consoleUI.DrawStat(22, 53, ctTrain.ToString());
                consoleUI.DrawStat(21, 55, tTrain.ToString());
                consoleUI.DrawStat(24, 57, dTrain.ToString());
                consoleUI.DrawStat(60, 43, $"Total Number of 10+ Trains: {trainCount}");
            }
        }
    }
}