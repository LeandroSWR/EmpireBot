using OpenQA.Selenium;

namespace CSGOEmpireBot
{
    public class CoinHandler
    {
        private List<Coin> previousCoins;
        public bool HasCoins => previousCoins.Count > 0;
        public int CurrentNumCoins => previousCoins.Count;

        public int CTCoinCounter { get; private set; } = 0;
        public int TCoinCounter { get; private set; } = 0;
        public int BonusCoinCounter { get; private set; } = 0;
        
        public int CTtrain { get; private set; } = 0;
        public int Ttrain { get; private set; } = 0;
        public int BonusTrain { get; private set; } = 0;
        public int CTBonusTrain { get; private set; } = 0;
        public int TBonusTrain { get; private set; } = 0;
        public int TrainCounter { get; private set; } = 0;
        
        public CoinHandler()
        {
            previousCoins = new List<Coin>();
        }

        public void UpdateCoinsFromRoll(List<IWebElement> previousRolls)
        {
            if (previousCoins.Count == 0)
            {
                for (int i = 0; i < 10; i++)
                {
                    previousCoins.Add(GetCoinFromString(previousRolls[i].GetAttribute("class").Split(' ')[0]));
                }
            }
            else
            {
                previousCoins.Add(GetCoinFromString(previousRolls[9].GetAttribute("class").Split(' ')[0]));
            }

            UpdateLongestTrains();
        }
        public IEnumerable<Coin> GetLastXCoins(int numberOfCoins)
        {
            IEnumerable<Coin> retVal = previousCoins.Skip(Math.Max(0, previousCoins.Count - numberOfCoins));

            if (!retVal.Any())
            {
                return previousCoins;
            }

            return retVal;
        }
            

        private void UpdateLongestTrains()
        {
            int currentCTTrain = 0;
            int currentTTrain = 0;
            int currentDTrain = 0;
            int currentCTDTrain = 0;
            int currentTDTrain = 0;

            for (int i = 0; i < previousCoins.Count; i++)
            {
                switch (previousCoins[i])
                {
                    case Coin.Bonus:
                        currentDTrain++;
                        currentCTDTrain++;
                        currentTDTrain++;

                        if (currentDTrain > BonusTrain)
                        {
                            BonusTrain = currentDTrain;
                        }
                        if (currentCTDTrain > CTBonusTrain)
                        {
                            CTBonusTrain = currentCTDTrain;
                        }
                        if (currentTDTrain > TBonusTrain)
                        {
                            TBonusTrain = currentTDTrain;
                        }

                        currentCTTrain = 0;
                        currentTTrain = 0;
                        break;

                    case Coin.CT:
                        currentCTTrain++;
                        currentCTDTrain++;

                        if (currentCTTrain > CTtrain)
                        {
                            CTtrain = currentCTTrain;
                        }
                        if (currentCTDTrain > CTBonusTrain)
                        {
                            CTBonusTrain = currentCTDTrain;
                        }
                        if (currentTTrain >= 10)
                        {
                            TrainCounter++;
                        }
                        if (currentTDTrain >= 10)
                        {
                            TrainCounter++;
                        }

                        currentTTrain = 0;
                        currentTDTrain = 0;
                        currentDTrain = 0;
                        break;

                    case Coin.T:
                        currentTTrain++;
                        currentTDTrain++;

                        if (currentTTrain > Ttrain)
                        {
                            Ttrain = currentTTrain;
                        }
                        if (currentTDTrain > TBonusTrain)
                        {
                            TBonusTrain = currentTDTrain;
                        }
                        if (currentCTTrain >= 10)
                        {
                            TrainCounter++;
                        }
                        if (currentCTDTrain >= 10)
                        {
                            TrainCounter++;
                        }

                        currentCTTrain = 0;
                        currentCTDTrain = 0;
                        currentDTrain = 0;
                        break;
                }
            }
        }

        private Coin GetCoinFromString(string coin)
        {
            switch (coin)
            {
                default:
                case "coin-bonus":
                    return Coin.Bonus;
                case "coin-ct":
                    return Coin.CT;
                case "coin-t":
                    return Coin.T;
            }
        }
    }
}
