using System;
using System.IO;
using AnyGUI;
using WWUtils.Math;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MacMonogameGraphicsProvider
{
	public class GraphicsProviderImpl:DrawableGameComponent,GraphicsProvider {
		List<DrawspaceImpl> drawspaces = new List<DrawspaceImpl>();
		SpriteBatch spriteBatch=null;
		Game game;
		RasterizerState _rasterizerState = new RasterizerState() { ScissorTestEnable = true };

		Dictionary<string,Catalog> catalogCache = new Dictionary<string, Catalog>();
		public GuiComponent CurrentlyHeldObject { get; set;}

		public GraphicsProviderImpl (Game game,GraphicsDevice graphics):base(game)
		{
			this.game = game;
			game.Components.Add (this);
		}

		#region GraphicsProvider implementation

		public Image GetImage(string filename, Action<Image> loadedCB=null){
			Texture2D t2d = game.Content.Load<Texture2D> (filename);
			ImageImpl img = new ImageImpl (t2d,new Rect<float>(0,0,t2d.Width,t2d.Height));
			if (loadedCB != null) {
				loadedCB (img);
			}
			return img;
		}

		public Image GetImage (string filename,Rect<float> source, Action<Image> loadedCB=null)
		{
			Texture2D t2d = game.Content.Load<Texture2D> (filename);
			ImageImpl img = new ImageImpl (t2d,source);
			if (loadedCB != null) {
				loadedCB (img);
			}
			return img;
		}

		public Drawspace GetDrawspace(Vector3<float> position, Vector2<float> size,Action<Drawspace> loadedCB=null){
			DrawspaceImpl ds = new DrawspaceImpl ();
			ds.Position = position;
			ds.Size = size;
			drawspaces.Add (ds);
			if (loadedCB != null) {
				loadedCB (ds);
			}
			return ds;
		}

		public Catalog GetDefaultCatalog(Action<Catalog> loadedCB=null){
			return GetCatalog("default",loadedCB);
		}

		public Catalog GetCatalog (string name,Action<Catalog> loadedCB=null)
		{
			Catalog cat;
			if (catalogCache.ContainsKey(name)) {
				cat = catalogCache[name];
			} else {
				Stream xmlStream = TitleContainer.OpenStream("Content/" + name + ".cat");
				cat = new CatalogImpl(xmlStream);
			}
			if (loadedCB != null) {
				loadedCB (cat);
			}
			return cat;
		}


		public Font GetFont (string fontname,Action<Font> loadedCB=null)
		{
			SpriteFont sfnt = game.Content.Load<SpriteFont> (fontname);
			FontImpl font = new FontImpl (sfnt);
			if (loadedCB != null) {
				loadedCB (font);
			}
			return font;
		}

		public void DEBUG(string msg){
			Console.WriteLine (msg);
		}

		#endregion

		#region DrawableGameComponent overrides
		public override void Initialize ()
		{
			base.Initialize ();
			spriteBatch = new SpriteBatch (game.GraphicsDevice);
			MouseState mouseState = Mouse.GetState ();
			lastMousePos = new Vector2<float> (mouseState.X, mouseState.Y);
			lastWheelPos = mouseState.ScrollWheelValue;
			game.IsMouseVisible = true;
		}

		private ButtonState leftMouseDown= ButtonState.Released;
		private ButtonState rightMouseDown = ButtonState.Released;
		private ButtonState middleMouseDown = ButtonState.Released;
		private Vector2<float> lastMousePos;
		private int lastWheelPos;



		public Vector2<float> MousePosition { 
			get { return lastMousePos; }
		}

		public override void Update (GameTime gameTime)
		{
			MouseState mouseState = Mouse.GetState ();
			// check for button state changes
			if (leftMouseDown != mouseState.LeftButton) {
				leftMouseDown = mouseState.LeftButton;
				if (mouseState.LeftButton == ButtonState.Pressed) {
					if (_mouseDownListeners != null) {
						_mouseDownListeners (0);
					}
				} else {
					CurrentlyHeldObject = null;// button up release component
					if (_mouseUpListeners != null) {
						_mouseUpListeners (0);
					}
				}
			}
			if (rightMouseDown != mouseState.RightButton) {
				rightMouseDown = mouseState.RightButton;
				if (mouseState.RightButton== ButtonState.Pressed) {
					if (_mouseDownListeners != null) {
						_mouseDownListeners (1);
					}
				} else {
					if (_mouseUpListeners != null) {
						_mouseUpListeners (1);
					}
				}
			}
			if (middleMouseDown != mouseState.MiddleButton) {
				middleMouseDown = mouseState.MiddleButton;
				if (mouseState.MiddleButton== ButtonState.Pressed) {
					if (_mouseDownListeners != null) {
						_mouseDownListeners (2);
					}
				} else {
					if (_mouseUpListeners != null) {
						_mouseUpListeners (2);
					}
				}
			}
			// check for mouse move
			if ((lastMousePos.X!=mouseState.X)||(lastMousePos.Y!=mouseState.Y)){
				// drag held component
				if (CurrentlyHeldObject!=null) {
					CurrentlyHeldObject.DoDrag(new Vector2<float>(mouseState.X,mouseState.Y) - lastMousePos);
				}
				lastMousePos.X =mouseState.X;
				lastMousePos.Y =mouseState.Y;
				if (_mouseMoveListeners != null) {
					_mouseMoveListeners (lastMousePos);
				}

			}
			// check for scroll wheel
			if (lastWheelPos!=mouseState.ScrollWheelValue){
				lastWheelPos = mouseState.ScrollWheelValue;
				if (_mouseScrollListeners != null) {
					_mouseScrollListeners (lastWheelPos);
				}

			}

			// pass update to drawspaces
			foreach (DrawspaceImpl ds in drawspaces) {
				ds.Update ((long)(gameTime.ElapsedGameTime.TotalMilliseconds));
			}

		}

		public override void Draw (GameTime gameTime)
		{
			spriteBatch.Begin (SpriteSortMode.Immediate, BlendState.AlphaBlend,
				null, null, _rasterizerState);
			base.Draw (gameTime);
			foreach (DrawspaceImpl ds in drawspaces) {
				ds.Draw (spriteBatch);
			}
			spriteBatch.End ();
		}
		#endregion


		private Action<int> _mouseDownListeners=null; 
		public event Action<int> MouseDownListeners{
			add{
				if (_mouseDownListeners==null){
					_mouseDownListeners = value;
				} else {
					_mouseDownListeners = (Action<int>)Action<int>.Combine(value,_mouseDownListeners);
				}
			}
			remove {
				_mouseDownListeners = (Action<int>)Action<int>.Remove(_mouseDownListeners, value);
			}
		}

		private Action<int> _mouseUpListeners=null;
		public event Action<int> MouseUpListeners{
			add{
				if (_mouseUpListeners==null){
					_mouseUpListeners = value;
				} else {
					_mouseUpListeners = (Action<int>)Action<int>.Combine(value,_mouseUpListeners);
				}
			}
			remove {
				_mouseUpListeners = (Action<int>)Action<int>.Remove(_mouseUpListeners, value);
			}
		}

		private Action<Vector2<float>> _mouseMoveListeners=null;
		public event Action<Vector2<float>> MouseMoveListeners{
			add{
				if (_mouseMoveListeners==null){
					_mouseMoveListeners = value;
				} else {
					_mouseMoveListeners = (Action<Vector2<float>>)Action<Vector2<float>>.Combine(value,_mouseMoveListeners);
				}
			}
			remove {
				_mouseMoveListeners = (Action<Vector2<float>>)Action<Vector2<float>>.Remove(_mouseMoveListeners, value);
			}
		}

		private Action<int> _mouseScrollListeners;
		public event Action<int> MouseScrollListeners{
			add{
				if (_mouseScrollListeners==null){
					_mouseScrollListeners = value;
				} else {
					_mouseScrollListeners = (Action<int>)Action<int>.Combine(value,_mouseScrollListeners);
				}
			}
			remove {
				_mouseScrollListeners = (Action<int>)Action<int>.Remove(_mouseScrollListeners, value);
			}
		}

	}
}

