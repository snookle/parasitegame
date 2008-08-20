package 
{
	import flash.display.MovieClip;
	import flash.display.Sprite;
	import flash.geom.Point;
	
	import flash.events.Event
	/**
	* ...
	* @author Anthony Massingham for BlueDog Training
	*/
	public class Particle extends MovieClip 
	{
		
		public var position:Point = new Point();
		public var neighbors:Array = new Array();
		public var velocity:Vector2D = new Vector2D(0, 0);
		
		private var highlighted:Boolean = false;
		
		public var theSprite:Sprite = new Sprite();	
		
		public function Particle(x,y):void
		{		
			position.x = x;
			position.y = y;
			
			velocity.x = Math.random() * 5 - 2; 
			velocity.y = Math.random() * 5 - 2;
			
			addChild(theSprite);
			
			theSprite.graphics.beginFill(0x7B2D2D, 1);
			theSprite.graphics.drawCircle(this.x, this.y, 2);
			
			theSprite.alpha = 0.25;
			
			//addEventListener(Event.ENTER_FRAME, drawLinks);
		}
		
		public function highlight(boolean:Boolean):void
		{
			if (boolean)
			{
				if(!highlighted){
					theSprite.alpha = 1;
					highlighted = true;
				}
			} else {
				if (highlighted)
				{
					theSprite.alpha = 0.25;
					highlighted = false;
				}
			}			
		}
		
		public function drawLinks():void
		{
			this.graphics.clear();
			for (var i = 0; i < this.neighbors.length; i++)
			{
				var localPoint:Point = this.globalToLocal(new Point(neighbors[i].x, neighbors[i].y));
				var thisPoint:Point = this.globalToLocal(new Point(this.x, this.y));
				
				this.graphics.lineStyle(0);
				this.graphics.moveTo(thisPoint.x, thisPoint.y);
				this.graphics.lineTo(localPoint.x,localPoint.y);
			}
		}
		
		public function applyForce(forceVector:Vector2D):void
		{
			this.velocity.addTo(forceVector);
		}
		
		public function clearLinks():void
		{
			this.graphics.clear();
		}
		
		public function addNeighbour(theNeighbour:Particle):void
		{
			neighbors.push(theNeighbour);
		}
		
		public function removeNeighbour(theNeighbour:Particle):void
		{
			for (var i = 0; i < neighbors.length; i++)
			{
				if (neighbors[i] == theNeighbour)
				{
					neighbors.splice(i, 1);
					i = neighbors.length + 1;
				}
			}
		}
		
	}
	
}