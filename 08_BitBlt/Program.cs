using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

/*
//RGBA->RGB Conversion Test
Image hfu_trans_huge = new Image("img/hfu_transparent_huge.png");
Console.WriteLine($"Pixformatn: {Enum.GetName<PixFormat>(hfu_trans_huge.PixFormat)}");

Image hfu_rgb_huge = new Image(hfu_trans_huge.Width, hfu_trans_huge.Height, PixFormat.R8_G8_B8);

hfu_trans_huge.Blit(0, 0, hfu_rgb_huge.Width, hfu_rgb_huge.Height, hfu_rgb_huge, 0, 0);
hfu_rgb_huge.SaveAs("img/out/hfu_rgb_huge.jpg");
*/
/*
Image ibau = new Image("img/ibau_gross.jpg");
Image ibau_sw = new Image(ibau.Width, ibau.Height, PixFormat.I8);

ibau.Blit(0, 0, ibau.Width, ibau.Height, ibau_sw, 0, 0);

Image ibau_reconverted = new Image(ibau.Width, ibau.Height, PixFormat.R8_G8_B8);

ibau_sw.Blit(0, 0, ibau.Width, ibau.Height, ibau_reconverted, 0, 0);

ibau_reconverted.SaveAs("img/out/ibau_bw.jpg");
*/
/*
// RGB->SW->RGB Test
Image ibau = new Image("img/ibau_gross.jpg");
Image ibau_sw = new Image(ibau.Width, ibau.Height, PixFormat.I8);

ibau.Blit(0, 0, ibau.Width, ibau.Height, ibau_sw, 0, 0);
Image ibau_reconverted = new Image(ibau.Width, ibau.Height, PixFormat.R8_G8_B8);
ibau_sw.Blit(0, 0, ibau.Width, ibau.Height, ibau_reconverted, 0, 0);

ibau_reconverted.SaveAs("img/out/ibau_l8_RGB.jpg");
*/

/*
//RGBA->RGB Conversion Test
Image hfu_trans_huge = new Image("img/hfu_transparent_huge.png");
Console.WriteLine($"Pixformatn: {Enum.GetName<PixFormat>(hfu_trans_huge.PixFormat)}");

Image hfu_rgb_huge = new Image(hfu_trans_huge.Width, hfu_trans_huge.Height, PixFormat.R8_G8_B8);

hfu_trans_huge.BlitSse(0, 0, hfu_rgb_huge.Width, hfu_rgb_huge.Height, hfu_rgb_huge, 0, 0);
hfu_rgb_huge.SaveAs("img/out/hfu_rgb_huge.jpg");
*/

// ARGB to RGB conversion test
Image hfu_argb = new Image("img/hfu_transparent_huge.png");
Debug.Assert(hfu_argb.PixFormat == PixFormat.A8_R8_G8_B8);

Stopwatch sw = new Stopwatch();

long msArr = 0;
long msPtr = 0; 
long msSse = 0;

// cache warm-up for pointer access
Image hfu_rgb_ptrwarmup = new Image(hfu_argb.Width, hfu_argb.Height, PixFormat.R8_G8_B8);
hfu_argb.BlitPtr(0, 0, hfu_argb.Width, hfu_argb.Height, hfu_rgb_ptrwarmup, 0, 0);

// cache warm-up for sse access
Image hfu_rgb_ssewarmup = new Image(hfu_argb.Width, hfu_argb.Height, PixFormat.R8_G8_B8);
hfu_argb.BlitSse(0, 0, hfu_argb.Width, hfu_argb.Height, hfu_rgb_ssewarmup, 0, 0);

// cache warm-up for array access
Image hfu_rgb_arrwarmup = new Image(hfu_argb.Width, hfu_argb.Height, PixFormat.R8_G8_B8);
hfu_argb.Blit(0, 0, hfu_argb.Width, hfu_argb.Height, hfu_rgb_arrwarmup, 0, 0);


