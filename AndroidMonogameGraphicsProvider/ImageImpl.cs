using System;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WWUtils.Math;
using AnyGUI;

namespace AndroidMonogameGraphicsProvider
{
	public class ImageImpl: Image
	{
		Texture2D masterImage;
		Rect<float> subImage;

		public ImageImpl (Texture2D img):this(img,new Rect<float> (0, 0, img.Width, img.Height)){}

		public ImageImpl (Texture2D img, Rect<float> subImg)
		{
			this.subImage = (Rect<float>)subImg;
			masterImage = img;

		}

		public Rect<float> Source {
			get { return subImage; }
		}


		#region Image implementation
		public Image GetSubImage (Rect<float> subImageSpace)
		{
			return new ImageImpl (masterImage, subImage.Clip (
				new Rect<float> (subImage.position.X + subImageSpace.position.X,
					subImage.position.Y + subImageSpace.position.Y,
					subImageSpace.size.X, subImageSpace.size.Y)
			));

		}
		public Vector2<float> Size {
			get {
				return new Vector2<float> (subImage.size.X, subImage.size.Y);
			}
		}

		public bool Loaded {
			get { return true; } // synchronously loaded
		}
		#endregion

		// internal methods
		public void Draw (SpriteBatch spriteBatch, Rect<float> destinationRect, Vector2<float> handle, float rotation, 
			float depth)
		{

			Rectangle src = new Rectangle((int)(subImage.position.X), 
				(int)(subImage.position.Y), 
				(int)Math.Round(subImage.size.X), (int)Math.Round(subImage.size.Y));
			Vector2<float> scale = 
				new Vector2<float>(destinationRect.size.X / subImage.size.X, 
					destinationRect.size.Y / subImage.size.Y);
			spriteBatch.Draw(masterImage, new Vector2(destinationRect.position.X+(handle.X*scale.X), 
				destinationRect.position.Y+(handle.Y*scale.Y)),
				src, Color.White, rotation, new Vector2(handle.X,handle.Y),
				new Vector2(scale.X,scale.Y),SpriteEffects.None, 0);
		}

	}
}

