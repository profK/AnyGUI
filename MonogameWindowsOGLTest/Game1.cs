#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using AnyGUI;
using WWUtils;
using WWUtils.Math;
using WindowsMonogameOGLGraphicsProvider;
using System.Text.RegularExpressions;
#endregion
namespace MonogameWindowsOGLTest
{
	/// Default Project Template
	/// </summary>
	public class Game1 : Game
	{
		#region Fields

		GraphicsDeviceManager graphics;


		#endregion

		#region Initialization

		public Game1 ()
		{

			graphics = new GraphicsDeviceManager (this);

			Content.RootDirectory = "Content";

			graphics.IsFullScreen = false;
			Registry.Register(new GraphicsProviderImpl(this,graphics.GraphicsDevice));
			//quick regexp test


		}

		/// <summary>
		/// Overridden from the base Game.Initialize. Once the GraphicsDevice is setup,
		/// we'll use the viewport to initialize some values.
		/// </summary>
		protected override void Initialize ()
		{
			try {
				base.Initialize ();
			} catch (Exception e){
				Console.WriteLine (e);
			}
		}

		/// <summary>
		/// Load your graphics content.
		/// </summary>
		/// 
		private Window w=null,w2=null; 
		private RootPane popup = null;
		private DecoratedWindow scrollWindow=null;
		protected override void LoadContent ()
		{


			// TODO: use this.Content to load your game content here eg.

			w = new DecoratedWindow ();
			//w.Size=new Vector2<float>(600,200);
			w.LocalPosition = new Vector2<float> (50, 50);
			//w.Insets = defaultWindowState.Insets;
			Label l = new Label ("BigFont","Drag Me", new Vector3<byte>(255,0,255));
			l.LocalPosition= new Vector2<float> (0,0);
			w.AddChild (l);
			l.MouseDragListeners += delegate(GuiComponent c, Vector2<float> mouseDelta) {
				Console.WriteLine("Mouse drag: "+mouseDelta);
				c.LocalPosition += mouseDelta;
			};
			w2 = new Window();
			w2.Size = new Vector2<float>(200, 200);
			w2.LocalPosition = new Vector2<float>(200, 200);
			ButtonBar buttonBar = new ButtonBar(new string[]{"Show Window","PopUp"},
				new Action<string>[]{DoShow,ShowPopUp});
			w2.AddChild(buttonBar);
			w2.AllLoaded += delegate(GuiComponent obj) {
				w2.Pack();
			};
			popup = new Window();
			ButtonBar popupButtons = new ButtonBar(new String[]{"A button", "Another button", "Close"}
				,new Action<string>[]{DoPop,DoPop,DoPop});
			popupButtons.Vertical = true;
			popup.AddChild(popupButtons);
			popup.AllLoaded += delegate(GuiComponent obj) {
				popup.Pack();
				popup.Enabled = false; // hide
			};
			scrollWindow = new DecoratedWindow();
			scrollWindow.Layout = new GridLayoutManager(1, 1);
			ScrollPanel scrollpanel = new ScrollPanel();
			scrollWindow.AddChild(scrollpanel);
			scrollpanel.Layout = new HorizontalLayout();
			ImagePanel imgPanel = new ImagePanel("Thumbnails", 0, 7);
			scrollpanel.AddChild(imgPanel);
			scrollWindow.AllLoaded+= delegate(GuiComponent obj) {
				scrollWindow.Pack();
				scrollWindow.Size = new Vector2<float>(250,scrollWindow.Size.Y);
			};
			scrollpanel.ContentContainerDrag= new Vector3<float>(1.0f,float.MaxValue,float.MaxValue);
		}

		private void DoPop(string button){
			if (button == "Close") {
				popup.Enabled = false;
			}
		}

		private void DoShow(string buttonText){
			w.Enabled = true;
		}

		private void ShowPopUp(string buttonText){
			popup.Enabled = true;
		}

		#endregion

		#region Update and Draw

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update (GameTime gameTime)
		{
			// TODO: Add your update logic here			

			base.Update (gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself. 
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw (GameTime gameTime)
		{
			// Clear the backbuffer
			GraphicsDevice.Clear (Color.SkyBlue);
			//TODO: Add your drawing code here
			base.Draw (gameTime);
		}

		#endregion
	}
}

