using System;
using JavascriptGraphicsProvider;
using JSIL;
using WWUtils.Math;

namespace JavascriptGraphicsProviderTest
{
	class MainClass
	{
		public static void Main (string[] args)
		{
		}

		public void Run(){
			CanvasManager cm = new CanvasManager ();
			cm.SetDiv ("AnyGUIDiv");
			cm.PushState ();
			cm.FillText ("Some Text", 50, 50);
			Vector2<int> sz = new Vector2<int> (cm.MeasureTextWidth ("Some Text"),
				cm.GetFontHeight ("12px Arial"));
			int textWidth = sz.X;
			int textHeight = sz.Y;
			cm.FillText ("More Text", 50+textWidth, 50+textHeight);
			cm.SetFont ("24px Arial");
			cm.Rotate ((float)(20*Math.PI / 180));
			cm.FillText ("Still More Text", 50, 50+(textHeight*2));
			cm.PopState ();
			cm.LoadTextFile ("default.cat", delegate(string txt) {

				cm.FillText(txt,50,100);
			});
			cm.LoadImage("Content/Window_Frame_Template.png", delegate(object img) {
				cm.DEBUG("loaded="+img);
				Vector2<float> sx = cm.GetImageSize(img);
				cm.DrawImage(img,0,0,(int)sx.X,(int)sx.Y,200,200,(int)sx.X,(int)sx.Y);
			});

		
			cm.MouseMoveListeners += delegate(object ctxt, Vector2<float> pos) {
				cm.FillText(pos.X+","+pos.Y,50,200);
			};
		}
	}
}
