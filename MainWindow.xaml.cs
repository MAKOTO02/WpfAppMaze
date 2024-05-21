using Microsoft.Win32;
using SimpleMaze2;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfAppMaze
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // スライダーの値が変更されたときにズームを行うイベントハンドラを登録
            ZoomSlider.ValueChanged += ZoomSlider_ValueChanged;
        }

        private void ZoomSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // スライダーの値に応じて Canvas を拡大縮小する
            double zoomLevel = e.NewValue;
            ScaleTransform scaleTransform = new ScaleTransform(zoomLevel, zoomLevel);
            MazeCanvas.LayoutTransform = scaleTransform;
        }

        private void New_Click(object sender, RoutedEventArgs e)
        {
            // New メニューアイテムがクリックされたときの処理を記述する
            // 新しいメニューアイテムがクリックされたときの処理
            InputDialog dialog = new InputDialog(); // 新しいサブウィンドウを作成
            if (dialog.ShowDialog() == true)
            {
                int width = dialog.SelectedWidth; // サブウィンドウから入力された数値を取得
                int height = dialog.SelectedHeight;
                bool randomize = dialog.Randomize;
                bool showPath = dialog.ShowPath;
                DisplayMaze(width, height, randomize, showPath); // 迷路を表示するメソッドを呼び出し、入力されたサイズを渡す
            }
        }

        private void DisplayMaze(int mazeWidth, int mazeHeight, bool randomize, bool showPath)
        {
            // 迷路を表示するロジックを実装する
            SimpleMaze2.Maze maze = new SimpleMaze2.MazeSolver(mazeWidth, mazeHeight, SimpleMaze2.GenerationMethod.Kruskal).Solve(SimpleMaze2.SolveMethod.StabeleAstar, randomize, DistanceType.L1);

            // サイズに基づいた迷路の表示処理を行う
            MazeCanvas.Children.Clear();
            double cellSize = 20.0;
            double offset = 20.0;
            MazeCanvas.Width = mazeWidth * 20 + 40; // Assume each cell is 20x20
            MazeCanvas.Height = mazeHeight * 20 + 40;
            if (showPath)
            {
                for (int y = 0; y < mazeHeight; y++)
                {
                    for (int x = 0; x < mazeWidth; x++)
                    {
                        Cell cell = maze.At(maze.Index(x, y));

                        double top = y * cellSize + offset;
                        double left = x * cellSize + offset;

                        if ((cell.State & CellState.InRoute) > 0)
                        {
                            Rectangle rect = new Rectangle
                            {
                                Width = cellSize,
                                Height = cellSize,
                                Stroke = Brushes.Cyan,
                                Fill = Brushes.Cyan
                            };
                            Canvas.SetLeft(rect, left);
                            Canvas.SetTop(rect, top);
                            MazeCanvas.Children.Add(rect);

                        }
                    }
                }
            }
            

            for (int y = 0; y < mazeHeight; y++)
            {
                for (int x = 0; x < mazeWidth; x++)
                {
                    Cell cell = maze.At(maze.Index(x, y));

                    double top = y * cellSize + offset;
                    double left = x * cellSize + offset;

                    // 上の壁
                    if (y == 0 || (cell.EdgeFlags & EdgeFlags.Up) != 0)
                    {
                        DrawLine(left, top, left + cellSize, top);
                    }

                    // 左の壁
                    if (x == 0 || (cell.EdgeFlags & EdgeFlags.Left) != 0)
                    {
                        DrawLine(left, top, left, top + cellSize);
                    }

                    // 下の壁
                    if ((cell.EdgeFlags & EdgeFlags.Down) != 0)
                    {
                        DrawLine(left, top + cellSize, left + cellSize, top + cellSize);
                    }

                    // 右の壁
                    if ((cell.EdgeFlags & EdgeFlags.Right) != 0)
                    {
                        DrawLine(left + cellSize, top, left + cellSize, top + cellSize);
                    }

                    if ((cell.State & CellState.IsStart) > 0) DrawMarker(left + 5, top, "S");
                    if ((cell.State & CellState.IsEnd) > 0) DrawMarker(left + 5, top, "G");
                }
            }
        }

        private void DrawLine(double x1, double y1, double x2, double y2)
        {
            Line line = new Line
            {
                X1 = x1,
                Y1 = y1,
                X2 = x2,
                Y2 = y2,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };
            MazeCanvas.Children.Add(line);
        }

        private void DrawMarker(double x, double y, string text)
        {
            TextBlock marker = new TextBlock
            {
                Text = text,
                Foreground = Brushes.Red,
                FontSize = 14,
                FontWeight = FontWeights.Bold
            };

            Canvas.SetLeft(marker, x);
            Canvas.SetTop(marker, y);
            MazeCanvas.Children.Add(marker);
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            // Open メニューアイテムがクリックされたときの処理を記述する
        }
        private void SaveAsImage_Click(object sender, RoutedEventArgs e)
        {
            // Save メニューアイテムがクリックされたときの処理を記述する
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "PNG Image|*.png",
                Title = "Save Canvas as Image"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                SaveCanvasAsImage(MazeCanvas, saveFileDialog.FileName);
            }
        }

        private void SaveCanvasAsImage(Canvas canvas, string fileName)
        {
            double dpi = 96d;
            Size size = new Size(canvas.ActualWidth, canvas.ActualHeight);
            canvas.Measure(size);
            canvas.Arrange(new Rect(size));

            RenderTargetBitmap renderBitmap = new RenderTargetBitmap(
                (int)size.Width,
                (int)size.Height,
                dpi,
                dpi,
                PixelFormats.Pbgra32);
            renderBitmap.Render(canvas);

            using (FileStream outStream = new FileStream(fileName, FileMode.Create))
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
                encoder.Save(outStream);
            }
        }

        private void Cut_Click(object sender, RoutedEventArgs e)
        {
            // Save メニューアイテムがクリックされたときの処理を記述する
        }

        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            // Save メニューアイテムがクリックされたときの処理を記述する
        }

        private void Paste_Click(object sender, RoutedEventArgs e)
        {
            // Save メニューアイテムがクリックされたときの処理を記述する
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            // Exit メニューアイテムがクリックされたときの処理を記述する
            Application.Current.Shutdown();
        }
    }
}