using System;
using AnyGUI;
using WWUtils.Math;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace MacMonogameGraphicsProvider
{
	public class DrawspaceImpl:AbstractDrawspace
	{
		SpriteBatch spriteBatch;
		private Stack<Rectangle> clipStack = new Stack<Rectangle>();

		#region implemented abstract members of AbstractDrawSpace

		public override void SaveGraphicsState()
		{
			clipStack.Push(spriteBatch.GraphicsDevice.ScissorRectangle);
		}

		public override void RestoreGraphicsState()
		{
			spriteBatch.GraphicsDevice.ScissorRectangle = clipStack.Pop();
		}
			

		public override void DrawImage (Image image,  Rect<float> destinationRect, Vector2<float> Handle, float rotation = 0f)
		{
			//eventually repalce 0 below with a panel depth
			((ImageImpl)image).Draw (spriteBatch, destinationRect, Handle, rotation, NormalizeDepth(0)); 
		}

		public override void DrawText (Font font, string text, Vector2<float> position, Vector2<float> handle, float rotation, 
			Vector2<float> scale,Vector3<byte> color )
		{
			//eventually repalce 0 below with a panel depth
			((FontImpl)font).DrawText (spriteBatch,text, position, handle, rotation, scale,NormalizeDepth(0),color);
		}

		public override Rect<float> Clip{
			get {
				Rectangle xnaClip = spriteBatch.GraphicsDevice.ScissorRectangle;
				return new Rect<float> (xnaClip.X, xnaClip.Y,
						xnaClip.Width, xnaClip.Height);
			}

			set {
				spriteBatch.GraphicsDevice.ScissorRectangle =
					new Rectangle ((int)value.position.X, (int)value.position.Y,
						(int)Math.Ceiling(value.size.X), (int)Math.Ceiling(value.size.Y));
			}
		}

		public override bool Loaded {
			get {
				return true; // syncronously loaded
			}
		}
		#endregion

		internal void Draw (SpriteBatch spriteBatch)
		{
			this.spriteBatch = spriteBatch; // save for draw calls
			spriteBatch.GraphicsDevice.RasterizerState.ScissorTestEnable = true;
			base.Draw ();
		}

		private float NormalizeDepth(int d){
			return (float)(1.0 - ((double)d / int.MaxValue));
		}

	}
}

