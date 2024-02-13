using System.Diagnostics;
using static System.Console;

namespace Snake
{
    class Program
    {
        static void Main ()
        {
            if (OperatingSystem.IsWindows())
            {
                Console.WindowHeight = 16;
                Console.WindowWidth = 32;
            }

            int score = 5;

            // Initialize movement direction
            var currentMovement = Direction.Right;

            bool gameover = false;

            // Game loop
            while (true)
            {
                Clear ();

                // Check for collision with borders, if there is collision switch gameover variable to true
                gameover |= GUI.head.XPos == WindowWidth - 1 || GUI.head.XPos == 0 || GUI.head.YPos == WindowHeight - 1 || GUI.head.YPos == 0;

                GUI.DrawBorder ();

                GetScore(gameover, score);

                // Check for collision with target, if snake gets the target add 1 to score
                if (GUI.target.XPos == GUI.head.XPos && GUI.target.YPos == GUI.head.YPos)
                {
                    score++;
                    // Add a new target 
                    GUI.target = new GUI.Pixel (GUI.rand.Next (1, WindowWidth - 2), GUI.rand.Next (1, WindowHeight - 2), ConsoleColor.Cyan);
                    GetScore(gameover, score);
                }

                // Draw body for the snake
                for (int i = 0; i < GUI.body.Count; i++)
                {
                    GUI.DrawPixel (GUI.body[i]);
                    gameover |= GUI.body[i].XPos == GUI.head.XPos && GUI.body[i].YPos == GUI.head.YPos;
                }

                if (gameover)
                {
                    GetScore(gameover, score);
                    break;
                }

                GUI.DrawPixel (GUI.head);
                GUI.DrawPixel (GUI.target);

                // Control game speed
                var stopWatch = Stopwatch.StartNew();
                while (stopWatch.ElapsedMilliseconds <= 500)
                {
                    currentMovement = GetNextMovement (currentMovement);
                }

                // Add new head position to snake body
                GUI.body.Add (new GUI.Pixel (GUI.head.XPos, GUI.head.YPos, ConsoleColor.Green));

                switch (currentMovement)
                {
                    case Direction.Up:
                        GUI.head.YPos--;
                        break;
                    case Direction.Down:
                        GUI.head.YPos++;
                        break;
                    case Direction.Left:
                        GUI.head.XPos--;
                        break;
                    case Direction.Right:
                        GUI.head.XPos++;
                        break;
                }

                // Remove tail if snake exceeds its length
                if (GUI.body.Count > score)
                {
                    GUI.body.RemoveAt (0);
                }
            }
        }

        static void GetScore (bool gameover, int score) 
        {
            if (gameover) 
            {
                SetCursorPosition (WindowWidth / 5, WindowHeight / 2);
                WriteLine ($"Game over, Score: {score - 5}");
                SetCursorPosition (WindowWidth / 5, WindowHeight / 2 + 1);
            } else 
                {
                    SetCursorPosition (WindowWidth / 5, WindowHeight - 3);
                    WriteLine ($"Your score: {score - 5}");
                }
        }

        static Direction GetNextMovement (Direction currentMovement)
        {
            if (KeyAvailable)
            {
                var key = ReadKey (true).Key;

                switch (key)
                {
                    case ConsoleKey.UpArrow when currentMovement != Direction.Down:
                        return Direction.Up;
                    case ConsoleKey.DownArrow when currentMovement != Direction.Up:
                        return Direction.Down;
                    case ConsoleKey.LeftArrow when currentMovement != Direction.Right:
                        return Direction.Left;
                    case ConsoleKey.RightArrow when currentMovement != Direction.Left:
                        return Direction.Right;
                }
            }
            // If no key was pressed the movement doesn't change
            return currentMovement;
        }


        enum Direction
        {
            Up,
            Down,
            Right,
            Left
        }
    }

    class GUI
    {
        public static Random rand = new Random ();

        // Initialize head and target position
        public static Pixel head = new Pixel (WindowWidth / 2, WindowHeight / 2, ConsoleColor.Red);
        public static Pixel target = new Pixel (rand.Next (1, WindowWidth - 2), rand.Next (1, WindowHeight - 2), ConsoleColor.Cyan);

        // Initialize body of the snake
        public static List<Pixel> body = new List<Pixel> ();

        public static void DrawBorder ()
        {
            for (int i = 0; i < WindowWidth; i++)
            {
                SetCursorPosition (i, 0);
                Write ("■");

                SetCursorPosition (i, WindowHeight - 1);
                Write ("■");
            }

            for (int i = 0; i < WindowHeight; i++)
            {
                SetCursorPosition (1, i);
                Write ("■");

                SetCursorPosition (WindowWidth - 2, i);
                Write ("■");
            }
        }

        public static void DrawPixel (Pixel pixel)
        {
            SetCursorPosition (pixel.XPos, pixel.YPos);
            ForegroundColor = pixel.ScreenColor;
            Write ("■");
            SetCursorPosition (0, 0); // Move cursor to prevent flickering
        }

        // Struct to represent a pixel (character) on the console
        public struct Pixel
        {
            public Pixel (int xPos, int yPos, ConsoleColor color)
            {
                XPos = xPos;
                YPos = yPos;
                ScreenColor = color;
            }
            public int XPos { get; set; }
            public int YPos { get; set; }
            public ConsoleColor ScreenColor { get; set; }
        }
    }
}