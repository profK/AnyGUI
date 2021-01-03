using System;
using AnyGUI;
using WWUtils;
using WWUtils.Math;

namespace JavascriptGraphicsProvider
{
	public class FontImpl:Font
	{
		CanvasManager mgr;


		public FontImpl (CanvasManager cmgr,string fontname="24px Arial")
		{
			Name = fontname;
			mgr = cmgr;
		}

		public string Name{ get; internal set; }

		#region Font implementation
		public Vector2<float> MeasureText (string text)
		{
			mgr.PushState ();
			mgr.SetFont (Name);
			Vector2<float> sz = new Vector2<float> (mgr.MeasureTextWidth (text), mgr.GetFontHeight (Name));
			mgr.PopState ();
			return sz;
		}

		public bool Loaded {
			get { return true; } // not async in Canvas
		}
		#endregion
	}
}

