using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;

namespace SnakeGame
{
    public class Game
    {
        private readonly int rows;
        private readonly int cols;
        private readonly DispatcherTimer gameTimer;
        private List<Position> snake;
        private Position food;
        private Random random;
        private Direction currentDirection;
        private bool isGameOver;

        public Game(int rows, int cols)
        {
            this.rows = rows;
            this.cols = cols;
            gameTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(50) }; // Reduced interval for smoother gameplay
            gameTimer.Tick += GameTick;
            random = new Random();
            ResetGame();
        }

        public event EventHandler<Position[]> OnSnakeChanged;
        public event EventHandler<Position> OnFoodChanged;
        public event EventHandler OnGameOver;

        private void GameTick(object sender, EventArgs e)
        {
            MoveSnake();
            CheckCollisions();
            if (isGameOver)
            {
                gameTimer.Stop();
                OnGameOver?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                OnSnakeChanged?.Invoke(this, snake.ToArray());
                OnFoodChanged?.Invoke(this, food);  // Ensure food is redrawn
            }
        }

        public void StartGame()
        {
            if (!gameTimer.IsEnabled)
            {
                gameTimer.Start();
            }
        }

        public void PauseGame()
        {
            if (gameTimer.IsEnabled)
            {
                gameTimer.Stop();
            }
        }

        public void ResetGame()
        {
            snake = new List<Position> { new Position(rows / 2, cols / 2) };
            currentDirection = Direction.Right;
            isGameOver = false;
            GenerateFood();
            OnSnakeChanged?.Invoke(this, snake.ToArray());
            OnFoodChanged?.Invoke(this, food);
        }

        private void MoveSnake()
        {
            var head = snake.First();
            var newHead = head;
            switch (currentDirection)
            {
                case Direction.Up: newHead = new Position(head.Row - 1, head.Col); break;
                case Direction.Down: newHead = new Position(head.Row + 1, head.Col); break;
                case Direction.Left: newHead = new Position(head.Row, head.Col - 1); break;
                case Direction.Right: newHead = new Position(head.Row, head.Col + 1); break;
            }
            snake.Insert(0, newHead);
            if (newHead.Equals(food))
            {
                GenerateFood();
                OnFoodChanged?.Invoke(this, food);
            }
            else
            {
                snake.RemoveAt(snake.Count - 1);
            }
        }

        private void GenerateFood()
        {
            int row, col;
            do
            {
                row = random.Next(0, rows);
                col = random.Next(0, cols);
            } while (snake.Any(p => p.Row == row && p.Col == col));
            food = new Position(row, col);
        }

        private void CheckCollisions()
        {
            var head = snake.First();
            if (head.Row < 0 || head.Col < 0 || head.Row >= rows || head.Col >= cols ||
                snake.Skip(1).Any(p => p.Equals(head)))
            {
                isGameOver = true;
            }
        }

        public void ChangeDirection(Direction direction)
        {
            if ((currentDirection == Direction.Up && direction != Direction.Down) ||
                (currentDirection == Direction.Down && direction != Direction.Up) ||
                (currentDirection == Direction.Left && direction != Direction.Right) ||
                (currentDirection == Direction.Right && direction != Direction.Left))
            {
                currentDirection = direction;
            }
        }

        public Position[] GetSnake()
        {
            return snake.ToArray();
        }

        public Position GetFood()
        {
            return food;
        }
    }

    public struct Position
    {
        public int Row { get; }
        public int Col { get; }

        public Position(int row, int col)
        {
            Row = row;
            Col = col;
        }

        public override bool Equals(object obj)
        {
            if (obj is Position other)
            {
                return Row == other.Row && Col == other.Col;
            }
            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + Row.GetHashCode();
                hash = hash * 23 + Col.GetHashCode();
                return hash;
            }
        }
    }

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }
}
