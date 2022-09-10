using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace VectorGraphicEditor
{
    public class Picture : System.Windows.Controls.Canvas
    {
        private static int LayerCount = 1;

        private readonly List<Visual> _visuals;

        public Picture(int width, int height)
        {
            _visuals = new List<Visual>();

            Background = Brushes.Transparent;
            Tag = "Новый слой " + LayerCount++;
            Width = width;
            Height = height;
        }

        protected override int VisualChildrenCount => _visuals.Count;

        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index >= _visuals.Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            return _visuals[index];
        }

        public Visual Get()
        {
            return _visuals[0];
        }

        public void AddVisual(Visual visual)
        {
            _visuals.Add(visual);

            AddVisualChild(visual);
            AddLogicalChild(visual);
        }

        public void AddUIElement(UIElement uiElement)
        {
            _visuals.Add(uiElement);

            Children.Add(uiElement);
        }

        public void DeleteVisual(Visual visual)
        {
            _visuals.Remove(visual);

            RemoveVisualChild(visual);
            RemoveLogicalChild(visual);
        }

        public DrawingVisual GetVisual(Point point)
        {
            HitTestResult hitResult = VisualTreeHelper.HitTest(this, point);
            return hitResult.VisualHit as DrawingVisual;
        }

        private readonly List<DrawingVisual> hits = new List<DrawingVisual>();
        public List<DrawingVisual> GetVisuals(Geometry region)
        {
            hits.Clear();

            GeometryHitTestParameters parameters = new GeometryHitTestParameters(region);
            HitTestResultCallback callback = new HitTestResultCallback(HitTestCallback);

            VisualTreeHelper.HitTest(this, null, callback, parameters);
            return hits;
        }

        private HitTestResultBehavior HitTestCallback(HitTestResult result)
        {
            GeometryHitTestResult geometryResult = (GeometryHitTestResult)result;
            DrawingVisual visual = result.VisualHit as DrawingVisual;

            if (visual != null && geometryResult.IntersectionDetail == IntersectionDetail.FullyInside)
            {
                hits.Add(visual);
            }

            return HitTestResultBehavior.Continue;
        }
    }
}
