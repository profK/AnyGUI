using System;
using JSIL;
using Microsoft.CSharp;
using WWUtils.Math;
using System.Collections.Generic;

namespace JavascriptGraphicsProvider
{
	public class CanvasManager:CanvasInterface
	{
		object ctxt; // holds the Canvas context
		string divName;
		string fontName="14px Arial";
		Dictionary<string,int> fontHeights = new Dictionary<string, int>();
		object tickHandle=null;
		long lastTick;
		Vector2<float> mousePos = new Vector2<float>();

		public event Action<object,int> MouseDownListeners;
		public event Action<object,int> MouseUpListeners;
		public event Action<object, int> MouseScrollListeners;
		public event Action<object,Vector2<float>> MouseMoveListeners;
		public event Action<long> TickListeners;

		public CanvasManager ()
		{

		}

		public void SetBackgroundColor(byte r, byte g, byte b){
			///TBA
		}

		public void ClearScreen(){
			Verbatim.Expression (@"
				$0.clearRect(0, 0, $0.canvas.width, $0.canvas.height);
			", ctxt);
		}

		#region CanvasInterface implementation

		public void SetDiv (string divName)
		{
			this.divName = divName;
			Verbatim.Expression(@"
				var c = document.getElementById($1);
				$0 = c.getContext(""2d"");
				// 
				// set up mouse listeners
				var _self = this;
				function MouseMotion(canvas, evt) {
        			var rect = c.getBoundingClientRect();
          			var x = evt.clientX - rect.left;
          			var y = evt.clientY - rect.top;
					CanvasManager_DoMouseMotion.call(_self,$0,x,y);
      			}

				function MouseDown(canvas, evt) {
        			var rect = c.getBoundingClientRect();
          			var x = evt.clientX - rect.left;
          			var y = evt.clientY - rect.top;
					CanvasManager_DoMouseDown.call(_self,$0,x,y,evt.button);
      			}

				function MouseUp(canvas, evt) {
        			var rect = c.getBoundingClientRect();
          			var x = evt.clientX - rect.left;
          			var y = evt.clientY - rect.top;
					CanvasManager_DoMouseUp.call(_self,$0,x,y,evt.button);
      			}

				function MouseWheel(canvas, evt) {
        			var rect = c.getBoundingClientRect();
          			var x = evt.clientX - rect.left;
          			var y = evt.clientY - rect.top;
					CanvasManager_DoMouseWheel.call(_self,$0,x,y,evt.wheelDelta);
      			}
      
      			c.addEventListener('mousemove', function(evt) {
        			MouseMotion($0, evt);
      			}, false);

				c.addEventListener('mousedown', function(evt) {
        			MouseDown($0, evt);
      			}, false);

				c.addEventListener('mouseup', function(evt) {
        			MouseUp($0, evt);
      			}, false);
				c.addEventListener('mousewheel', function(evt) {
    				MouseWheel($0,evt);
				});
				//just for firefox which doesnt do mousewheel event
				c.addEventListener('DOMMouseScroll', function(evt) {
    				MouseWheel($0,evt);
				});

			",ctxt,divName);
			SetFont (fontName);
		}

		private void DoMouseMotion(object ctxt,int x, int y){
			mousePos.X = x;
			mousePos.Y = y;
			if (MouseMoveListeners != null) {
				MouseMoveListeners (ctxt, new Vector2<float> (x, y));
			}
		}

		private void DoMouseDown(object ctxt,int x, int y,int buttonNum){
			mousePos.X = x;
			mousePos.Y = y;
			if (MouseDownListeners != null) {
				MouseDownListeners (ctxt, buttonNum);
			}
		}

		private void DoMouseUp(object ctxt,int x, int y,int buttonNum){
			mousePos.X = x;
			mousePos.Y = y;
			if (MouseUpListeners != null) {
				MouseUpListeners (ctxt, buttonNum);
			}
		}

		private void DoMouseWheel(object ctxt,int x, int y,int wheelDelta){
			mousePos.X = x;
			mousePos.Y = y;
			if (MouseScrollListeners != null) {
				MouseScrollListeners (ctxt, wheelDelta);
			}
		}

		private long CurrentTimeMS(){
				return Verbatim.Expression(@"Date.now()");
		}

		public void SetAnimTick(int tickMS){
			if (tickHandle != null) {
				Verbatim.Expression (@"clearInterval($0)", tickHandle);
				tickHandle = 0;
			}
			if (tickMS > 0) {
				Verbatim.Expression(@"
					var _self = this;
					function Tick(){
						CanvasManager_DoTick.call(_self);
					}
					$0 = setInterval(Tick,$1);
				",tickHandle,tickMS);
			}
			lastTick = CurrentTimeMS();
		}

		public void DoTick(){
			long newTime = CurrentTimeMS();
			double deltaMS = newTime-lastTick;
			ClearScreen();
			if (TickListeners != null) {
				TickListeners ((long)deltaMS);
			}
			lastTick = newTime;
		}

		public void SetFont (string font)
		{
			fontName = font;
			Verbatim.Expression(@"
				var ctxt = $0;
				ctxt.font = $1;
			", ctxt, fontName);

		}

		public void SaveState(){
			Verbatim.Expression(@"
				$0.save();
			",ctxt);
		}

		public void RestoreState(){
			Verbatim.Expression(@"
				$0.restore();
			",ctxt);
		}

		public void SetClip (int x, int y, int width, int height)
		{
			Verbatim.Expression(@"
				var ctxt = $0;
				var x= $1;
				var y= $2;
				var width=$3;
				var height=$4;
				ctxt.beginPath();
	      		ctxt.moveTo(x,y);
	      		ctxt.lineTo(x+width,y);
	      		ctxt.lineTo(x+width,y+height);
	      		ctxt.lineTo(x,y+height);
	      		ctxt.lineTo(x,y);
	      		ctxt.clip();
			",ctxt,x,y,width,height);
		}

		public void Translate (int x, int y)
		{
			Verbatim.Expression(@"
				var ctxt = $0;
				var x= $1;
				var y= $2;
				ctxt.translate(x,y);
			",ctxt,x,y);
		}

		public void Rotate (float radians)
		{
			Verbatim.Expression(@"
				var ctxt = $0;
				var radians= $1;
				ctxt.rotate(radians);
			",ctxt,radians);
		}

		public void Scale (float x, float y)
		{
			Verbatim.Expression(@"
				var ctxt = $0;
				ctxt.scale($1,$2);
			",ctxt,x,y);
		}

		public void DrawImage (object image, int sx, int sy, int swidth, int sheight, int x, int y, int width, int height)
		{
			Verbatim.Expression(@"
				var ctxt = $0;
				var image= $1;
				var sx = $2;
				var sy = $3;
				var swidth=$4;
				var sheight=$5;
				var x=$6;
				var y=$7;
				var width=$8
				var height=$9;
				//console.log(""Draw ""+image+"" ""+sx+"",""+sy+"",""+swidth+"",""+sheight+
				//			"" ""+x+"",""+y+"",""+width+"",""+height);
				ctxt.drawImage(image,sx,sy,swidth,sheight,x,y,width,height);
			",ctxt,image,sx,sy,swidth,sheight,x,y,width,height);
		}

		public void FillText (string txt, int x, int y)
		{
			Verbatim.Expression(@"
				var ctxt = $0;
				var txt= $1;
				var x=$2;
				var y=$3; 
				ctxt.fillText(txt,x,y);
			",ctxt,txt,x,y);
		}

		public int MeasureTextWidth (string txt)
		{
			return Verbatim.Expression(@"
				$0.measureText($1).width
			",ctxt,txt);
		}
		/// <summary>
		/// Measures the size of the text.
		/// code courtesy of http://stackoverflow.com/questions/1134586/how-can-you-find-the-height-of-text-on-an-html-canvas
		/// </summary>
		/// <returns>The text size.</returns>
		public int GetFontHeight (string fontName)
		{
			// store height
			if (!fontHeights.ContainsKey(fontName)) {
				string[] parsed = ParseFont(fontName);
				string fontStyle = "font-family: " + parsed[2] + "; font-size: " + parsed[0] + parsed[1] + ";";
				fontHeights[fontName] = MeasureFontHeight(fontStyle);
			}
			return fontHeights[fontName]; //dummy for the C# compiler
		}

		private int MeasureFontHeight(string font){
			Verbatim.Expression(@"
				var body = document.getElementsByTagName(""body"")[0];
  				var dummy = document.createElement(""div"");
  				var dummyText = document.createTextNode(""M"");
  				dummy.appendChild(dummyText);
  				dummy.setAttribute(""style"", $0);
  				body.appendChild(dummy);
  				var height = dummy.offsetHeight;
  				body.removeChild(dummy);
				return height;
			", font);
			return 0; // dummy to make compiler happy
		}

		Dictionary<string,object> imageCache = new Dictionary<string, object>();
		private void AddToImageCache(string name,object imageObj){
			imageCache[name] = imageObj;
		}

		public void LoadImage (string imageUrl,Action<object> loadedCB)
		{
			if (imageCache.ContainsKey(imageUrl)) {
				object cacheObj = imageCache[imageUrl];
				if (Verbatim.Expression(@"$0.Loaded==true", cacheObj)) {
					loadedCB(cacheObj);
				} else {
					Verbatim.Expression(@"
						$0.onloadCBList.push($1);
					", cacheObj, loadedCB);
				}
			} else {
				Verbatim.Expression(@"
					var imageObj = new Image();
					imageObj.Loaded = false;
					imageObj.onloadCBList = [$1];
	      			imageObj.onload= function(){
						imageObj.Loaded=true;
	      				imageObj.onloadCBList.forEach(
							function(entry){
								entry(imageObj);
							}
						);
	      			};
					CanvasManager_AddToImageCache.call(this,$1,imageObj);
					imageObj.src = $0;
				", imageUrl, loadedCB);
			}
		}


		public void LoadTextFile (string fileUrl,Action<string> loadedCB)
		{
			Verbatim.Expression(@"
				var oReq = new XMLHttpRequest();
				oReq.onload = function(){
					$1(oReq.responseText);
				}
				oReq.open(""get"", $0,true);
				oReq.send(null);
			",fileUrl,loadedCB);
		}

		public void PushState(){
			Verbatim.Expression (@"
				$0.save();
			", ctxt);
		}

		public void PopState(){
			Verbatim.Expression (@"
				$0.restore();
			", ctxt);
		}

		public void DEBUG(string s){
			Verbatim.Expression (@"
				console.log($0);
			", s);
		}

		public Vector2<float> GetMousePosition ()
		{
			return mousePos;
		}

		public Vector2<float> GetImageSize (object jsImage)
		{
			Vector2<float> size = new Vector2<float> ();
			Verbatim.Expression (@"
				$1.X = $0.naturalWidth;
				$1.Y = $0.naturalHeight;
			", jsImage,size);
			return size;
		}

		public void SetFillColor (byte r, byte g, byte b)
		{
			Verbatim.Expression (@"
				$0.fillStyle = ""rgb(""+$1+"",""+$2+"",""+$3+"")"";
			", ctxt,r,g,b);
		}

		private string[] ParseFont(string fnt){
			string[] results = new String[3];
			Verbatim.Expression(@"
				var matches = /([0-9]+)(pt|px)\s*([a-zA-Z]+)/g.exec($0);
				$1[0] = matches[1];
				$1[1] = matches[2];
				$1[2] = matches[3];
			", fnt,results);
			DEBUG("matches = " +results);
			return (results);
		}

			
		#endregion
	}
}