for (int i = 0; i < 100; i++)
{
    // stopwatch pointer access
    Image hfu_rgb_ptr = new Image(hfu_argb.Width, hfu_argb.Height, PixFormat.R8_G8_B8);
    sw.Restart();
    hfu_argb.BlitPtr(0, 0, hfu_argb.Width, hfu_argb.Height, hfu_rgb_ptr, 0, 0);
    msPtr += sw.ElapsedMilliseconds;
    sw.Stop();

    // stopwatch sse access
    Image hfu_rgb_sse = new Image(hfu_argb.Width, hfu_argb.Height, PixFormat.R8_G8_B8);
    sw.Restart();
    hfu_argb.BlitSse(0, 0, hfu_argb.Width, hfu_argb.Height, hfu_rgb_sse, 0, 0);
    msSse += sw.ElapsedMilliseconds;
    sw.Stop();

    // stopwatch array access
    Image hfu_rgb_arr = new Image(hfu_argb.Width, hfu_argb.Height, PixFormat.R8_G8_B8);
    sw.Restart();
    hfu_argb.Blit(0, 0, hfu_argb.Width, hfu_argb.Height, hfu_rgb_arr, 0, 0);
    msArr += sw.ElapsedMilliseconds;
    sw.Stop();
}

Console.WriteLine($"Array Access duration: {msArr/1000.0}s; Pointer Access duration: {msPtr/1000.0}s; SSE Access duration: {msSse/1000.0}s; ");

