using System.Diagnostics;
using static System.Console;

namespace SnakeGame
{
    class Program
    {
        static void Main()
        {
            Game game = new Game(16, 32);
            game.Run();
        }
    }

    class Game
    {
        private readonly int _windowHeight;
        private readonly int _windowWidth;
        private Direction _currentMovement;
        private Pixel _head;
        private Pixel _target;
        private List<Pixel> _body;
        private int _score = 5;
        private bool _isGameOver;
        private readonly GUI _gui;
        Random rand = new Random();

        public Game(int windowHeight, int windowWidth)
        {
            _windowHeight = windowHeight;
            _windowWidth = windowWidth;

            _currentMovement = Direction.Right;

            _head = new Pixel(_windowWidth / 2, _windowHeight / 2, ConsoleColor.Red);
            _target = new Pixel(rand.Next (1, _windowWidth - 2), rand.Next (1, _windowHeight - 2), ConsoleColor.Cyan);
            _body = new List<Pixel>();

            _gui = new GUI(_windowHeight, _windowWidth);
        }

        public void Run()
        {
            while(!_isGameOver)
            {
                _gui.Clear();
                _gui.DrawBorder(ConsoleColor.Cyan);
                _gui.GetScore(_isGameOver, _score);

                ProcessInput();
                Update();
                _gui.Render(_head, _target, _body, _isGameOver);

                // Game speed
                var stopWatch = Stopwatch.StartNew();
                while (stopWatch.ElapsedMilliseconds <= 500)
                {
                    _currentMovement = ProcessInput();
                }
            }
        }

        public Direction ProcessInput()
        {
            if (KeyAvailable)
            {
                var key = ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.UpArrow when _currentMovement != Direction.Down:
                        return Direction.Up;
                    case ConsoleKey.DownArrow when _currentMovement != Direction.Up:
                        return Direction.Down;
                    case ConsoleKey.LeftArrow when _currentMovement != Direction.Right:
                        return Direction.Left;
                    case ConsoleKey.RightArrow when _currentMovement != Direction.Left:
                        return Direction.Right;
                }
            }
            // Return current movement if no new input
            return _currentMovement;
        }

        public void Update()
        {
            // Check for collision with borders, if there is collision switch gameover variable to true
            _isGameOver |= _head.XPos == _windowWidth - 2 
                    || _head.XPos == 1 
                    || _head.YPos == _windowHeight - 1 
                    || _head.YPos == 0;

            _gui.GetScore(_isGameOver, _score);

            switch(_currentMovement)
            {
                case Direction.Up:
                    _head.YPos--;
                    break;
                case Direction.Down:
                    _head.YPos++;
                    break;
                case Direction.Left:
                    _head.XPos--;
                    break;
                case Direction.Right:
                    _head.XPos++;
                    break;
            }

            // Remove tail if snake exceeds its length
            if(_body.Count > _score)
            {
                _body.RemoveAt (0);
            }

            // Check for collision with target, if snake gets the target add 1 to score
            if(_target.XPos == _head.XPos && _target.YPos == _head.YPos)
            {
                _score++;
                // Add a new target 
                _target = new Pixel (rand.Next (1, _windowWidth - 2), rand.Next (1, _windowHeight - 2), ConsoleColor.Cyan);
                _gui.GetScore(_isGameOver, _score);
            }
        }
    }

    class GUI
    {
        private readonly int _windowHeight;
        private readonly int _windowWidth;

        public GUI(int windowHeight, int windowWidth)
        {
            _windowHeight = windowHeight;
            _windowWidth = windowWidth;
        }

        public void Clear()
        {
            Console.Clear();
        }

        public void DrawBorder(ConsoleColor borderColor)
        {
            ForegroundColor = borderColor;

            for (int i = 0; i < _windowWidth; i++)
            {
                SetCursorPosition(i, 0);
                Write("■");

                SetCursorPosition(i, _windowHeight - 1);
                Write("■");
            }

            for (int i = 0; i < _windowHeight; i++)
            {
                SetCursorPosition(1, i);
                Write("■");

                SetCursorPosition(_windowWidth - 2, i);
                Write("■");
            }

            ResetColor();
        }

        public void GetScore(bool _isGameOver, int _score) 
        {
            if(_isGameOver) 
            {
                SetCursorPosition(_windowWidth / 5, _windowHeight / 2);
                WriteLine("Game over");
            } else 
                {
                    SetCursorPosition(_windowWidth / 5, _windowHeight + 1);
                    WriteLine($"Your score: {_score - 5}");
                }
        }

        public void Render(Pixel head, Pixel target, List<Pixel> body, bool isGameOver)
        {
            DrawPixel(head);
            DrawPixel(target);

            // Draw body for the snake
            for(int i = 0; i < body.Count; i++)
            {
                DrawPixel(body[i]);
                isGameOver |= body[i].XPos == head.XPos && body[i].YPos == head.YPos;
            }

            body.Add(new Pixel(head.XPos, head.YPos, ConsoleColor.Green));
        }

        public static void DrawPixel(Pixel pixel)
        {
            SetCursorPosition(pixel.XPos, pixel.YPos);
            ForegroundColor = pixel.ScreenColor;
            Write("■");
            SetCursorPosition(0, 0);
        }
    }

    public struct Pixel
    {
        public Pixel(int xPos, int yPos, ConsoleColor color)
        {
            XPos = xPos;
            YPos = yPos;
            ScreenColor = color;
        }
        public int XPos { get; set; }
        public int YPos { get; set; }
        public ConsoleColor ScreenColor { get; set; }
    }

    enum Direction
    {
        Up,
        Down,
        Right,
        Left
    }
}
