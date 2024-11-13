using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

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
        bool isDrawing = false;
        private void myCanvas_MouseEnter(object sender, MouseEventArgs e)
        {
            myCanvas.Cursor = Cursors.Pen;
        }

        private void myCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            myCanvas.Cursor = Cursors.Cross;
            start = e.GetPosition(myCanvas);

            strokeColor = (Color)strokeColorPicker.SelectedColor;
            fillColor = (Color)fillColorPicker.SelectedColor;

            Brush brush = new SolidColorBrush(strokeColor);
            Brush fill = new SolidColorBrush(fillColor);

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
                    break;
            }
            isDrawing = true;

            DisplayStatus(start, dest);
        }

        private void DisplayStatus(Point start, Point dest)
        {
            pointLabel.Content = $"({Convert.ToInt32(start.X)}, {Convert.ToInt32(start.Y)}) -  ({Convert.ToInt32(dest.X)}, {Convert.ToInt32(dest.Y)})";
        }

        public MainWindow()
        {
            InitializeComponent();
            strokeColorPicker.SelectedColor = strokeColor;
            fillColorPicker.SelectedColor = fillColor;
        }

        private void myCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            dest = e.GetPosition(myCanvas);
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
                        break;
                }
            }
            DisplayStatus(start, dest);
        }

        private void myCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //strokeColor = (Color)strokeColorPicker.SelectedColor;
            //Brush brush = new SolidColorBrush(strokeColor);
            //Line line = new Line
            //{
            //    Stroke = brush,
            //    StrokeThickness = strokeThickness,
            //    X1 = start.X,
            //    Y1 = start.Y,
            //    X2 = dest.X,
            //    Y2 = dest.Y
            //};
            //myCanvas.Children.Add(line);
            isDrawing = false;
        }

        private void strokeThicknessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            strokeThickness = Convert.ToInt32(strokeThicknessSlider.Value);
        }

        private void ShapeButton_Click(object sender, RoutedEventArgs e)
        {
            var targetRadioButton = sender as RadioButton;
            shapeType = targetRadioButton.Tag.ToString();
            shapeLabel.Content = shapeType;
        }

    }
}