using CommandLine;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace CatlaAscii
{
    class Program
    {
        private const char FirstChar = (char)32;
        private const char LastChar = (char)126;

        private static readonly Dictionary<Color, ConsoleColor> ConsoleColors = new Dictionary<Color, ConsoleColor>()
        {
            { Color.Black, ConsoleColor.Black },
            { Color.DarkBlue, ConsoleColor.DarkBlue },
            { Color.DarkGreen, ConsoleColor.DarkGreen },
            { Color.DarkCyan, ConsoleColor.DarkCyan },
            { Color.DarkRed, ConsoleColor.DarkRed },
            { Color.DarkMagenta, ConsoleColor.DarkMagenta },
            { Color.Orange, ConsoleColor.DarkYellow },
            {Color.Gray, ConsoleColor.Gray },
            {Color.DarkGray, ConsoleColor.DarkGray },
            {Color.Blue, ConsoleColor.Blue },
            {Color.Green, ConsoleColor.Green },
            {Color.Cyan, ConsoleColor.Cyan },
            {Color.Red, ConsoleColor.Red },
            {Color.Magenta, ConsoleColor.Magenta },
            {Color.Yellow, ConsoleColor.Yellow },
            {Color.White, ConsoleColor.White },
        };

        static int Main(string[] args)
        {
            int returnCode = Parser.Default
                            .ParseArguments<CmdOption>(args)
                            .MapResult(
                            (CmdOption opts) => ConvertImage(opts),
                            (err => 1));

            Console.Write("Press any key to continue...");
            Console.ReadKey(true);
            return returnCode;
        }

        static int ConvertImage(CmdOption opts)
        {
            WeightedChar[] weightedCharArray = new WeightedChar[LastChar - FirstChar];
            int index = 0;
            for (char c = FirstChar; c < LastChar + 1; c++)
            {
                if (c == '"')
                    continue;

                weightedCharArray[index] = new WeightedChar(c, opts.IsNegative);
                index++;
            }

            // Scale weightedCharArray to the full [0, 255] range
            double min = weightedCharArray.Min(w => w.Weight);
            double max = weightedCharArray.Max(w => w.Weight);

            weightedCharArray = weightedCharArray.Select(w => { w.Weight = byte.MaxValue * (w.Weight - min) / (max - min); return w; }).ToArray();

            ImageConverter imageConverter = new ImageConverter(opts.ImagePath);

            imageConverter.ConvertToAscii(opts.Width, weightedCharArray, out string[,] asciiContent, out Color[,] asciiColor);
            imageConverter.Dispose();

            string header = asciiContent.GetLength(0) + " " + asciiContent.GetLength(1);
            string flattenedAsciiContent = "";
            string flattenedAciiColor = "";

            for (int h = 0; h < asciiContent.GetLength(1); h++)
            {
                for (int w = 0; w < asciiContent.GetLength(0); w++)
                {
                    ConsoleColor originalColor = Console.ForegroundColor;
                    ConsoleColor closestColor = ConsoleColors.MinBy(c =>
                     Math.Pow(asciiColor[w, h].R - c.Key.R, 2.0)
                    + Math.Pow(asciiColor[w, h].G - c.Key.G, 2.0)
                    + Math.Pow(asciiColor[w, h].B - c.Key.B, 2.0)).Value;
                    if (opts.IsColored)
                        Console.ForegroundColor = closestColor;
                    Console.Write(asciiContent[w, h]);
                    Console.ForegroundColor = originalColor;

                    flattenedAsciiContent += asciiContent[w, h];
                    flattenedAciiColor += (int)closestColor + " ";
                }
                Console.Write("\r\n");
            }

            if (opts.OutputPath != null && opts.OutputPath.Length > 0)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(opts.OutputPath));
                FileStream fs = File.Create(opts.OutputPath);
                fs.Close();
                File.WriteAllLines(opts.OutputPath, new string[] { header, flattenedAsciiContent, flattenedAciiColor });
            }
            return 0;
        }
    }
}