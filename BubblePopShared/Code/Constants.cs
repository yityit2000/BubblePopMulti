using Microsoft.Xna.Framework;
using System.Linq;


namespace BubblePop
{
    class Constants
    {
        // Size of side length of the circle sprites
        public static float WORLD_UNIT = 64f;
        public static float BUBBLE_RADIUS = WORLD_UNIT / 2f;

        /* This screen width and height is used when testing what the game will look like
         * on a phone and will be used for the final design of the game. */
        public static float SCREEN_WIDTH = 576f;
        public static float SCREEN_HEIGHT = 960f;

        public static Vector2 RADIUS_VECTOR = new Vector2(BUBBLE_RADIUS);

        // Defining the length and width of the grid
        public static int GRID_WIDTH_IN_UNITS = 8;
        public static int GRID_HEIGHT_IN_UNITS = 10;

        /* This screen width and height is used for testing purposes, when it might be
         * desirable to comfortably show any size grid of bubbles. */
        //public static float SCREEN_WIDTH = (GRID_WIDTH_IN_UNITS + 2) * WORLD_UNIT ;    
        //public static float SCREEN_HEIGHT = (GRID_HEIGHT_IN_UNITS + 2) * WORLD_UNIT;

        public static Vector2 BUBBLE_GRID_ORIGIN = new Vector2(WORLD_UNIT / 2, SCREEN_HEIGHT - (WORLD_UNIT * (1 + GRID_HEIGHT_IN_UNITS)));
        public static float BOTTOM_OF_GRID = SCREEN_HEIGHT - 2 * WORLD_UNIT;

        public static int STARTING_DIFFICULTY = 3;

        // A constant integer representing a number that will never be an index of a List<Bubble>, easier to read when used
        public static int NO_BUBBLE = -5;

        // This will set an amount for the main scoring unit of the game, per bubble.
        public static int SCORING_UNIT = 1;

        // This defines all the types of powerups in this game. Might make this an enum later, we'll see.
        public static string[] POWERUP_TYPES = new string[] { "ClearColorPowerup" };

        // This represents the position to place the Powerup UI in relation to a bubble's position. Found through trial and error.
        internal static Vector2 POWERUP_UI_POSITION_OFFSET = new Vector2((int)(BUBBLE_RADIUS * 1.5), (int)(BUBBLE_RADIUS * 1.6));
    }
}
