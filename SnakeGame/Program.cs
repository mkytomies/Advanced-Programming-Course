﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using static System.Console;

namespace Snake
{
    class Program
    {
        static void Main ()
        {
            // Define screen size
            Console.WindowHeight = 16;
            Console.WindowWidth = 32;

            // Variable for random number
            var rand = new Random ();

            // Variable for storing the score
            var score = 5;

            // Initialize head and target position
            var head = new Pixel (WindowWidth / 2, WindowHeight / 2, ConsoleColor.Red);
            var target = new Pixel (rand.Next (1, WindowWidth - 2), rand.Next (1, WindowHeight - 2), ConsoleColor.Cyan);

            // Initialize body of the snake
            var body = new List<Pixel> ();

            // Initialize movement direction
            var currentMovement = Direction.Right;

            // Set gameover to be false at default
            var gameover = false;

            // Game loop
            while (true)
            {
                // Clear the console
                Clear ();

                // Check for collision with borders, if there is collision switch gameover variable to true
                gameover |= head.XPos == WindowWidth - 1 || head.XPos == 0 || head.YPos == WindowHeight - 1 || head.YPos == 0;

                // Draw border for the game
                DrawBorder ();

                // Check for collision with target, if snake gets target add 1 to score
                if (target.XPos == head.XPos && target.YPos == head.YPos)
                {
                    score++;
                    // Add a new target 
                    target = new Pixel (rand.Next (1, WindowWidth - 2), rand.Next (1, WindowHeight - 2), ConsoleColor.Cyan);
                }

                // Draw body for the snake
                for (int i = 0; i < body.Count; i++)
                {
                    DrawPixel (body[i]);
                    gameover |= body[i].XPos == head.XPos && body[i].YPos == head.YPos;
                }

                // Exit game loop if game over
                if (gameover)
                {
                    break;
                }

                // Draw snake head and target
                DrawPixel (head);
                DrawPixel (target);

                // Control game speed
                var stopWatch = Stopwatch.StartNew();
                while (stopWatch.ElapsedMilliseconds <= 500)
                {
                    currentMovement = GetNextMovement (currentMovement);
                }

                // Add new head position to snake body
                body.Add (new Pixel (head.XPos, head.YPos, ConsoleColor.Green));

                // Move the snake according to current movement direction
                switch (currentMovement)
                {
                    case Direction.Up:
                        head.YPos--;
                        break;
                    case Direction.Down:
                        head.YPos++;
                        break;
                    case Direction.Left:
                        head.XPos--;
                        break;
                    case Direction.Right:
                        head.XPos++;
                        break;
                }

                // Remove tail if snake exceeds its length
                if (body.Count > score)
                {
                    body.RemoveAt (0);
                }
            }
            
            // Tell user that the game is over
            SetCursorPosition (WindowWidth / 5, WindowHeight / 2);
            WriteLine ($"Game over, Score: {score - 5}");
            SetCursorPosition (WindowWidth / 5, WindowHeight / 2 + 1);
            ReadKey ();
        }

        // Method to get next movement based on user input
        static Direction GetNextMovement (Direction currentMovement)
        {
            // Check if a key is available
            if (KeyAvailable)
            {
                // Read the key pressed
                var key = ReadKey (true).Key;

                // Determine the next movement
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

        // Method to draw a pixel on the console
        static void DrawPixel (Pixel pixel)
        {
            SetCursorPosition (pixel.XPos, pixel.YPos);
            ForegroundColor = pixel.ScreenColor;
            Write ("■");
            SetCursorPosition (0, 0); // Move cursor to prevent flickering
        }

        // Method to draw game border
        static void DrawBorder ()
        {
            // Draw horizontal border 
            for (int i = 0; i < WindowWidth; i++)
            {
                SetCursorPosition (i, 0);
                Write ("■");

                SetCursorPosition (i, WindowHeight - 1);
                Write ("■");
            }

            // Draw vertical border
            for (int i = 0; i < WindowHeight; i++)
            {
                SetCursorPosition (1, i);
                Write ("■");

                SetCursorPosition (WindowWidth - 2, i);
                Write ("■");
            }
        }

        // Struct to represent a pixel (character) on the console
        struct Pixel
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

        // Enum to represent movement directions
        enum Direction
        {
            Up,
            Down,
            Right,
            Left
        }
    }
}