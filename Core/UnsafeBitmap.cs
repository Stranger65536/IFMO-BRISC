using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace BRISC.Core
{
    /// <summary>
    /// Provides fast access to bitmap data via unsafe code
    /// </summary>
    /// <remarks>
    /// This class wraps the Bitmap class and provides additional 
    /// functionality, in the form of GetPixel() and SetPixel() methods 
    /// that work much faster than the ordinary ones, thanks to 
    /// pointers and unsafe code. To use these functions, you must 
    /// first lock the bitmap (make sure you unlock it when you're 
    /// done). Use the PixelData class to represent colors.
    /// </remarks>
    /// <example>
    /// <code>
    /// UnsafeBitmap uBmp = new UnsafeBitmap(Image.FromFile("image.png"));
    /// UnsafeBitmap.PixelData pData;
    /// uBmp.LockBitmap();
    /// pData.red = pData.blue = pData.green = 255;
    /// uBmp.SetPixel(0, 0, pData);
    /// uBmp.UnlockBitmap();
    /// picbox.Image = uBmp.Bitmap;
    /// </code>
    /// </example>
    public unsafe class UnsafeBitmap
    {
        /// <summary>
        /// Main bitmap object
        /// </summary>
        private Bitmap bitmap;

        /// <summary>
        /// Number of bytes in a row
        /// </summary>
        private int width;

        /// <summary>
        /// Stores locked pixel data
        /// </summary>
        private BitmapData bitmapData = null;

        /// <summary>
        /// Pointer to first pixel
        /// </summary>
        private Byte* pBase = null;

        /// <summary>
        /// Creates a new UnsafeBitmap object based on an existing Bitmap
        /// </summary>
        /// <param name="bitmap">Bitmap object to use</param>
        public UnsafeBitmap(Bitmap bitmap)
        {
            this.bitmap = new Bitmap(bitmap);
        }

        /// <summary>
        /// Create a new, blank UnsafeBitmap object with a given size
        /// </summary>
        /// <param name="width">Width (in pixels) of new bitmap</param>
        /// <param name="height">Height (in pixels) of new bitmap</param>
        public UnsafeBitmap(int width, int height)
        {
            this.bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
        }

        /// <summary>
        /// Create a new UnsafeBitmap object based on an existing Bitmap, but 
        /// scaled to a new width and height (NOTE: currently only scales when 
        /// both new dimensions are greater than the original size).
        /// </summary>
        /// <param name="bitmap">Bitmap object to use</param>
        /// <param name="width">Width (in pixels) of new bitmap</param>
        /// <param name="height">Height (in pixels) of new bitmap</param>
        public UnsafeBitmap(Bitmap bitmap, int width, int height)
        {
            if (width > bitmap.Width && height > bitmap.Height)
            {
                // calculate actual size of new image
                int blockWidth = width / bitmap.Width;
                int blockHeight = height / bitmap.Height;
                int actualWidth = bitmap.Width * blockWidth;
                int actualHeight = bitmap.Height * blockHeight;

                // create blank bitmap for resized image
                this.bitmap = new Bitmap(actualWidth, actualHeight, PixelFormat.Format24bppRgb);

                // create work bitmap for original image
                UnsafeBitmap temp = new UnsafeBitmap(bitmap);

                // lock bitmaps and copy image data from original image to new one
                LockBitmap();
                temp.LockBitmap();
                for (int r = 0; r < actualHeight; r++)
                {
                    for (int c = 0; c < actualWidth; c++)
                    {
                        SetPixel(c, r, temp.GetPixel(c / blockWidth, r / blockHeight));
                    }
                }
                temp.UnlockBitmap();
                UnlockBitmap();
            }
            else
            {
                this.bitmap = bitmap;
            }
        }

        /// <summary>
        /// Dispose of the Bitmap object
        /// </summary>
        public void Dispose()
        {
            bitmap.Dispose();
        }

        /// <summary>
        /// Returns a reference to the actual Bitmap object
        /// </summary>
        public Bitmap Bitmap
        {
            get
            {
                return (bitmap);
            }
        }

        /// <summary>
        /// Returns the image size in pixels
        /// </summary>
        private Point PixelSize
        {
            get
            {
                GraphicsUnit unit = GraphicsUnit.Pixel;
                RectangleF bounds = bitmap.GetBounds(ref unit);

                return new Point((int)bounds.Width, (int)bounds.Height);
            }
        }

        /// <summary>
        /// Prepares the bitmap for unsafe operations. Call this before using GetPixel() or SetPixel().
        /// </summary>
        public void LockBitmap()
        {
            GraphicsUnit unit = GraphicsUnit.Pixel;
            RectangleF boundsF = bitmap.GetBounds(ref unit);
            Rectangle bounds = new Rectangle((int)boundsF.X,
             (int)boundsF.Y,
             (int)boundsF.Width,
             (int)boundsF.Height);

            // Figure out the number of bytes in a row
            // This is rounded up to be a multiple of 4
            // bytes, since a scan line in an image must always be a multiple of 4 bytes
            // in length.
            width = (int)boundsF.Width * sizeof(PixelData);
            if (width % 4 != 0)
            {
                width = 4 * (width / 4 + 1);
            }
            bitmapData =
             bitmap.LockBits(bounds, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            pBase = (Byte*)bitmapData.Scan0.ToPointer();
        }

        /// <summary>
        /// Get color data for a given pixel
        /// </summary>
        /// <param name="x">Column of pixel (zero-based)</param>
        /// <param name="y">Row of pixel (zero-based)</param>
        /// <returns>PixelData color structure</returns>
        public PixelData GetPixel(int x, int y)
        {
            PixelData returnValue = *PixelAt(x, y);
            return returnValue;
        }

        /// <summary>
        /// Set color data for a given pixel
        /// </summary>
        /// <param name="x">Column of pixel (zero-based)</param>
        /// <param name="y">Row of pixel (zero-based)</param>
        /// <param name="colour">PixelData color structure</param>
        public void SetPixel(int x, int y, PixelData colour)
        {
            PixelData* pixel = PixelAt(x, y);
            *pixel = colour;
        }

        /// <summary>
        /// Releases the bitmap data after unsafe operations. Call this after using GetPixel() or SetPixel().
        /// </summary>
        public void UnlockBitmap()
        {
            bitmap.UnlockBits(bitmapData);
            bitmapData = null;
            pBase = null;
        }

        /// <summary>
        /// Obtain a reference to a particular pixel
        /// </summary>
        /// <param name="x">Column of pixel (zero-based)</param>
        /// <param name="y">Row of pixel (zero-based)</param>
        /// <returns>Pointer to PixelData color structure</returns>
        private PixelData* PixelAt(int x, int y)
        {
            return (PixelData*)(pBase + y * width + x * sizeof(PixelData));
        }

        /// <summary>
        /// Stores 8-bit color RGB component data (24 bpp)
        /// </summary>
        public struct PixelData
        {
            /// <summary>
            /// Blue component value (0-255)
            /// </summary>
            public byte blue;
            /// <summary>
            /// Green component value (0-255)
            /// </summary>
            public byte green;
            /// <summary>
            /// Red component value (0-255)
            /// </summary>
            public byte red;
        }
    }
}