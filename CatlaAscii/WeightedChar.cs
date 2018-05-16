using System.Drawing;

namespace CatlaAscii
{
    public class WeightedChar
    {
        public char Character { get; set; }
        public double Weight { get; set; }
        public SizeF CharacterSize { get; set; }

        public WeightedChar(char Character, bool isNegative)
        {
            this.Character = Character;

            Image characterImage = new Bitmap(1, 1);
            Graphics characterGraphics = Graphics.FromImage(characterImage);
            this.CharacterSize = characterGraphics.MeasureString(this.Character.ToString(), System.Drawing.SystemFonts.DefaultFont);

            characterGraphics.Dispose();
            characterImage.Dispose();

            characterImage = new Bitmap((int)CharacterSize.Width, (int)CharacterSize.Height);
            characterGraphics = Graphics.FromImage(characterImage);
            characterGraphics.Clear(Color.White);

            Brush characterBrush = new SolidBrush(Color.Black);
            characterGraphics.DrawString(Character.ToString(), System.Drawing.SystemFonts.DefaultFont, characterBrush, (CharacterSize.Width - CharacterSize.Width) / 2, 0);
            characterGraphics.Save();

            characterBrush.Dispose();
            characterGraphics.Dispose();

            Bitmap characterBitmap = new Bitmap(characterImage);
            double brightnessSum = 0;
            for (int i = 0; i < characterBitmap.Width; i++)
            {
                for (int j = 0; j < characterBitmap.Height; j++)
                {
                    Color pixel = characterBitmap.GetPixel(i, j);
                    brightnessSum = brightnessSum + (pixel.R * 0.3
                                                     + pixel.G * 0.59
                                                     + pixel.B * 0.11);
                }
            }

            this.Weight = brightnessSum / (characterBitmap.Width * characterBitmap.Height);
            if (isNegative)
                this.Weight = 255 - this.Weight;

            characterImage.Dispose();
            characterBitmap.Dispose();
        }
    }
}
