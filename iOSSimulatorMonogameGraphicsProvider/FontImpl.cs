using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace iOSSimulatorMonogameGraphicsProvider
{
	public class FontImpl:AnyGUI.Font
	{
		SpriteFont xnaFont;
		public FontImpl (SpriteFont font)
		{
			xnaFont = font;
		}

		#region Font implementation

		public WWUtils.Math.Vector2<float> MeasureText (string text)
		{
			Vector2 sz = xnaFont.MeasureString (text);
			return new WWUtils.Math.Vector2<float> (
				sz.X, sz.Y);
		}


		public bool Loaded {
			get {
				return true; // synchronous load
			}
		}
		#endregion
		//internal functions
		internal void DrawText (SpriteBatch spriteBatch, string text, WWUtils.Math.Vector2<float> position, 
			WWUtils.Math.Vector2<float> handle, float rotation, WWUtils.Math.Vector2<float> scale, float depth, 
			WWUtils.Math.Vector3<byte> color)
		{
			Color xnaColor = new Color (color.X, color.Y, color.Z);
			Vector2 pos = new Vector2 (position.X, position.Y);
			spriteBatch.DrawString (xnaFont, text, pos, xnaColor, rotation,
				new Vector2 (handle.X, handle.Y), new Vector2 (scale.X, scale.Y), SpriteEffects.None, depth);
		}
	}
}

