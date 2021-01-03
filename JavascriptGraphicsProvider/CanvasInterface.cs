using System;
using WWUtils.Math;

namespace JavascriptGraphicsProvider
{
	public interface CanvasInterface
	{
		void SaveState();
		void RestoreState();
		void SetDiv (string divName);
		void SetFont(string fontName);
		void SetClip (int x, int y, int width, int height);
		void Translate (int x, int y);
		void Rotate(float radians);
		void DrawImage(object image,int sx, int sy, int swidth, int sheight,
			int x, int y, int width, int height);
		void FillText (string txt, int x, int y);
		int MeasureTextWidth (string txt);
		int GetFontHeight(string font);
		// asset loading
		void LoadImage (string imageUrl, Action<object> loadedCB);
		void LoadTextFile (string fileUrl,Action<string> loadedCB);
		void PushState();
		void PopState();
		void SetBackgroundColor(byte r, byte g, byte b);
		void ClearScreen();
		void DEBUG(string s);
		event Action<object,Vector2<float>> MouseMoveListeners ;
	}
}

