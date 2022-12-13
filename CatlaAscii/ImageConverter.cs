using System;
using System.Drawing;
using System.Linq;
using System.Numerics;

namespace CatlaAscii
{
    public class ImageConverter : IDisposable
    {
        private Image Image;

        public ImageConverter(string filename)
        {
            Image = Image.FromFile(filename);
        }

        public void ConvertToAscii(int width, WeightedChar[] weightedCharArray, out string[,] ascii, out Color[,] colors)
        {
            if (width > Image.Width)
                width = Image.Width;

            Bitmap bitmap = (Bitmap)Image;

            float averageCharacterAspectRatio = 8f / 16f; // from console properties
            float scalingFactor_w = (float)Image.Width / width;
            float scalingFactor_h = scalingFactor_w / averageCharacterAspectRatio;

            int height = (int)(Image.Height / scalingFactor_h);

            ascii = new string[width, height];
            colors = new Color[width, height];

            for (int w = 0; w < width; w++)
            {
                for (int h = 0; h < height; h++)
                {
                    Vector2 s1 = new Vector2(w * scalingFactor_w, h * scalingFactor_h);
                    Vector2 s2 = new Vector2(s1.X + scalingFactor_w, s1.Y + scalingFactor_h);

                    int begin_x = (int)MathF.Floor(s1.X);
                    int begin_y = (int)MathF.Floor(s1.Y);

                    int end_x = (int)MathF.Floor(s2.X);
                    int end_y = (int)MathF.Floor(s2.Y);

                    Vector2 deviation1 = new Vector2(1 - s1.X + begin_x, 1 - s1.Y + begin_y);
                    Vector2 deviation2 = new Vector2(end_x + 1 - s2.X, end_y + 1 - s2.Y);

                    double grayscale = 0;
                    Vector3 rgb = Vector3.Zero;

                    float dy = deviation1.Y;

                    for (int j = begin_y; j <= end_y; j++)
                    {
                        if (j == end_y)
                            dy -= deviation2.Y;

                        float dyf = dy * (1f / scalingFactor_h);
                        float dx = deviation1.X;
                        for (int i = begin_x; i <= end_x; i++)
                        {
                            if (i == end_x)
                                dx -= deviation2.X;

                            float region = dyf * dx * (1f / scalingFactor_w);

                            Color col = bitmap.GetPixel(Math.Clamp(i, 0, Image.Width - 1), Math.Clamp(j, 0, Image.Height - 1));
                            grayscale += region * (col.R * 0.3 + col.G * 0.59 + col.B * 0.11);
                            rgb = rgb + new Vector3(region * col.R, region * col.G, region * col.B);
                            dx = 1;
                        }
                        dy = 1;
                    }

                    ascii[w, h] = weightedCharArray.MinBy(v => Math.Abs(grayscale - v.Weight)).Character.ToString();
                    colors[w, h] = Color.FromArgb((int)rgb.X, (int)rgb.Y, (int)rgb.Z);
                }
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Image.Dispose();
                }

                disposedValue = true;
            }
        }
        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion
    }
}
