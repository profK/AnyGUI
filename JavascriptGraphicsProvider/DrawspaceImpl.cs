using System;
using AnyGUI;
using WWUtils.Math;

namespace JavascriptGraphicsProvider
{
	public class DrawspaceImpl:Drawspace
	{
		CanvasManager mgr;
		Vector3<float> pos;
		Vector2<float> sz;
		Rect<float> _clip;
		public DrawspaceImpl (CanvasManager cmgr, Vector3<float> position, Vector2<float> size)
		{
			mgr = cmgr;
			pos = position;
			sz = size;

			mgr.TickListeners += delegate(long deltaMS) {
				Update(deltaMS);
				Draw();
			};
		}

		#region Drawspace implementation

		public event Action<long> UpdateListeners;

		public event Action<Drawspace> DrawListeners;

		public void SaveGraphicsState()
		{
			mgr.SaveState();
		}

		public void RestoreGraphicsState()
		{
			mgr.RestoreState();
		}

		public void DrawImage (Image image, WWUtils.Math.Vector2<float> position, WWUtils.Math.Vector2<float> Handle, float rotation = 0f)
		{
			DrawImage (image, position, Handle, rotation, new Vector2<float> (1, 1));
		}

		public void DrawImage (Image image, WWUtils.Math.Vector2<float> position, WWUtils.Math.Vector2<float> Handle, float rotation, WWUtils.Math.Vector2<float> scale)
		{
			ImageImpl img = image as ImageImpl;
			DrawImage (image, new Rect<float> (position.X, position.Y, img.subimage.size.X * scale.X, 
				img.subimage.size.Y * scale.Y),
				Handle, rotation);
		}

		public void DrawImage (Image image, WWUtils.Math.Rect<float> destinationRect)
		{
			DrawImage (image, destinationRect, new Vector2<float> (0, 0), 0f);
		}

		public void DrawImage (Image image, WWUtils.Math.Rect<float> destinationRect, WWUtils.Math.Vector2<float> Handle, float rotation)
		{
			mgr.PushState ();
			mgr.Translate ((int)-Handle.X, (int)-Handle.Y);
			mgr.Rotate (rotation);
			ImageImpl img = image as ImageImpl;
			mgr.DrawImage (img.JSImage, (int)img.subimage.position.X, (int)img.subimage.position.Y,
				(int)Math.Round(img.subimage.size.X), (int)Math.Round(img.subimage.size.Y), 
				(int)destinationRect.position.X, (int)destinationRect.position.Y,
				(int)Math.Round(destinationRect.size.X), (int)Math.Round(destinationRect.size.Y));
			mgr.PopState ();
		}

		public void DrawText (Font font, string text, WWUtils.Math.Vector2<float> position, WWUtils.Math.Vector2<float> Handle, float rotation)
		{
			DrawText (font, text, position, Handle, rotation, new Vector2<float> (1, 1), 
				new Vector3<byte> (255, 255, 255));
		}

		public void DrawText (Font font, string text, WWUtils.Math.Vector2<float> position, WWUtils.Math.Vector2<float> Handle, float rotation, WWUtils.Math.Vector2<float> scale, WWUtils.Math.Vector3<byte> color)
		{
			FontImpl fnt = font as FontImpl;
			mgr.PushState ();
			mgr.SetFont(fnt.Name);
			mgr.SetFillColor (color.X, color.Y, color.Z);
			mgr.Translate ((int)-Handle.X, (int)-Handle.Y);
			mgr.Rotate (rotation);
			mgr.Scale (scale.X, scale.Y);
			// need to add text height because CanvasManager palces baseline at origin
			mgr.FillText (text, (int)position.X, (int)(position.Y+mgr.GetFontHeight(fnt.Name)));
			mgr.PopState ();
		}

		public void Draw ()
		{
			mgr.PushState ();
			//Clip = new Rect<int> (pos.X, pos.Y, sz.X, sz.Y);
			DrawListeners (this);
			mgr.PopState ();
		}

		public void Update (long deltaSec)
		{
			UpdateListeners (deltaSec);
		}

		public WWUtils.Math.Vector2<float> Position {
			get {
				return pos;
			}
			set {
				pos = value;
			}
		}

		public WWUtils.Math.Vector2<float> Size {
			get {
				return sz;
			}
			set {
				sz = value;
			}
		}

		public WWUtils.Math.Rect<float> Clip {
			get{
				return _clip;
			}
			set {
				mgr.SetClip ((int)value.position.X, (int)value.position.Y, 
					(int)Math.Ceiling(value.size.X), (int)Math.Ceiling(value.size.Y));
				_clip = value;
			}
		}

		public bool Loaded {
			get { return true; } // not async
		}

		#endregion


	}
}

