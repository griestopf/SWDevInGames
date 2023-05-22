using System.Drawing;
using System.Drawing.Imaging;

// See https://aka.ms/new-console-template for more information
Image imgIBau = new Image("img/ibau_gross.jpg");
Console.WriteLine("ibau_gross.jpg geladen");
Image imgHFU = new Image("img/hfu.jpg");
Console.WriteLine("hfu.jpg geladen");
imgHFU.Blit(0, 0, 200, 71, imgIBau, 10, 10);
imgIBau.SaveAs("img/ibau_mit_logo.jpg");


public enum PixFormat
{
    R8_G8_B8,
    A8_R8_G8_B8,
    I8,
    R32_G32_B32,
    R32_G32_B32_A32,
    I32,
}



public class Image
{
    private byte[] _pixels;

    public int Width {init; get;}
    public int Height {init; get;}

    public PixFormat PixFormat;

    public int BytesPerPixel =>
        PixFormat switch {
            PixFormat.R8_G8_B8         => 3,
            PixFormat.A8_R8_G8_B8      => 4,
            PixFormat.I8               => 1,
            PixFormat.R32_G32_B32      => 3*4,
            PixFormat.R32_G32_B32_A32  => 4*4,
            PixFormat.I32              => 1*4,
            _                          => throw new ArgumentException("Don't know BytesPerPixel for pixel format: " + PixFormat)
        };

    private static void BlitClip(ref int iSrc, int sizeSrc, ref int sizeBlk, ref int iDst, int sizeDst)
    {
        int deltaMin = Math.Min(Math.Min(iSrc, iDst), 0);
        int deltaMax = Math.Max(Math.Max(iSrc+sizeBlk-sizeSrc, iDst+sizeBlk-sizeDst), 0);

        // Wende durch Überlappung entstehende Werte (deltaMin/deltaMax) auf die "ref"-Werte an.
        iSrc += deltaMin;
        iDst += deltaMin;
        sizeBlk += deltaMin;
        sizeBlk -= deltaMax;
        if (sizeBlk < 0)
            sizeBlk = 0;
    }

    delegate void CopyLineFunc(int iSrc, int iDst, int nPixels);

    public void Blit(int xs, int ys, int w, int h, Image dest, int xd, int yd)
    {
        BlitClip(ref xs, Width,  ref w, ref xd, dest.Width);
        BlitClip(ref ys, Height, ref h, ref yd, dest.Height);

        CopyLineFunc copyLine;

        if (PixFormat == dest.PixFormat)
        {
            copyLine = (int iSrc, int iDst, int nPixels) =>
            {
                Array.Copy(_pixels, iSrc, dest._pixels, iDst, nPixels * BytesPerPixel);
            };
        }
        else
        {
            switch(PixFormat)
            {
                case PixFormat.R8_G8_B8:
                    switch (dest.PixFormat)
                    {
                        case PixFormat.R8_G8_B8:
                            throw new Exception("Cannot handle combination of Source and Destination Pixel Format");
                            break;
                        case PixFormat.A8_R8_G8_B8:
                            copyLine = (int iSrc, int iDst, int nPixels) =>
                            {
                                for (int x = 0; x < nPixels; x++)
                                {
                                    int iByteDst = iDst + x * dest.BytesPerPixel;
                                    int iByteSrc = iSrc + x * BytesPerPixel;
                                    dest._pixels[iDst ] = 255;
                                    dest._pixels[iDst+1] = _pixels[iSrc];
                                    dest._pixels[iDst+2] = _pixels[iSrc+1];
                                    dest._pixels[iDst+3] = _pixels[iSrc+2];
                                }                                ;
                            };
                            break;
                        case PixFormat.I8:                
                            throw new Exception("Cannot handle combination of Source and Destination Pixel Format");
                            break;
                        default:
                             throw new Exception("Cannot handle combination of Source and Destination Pixel Format");
                   }
                    break;
                case PixFormat.A8_R8_G8_B8:
                    switch (dest.PixFormat)
                    {
                        case PixFormat.R8_G8_B8:
                             throw new Exception("Cannot handle combination of Source and Destination Pixel Format");
                           break;
                        case PixFormat.A8_R8_G8_B8:
                             throw new Exception("Cannot handle combination of Source and Destination Pixel Format");
                           break;
                        case PixFormat.I8:                
                             throw new Exception("Cannot handle combination of Source and Destination Pixel Format");
                           break;
                       default:
                             throw new Exception("Cannot handle combination of Source and Destination Pixel Format");
                    }
                    break;
                case PixFormat.I8:                
                    switch (dest.PixFormat)
                    {
                        case PixFormat.R8_G8_B8:
                             throw new Exception("Cannot handle combination of Source and Destination Pixel Format");
                           break;
                        case PixFormat.A8_R8_G8_B8:
                             throw new Exception("Cannot handle combination of Source and Destination Pixel Format");
                           break;
                        case PixFormat.I8:                
                             throw new Exception("Cannot handle combination of Source and Destination Pixel Format");
                           break;
                        default:
                             throw new Exception("Cannot handle combination of Source and Destination Pixel Format");

                    }
                    break;
                default:
                        throw new Exception("Cannot handle combination of Source and Destination Pixel Format");
            }

        }

        for (int y = 0; y < h; y++)
        {
            int iSrc = ((ys + y)*Width      + xs) * BytesPerPixel;
            int iDst = ((yd + y)*dest.Width + xd) * dest.BytesPerPixel;

            copyLine(iSrc, iDst , w);
            // Array.Copy(_pixels, iSrc, dest._pixels, iDst, w * BytesPerPixel);
        }
    }

