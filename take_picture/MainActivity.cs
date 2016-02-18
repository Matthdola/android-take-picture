using System;
using System.Collections.Generic;


using Android.App;
using Android.Widget;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Provider;
using Java.IO;
using Environment = Android.OS.Environment;
using Uri = Android.Net.Uri;

namespace take_picture
{
	[Activity (Label = "Picture", MainLauncher = true, Icon = "@mipmap/icon")]
	public class MainActivity : Activity
	{
		private ImageView _imageView;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			if (IsThereAnAppToTakePictures ()) {

				CreateDirectoryForPictures ();

				Button button = FindViewById<Button> (Resource.Id.myButton);
				_imageView = FindViewById<ImageView> (Resource.Id.image_view1);

				button.Click += TakePicture;
			}
		}

		protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult (requestCode, resultCode, data);

			//Make it available in the gallery

			var mediaScanIntent = new Intent (Intent.ActionMediaScannerScanFile);
			Uri contentUri = Uri.FromFile (App._file);
			mediaScanIntent.SetData (contentUri);
			SendBroadcast (mediaScanIntent);

			//Display in ImageView. We will resize the bitmap to fit the display
			//Loading the full sized image will consume to much memory
			//and cause the application to crash

			int height = Resources.DisplayMetrics.HeightPixels;
			int width = _imageView.Height;
			App.bitmap = App._file.Path.LoadAnResizeBitmap (width, height);
			if (App.bitmap != null) {
				_imageView.SetImageBitmap (App.bitmap);
				App.bitmap = null;
			}

			//Dispose of the Java side bitmap
			GC.Collect();
		}

		private void CreateDirectoryForPictures()
		{
			App._dir = new File (
				Environment.GetExternalStoragePublicDirectory (
					Environment.DirectoryPictures), "CameraAppDemo");
			if (!App._dir.Exists ()) {
				App._dir.Mkdirs ();
			}
		}

		private bool IsThereAnAppToTakePictures()
		{
			Intent intent = new Intent (MediaStore.ActionImageCapture);
			IList<ResolveInfo> availableActivities = 
				PackageManager.QueryIntentActivities (intent, PackageInfoFlags.MatchDefaultOnly);
			
			return availableActivities != null && availableActivities.Count >= 1;
		}

		private void TakePicture(object sender, EventArgs e)
		{
			var intent = new Intent (MediaStore.ActionImageCapture);
			App._file = new File(App._dir, String.Format("photo_{0}.jpg", Guid.NewGuid()));

			intent.PutExtra (MediaStore.ExtraOutput, Uri.FromFile (App._file));
			StartActivityForResult (intent, 0);
		}
	}

	public static class App {
		public static File _file;
		public static File _dir;
		public static Bitmap bitmap;
	}
}


