using System.Windows.Controls;
using System.Windows.Media;

namespace VectorGraphicEditor
{
    internal class Canvas : InkCanvas
    {
        public Canvas()
        {
            Background = Brushes.Transparent;
            EditingMode = InkCanvasEditingMode.None;
        }

        protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
        }
    }
}
