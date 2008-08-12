package  
{
	import flash.display.MovieClip;
	import flash.display.Sprite;
	import flash.text.TextField;
	import flash.events.Event;
	/**
	* Document Class for Simple Parasite Tests
	* @author Anthony Massingham
	*/
	public class ParasiteTestClass extends MovieClip
	{
		private const GRAVITY:Number = 9;
		private const DRAG:Number = 0.98;
		private const GROUNDLEVEL:Number = 500;
		
		public function ParasiteTestClass() 
		{
			var theParasite:Parasite = new Parasite();
			addChild(theParasite);
			
			theParasite.x = width / 2;
			theParasite.y = height / 2;
			
			theParasite.initParasite();
			
			// Draw Ground Level
			var theGround:Sprite = new Sprite();
			theGround.graphics.beginFill(0x9B4242, 0.5);
			theGround.graphics.drawRect(0, 0, width, 100);
			theGround.graphics.endFill();
			addChild(theGround);
			
			theGround.x = 0;
			theGround.y = GROUNDLEVEL;
			
			addEventListener(Event.ENTER_FRAME, update);
			
			this.setChildIndex(status, this.numChildren-1);
			status.detailstext.htmlText = "Parasite Test Class : v1 [ Compiled 9/08/2008 11:06 AM ]"
		}
		
		public function update(event:Event):void
		{
			// Apply gravity globally ?
		}
		
	}
	
}