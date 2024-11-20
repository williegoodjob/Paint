using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;


namespace Paint
{
    public partial class MainWindow : Window
    {
        Point start = new Point { X = 0, Y = 0 };
        Point dest = new Point { X = 0, Y = 0 };
        Color strokeColor = Colors.Red;
        Color fillColor = Colors.Red;
        int strokeThickness = 1;
        string shapeType = "";
        string actionType = "";
        bool isDrawing = false;
        public MainWindow()
        {
            InitializeComponent();
            strokeColorPicker.SelectedColor = strokeColor;
            fillColorPicker.SelectedColor = fillColor;
        }
        private void DisplayStatus()
        {
            pointLabel.Content = $"({Convert.ToInt32(start.X)}, {Convert.ToInt32(start.Y)}) -  ({Convert.ToInt32(dest.X)}, {Convert.ToInt32(dest.Y)})";
            int lineCount = myCanvas.Children.OfType<Line>().Count();
            int rectangleCount = myCanvas.Children.OfType<Rectangle>().Count();
            int ellipseCount = myCanvas.Children.OfType<Ellipse>().Count();
            int polylineCount = myCanvas.Children.OfType<Polyline>().Count();
            statusLabl.Content = $"Lines: {lineCount}, Rectangles: {rectangleCount}, Ellipses: {ellipseCount}, Polylines: {polylineCount}";
        }
        private void myCanvas_MouseEnter(object sender, MouseEventArgs e)
        {
        }

        private void myCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            start = e.GetPosition(myCanvas);

            Brush brush = new SolidColorBrush(strokeColor);
            Brush fill = new SolidColorBrush(fillColor);
            if (actionType == "draw")
            {
                myCanvas.Cursor = Cursors.Cross;
                switch (shapeType)
                {
                    case "line":
                        Line line = new Line
                        {
                            Stroke = brush,
                            StrokeThickness = strokeThickness,
                            X1 = start.X,
                            Y1 = start.Y,
                            X2 = dest.X,
                            Y2 = dest.Y
                        };
                        myCanvas.Children.Add(line);
                        break;
                    case "rectangle":
                        Rectangle rectangle = new Rectangle
                        {
                            Stroke = brush,
                            StrokeThickness = strokeThickness,
                            Width = 0,
                            Height = 0,
                            Fill = fill
                        };
                        myCanvas.Children.Add(rectangle);
                        break;
                    case "ellipse":
                        Ellipse ellipse = new Ellipse
                        {
                            Stroke = brush,
                            StrokeThickness = strokeThickness,
                            Width = 0,
                            Height = 0,
                            Fill = fill
                        };
                        myCanvas.Children.Add(ellipse);
                        break;
                    case "polyline":
                        Polyline polyline = new Polyline
                        {
                            Stroke = brush,
                            Fill = fill,
                            StrokeThickness = strokeThickness
                        };
                        myCanvas.Children.Add(polyline);
                        break;
                }
                isDrawing = true;
            }
            else if (actionType == "erase")
            {
                var elements = e.OriginalSource as Shape;
                myCanvas.Children.Remove(elements as UIElement);
            }

            DisplayStatus();
        }
        private void myCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isDrawing == true)
            {
                myCanvas.Cursor = Cursors.Pen;
                isDrawing = false;
            }
        }
        private void myCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            dest = e.GetPosition(myCanvas);
            switch (actionType)
            {
                case "draw":
                    if (isDrawing == true)
                    {
                        Point origin;
                        double sizeW, sizeH;
                        origin.X = Math.Min(start.X, dest.X);
                        origin.Y = Math.Min(start.Y, dest.Y);
                        sizeW = Math.Abs(start.X - dest.X);
                        sizeH = Math.Abs(start.Y - dest.Y);

                        switch (shapeType)
                        {
                            case "line":
                                var line = myCanvas.Children.OfType<Line>().LastOrDefault();
                                line.X2 = dest.X;
                                line.Y2 = dest.Y;
                                break;
                            case "rectangle":
                                var rectangle = myCanvas.Children.OfType<Rectangle>().LastOrDefault();
                                rectangle.Width = sizeW;
                                rectangle.Height = sizeH;
                                rectangle.SetValue(Canvas.LeftProperty, origin.X);
                                rectangle.SetValue(Canvas.TopProperty, origin.Y);
                                break;
                            case "ellipse":
                                var ellipse = myCanvas.Children.OfType<Ellipse>().LastOrDefault();
                                ellipse.Width = sizeW;
                                ellipse.Height = sizeH;
                                ellipse.SetValue(Canvas.LeftProperty, origin.X);
                                ellipse.SetValue(Canvas.TopProperty, origin.Y);
                                break;
                            case "polyline":
                                var polyline = myCanvas.Children.OfType<Polyline>().LastOrDefault();
                                polyline.Points.Add(dest);
                                break;
                        }
                    }
                    break;
                case "erase":
                    var shape = e.OriginalSource as Shape;
                    myCanvas.Children.Remove(shape);
                    if (myCanvas.Children.Count == 0)
                        myCanvas.Cursor = Cursors.Arrow;
                    break;
            }
            DisplayStatus();
        }
        private void strokeThicknessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            strokeThickness = Convert.ToInt32(strokeThicknessSlider.Value);
        }
        private void ShapeButton_Click(object sender, RoutedEventArgs e)
        {
            var targetRadioButton = sender as RadioButton;
            actionType = "draw";
            myCanvas.Cursor = Cursors.Pen;
            shapeType = targetRadioButton.Tag.ToString();
            shapeLabel.Content = shapeType;
        }
        private void fillColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            fillColor = (Color)fillColorPicker.SelectedColor;
        }
        private void strokeColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            strokeColor = (Color)strokeColorPicker.SelectedColor;
        }
        private void eraserButton_Click(object sender, RoutedEventArgs e)
        {
            actionType = "erase";
            if (myCanvas.Children.Count > 0)
            {
                myCanvas.Cursor = Cursors.Hand;
            }
        }
        private void trashcanButton_Click(object sender, RoutedEventArgs e)
        {
            myCanvas.Children.Clear();
        }
        private void SaveCanvasMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Title = "儲存畫布",
                Filter = "PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|All Files (*.*)|*.*",
                DefaultExt = ".png"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                int w = Convert.ToInt32(myCanvas.RenderSize.Width);
                int h = Convert.ToInt32(myCanvas.RenderSize.Height);

                // 創建 RenderTargetBitmap
                RenderTargetBitmap renderBitmap = new RenderTargetBitmap(w, h, 96d, 96d, PixelFormats.Pbgra32);

                // 渲染 Canvas
                renderBitmap.Render(myCanvas);

                // 選擇適當的 BitmapEncoder
                BitmapEncoder encoder;
                string extension = System.IO.Path.GetExtension(saveFileDialog.FileName).ToLower();
                switch (extension)
                {
                    case ".jpg":
                        encoder = new JpegBitmapEncoder();
                        break;
                    default:
                        encoder = new PngBitmapEncoder();
                        break;
                }

                // 將 RenderTargetBitmap 添加到編碼器的幀中
                encoder.Frames.Add(BitmapFrame.Create(renderBitmap));

                // 儲存影像到檔案
                using (FileStream outStream = new FileStream(saveFileDialog.FileName, FileMode.Create))
                {
                    encoder.Save(outStream);
                }
            }
        }
    }
}