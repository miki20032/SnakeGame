using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SnakeGame
{
    public partial class MainWindow : Window
    {
        private const int Rows = 20;
        private const int Cols = 20;
        private const int SquareSize = 20;
        private readonly Game game;
        private readonly Rectangle[,] grid;

        public MainWindow()
        {
            InitializeComponent();
            grid = new Rectangle[Rows, Cols];
            InitializeGrid();
            game = new Game(Rows, Cols);
            game.OnSnakeChanged += Game_OnSnakeChanged;
            game.OnFoodChanged += Game_OnFoodChanged;
            game.OnGameOver += Game_OnGameOver;
            game.StartGame();
        }

        private void InitializeGrid()
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Cols; col++)
                {
                    var rect = new Rectangle
                    {
                        Width = SquareSize,
                        Height = SquareSize,
                        Fill = Brushes.White,
                        Stroke = Brushes.Gray
                    };
                    grid[row, col] = rect;
                    GameCanvas.Children.Add(rect);
                    Canvas.SetLeft(rect, col * SquareSize);
                    Canvas.SetTop(rect, row * SquareSize);
                }
            }
        }

        private void Game_OnSnakeChanged(object sender, Position[] snake)
        {
            foreach (var rect in grid)
            {
                rect.Fill = Brushes.White;
            }
            foreach (var position in snake)
            {
                grid[position.Row, position.Col].Fill = Brushes.Green;
            }
        }

        private void Game_OnFoodChanged(object sender, Position food)
        {
            grid[food.Row, food.Col].Fill = Brushes.Red;
        }

        private void Game_OnGameOver(object sender, EventArgs e)
        {
            MessageBox.Show("Game Over!");
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    game.ChangeDirection(Direction.Up);
                    break;
                case Key.Down:
                    game.ChangeDirection(Direction.Down);
                    break;
                case Key.Left:
                    game.ChangeDirection(Direction.Left);
                    break;
                case Key.Right:
                    game.ChangeDirection(Direction.Right);
                    break;
            }
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            game.StartGame();
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            game.PauseGame();
        }

        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            game.ResetGame();
            UpdateBoard();
        }

        private void UpdateBoard()
        {
            foreach (var rect in grid)
            {
                rect.Fill = Brushes.White;
            }
            foreach (var position in game.GetSnake())
            {
                grid[position.Row, position.Col].Fill = Brushes.Green;
            }
            grid[game.GetFood().Row, game.GetFood().Col].Fill = Brushes.Red;
        }
    }
}
