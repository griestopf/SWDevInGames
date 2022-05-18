using System.Drawing;
using System.Drawing.Imaging;

Image ibau = new Image("img/ibau_gross.jpg");
// Image hfulogo = new Image("img/hfu.jpg");

Image ibau_sw = new Image(ibau.Width, ibau.Height, PixFmt.L8);

ibau.BltPtr(0, 0, ibau.Width, ibau.Height, ibau_sw, 0, 0);
Image ibau_reconverted = new Image(ibau.Width, ibau.Height, PixFmt.R8_G8_B8);
ibau_sw.BltPtr(0, 0, ibau.Width, ibau.Height, ibau_reconverted, 0, 0);

ibau_reconverted.SaveAs("img/out/ibau_l8_RGB.jpg");




public enum PixFmt
{
    R8_G8_B8,
    A8_R8_G8_B8,
    L8,
}

public class Image
{
    byte[] _pixels;

    public int Width { get; private set;}
    public int Height { get; private set;}
    public PixFmt PixFormat { get; private set;}

    public Image(int width, int height, PixFmt pixFormat)
    {
        Width = width;
        Height = height;
        PixFormat = pixFormat;

        _pixels = new byte[Width * Height * BytesPerPixel];
    }  

    public Image(string path)
    {
        Bitmap bm = new Bitmap(path);
        Width = bm.Width;
        Height = bm.Height;

        PixFormat = bm.PixelFormat switch
        {
            PixelFormat.Format24bppRgb  => PixFmt.R8_G8_B8,
            PixelFormat.Format32bppArgb => PixFmt.A8_R8_G8_B8,
            _                           => throw new ArgumentException("Unkown pixel format in " + path)
        };

         _pixels = new byte[Width * Height * BytesPerPixel];

        int bpp = BytesPerPixel;

        switch (PixFormat)
        {
            case PixFmt.R8_G8_B8:
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
            case PixFmt.A8_R8_G8_B8:
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


    public void SaveAs(string path)
    {
        PixelFormat pf = PixFormat switch 
        {
            PixFmt.R8_G8_B8    => PixelFormat.Format24bppRgb,
            PixFmt.A8_R8_G8_B8 => PixelFormat.Format32bppArgb,
            _   => throw new ArgumentException($"Cannot save pixel format {Enum.GetName<PixFmt>(PixFormat)}.")
        };

        Bitmap bm = new Bitmap(Width, Height, pf);

        int bpp = BytesPerPixel;

        switch (PixFormat)
        {
            case PixFmt.R8_G8_B8:
                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        int pixIndex = (y * Width + x) * bpp;
                        bm.SetPixel(x, y, Color.FromArgb(_pixels[pixIndex + 0], _pixels[pixIndex + 1], _pixels[pixIndex + 2]));
                    }
                }                
                break;
            case PixFmt.A8_R8_G8_B8:
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


    private static void ClipBlt(int sizeSrc, ref int iSrc, int sizeDst, ref int iDst, ref int sizeBlk)
    {
        // Adjust left border
        // The negative number with the biggest magnitude of negative start indices (or 0, if both are 0 or bigger).
        // int iDeltaL = M.Min(0, M.Min(iDst, iSrc));
        int iDeltaL = (iDst < iSrc) ? iDst : iSrc;
        if (iDeltaL > 0)
            iDeltaL = 0;

        // Adjust right border
        // The biggest overlap over the right border (or 0 if no overlap).
        // int iDeltaR = M.Max(0, M.Max(iDst + sizeBlk - sizeDst, iSrc + sizeBlk - sizeSrc));
        int dstRb = iDst + sizeBlk - sizeDst;
        int srcRb = iSrc + sizeBlk - sizeSrc;
        int iDeltaR = (dstRb > srcRb) ? dstRb : srcRb;
        if (iDeltaR < 0)
            iDeltaR = 0;

        iDst -= iDeltaL;
        iSrc -= iDeltaL;
        sizeBlk += iDeltaL;
        sizeBlk -= iDeltaR;
        if (sizeBlk < 0)
            sizeBlk = 0;
    }


    unsafe delegate void CopyLinePtr(byte *pSrc, int iSrc, byte* pDst, int iDst, int nPixels);

    unsafe public void BltPtr(int xSrc, int ySrc, int w, int h, Image dst, int xDst, int yDst)
    {
        ClipBlt(Width, ref xSrc, dst.Width, ref xDst, ref w);
        ClipBlt(Height, ref ySrc, dst.Height, ref yDst, ref h);

        CopyLinePtr? copyLine = null;

        fixed (byte* pSrcPxl = _pixels, pDstPxl = dst._pixels)
        {
            if (PixFormat == dst.PixFormat)
            {
                copyLine = (pSrc, iSrc, pDstPxl, iDst, nPixels) =>
                {
                    // Array.Copy(srcPxl, iSrc, dstPxl, iDst, nPixels * BytesPerPixel);
                };
            }
            else
            {
                switch (PixFormat)
                {
                    case PixFmt.R8_G8_B8:
                        switch (dst.PixFormat)
                        {
                            case PixFmt.A8_R8_G8_B8:

                                break;
                            case PixFmt.L8:
                                copyLine = (pSrc, iSrc, pDst, iDst, nPixels) =>
                                {
                                    for (int x = 0; x < nPixels; x++)
                                    {
                                        int iSrcLine = iSrc + x * (BytesPerPixel);
                                        int col = *(pSrc + iSrcLine    ) * 2;
                                        col +=    *(pSrc + iSrcLine + 1) * 3;
                                        col +=    *(pSrc + iSrcLine + 2);
                                        col /= 6;
                                        *(pDst + iDst + x * dst.BytesPerPixel) = (byte) col;
                                    }
                                };
                                break;
                        }
                        break;
                    case PixFmt.A8_R8_G8_B8:
                        switch (dst.PixFormat)
                        {
                            case PixFmt.R8_G8_B8:
                                break;
                            case PixFmt.L8:

                                break;
                        }
                        break;
                    case PixFmt.L8:
                        switch (dst.PixFormat)
                        {
                            case PixFmt.R8_G8_B8:
                                copyLine = (pSrc, iSrc, pDst, iDst, nPixels) =>
                                {
                                    for (int x = 0; x < nPixels; x++)
                                    {
                                        byte intensity = *(pSrc + iSrc + x * BytesPerPixel);

                                        int iDstLine = iDst + x * dst.BytesPerPixel;
                                        *(pDst + iDstLine  ) = intensity;
                                        *(pDst + iDstLine+1) = intensity;
                                        *(pDst + iDstLine+2) = intensity;
                                    }
                                };
                                break;
                            case PixFmt.A8_R8_G8_B8:

                                break;
                        }
                        break;
                }

            }

            if (copyLine == null)
                throw new ArgumentException($"Cannot convert pixels from {Enum.GetName<PixFmt>(PixFormat)} to {Enum.GetName<PixFmt>(dst.PixFormat)}");

            for (int y = 0; y < h; y++)
            {
                    int iSrc =  ((ySrc + y) * Width     + xSrc) * BytesPerPixel;
                    int iDst =  ((yDst + y) * dst.Width + xDst) * dst.BytesPerPixel;
                    copyLine(pSrcPxl, iSrc, pDstPxl, iDst, w);
            }
        }

    }



    ///////////////////////////////////////////////////////////////////////////

    delegate void CopyLine(byte[] srcPxl, int iSrc, byte[] dstPxl, int iDst, int nPixels);

    public void Blt(int xSrc, int ySrc, int w, int h, Image dst, int xDst, int yDst)
    {
        ClipBlt(Width, ref xSrc, dst.Width, ref xDst, ref w);
        ClipBlt(Height, ref ySrc, dst.Height, ref yDst, ref h);

        CopyLine? copyLine = null;

        if (PixFormat == dst.PixFormat)
        {
            copyLine = (srcPxl, iSrc, dstPxl, iDst, nPixels) =>
            {
                Array.Copy(srcPxl, iSrc, dstPxl, iDst, nPixels * BytesPerPixel);
            };
        }
        else
        {
            switch (PixFormat)
            {
                case PixFmt.R8_G8_B8:
                    switch (dst.PixFormat)
                    {
                        case PixFmt.A8_R8_G8_B8:

                            break;
                        case PixFmt.L8:
                            copyLine = (srcPxl, iSrc, dstPxl, iDst, nPixels) =>
                            {
                                for (int x = 0; x < nPixels; x++)
                                {
                                    int iSrcLine = iSrc + x * (BytesPerPixel);
                                    int col = srcPxl[iSrcLine  ] * 2;
                                    col +=    srcPxl[iSrcLine+1] * 3;
                                    col +=    srcPxl[iSrcLine+2];
                                    col /= 6;
                                    dstPxl[iDst + x * dst.BytesPerPixel] = (byte) col;
                                }
                            };
                            break;
                    }
                    break;
                case PixFmt.A8_R8_G8_B8:
                    switch (dst.PixFormat)
                    {
                        case PixFmt.R8_G8_B8:
                            break;
                        case PixFmt.L8:

                            break;
                    }
                    break;
                case PixFmt.L8:
                    switch (dst.PixFormat)
                    {
                        case PixFmt.R8_G8_B8:
                            copyLine = (srcPxl, iSrc, dstPxl, iDst, nPixels) =>
                            {
                                for (int x = 0; x < nPixels; x++)
                                {
                                    byte intensity = srcPxl[iSrc + x * BytesPerPixel];

                                    int iDstLine = iDst + x * dst.BytesPerPixel;
                                    dstPxl[iDstLine  ] = intensity;
                                    dstPxl[iDstLine+1] = intensity;
                                    dstPxl[iDstLine+2] = intensity;
                                }
                            };
                            break;
                        case PixFmt.A8_R8_G8_B8:

                            break;
                    }
                    break;
            }

        }

        if (copyLine == null)
            throw new ArgumentException($"Cannot convert pixels from {Enum.GetName<PixFmt>(PixFormat)} to {Enum.GetName<PixFmt>(dst.PixFormat)}");

        for (int y = 0; y < h; y++)
        {
                int iSrc =  ((ySrc + y) * Width     + xSrc) * BytesPerPixel;
                int iDst =  ((yDst + y) * dst.Width + xDst) * dst.BytesPerPixel;
                copyLine(_pixels, iSrc, dst._pixels, iDst, w);
        }
    }





    public int BytesPerPixel => PixFormat switch 
    {
        PixFmt.R8_G8_B8    => 3,
        PixFmt.A8_R8_G8_B8 => 4,
        PixFmt.L8          => 1,        
        _   => throw new ArgumentException($"Unkown pixel size for format {Enum.GetName<PixFmt>(PixFormat)}.")
    };

}