using System;
using AnyGUI;
using WWUtils.Math;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace JavascriptGraphicsProvider{

	public class GraphicsProviderImpl: GraphicsProvider
	{
		CanvasManager mgr;

		public GuiComponent CurrentlyHeldObject { get; set;}
		private Vector2<float> lastMousePos;

		Dictionary<string,CatalogImpl> catalogCache = new Dictionary<string, CatalogImpl>();

		public GraphicsProviderImpl (int updateTickMS = 100, string canvasDivName="AnyGUIDiv")
		{
			mgr = new CanvasManager ();
			mgr.SetDiv (canvasDivName);
			mgr.MouseDownListeners += delegate(object ctxt, int buttonNum) {
				if (_mouseDownListeners!=null){
					_mouseDownListeners(buttonNum);
				}
			};
			mgr.MouseUpListeners += delegate(object ctxt, int buttonNum) {
				if (buttonNum==0){
					CurrentlyHeldObject = null;
				}
				if (_mouseUpListeners!=null){
					_mouseUpListeners(buttonNum);
				}
			};
			mgr.MouseMoveListeners += delegate(object ctxt, Vector2<float> pos) {
				if (CurrentlyHeldObject!=null) {
					CurrentlyHeldObject.DoDrag(new Vector2<float>(pos.X,pos.Y) - lastMousePos);
				}
				if (_mouseMoveListeners!=null){
					_mouseMoveListeners(pos);
				}
				lastMousePos = pos;
			};
			mgr.MouseScrollListeners += delegate(object ctxt, int wheeldelta) {
				if (_mouseScrollListeners!=null){
					_mouseScrollListeners(wheeldelta);
				}
			};
			/*mgr.TickListeners += delegate(long deltaMS) {
				if (TickListeners!=null){
					TickListeners(deltaMS);
				}
			};*/
			mgr.SetAnimTick(updateTickMS);
		}

		#region GraphicsProvider implementation
		// events
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

		//public event Action<long> TickListeners;

		//methods

		public Image GetImage(string filename, Action<Image> loadedCB=null){

			ImageImpl img = new ImageImpl ();
			mgr.LoadImage ("Content/"+filename, delegate(object jsImage) {
				img.JSImage=jsImage;
				Vector2<float> sz = mgr.GetImageSize(jsImage);
				img.subimage = new Rect<float>(0,0,sz.X,sz.Y);
				img.Loaded=true;
				if (loadedCB!=null){
					loadedCB(img);
				}
			});
			return img;
		}

		public Image GetImage (string filename,Rect<float> source, Action<Image> loadedCB=null)
		{
			ImageImpl img = new ImageImpl ();
			mgr.LoadImage ("Content/"+filename, delegate(object jsImage) {
				img.JSImage=jsImage;
				img.subimage = (Rect<float>)source;
				img.Loaded=true;
				if (loadedCB!=null){
					loadedCB(img);
				}
			});
			return img;
		}

		public Drawspace GetDrawspace (WWUtils.Math.Vector3<float> position, WWUtils.Math.Vector2<float> size, 
			Action<Drawspace> loadedCB=null)
		{
			Drawspace ds = new DrawspaceImpl (mgr,position, size);

			if (loadedCB != null) {
				loadedCB (ds);
			}
			return ds;
		}

		public Font GetFont (string fontname,Action<Font> loadedCB=null)
		{
			FontImpl font= new FontImpl (mgr,fontname);

			if (loadedCB != null) {
				loadedCB (font);
			}
			return font;

		}

		public Catalog GetDefaultCatalog (Action<Catalog> loadedCB = null){
			return GetCatalog ("default", loadedCB);
		}

		public Catalog GetCatalog (string name , Action<Catalog> loadedCB=null)
		{
			CatalogImpl catalog;
			if (catalogCache.ContainsKey(name)) {
				catalog = catalogCache[name];
				loadedCB(catalog);
			} else {
				catalog = new CatalogImpl();
				mgr.LoadTextFile("Content/" + name + ".cat", delegate(string txt) {
					catalog.SetXML(new MemoryStream(Encoding.UTF8.GetBytes(txt ?? "")));
					catalogCache[name] = catalog;
					if (loadedCB != null) {
						loadedCB(catalog);
					}
				});
			}
			return catalog;
		}

		public void DEBUG (string msg)
		{
			mgr.DEBUG (msg);
		}

		public WWUtils.Math.Vector2<float> MousePosition {
			get {
				return mgr.GetMousePosition ();
			}
		}
		#endregion
	}
}

