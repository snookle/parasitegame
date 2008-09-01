package 
{
	import flash.display.MovieClip;
	import flash.display.Sprite;
	import flash.geom.Point;
	
	import flash.events.Event
	/**
	* ...
	* @author Anthony Massingham
	*/
	public class Particle extends MovieClip 
	{
		
		public var position:Vector2D = new Vector2D(0,0);
		public var neighbors:Array = new Array();
		public var velocity:Vector2D = new Vector2D(0, 0);
		
		public var particlePoints:Array = new Array();
		public var prevPos:Vector2D = new Vector2D(0, 0);
		
		public var particleSize:Number = 2;
		
		private var highlighted:Boolean = false;
		
		public var theSprite:Sprite = new Sprite();
		public var thePoints:Sprite = new Sprite();
		
		public function Particle(x,y,partSize):void
		{			
			velocity.x = Math.random() * 5 - 2; 
			velocity.y = Math.random() * 5 - 2;
			
			particleSize = partSize;
			
			addChild(theSprite);
			addChild(thePoints);
			
			theSprite.graphics.beginFill(0x7B2D2D, 1);
			//theSprite.graphics.drawCircle(this.x, this.y, partSize);
			theSprite.graphics.drawRect(this.x-partSize/2, this.y-partSize/2, partSize,partSize);
			//addEventListener(Event.ENTER_FRAME, update);
			
			//createPoints();
			//var theBlob:MovieClip = new Blob_img();
			//this.addChild(theBlob);
		}
		
		public function createPoints():void
		{
			var thisPoint:Point = this.globalToLocal(new Point(this.x, this.y));
			// Left Point
			var leftPoint:Vector2D = new Vector2D(this.x - 10, this.y);
			particlePoints.push(leftPoint);
			
			// Top Point
			var topPoint:Vector2D = new Vector2D(this.x, this.y - 10);
			particlePoints.push(topPoint);
			
			// Right Point
			var rightPoint:Vector2D = new Vector2D(this.x + 10, this.y);
			particlePoints.push(rightPoint);
			
			// Bottom Point
			var bottomPoint:Vector2D = new Vector2D(this.x, this.y + 10);
			particlePoints.push(bottomPoint);
			
			// draw dots at each of hte points
			thePoints.graphics.beginFill(0xD751D7);
			thePoints.graphics.drawCircle(leftPoint.x, leftPoint.y, 1);
			thePoints.graphics.endFill();
			
			thePoints.graphics.beginFill(0xD751D7);
			thePoints.graphics.drawCircle(topPoint.x, topPoint.y, 1);
			thePoints.graphics.endFill();
			
			thePoints.graphics.beginFill(0xD751D7);
			thePoints.graphics.drawCircle(rightPoint.x, rightPoint.y, 1);
			thePoints.graphics.endFill();
			
			thePoints.graphics.beginFill(0xD751D7);
			thePoints.graphics.drawCircle(bottomPoint.x, bottomPoint.y, 1);
			thePoints.graphics.endFill();
		}
		
		public function highlight(theColour:uint):void
		{
			theSprite.graphics.clear();
			theSprite.graphics.beginFill(theColour);
			//theSprite.graphics.drawCircle(theSprite.x, theSprite.y, particleSize);	
			theSprite.graphics.drawRect(theSprite.x - (particleSize/2), theSprite.y-(particleSize/2), particleSize,particleSize);
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
		
		public function updatePosition(posit:Vector2D):void {
			this.x = posit.x;
			this.y = posit.y;
			
			position = posit;
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