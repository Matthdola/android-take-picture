using System;
using System.IO;

using Android.Graphics;

namespace take_picture
{
	public static class BitmapHelpers
	{
		public static Bitmap LoadAnResizeBitmap (this string fileName, int width, int height)
		{
			//First we get the dimension of the file o the disk
			BitmapFactory.Options options = new BitmapFactory.Options { InJustDecodeBounds = true};
			BitmapFactory.DecodeFile (fileName, options);

			//Next we calculate the ration that we need to resize the image by
			//in order to fit the requested dimensions
			int outHeight = options.OutHeight;
			int outWidth = options.OutWidth;
			int inSampleSize = 1;

			if (outHeight > height || outWidth > width) {
				inSampleSize = outWidth > outHeight ? outHeight / height : outWidth / width;
			}


			//Now we will load the image and have BitmapFactory resize for us
			options.InSampleSize = inSampleSize;
			options.InJustDecodeBounds = false;
			Bitmap resizedBitmap = BitmapFactory.DecodeFile(fileName, options);

			return resizedBitmap;
		}

	}
}

