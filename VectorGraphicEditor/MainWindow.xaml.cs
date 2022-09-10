using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using VectorGraphicEditor.Utility;

namespace VectorGraphicEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private DrawingBoard CurrentDrawingBoard => (DrawingBoard)((TabItem)tabBoard.SelectedItem).Content;

        private void AddLayer_Click(object sender, RoutedEventArgs e)
        {
            if (tabBoard.SelectedItem != null)
            {
                CurrentDrawingBoard.AddPicture();
            }
            else
            {
                MessageBox.Show("Создайте область отрисовки!");
            }
        }

        private void DeleteLayer_Click(object sender, RoutedEventArgs e)
        {
            if (layerList.Items.Count == 1)
            {
                MessageBox.Show("Вы не можете удалить единственный слой!");
            }

            if (layerList.SelectedItem != null)
            {
                int index = layerList.SelectedIndex;
                CurrentDrawingBoard.DeletePicture((Picture)layerList.SelectedItem);
                layerList.SelectedIndex = index == layerList.Items.Count ? index - 1 : index;
            }
            else
            {
                MessageBox.Show("Выберите слой для удаления!");
            }
        }

        private void NewDrawingBoard(BitmapDescription bitmapDescription)
        {
            DrawingBoard drawingBoard = new DrawingBoard(bitmapDescription);

            Binding binding = new Binding
            {
                Source = layerList,
                Path = new PropertyPath("SelectedItem")
            };
            drawingBoard.SetBinding(DrawingBoard.CurrentPictureProperty, binding);

            binding = new Binding
            {
                Source = toolList,
                Path = new PropertyPath("SelectedItem")
            };
            drawingBoard.SetBinding(DrawingBoard.DrawingModeProperty, binding);

            binding = new Binding
            {
                Source = colorPicker,
                Path = new PropertyPath("SelectedColor")
            };
            drawingBoard.SetBinding(DrawingBoard.ColorProperty, binding);

            binding = new Binding
            {
                Source = penSize,
                Path = new PropertyPath("Value")
            };
            drawingBoard.SetBinding(DrawingBoard.PenThicknessProperty, binding);

            TabItem tabItem = new TabItem
            {
                Header = bitmapDescription.Name,
                Content = drawingBoard
            };

            tabBoard.Items.Add(tabItem);
            tabBoard.SelectedItem = tabItem;

            layerList.SelectedIndex = 0;
        }

        private void NewCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            NewFileDialog dialog = new NewFileDialog
            {
                Owner = this
            };
            if (dialog.ShowDialog() == true)
            {
                NewDrawingBoard(dialog.BitmapDescription);
            }
        }

        private void OpenCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            string filename = BitmapHelper.Open();
            if (filename != null)
            {
                BitmapImage imgSource = BitmapHelper.GetBitmapImage(filename);

                BitmapDescription bitmapDescription = new BitmapDescription
                {
                    Name = filename,
                    Width = imgSource.PixelWidth,
                    Height = imgSource.PixelHeight,
                    DPI_X = imgSource.DpiX,
                    DPI_Y = imgSource.DpiY
                };

                NewDrawingBoard(bitmapDescription);

                CurrentDrawingBoard.AddBitmap(imgSource);
            }
        }

        private void SaveCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            string filename = CurrentDrawingBoard.Save();

            if (filename != null)
            {
                ((TabItem)tabBoard.SelectedItem).Header = filename;
            }
        }

        private void CanSaveCommandExecuted(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = tabBoard.SelectedItem != null && CurrentDrawingBoard != null;
        }

        private void SaveAsCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CurrentDrawingBoard.SaveAs();
        }

        private void CanSaveAsCommandExecuted(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = tabBoard.SelectedItem != null && CurrentDrawingBoard != null;
        }
    }
}
