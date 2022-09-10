using System;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace VectorGraphicEditor.Utility
{
    internal static class BitmapHelper
    {
        public static void Save(BitmapSource bitmap, string path)
        {
            using (FileStream fileStream = new FileStream(path, FileMode.Create))
            {
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmap));
                encoder.Save(fileStream);
            }
        }

        public static void GetPicFromControl(FrameworkElement element, string type, string outputPath)
        {
            RenderTargetBitmap bitmapRender = new RenderTargetBitmap((int)element.ActualWidth, (int)element.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            bitmapRender.Render(element);
            BitmapEncoder encoder = null;
            switch (type.ToUpper())
            {
                case ".BMP":
                    encoder = new BmpBitmapEncoder();
                    break;
                case ".GIF":
                    encoder = new GifBitmapEncoder();
                    break;
                case ".JPEG":
                    encoder = new JpegBitmapEncoder();
                    break;
                case ".PNG":
                    encoder = new PngBitmapEncoder();
                    break;
                case ".TIFF":
                    encoder = new TiffBitmapEncoder();
                    break;
                default:
                    break;
            }

            encoder.Frames.Add(BitmapFrame.Create(bitmapRender));
            if (!Directory.Exists(Path.GetDirectoryName(outputPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
            }

            using (FileStream file = File.Create(outputPath))
            {
                encoder.Save(file);
            }
        }

        public static string SaveAs(BitmapSource bitmap, string fileName)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "PNG(*.png)|*.png|JPG(*.jpg)|*.jpg|BMP(*.bmp)|*.bmp|GIF(*.gif)|*.gif|TIF(*.tif)|*.tif",
                FileName = fileName
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                Save(bitmap, saveFileDialog.FileName);
                return saveFileDialog.FileName;
            }

            return null;
        }

        public static string Open()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "PNG(*.png)|*.png|JPG(*.jpg)|*.jpg|BMP(*.bmp)|*.bmp|GIF(*.gif)|*.gif|TIF(*.tif)|*.tif"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                return openFileDialog.FileName;
            }

            return null;
        }

        public static BitmapImage GetBitmapImage(string imagePath)
        {
            BitmapImage bitmap = new BitmapImage();

            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.UriSource = new Uri(imagePath);
            bitmap.EndInit();

            return bitmap.Clone();
        }
    }
}
