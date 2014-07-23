using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;

namespace NOS02
{
    class Program
    {
        static void Main(string[] args)
        {
            ImageRecognizer ir = new ImageRecognizer(new Bitmap(@"input.bmp"));

            Stopwatch sw = new Stopwatch();
            sw.Start();

            ir.RecognizeImage();

            ir.PrintOutput();

            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);

            ir.DisposImage();
        }
    }
}
