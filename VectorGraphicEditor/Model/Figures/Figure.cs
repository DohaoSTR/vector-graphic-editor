using System.Windows.Media;

namespace VectorGraphicEditor.Figures
{
    public abstract class Figure : DrawingVisual
    {
        protected abstract void OnRender(DrawingContext drawingContext);

        public void Render()
        {
            using (DrawingContext drawingContext = RenderOpen())
            {
                OnRender(drawingContext);
            }
        }
    }
}
