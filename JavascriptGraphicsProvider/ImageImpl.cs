using System;
using AnyGUI;
using WWUtils.Math;

namespace JavascriptGraphicsProvider
{
	public class ImageImpl:Image
	{


		public ImageImpl ()
		{
			Loaded = false;
			JSImage = null;
		}
			

		public object JSImage {
			get;
			internal set;
		}

		public Rect<float> subimage {
			get;
			internal set;
		}

		public Rect<float> Source {
			get { return subimage; }
		}



		#region Image implementation

		public Image GetSubImage (WWUtils.Math.Rect<float> subImageSpace)
		{
			ImageImpl sub = new ImageImpl ();
			sub.JSImage = JSImage;
			sub.subimage = new Rect<float> (subimage.position.X + subImageSpace.position.X,
				subimage.position.Y + subImageSpace.position.Y,
				subImageSpace.size.X, subImageSpace.size.Y);
			sub.Loaded = true;
			return sub;
		}

		public WWUtils.Math.Vector2<float> Size {
			get {
				return subimage.size;
			}
		}

		public bool Loaded {
			get;
			internal set;
		}

		public event Action<Image> LoadedCB;
		#endregion
	}
}

