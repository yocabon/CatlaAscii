using CommandLine;

namespace CatlaAscii
{
    internal class CmdOption
    {
        [Option('i', "image", Required = true, HelpText = "Image Path")]
        public string ImagePath { get; set; }

        [Option('w', "width", Default = 48, HelpText = "Output width")]
        public int Width { get; set; }

        [Option('n', "negative", Default = false, HelpText = "Use black background")]
        public bool IsNegative { get; set; }

        [Option('c', "colored", Default = false, HelpText = "colored output")]
        public bool IsColored { get; set; }

        [Option('o', "out", Required = false, HelpText = "output path")]
        public string OutputPath { get; set; }
    }
}
