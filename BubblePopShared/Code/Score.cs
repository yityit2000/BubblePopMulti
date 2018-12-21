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

        // This integer houses the current scoring algorithm used. Basically every connected bubble in a batch gives one
        // more point than the previous. So 2 bubbles give 3 points (1 + 2), 3 give 6 (1 + 2 + 3), 4 give 10 (1 + 2 + 3 + 4),
        // and so on. We then multiply by a scoring unit in case we want to change the base score for a bubble, and then
        // multiply by the level, awarding more points for the same amount of bubbles in later levels since it's more
        // difficult to get larger groups (for now. No powerups yet). This is likely temporary.
        public int ScoreOfCurrentBatch(int numberInBatch, int level)
        {
            int scoreToCalculate = 0;

            if (numberInBatch < 2)
            {
                return scoreToCalculate;
            }

            for (int i = 1; i < numberInBatch + 1; i++)
            {
                scoreToCalculate += i * Constants.SCORING_UNIT * level;
            }
            return scoreToCalculate;
        }
    }
}
