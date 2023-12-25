using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace CaptchaSnaking
{
    internal class Program
    {
        static int[]? FindStartOfCaptcha(Image<Rgb24> img)
        {
            var blackColor = new Rgb24(0, 0, 0);

            for (int i = 0; i < img.Width; i++)
            {
                for (int j = 0; j < img.Height; j++)
                {
                    if (img[i, j] == blackColor)
                    {
                        return new int[2] { i, j };
                    }
                }
            }
            return null;
        }

        static int[]? FindEndOfCaptcha(Image<Rgb24> img)
        {
            var blackColor = new Rgb24(0, 0, 0);

            for (int i = img.Width - 1; i > 0; i--)
            {
                for (int j = 0; j < img.Height; j++)
                {
                    if (img[i, j] == blackColor)
                    {
                        return new int[2] { i, j };
                    }
                }
            }
            return null;
        }

        static void Main(string[] args)
        {
            string path = "C:\\Users\\YaKIT\\source\\repos\\CaptchaSnaking\\captcha.jpg";
            
            using var img = Image.Load<Rgb24>(path);
            img.Mutate(x => x.AdaptiveThreshold());

            using var newImg = img.Clone();

            var captchaStart = new int[2];
            var captchaEnd = new int[2];
            
            var parallelOptions = new ParallelOptions()
            {
                MaxDegreeOfParallelism = 4,
            };

            captchaStart = FindStartOfCaptcha(img);
            Console.WriteLine(captchaStart[0] + " " + captchaStart[1]);

            captchaEnd = FindEndOfCaptcha(img);
            Console.WriteLine(captchaEnd[0] + " " + captchaEnd[1]);

            var cropRect = new Rectangle(captchaStart[0], 0, captchaEnd[0] - captchaStart[0], newImg.Height);
            newImg.Mutate(x => x.Crop(cropRect));
            newImg.SaveAsJpeg("C:\\Users\\YaKIT\\source\\repos\\CaptchaSnaking\\output.jpg");
        }
    }
}
