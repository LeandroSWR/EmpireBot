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
        CoinHandler coinHandler = new CoinHandler();

        // Wait for the page to load (adjust as needed)
        Thread.Sleep(2500);

        bool rollGoing = false;

        // Run the loop until the application is closed
        while (true)
        {
            // Wait for a short period before checking again (you can adjust the time as needed)
            Thread.Sleep(10);

            // Get the current rolling timer text
            string rollingText = "Uninitialized";
            rollingText = analyzer.GetRollingText();

            if (rollingText == "0.00" || rollingText == "")
            {
                rollGoing = true;
            }

            // Update Roll info
            string rollInfo = rollGoing ? "True " : rollingText;
            ConsoleColor rollColor = rollGoing ? ConsoleColor.Green : ConsoleColor.Red;
            consoleUI.DrawStat(13, 41, rollInfo, rollColor);
            

            // Check if the page source has changed, indicating a dice roll
            if (rollGoing && rollingText != "Uninitialized" && rollingText != "0.00" && rollingText != "")
            {
                List<IWebElement> previousRolls = analyzer.GetPreviousRolls();

                coinHandler.UpdateCoinsFromRoll(previousRolls);

                rollGoing = false;

                // Draw the Menu
                consoleUI.DrawMenu();

                // Update Last 30
                if (coinHandler.HasCoins)
                {
                    int xVal = 93;
                    int yVal = 13;
                    List<Coin> last90 = coinHandler.GetLastXCoins(90).ToList();
                    for (int i = last90.Count - 1, coinsDrew = 1; i >= 0; i--, coinsDrew++)
                    {
                        ConsoleColor color = last90[i] == Coin.T ? ConsoleColor.DarkYellow : last90[i] == Coin.CT ? ConsoleColor.Blue : ConsoleColor.White;
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
                if (coinHandler.CurrentNumCoins > 100)
                {
                    List<Coin> last100Coins = coinHandler.GetLastXCoins(100).ToList();
                    for (int i = 0; i < 3; i++)
                    {
                        int currentX = 32 + i * 14;
                        ConsoleColor color = i == 0 ? ConsoleColor.Blue : i == 1 ? ConsoleColor.White : ConsoleColor.DarkYellow;
                        consoleUI.DrawCoin(currentX, 27, color);

                        // Update Text
                        string numOfCoins = last100Coins.Count(s => s == (i == 0 ? Coin.CT : i == 1 ? Coin.Bonus : Coin.T)).ToString();
                        consoleUI.DrawStat(currentX + 4, 28, numOfCoins);
                    }
                }
                else
                {
                    consoleUI.DrawStat(33, 28, "Don't have 100 coins registered yet");
                }

                // Update Last 1000
                if (coinHandler.CurrentNumCoins > 1000)
                {
                    List<Coin> last100Coins = coinHandler.GetLastXCoins(1000).ToList();
                    for (int i = 0; i < 3; i++)
                    {
                        int currentX = 32 + i * 14;
                        ConsoleColor color = i == 0 ? ConsoleColor.Blue : i == 1 ? ConsoleColor.White : ConsoleColor.DarkYellow;
                        consoleUI.DrawCoin(currentX, 34, color);

                        // Update Text
                        string numOfCoins = last100Coins.Count(s => s == (i == 0 ? Coin.CT : i == 1 ? Coin.Bonus : Coin.T)).ToString();
                        consoleUI.DrawStat(currentX + 4, 35, numOfCoins);
                    }
                }
                else
                {
                    consoleUI.DrawStat(33, 35, "Don't have 1000 coins registered yet");
                }

                // Calculate probabilities based on the assumption that it always collapses to the main probability
                // *****************
                double expectedBonus = coinHandler.CurrentNumCoins * (1.0 / 15);
                double expectedOrange = coinHandler.CurrentNumCoins * (7.0 / 15);
                double expectedBlack = coinHandler.CurrentNumCoins * (7.0 / 15);

                double underrepresentedBonus = Math.Max(expectedBonus - coinHandler.BonusCoinCounter, 0);
                double underrepresentedOrange = Math.Max(expectedOrange - coinHandler.TCoinCounter, 0);
                double underrepresentedBlack = Math.Max(expectedBlack - coinHandler.CTCoinCounter, 0);
                
                double totalUnderrepresented = underrepresentedBonus + underrepresentedOrange + underrepresentedBlack;

                double convergenceProbabilityBonus = underrepresentedBonus / totalUnderrepresented;
                double convergenceProbabilityOrange = underrepresentedOrange / totalUnderrepresented;
                double convergenceProbabilityBlack = underrepresentedBlack / totalUnderrepresented;
                // *****************

                // Update Longest Trains text
                consoleUI.DrawStat(23, 43, $"{convergenceProbabilityBlack * 100:0.00}%");
                consoleUI.DrawStat(22, 45, $"{convergenceProbabilityOrange * 100:0.00}%");
                consoleUI.DrawStat(25, 47, $"{convergenceProbabilityBonus * 100:0.00}%");
                consoleUI.DrawStat(24, 49, coinHandler.CTBonusTrain.ToString());
                consoleUI.DrawStat(23, 51, coinHandler.TBonusTrain.ToString());
                consoleUI.DrawStat(22, 53, coinHandler.CTtrain.ToString());
                consoleUI.DrawStat(21, 55, coinHandler.Ttrain.ToString());
                consoleUI.DrawStat(24, 57, coinHandler.BonusTrain.ToString());
                consoleUI.DrawStat(60, 43, $"Total Number of 10+ Trains: {coinHandler.TrainCounter}");
            }
        }
    }
}