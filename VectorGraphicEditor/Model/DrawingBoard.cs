using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VectorGraphicEditor.Figures;
using VectorGraphicEditor.Utility;

namespace VectorGraphicEditor
{
    internal class DrawingBoard : Label
    {
        private readonly Canvas _inkCanvas;

        private DrawingVisual _figure = null;

        public BitmapDescription BitmapDescription { get; set; }

        public DrawingBoard(BitmapDescription bitmapDescription)
        {
            BitmapDescription = bitmapDescription;

            HorizontalContentAlignment = HorizontalAlignment.Center;
            VerticalContentAlignment = VerticalAlignment.Center;
            Background = new SolidColorBrush(Colors.Gray);

            _inkCanvas = new Canvas
            {
                Width = bitmapDescription.Width,
                Height = bitmapDescription.Height,
                Background = (Brush)FindResource("CheckerBrush")
            };

            Content = _inkCanvas;

            AddPicture();
        }


        private readonly ObservableCollection<Picture> _pictures = new ObservableCollection<Picture>();

        public ObservableCollection<Picture> Pictures => _pictures;

        public void AddPicture()
        {
            Picture picture = new Picture((int)_inkCanvas.Width, (int)_inkCanvas.Height);

            _inkCanvas.Children.Add(picture);
            _pictures.Insert(0, picture);
        }

        internal void DeletePicture(Picture picture)
        {
            if (_pictures.Count > 1)
            {
                _inkCanvas.Children.Remove(picture);
                _pictures.Remove(picture);
            }
        }

        private Point _startPoint;

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            if ((int)DrawingMode >= (int)DrawingMode.Line)
            {
                _figure = new DrawingVisual();

                CurrentPicture.AddVisual(_figure);

                _startPoint = e.GetPosition(_inkCanvas);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (_figure != null)
            {
                using (DrawingContext drawingContext = _figure.RenderOpen())
                {
                    Pen pen = new Pen(new SolidColorBrush(Color), PenThickness);

                    Point endPoint = e.GetPosition(_inkCanvas);

                    switch (DrawingMode)
                    {
                        case DrawingMode.Line:
                            drawingContext.DrawLine(pen, _startPoint, endPoint);
                            break;

                        case DrawingMode.Rectangle:
                            drawingContext.DrawRectangle(Brushes.Transparent, pen, new Rect(_startPoint, endPoint));
                            break;

                        case DrawingMode.Ellipse:
                            Point centerPoint = new Point((_startPoint.X + endPoint.X) / 2, (_startPoint.Y + endPoint.Y) / 2);
                            drawingContext.DrawEllipse(Brushes.Transparent, pen, centerPoint,
                                Math.Abs(_startPoint.X - endPoint.X) / 2, Math.Abs(_startPoint.Y - endPoint.Y) / 2);
                            break;
                    }
                }
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            if (DrawingMode == DrawingMode.Pen)
            {
                InkPresenter inkPresenter = new InkPresenter
                {
                    Strokes = _inkCanvas.Strokes
                };

                CurrentPicture.AddUIElement(inkPresenter);
                _inkCanvas.Strokes = new StrokeCollection();
            }
            else
            {
                _figure = null;
            }
        }

        public DrawingMode DrawingMode
        {
            get => (DrawingMode)GetValue(DrawingModeProperty);
            set => SetValue(DrawingModeProperty, value);
        }

        public static readonly DependencyProperty DrawingModeProperty =
            DependencyProperty.Register("DrawingMode", typeof(DrawingMode), typeof(DrawingBoard),
            new PropertyMetadata((DrawingMode)0, new PropertyChangedCallback(DrawingModePropertyChanged)));

        private static void DrawingModePropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            DrawingMode drawingMode = (DrawingMode)e.NewValue;
            DrawingBoard drawingBoard = dependencyObject as DrawingBoard;

            if (drawingMode != DrawingMode.Pen)
            {
                drawingBoard._inkCanvas.EditingMode = InkCanvasEditingMode.None;
            }
            else
            {
                drawingBoard._inkCanvas.EditingMode = InkCanvasEditingMode.Ink;
            }
        }

        public Picture CurrentPicture
        {
            get => (Picture)GetValue(CurrentPictureProperty);
            set => SetValue(CurrentPictureProperty, value);
        }

        public static readonly DependencyProperty CurrentPictureProperty =
            DependencyProperty.Register("CurrentPicture", typeof(Picture), typeof(DrawingBoard),
            new PropertyMetadata(null));

        public Color Color
        {
            get => (Color)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(DrawingBoard),
            new PropertyMetadata(Colors.Black, new PropertyChangedCallback(ColorPropertyChanged)));

        private static void ColorPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            DrawingBoard drawingBoard = dependencyObject as DrawingBoard;
            DrawingAttributes drawingAttributes = drawingBoard._inkCanvas.DefaultDrawingAttributes;
            drawingAttributes.Color = drawingBoard.Color;
        }

        public double PenThickness
        {
            get => (double)GetValue(PenThicknessProperty);
            set => SetValue(PenThicknessProperty, value);
        }

        public static readonly DependencyProperty PenThicknessProperty =
            DependencyProperty.Register("PenThickness", typeof(double), typeof(DrawingBoard),
            new PropertyMetadata(1.0, new PropertyChangedCallback(PenThicknessPropertyChanged)));

        private static void PenThicknessPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            DrawingBoard drawingBoard = dependencyObject as DrawingBoard;
            DrawingAttributes drawingAttributes = drawingBoard._inkCanvas.DefaultDrawingAttributes;

            drawingAttributes.Width = drawingAttributes.Height = drawingBoard.PenThickness;
        }

        public void AddBitmap(BitmapSource image)
        {
            DrawingVisual drawingVisual = new DrawingVisual();

            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                drawingContext.DrawImage(image, new Rect(0, 0, image.PixelWidth, image.PixelHeight));
            }

            CurrentPicture.AddVisual(drawingVisual);
        }

        internal BitmapSource ToBitmap()
        {
            BitmapDescription bitmapDescription = BitmapDescription;
            RenderTargetBitmap bitmap = new RenderTargetBitmap(bitmapDescription.Width,
                bitmapDescription.Height, bitmapDescription.DPI_X, bitmapDescription.DPI_Y, PixelFormats.Default);

            bitmap.Render(_inkCanvas);

            return bitmap;
        }

        internal void SetFileName(string filename)
        {
            BitmapDescription.Name = filename;
        }

        internal string Save()
        {
            BitmapDescription bitmapDescription = BitmapDescription;

            if (File.Exists(bitmapDescription.Name))
            {
                BitmapHelper.Save(ToBitmap(), bitmapDescription.Name);
            }
            else
            {
                bitmapDescription.Name = BitmapHelper.SaveAs(ToBitmap(), bitmapDescription.Name);
            }

            return bitmapDescription.Name;
        }

        internal void SaveAs()
        {
            BitmapHelper.SaveAs(ToBitmap(), BitmapDescription.Name);
        }
    }
}
