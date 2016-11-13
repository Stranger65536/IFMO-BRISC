using System.Drawing;
using System.Drawing.Imaging;


namespace BRISC.Core
{
    public unsafe class UnsafeBitmap
    {
        private BitmapData bitmapData;


        private byte* pBase = null;


        private int width;


        public UnsafeBitmap(Bitmap bitmap)
        {
            Bitmap = new Bitmap(bitmap);
        }


        public UnsafeBitmap(int width, int height)
        {
            Bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
        }


        public UnsafeBitmap(Bitmap bitmap, int width, int height)
        {
            if (width > bitmap.Width && height > bitmap.Height)
            {
                // calculate actual size of new image
                var blockWidth = width/bitmap.Width;
                var blockHeight = height/bitmap.Height;
                var actualWidth = bitmap.Width*blockWidth;
                var actualHeight = bitmap.Height*blockHeight;

                // create blank bitmap for resized image
                Bitmap = new Bitmap(actualWidth, actualHeight, PixelFormat.Format24bppRgb);

                // create work bitmap for original image
                var temp = new UnsafeBitmap(bitmap);

                // lock bitmaps and copy image data from original image to new one
                LockBitmap();
                temp.LockBitmap();
                for (var r = 0; r < actualHeight; r++)
                {
                    for (var c = 0; c < actualWidth; c++)
                    {
                        SetPixel(c, r, temp.GetPixel(c/blockWidth, r/blockHeight));
                    }
                }
                temp.UnlockBitmap();
                UnlockBitmap();
            }
            else
            {
                Bitmap = bitmap;
            }
        }


        public Bitmap Bitmap { get; }


        private Point PixelSize
        {
            get
            {
                var unit = GraphicsUnit.Pixel;
                var bounds = Bitmap.GetBounds(ref unit);

                return new Point((int) bounds.Width, (int) bounds.Height);
            }
        }


        public void Dispose()
        {
            Bitmap.Dispose();
        }


        public void LockBitmap()
        {
            var unit = GraphicsUnit.Pixel;
            var boundsF = Bitmap.GetBounds(ref unit);
            var bounds = new Rectangle((int) boundsF.X,
                (int) boundsF.Y,
                (int) boundsF.Width,
                (int) boundsF.Height);

            // Figure out the number of bytes in a row
            // This is rounded up to be a multiple of 4
            // bytes, since a scan line in an image must always be a multiple of 4 bytes
            // in length.
            width = (int) boundsF.Width*sizeof(PixelData);
            if (width%4 != 0)
            {
                width = 4*(width/4 + 1);
            }
            bitmapData =
                Bitmap.LockBits(bounds, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            pBase = (byte*) bitmapData.Scan0.ToPointer();
        }


        public PixelData GetPixel(int x, int y)
        {
            var returnValue = *PixelAt(x, y);
            return returnValue;
        }


        public void SetPixel(int x, int y, PixelData colour)
        {
            var pixel = PixelAt(x, y);
            *pixel = colour;
        }


        public void UnlockBitmap()
        {
            Bitmap.UnlockBits(bitmapData);
            bitmapData = null;
            pBase = null;
        }


        private PixelData* PixelAt(int x, int y)
        {
            return (PixelData*) (pBase + y*width + x*sizeof(PixelData));
        }


        public struct PixelData
        {
            public byte blue;


            public byte green;


            public byte red;
        }
    }
}