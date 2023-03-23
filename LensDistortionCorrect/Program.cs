using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace LensDistortionCorrect
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Bitmap bitmapImg = LoadBitmap("C:\\Users\\User\\Desktop\\IMG_20211011_170030.jpg");

            byte[,,] imgAsArray = BitmapToByteRgb(bitmapImg);

            Bitmap bmp = ByteRgbToBitmap(imgAsArray);   

            SaveBitmap(bmp, "C:\\Users\\User\\Desktop\\qwe1.jpg", 95);
        }

        // сохранение изображения из Bitmap в файл
        public static void SaveBitmap(Bitmap bmp,string path, int quality)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            ImageCodecInfo ici = null;
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.MimeType == "image/jpeg")
                    ici = codec;
            }

            EncoderParameters ep = new EncoderParameters();
            ep.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, (long)quality);

            bmp.Save(path, ici, ep);
        }

        // чтение изображения из файла в Bitmap
        public static Bitmap LoadBitmap(string fileName)
        {
            using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return new Bitmap(fileStream);
            }
        }

        // получение массива пикселей изображения из Bitmap
        public unsafe static byte[,,] BitmapToByteRgb(Bitmap bmp)
        {
            int width = bmp.Width,
                height = bmp.Height;
            byte[,,] res = new byte[3, height, width];
            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

            try
            {
                byte* curpos;

                fixed (byte* _res = res)
                {
                    byte*
                        _r = _res,
                        _g = _res + width * height,
                        _b = _res + 2 * width * height;

                    for (int h = 0; h < height; h++)
                    {
                        curpos = ((byte*)bmpData.Scan0) + h * bmpData.Stride;

                        for (int w = 0; w < width; w++)
                        {
                            *_b = *(curpos++); ++_b;
                            *_g = *(curpos++); ++_g;
                            *_r = *(curpos++); ++_r;
                        }
                    }
                }
            }
            finally
            {
                bmp.UnlockBits(bmpData);
            }
            return res;
        }

        // получение Bitmap из массива пикселей 
        public unsafe static Bitmap ByteRgbToBitmap(byte[,,] res)
        {
            int width = res.GetLength(2),
                height = res.GetLength(1);

            Bitmap bmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, bmp.PixelFormat);

            try
            {
                byte* curpos;

                fixed (byte* _res = res)
                {
                    byte*
                        _r = _res,
                        _g = _res + width * height,
                        _b = _res + 2 * width * height;

                    for (int h = 0; h < height; h++)
                    {
                        curpos = ((byte*)bmpData.Scan0) + h * bmpData.Stride;

                        for (int w = 0; w < width; w++)
                        {
                            *(curpos++) = *_b; ++_b;
                            *(curpos++) = *_g; ++_g;
                            *(curpos++) = *_r; ++_r;
                        }
                    }
                }
            }
            finally
            {
                bmp.UnlockBits(bmpData);
            }

            return bmp;
        }
    }
}
