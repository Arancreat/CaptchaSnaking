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

        static void CropCaptcha(Image<Rgb24> img)
        {
            img.Mutate(x => x.BinaryThreshold(0.9f, BinaryThresholdMode.Luminance));
            //img.Mutate(x => x.BlackWhite());

            var captchaStart = FindStartOfCaptcha(img);
            var captchaEnd = FindEndOfCaptcha(img);

            var cropRect = new Rectangle(captchaStart[0], 0, captchaEnd[0] - captchaStart[0], img.Height);
            img.Mutate(x => x.Crop(cropRect));
        }

        static void SnakeCaptcha(Image<Rgb24> img)
        {
            var blackColor = new Rgb24(0, 0, 0);
            var redColor = new Rgb24(255, 0, 0);

            for (int k = 0; k < img.Width; k++)
            {
                int i = k;
                int j = 0;
                int stepCount = 0;
                var previousSteps = new Stack<int[]>();
                bool isPreviousStep = false;

                if (i < img.Width / 2) img[i, j] = redColor;
                while (j != img.Height - 1 && stepCount < img.Height * 6)
                {
                    if (isPreviousStep)
                    {
                        if (i - 1 > 0 && img[i - 1, j] != redColor && img[i - 1, j] != blackColor)
                        {
                            i--;
                            img[i, j] = redColor;
                            previousSteps.Push([i, j]);
                            isPreviousStep = false;
                        }
                        else if (i + 1 < img.Width && img[i + 1, j] != redColor && img[i + 1, j] != blackColor)
                        {
                            i++;
                            img[i, j] = redColor;
                            previousSteps.Push([i, j]);
                            isPreviousStep = false;
                        }
                        else if (j + 1 < img.Height && img[i, j + 1] != redColor && img[i, j + 1] != blackColor)
                        {
                            j++;
                            img[i, j] = redColor;
                            previousSteps.Push([i, j]);
                            isPreviousStep = false;
                        }
                        else if (previousSteps.Count != 0)
                        {
                            var step = previousSteps.Pop();
                            i = step[0];
                            j = step[1];
                        }
                    }
                    else
                    {
                        if (j + 1 < img.Height && img[i, j + 1] != redColor && img[i, j + 1] != blackColor)
                        {
                            j++;
                            img[i, j] = redColor;
                            previousSteps.Push([i, j]);
                        }
                        else if (i + 1 < img.Width && img[i + 1, j] != redColor && img[i + 1, j] != blackColor)
                        {
                            i++;
                            img[i, j] = redColor;
                            previousSteps.Push([i, j]);
                        }
                        else if (i - 1 > 0 && img[i - 1, j] != redColor && img[i - 1, j] != blackColor)
                        {
                            i--;
                            img[i, j] = redColor;
                            previousSteps.Push([i, j]);
                        }
                        else if (previousSteps.Count > 5)
                        {
                            previousSteps.Pop();
                            previousSteps.Pop();
                            previousSteps.Pop();
                            previousSteps.Pop();
                            var step = previousSteps.Pop();
                            i = step[0];
                            j = step[1];
                            isPreviousStep = true;
                        }
                    }

                    stepCount++;
                }
            }
        }

        static void Main(string[] args)
        {
            string input = "C:\\Users\\YaKIT\\source\\repos\\CaptchaSnaking\\5ecd0870-a3f3-4b12-a7d9-8c5d355020a1.jpg";
            string output = "C:\\Users\\YaKIT\\source\\repos\\CaptchaSnaking\\output.jpg";
            using var img = Image.Load<Rgb24>(input);
            CropCaptcha(img);
            SnakeCaptcha(img);
            img.SaveAsJpeg(output);
        }
    }
}