hfu_rgb_ssewarmup.SaveAs("img/out/hfu_rgb_huge.jpg");


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
    byte[] _pixels;

    public int Width { get; init;}
    public int Height { get; init;}
    public PixFormat PixFormat { get; init;}

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


    public Image(int width, int height, PixFormat pixFormat)
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


    private static void BlitClip(ref int iSrc, int sizeSrc, ref int sizeBlk, ref int iDst, int sizeDst)
    {
        // Adjust left/upper border
        // The negative number with the biggest magnitude of negative start indices (or 0, if both are 0 or bigger).
        int iDeltaMin = Math.Min(0, Math.Min(iDst, iSrc));

        // Adjust right/lower border
        // The biggest overlap over the right/lower border (or 0 if no overlap).
        int iDeltaMax = Math.Max(0, Math.Max(iDst + sizeBlk - sizeDst, iSrc + sizeBlk - sizeSrc));

        iDst -= iDeltaMin;
        iSrc -= iDeltaMin;
        sizeBlk += iDeltaMin;
        sizeBlk -= iDeltaMax;
        if (sizeBlk < 0)
            sizeBlk = 0;
    }


    #region ArrayAccess
    ///////////////////////////////////////////////////////////////////////////
    // Array access
    ///////////////////////////////////////////////////////////////////////////

    delegate void CopyLine(byte[] srcPxl, int iSrc, byte[] dstPxl, int iDst, int nPixels);

    public void Blit(int xSrc, int ySrc, int w, int h, Image dst, int xDst, int yDst)
    {
        BlitClip(ref xSrc, Width,  ref w, ref xDst, dst.Width);
        BlitClip(ref ySrc, Height, ref h, ref yDst, dst.Height);

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
                case PixFormat.R8_G8_B8:
                    switch (dst.PixFormat)
                    {
                        case PixFormat.A8_R8_G8_B8:

                            break;
                        case PixFormat.I8:
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
                case PixFormat.A8_R8_G8_B8:
                    switch (dst.PixFormat)
                    {
                        case PixFormat.R8_G8_B8:
                            copyLine = (srcPxl, iSrc, dstPxl, iDst, nPixels) =>
                            {
                                int nBytesSrc = nPixels * BytesPerPixel;
                                int iByteSrc = iSrc;
                                int iByteDst = iDst;
                                for (int x = 0; x < nPixels; x++)
                                {
                                    iByteSrc++;                                 // skip alpha
                                    dstPxl[iByteDst++] = srcPxl[iByteSrc++];    // copy R
                                    dstPxl[iByteDst++] = srcPxl[iByteSrc++];    // copy G
                                    dstPxl[iByteDst++] = srcPxl[iByteSrc++];    // copy B
                                }
                            };

                            break;
                        case PixFormat.I8:
                            break;
                    }
                    break;
                case PixFormat.I8:
                    switch (dst.PixFormat)
                    {
                        case PixFormat.R8_G8_B8:
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
                        case PixFormat.A8_R8_G8_B8:

                            break;
                    }
                    break;
            }

        }

        if (copyLine == null)
            throw new ArgumentException($"Cannot convert pixels from {Enum.GetName<PixFormat>(PixFormat)} to {Enum.GetName<PixFormat>(dst.PixFormat)}");

        for (int y = 0; y < h; y++)
        {
                int iSrc =  ((ySrc + y) * Width     + xSrc) * BytesPerPixel;
                int iDst =  ((yDst + y) * dst.Width + xDst) * dst.BytesPerPixel;
                copyLine(_pixels, iSrc, dst._pixels, iDst, w);
                // Array.Copy(_pixels, iSrc, dest._pixels, iDst, w * BytesPerPixel);
        }
    }
    #endregion

    #region PointerAccess
    ///////////////////////////////////////////////////////////////////////////
    // Pointer access
    ///////////////////////////////////////////////////////////////////////////


    unsafe delegate void CopyLinePtr(byte *pSrc, int iSrc, byte* pDst, int iDst, int nPixels);

    unsafe public void BlitPtr(int xSrc, int ySrc, int w, int h, Image dst, int xDst, int yDst)
    {
        BlitClip(ref xSrc, Width,  ref w, ref xDst, dst.Width);
        BlitClip(ref ySrc, Height, ref h, ref yDst, dst.Height);

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
                    case PixFormat.R8_G8_B8:
                        switch (dst.PixFormat)
                        {
                            case PixFormat.A8_R8_G8_B8:
                                break;
                            case PixFormat.I8:
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
                    case PixFormat.A8_R8_G8_B8:
                        switch (dst.PixFormat)
                        {
                            case PixFormat.R8_G8_B8:
                                copyLine = (pSrc, iSrc, pDst, iDst, nPixels) =>
                                {
                                    pSrc += iSrc;
                                    pDst += iDst;
                                    byte* pEndSrc = pSrc + nPixels * BytesPerPixel;

                                    while (pSrc < pEndSrc)
                                    {
                                        pSrc++;             // skip alpha
                                        *pDst++ = *pSrc++;  // copy R
                                        *pDst++ = *pSrc++;  // copy G
                                        *pDst++ = *pSrc++;  // copy B
                                    }
                                };

                                break;
                            case PixFormat.I8:

                                break;
                        }
                        break;
                    case PixFormat.I8:
                        switch (dst.PixFormat)
                        {
                            case PixFormat.R8_G8_B8:
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
                            case PixFormat.A8_R8_G8_B8:

                                break;
                        }
                        break;
                }

            }

            if (copyLine == null)
                throw new ArgumentException($"Cannot convert pixels from {Enum.GetName<PixFormat>(PixFormat)} to {Enum.GetName<PixFormat>(dst.PixFormat)}");

            for (int y = 0; y < h; y++)
            {
                    int iSrc =  ((ySrc + y) * Width     + xSrc) * BytesPerPixel;
                    int iDst =  ((yDst + y) * dst.Width + xDst) * dst.BytesPerPixel;
                    copyLine(pSrcPxl, iSrc, pDstPxl, iDst, w);
            }
        }

    }
    #endregion

    #region  StreamingSIMDExtensionAccess

   ///////////////////////////////////////////////////////////////////////////
   // SSE (Streaming SIMD Extensions) access
   ///////////////////////////////////////////////////////////////////////////
   // Using the SSSE3 (Supplemental Streaming SIMD Extension v3) intrinsic _mm_shuffle_epi8(__m128i a, __m128i b);
   // See: https://www.intel.com/content/www/us/en/develop/documentation/cpp-compiler-developer-guide-and-reference/top/compiler-reference/intrinsics/intrinsics-for-ssse3/shuffle-intrinsics.html
   // For an overview of all intel-based vector/SIMD/"Multimedia" intrinsics, see: https://www.intel.com/content/www/us/en/docs/intrinsics-guide/index.html
   // Nice: Intel Pentium MMX Campaign TV-Add from 1997: https://www.youtube.com/watch?v=5zyjSBSvqPc

    unsafe public void BlitSse(int xSrc, int ySrc, int w, int h, Image dst, int xDst, int yDst)
    {
        BlitClip(ref xSrc, Width,  ref w, ref xDst, dst.Width);
        BlitClip(ref ySrc, Height, ref h, ref yDst, dst.Height);

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
                    case PixFormat.R8_G8_B8:
                        switch (dst.PixFormat)
                        {
                            case PixFormat.A8_R8_G8_B8:
                                break;
                            case PixFormat.I8:
                                /*
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
                                };*/
                                break;
                        }
                        break;
                    case PixFormat.A8_R8_G8_B8:
                        switch (dst.PixFormat)
                        {
                            case PixFormat.R8_G8_B8:

                                if (Ssse3.IsSupported)
                                {
                                    byte[] arrShufMask = {1, 2, 3, 5, 6, 7, 9, 10, 11, 13, 14, 15, 0xFF, 0xFF, 0xFF, 0xFF};
                                    fixed (byte *pShufMask = arrShufMask)
                                    {
                                        var vShufMask = Ssse3.LoadVector128(pShufMask);

                                        copyLine = (pSrc, iSrc, pDst, iDst, nPixels) =>
                                        {
                                            pSrc += iSrc;
                                            pDst += iDst;
                                            int nBytes = nPixels * BytesPerPixel;
                                            int nBytesPacked = nBytes - nBytes % 16;
                                            byte* pEndSrc       = pSrc + nBytes;
                                            byte* pEndSrcPacked = pSrc + nBytesPacked;

                                            while (pSrc < pEndSrcPacked)
                                            {
                                                Vector128<byte> vSrc = Ssse3.LoadVector128(pSrc);
                                                Vector128<byte> vDst = Ssse3.Shuffle(vSrc, vShufMask);
                                                Ssse3.Store(pDst, vDst);
                                                pSrc+=16;
                                                pDst+=12;
                                            }

                                            while (pSrc < pEndSrc)
                                            {
                                                pSrc++;             // skip alpha
                                                *pDst++ = *pSrc++;  // copy R
                                                *pDst++ = *pSrc++;  // copy G
                                                *pDst++ = *pSrc++;  // copy B
                                            }
                                        };
                                    }
                                }
                                else // fallback to pointer access
                                {
                                    copyLine = (pSrc, iSrc, pDst, iDst, nPixels) =>
                                    {
                                        pSrc += iSrc;
                                        pDst += iDst;
                                        byte* pEndSrc = pSrc + nPixels * BytesPerPixel;

                                        while (pSrc < pEndSrc)
                                        {
                                            pSrc++;             // skip alpha
                                            *pDst++ = *pSrc++;  // copy R
                                            *pDst++ = *pSrc++;  // copy G
                                            *pDst++ = *pSrc++;  // copy B
                                        }
                                    };
                                }

                                break;
                            case PixFormat.I8:

                                break;
                        }
                        break;
                    case PixFormat.I8:
                        switch (dst.PixFormat)
                        {
                            case PixFormat.R8_G8_B8:
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
                            case PixFormat.A8_R8_G8_B8:

                                break;
                        }
                        break;
                }

            }

            if (copyLine == null)
                throw new ArgumentException($"Cannot convert pixels from {Enum.GetName<PixFormat>(PixFormat)} to {Enum.GetName<PixFormat>(dst.PixFormat)}");

            for (int y = 0; y < h; y++)
            {
                    int iSrc =  ((ySrc + y) * Width     + xSrc) * BytesPerPixel;
                    int iDst =  ((yDst + y) * dst.Width + xDst) * dst.BytesPerPixel;
                    copyLine(pSrcPxl, iSrc, pDstPxl, iDst, w);
            }
        }

    }

    #endregion
}