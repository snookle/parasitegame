package  
{
	
	import flash.display.MovieClip
	
	import flash.events.Event;
	
	/**
	* ...
	* @author Anthony Massingham
	*/
	public class ParasiteElement extends MovieClip
	{
		
		protected var _theWeight:Number;
		
		protected var _xVel:Number;
		protected var _xPos:Number;
		protected var _targetX:Number = 0;
		
		protected var theColour:uint;
		
		public function ParasiteElement(theWeight:Number = 10) 
		{
			_theWeight = theWeight;
			/*this.graphics.beginFill(theColour);
			this.graphics.drawCircle(0, 0, _theWeight);
			this.graphics.endFill();
			*/
			
			this.addEventListener(Event.ENTER_FRAME, update);
		}
		
		private function update(event:Event):void
		{
			// Update X value based on xPosition
			var ax:Number = (_targetX - this.x) * 0.7;
			
			this.x += ax;
		}
		
		public function get theWeight():Number
		{
			return _theWeight;
		}
		
		public function flip():void
		{
			this.scaleX *= -1;
			this.x += this.width;
		}
		
		public function set xVel(xValue:Number):void
		{
			_xVel = xValue;
			_targetX += _xVel;
		}
		
		public function set targetX(xValue:Number):void
		{
			_targetX = xValue;
		}
		
	}
	
}