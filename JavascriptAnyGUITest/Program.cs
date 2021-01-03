using System;
using WWUtils;
using WWUtils.Math;
using JavascriptGraphicsProvider;
using AnyGUI;


namespace JavascriptAnyGUITest
{
	class MainClass
	{
		public static void Main (string[] args)
		{

		}

		// staic constructor
		static MainClass(){
			Registry.Register (new GraphicsProviderImpl ());
		}

		Window w,w2,popup = null,scrollWindow;
		public void Run(){
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
			scrollpanel.ContentContainerDrag= new Vector3<float>(0.75f,float.MaxValue,float.MaxValue);

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
	}
}
