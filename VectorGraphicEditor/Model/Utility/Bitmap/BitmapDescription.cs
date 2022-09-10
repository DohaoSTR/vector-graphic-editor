namespace VectorGraphicEditor.Utility
{
    public class BitmapDescription
    {
        public string Name { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public double DPI_X { get; set; }

        public double DPI_Y { get; set; }

        public BitmapDescription()
        {
            Name = "Новый слой";
            Width = 800;
            Height = 600;
            DPI_X = 96;
            DPI_Y = 96;
        }
    }
}
