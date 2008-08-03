package 
{
	import flash.display.MovieClip;
	import flash.display.Sprite;
	import flash.events.MouseEvent;
	import flash.geom.Point;
	
	/**
	* ...
	* @author Anthony Massingham
	*/
	public class APoint extends MovieClip
	{
		public var position:Point;
		public var sprite:Sprite;
		
		public var linkDistance:Point;
		public var linkTarget:Point;
		
		public var distanceLimit:Number = 50;
		
		public var weight:Number;
		
		public var prevPart:APoint;
		public var nextPart:APoint;
		
		public var detailsText:String = "";
		
		public var mouseHeld:Boolean = false;
		
		public function APoint(_weight:Number):void
		{
			sprite = new Sprite;
			addChild(sprite);
			
			weight = _weight;
			
			sprite.graphics.lineStyle(0, 0x23301F);
			sprite.graphics.beginFill(0x4B6743,0.5);
			sprite.graphics.drawCircle(0, 0, 10);
			sprite.graphics.moveTo(0, 0);
			sprite.graphics.lineTo(10, 0);
			//sprite.x -= sprite.width / 2;
			//sprite.y -= sprite.height / 2;
			sprite.graphics.endFill();
			
			// Draw Limits
			sprite.graphics.lineStyle(0, 0x23301F, 0.2);
			sprite.graphics.drawCircle(0, 0, distanceLimit);
			
			this.addEventListener(MouseEvent.MOUSE_DOWN, holdMouse);
		}
		
		public function holdMouse(event:MouseEvent):void
		{
			mouseHeld = true;
			this.startDrag();
		}
		
		public function releaseMouse(event:MouseEvent):void
		{
			mouseHeld = false;
			this.stopDrag();
		}
		
		public function addNextPart(thePart:APoint):void
		{
			nextPart = thePart;
		}
		
		public function addPrevPart(thePart:APoint):void
		{
			prevPart = thePart;
		}
		
		public function updatePoint():void
		{
			detailsText = "";
			this.graphics.clear();
			// Update Method
			if (nextPart != null) {
				var nextPartPoint:Point = globalToLocal(new Point(nextPart.x, nextPart.y));
				
				this.graphics.lineStyle(0, 0x8E4040, 0.5);
				this.graphics.moveTo(0, 0);
				this.graphics.lineTo(nextPartPoint.x, nextPartPoint.y);
				
				//trace("DISTANCE BETWEEN POINT " + this.name + " and " + nextPart.name + " : " + calculateDistance(0, 0, nextPartPoint.x, nextPartPoint.y));
				var nextDistance:Number = calculateDistance(0, 0, nextPartPoint.x, nextPartPoint.y);
				detailsText += "DISTANCE BETWEEN POINT " + this.name + " and " + nextPart.name + " : " + nextDistance.toFixed(2) +"\n";
				
				if (nextDistance > distanceLimit)
				{
					detailsText += this.name + " too far away!!\n";
					
					var theAngleStepOne:Number = nextPartPoint.y / nextPartPoint.x;
					var theAngle:Number = Math.atan(theAngleStepOne);
					
					if (nextPartPoint.x < 0)
					{
						theAngleStepOne = nextPartPoint.y / (nextPartPoint.x * -1);
						theAngle = (180 * (Math.PI / 180)) - Math.atan(theAngleStepOne);
					}
					
					var xPos:Number = Math.cos(theAngle) * distanceLimit;
					var yPos:Number = Math.sin(theAngle) * distanceLimit;
					this.graphics.clear();
					this.graphics.beginFill(0x364D98);
					this.graphics.drawCircle(xPos, yPos, 2);
					
					var newPoint:Point = localToGlobal(new Point(xPos, yPos));
					
					nextPart.x = newPoint.x;
					nextPart.y = newPoint.y;
				}
			}
			
			if (prevPart != null) {
				var prevPartPoint:Point = globalToLocal(new Point(prevPart.x, prevPart.y));
				
				this.graphics.lineStyle(0, 0x8E4040, 0.5);
				this.graphics.moveTo(0, 0);
				this.graphics.lineTo(prevPartPoint.x, prevPartPoint.y);
				//trace("DISTANCE BETWEEN POINT " + this.name + " and " + prevPart.name + " : " + calculateDistance(0, 0, prevPartPoint.x, prevPartPoint.y));
				
				var previousDistance:Number = calculateDistance(0, 0, prevPartPoint.x, prevPartPoint.y);
				detailsText += "DISTANCE BETWEEN POINT " + this.name + " and " + prevPart.name + " : " + previousDistance.toFixed(2) + "\n";
			}

		}
		
		private function calculateDistance(x1:Number, y1:Number, x2:Number, y2:Number):Number
		{
			return(Math.sqrt(Math.pow((x2 - x1),2) + Math.pow((y2 - y1),2)));
		}
	}
	
}