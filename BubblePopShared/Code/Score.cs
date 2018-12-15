using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BubblePop
{
    class Score
    {
        private int gameScore;
        public int GameScore
        {
            get { return gameScore; }
        }

        public Score()
        {
            gameScore = 0;
        }

        public void Add(int numberOfClearedBubbles, int level)
        {
            gameScore += ScoreOfCurrentBatch(numberOfClearedBubbles, level);
        }

        public int ScoreOfCurrentBatch(int numberInBatch, int level)
        {
            int scoreToCalculate = 0;
            for (int i = 1; i < numberInBatch + 1; i++)
            {
                scoreToCalculate += i * Constants.SCORING_UNIT * level;
            }
            return scoreToCalculate;
        }
    }
}
