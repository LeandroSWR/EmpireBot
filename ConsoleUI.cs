using Newtonsoft.Json.Linq;
using System;

namespace CSGOEmpireBot
{
    public class ConsoleUI
    {
        private readonly string[] menuSprite = {
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
        private readonly string coinSprite = "██";

        public ConsoleUI()
        {
            Console.SetWindowSize(105, 65);
            Console.CursorVisible = false;
        }

        public void DrawMenu()
        {
            Console.ForegroundColor = ConsoleColor.White;
            foreach (string row in menuSprite)
            {
                Console.WriteLine(row);
            }
        }

        public void DrawCoin(int xPos, int yPos, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            for (int i = 0; i < 3; i++)
            {
                Console.SetCursorPosition(xPos, yPos + i);
                Console.Write(coinSprite);
            }
        }

        public void DrawStat(int xPos, int yPos, string value, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            Console.SetCursorPosition(xPos, yPos);
            Console.Write(value);
        }
    }
}