    public void SaveAs(string path)
    {
        PixelFormat pf = PixFormat switch 
        {
            PixFormat.R8_G8_B8    => PixelFormat.Format24bppRgb,
            PixFormat.A8_R8_G8_B8 => PixelFormat.Format32bppArgb,
            _   => throw new ArgumentException($"Cannot save pixel format {Enum.GetName<PixFormat>(PixFormat)}.")
        };

        Bitmap bm = new Bitmap(Width, Height, pf);

        int bpp = BytesPerPixel;

        switch (PixFormat)
        {
            case PixFormat.R8_G8_B8:
                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        int pixIndex = (y * Width + x) * bpp;
                        bm.SetPixel(x, y, Color.FromArgb(_pixels[pixIndex + 0], _pixels[pixIndex + 1], _pixels[pixIndex + 2]));
                    }
                }                
                break;
            case PixFormat.A8_R8_G8_B8:
                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        int pixIndex = (y * Width + x) * bpp;
                        bm.SetPixel(x, y, Color.FromArgb(_pixels[pixIndex + 0], _pixels[pixIndex + 1], _pixels[pixIndex + 2], _pixels[pixIndex + 3]));
                    }
                }                
                break;                
        }

        bm.Save(path);
    }

    public Image(string path)
    {
        Bitmap bm = new Bitmap(path);
        Width = bm.Width;
        Height = bm.Height;

        PixFormat = bm.PixelFormat switch
        {
            PixelFormat.Format24bppRgb  => PixFormat.R8_G8_B8,
            PixelFormat.Format32bppArgb => PixFormat.A8_R8_G8_B8,
            _                           => throw new ArgumentException("Unkown pixel format in " + path)
        };

         _pixels = new byte[Width * Height * BytesPerPixel];

        int bpp = BytesPerPixel;

        switch (PixFormat)
        {
            case PixFormat.R8_G8_B8:
                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        Color col = bm.GetPixel(x, y);

                        int pixIndex = (y * Width + x) * bpp;
                        _pixels[pixIndex + 0] = col.R;
                        _pixels[pixIndex + 1] = col.G;
                        _pixels[pixIndex + 2] = col.B;
                    }
                }                
                break;
            case PixFormat.A8_R8_G8_B8:
                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        Color col = bm.GetPixel(x, y);

                        int pixIndex = (y * Width + x) * bpp;
                        _pixels[pixIndex + 0] = col.A;
                        _pixels[pixIndex + 1] = col.R;
                        _pixels[pixIndex + 2] = col.G;
                        _pixels[pixIndex + 3] = col.B;
                    }
                }                
                break;                
        }
    }

    public Image(int width, int height, PixFormat pf)
    {
        PixFormat = pf;
       
        Width = width;
        Height = height;

        _pixels = new byte[Width * Height * BytesPerPixel];
    }
}