package 
{
	import flash.display.MovieClip;
	import flash.display.Sprite;
	import flash.events.MouseEvent;
	import flash.events.KeyboardEvent;
	import flash.ui.Keyboard;
	import flash.geom.Point;
	
	/**
	* ...
	* @author Anthony Massingham
	*/
	public class APoint extends MovieClip
	{
		
		public const GRAVITY:Number = 9;
		public const DRAG:Number = 0.95;
		
		public var position:Point;
		public var sprite:Sprite;
		
		public var linkDistance:Point;
		public var linkTarget:Point;
		
		public var targetPoint:Point
		
		public var velocity:Point;
		
		public var distanceLimit:Number = 15;
		public var thresholdLimit:Number = 15;
		
		private var defaultWeight:Number;
		public var weight:Number;
		
		public var prevPart:APoint;
		public var nextPart:APoint;
		
		public var detailsText:String = "";
		
		public var mouseHeld:Boolean = false;
		
		private var leftMovement:Boolean = false;
		private var rightMovement:Boolean = false;
		
		public function APoint(_weight:Number):void
		{
			sprite = new Sprite;
			addChild(sprite);
			
			weight = _weight;
			defaultWeight = weight;
			
			velocity = new Point(0, 0);
			
			sprite.graphics.lineStyle(0, 0x23301F);
			sprite.graphics.beginFill(0x4B6743,0.5);
			sprite.graphics.drawCircle(0, 0, 10*_weight+1);
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
		
		public function init():void
		{
			targetPoint = new Point(this.x, this.y);
			
			if (this.name == "HEAD")
			{
				this.stage.addEventListener(KeyboardEvent.KEY_UP, keyReleased);
				this.stage.addEventListener(KeyboardEvent.KEY_DOWN, keyPressed);
			}
		}
		
		private function keyPressed(event:KeyboardEvent):void
		{
			if (event.keyCode == Keyboard.LEFT)
			{
				leftMovement = true;
				this.rotation = 180;
			} else if (event.keyCode == Keyboard.RIGHT)
			{
				rightMovement = true;
				this.rotation = 0;
			}
		}
		
		private function keyReleased(event:KeyboardEvent):void
		{
			leftMovement = false;
			rightMovement = false;
		}
		
		public function holdMouse(event:MouseEvent):void
		{
			if(this.name == "TAIL"){
				mouseHeld = true;
				this.startDrag();
			}
		}
		
		public function releaseMouse(event:MouseEvent):void
		{
			if(this.name == "TAIL"){
				mouseHeld = false;
				this.stopDrag();
				
				targetPoint.x = this.x;
				targetPoint.y = this.y;
			}
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
			if (leftMovement)
			{
				this.velocity.x -= 5;
				trace("left");
			} else if (rightMovement)
			{
				this.velocity.x += 5;
			}			
			
			detailsText = "";
			this.graphics.clear();
			// Update Method
			if (nextPart != null) {
				var nextPartPoint:Point = globalToLocal(new Point(nextPart.x, nextPart.y));
				
				var nextDistance:Number = calculateDistance(0, 0, nextPartPoint.x, nextPartPoint.y);
				detailsText += "DISTANCE BETWEEN POINT " + this.name + " and " + nextPart.name + " : " + nextDistance.toFixed(2) +"\n";
				detailsText += this.name + " WEIGHT : " + this.weight+"\n";
				
				if (nextDistance > distanceLimit)
				{
					detailsText += this.name + " too far away!!\n";
					var theAngleStepOne:Number = nextPartPoint.y / nextPartPoint.x;
					var theAngle:Number = Math.atan(theAngleStepOne);
					
					if (nextPartPoint.x <= 0)
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
					
					//nextPart.targetPoint.x = newPoint.x;
					//nextPart.targetPoint.y = newPoint.y;
					
					nextPart.velocity.x += (newPoint.x - nextPart.x) * this.weight;
					nextPart.velocity.y += (newPoint.y - nextPart.y) * this.weight;
					
					nextPart.sprite.rotation = theAngle / (Math.PI/180);
				}
			}
			
			if (prevPart != null) {
				var prevPartPoint:Point = globalToLocal(new Point(prevPart.x, prevPart.y));
				
				var previousDistance:Number = calculateDistance(0, 0, prevPartPoint.x, prevPartPoint.y);
				detailsText += "DISTANCE BETWEEN POINT " + this.name + " and " + prevPart.name + " : " + previousDistance.toFixed(2) + "\n";
				detailsText += this.name + " WEIGHT : " + this.weight + "\n";
				
				if (previousDistance > distanceLimit)
				{
					detailsText += this.name + " too far away!!\n";
					var theAngleStepOne:Number = prevPartPoint.y / prevPartPoint.x;
					var theAngle:Number = Math.atan(theAngleStepOne);
					
					if (prevPartPoint.x <= 0)
					{
						theAngleStepOne = prevPartPoint.y / (prevPartPoint.x * -1);
						theAngle = (180 * (Math.PI / 180)) - Math.atan(theAngleStepOne);
					}
					
					var xPos:Number = Math.cos(theAngle) * distanceLimit;
					var yPos:Number = Math.sin(theAngle) * distanceLimit;
					this.graphics.clear();
					this.graphics.beginFill(0x364D98);
					this.graphics.drawCircle(xPos, yPos, 2);
					
					var newPoint:Point = localToGlobal(new Point(xPos, yPos));
					
					//nextPart.targetPoint.x = newPoint.x;
					//nextPart.targetPoint.y = newPoint.y;
					
					if(prevPart.weight<=this.weight){
						prevPart.velocity.x += (newPoint.x - prevPart.x) * this.weight;
						prevPart.velocity.y += (newPoint.y - prevPart.y) * this.weight;
					}
					
					prevPart.sprite.rotation = theAngle / (Math.PI/180);
				} else {
				}
			}
			
			if (mouseHeld)
			{
				targetPoint.x = this.x;
				targetPoint.y = this.y;
			} else {
				if (this.y < 350)
				{
					this.velocity.y += GRAVITY*DRAG;
				}
			}
			
			// Move X
			//var ax:Number = (targetPoint.x - this.x) * 0.6;
			//this.x += ax;
			
			this.x += this.velocity.x;
			
			// Move Y
			//var ay:Number = (targetPoint.y - this.y) * 0.6;
			//this.y += ay;
			
			this.y += this.velocity.y;
			
			this.velocity.x *= 0.3;
			this.velocity.y *= 0.3;			
		}
		
		private function calculateDistance(x1:Number, y1:Number, x2:Number, y2:Number):Number
		{
			return(Math.sqrt(Math.pow((x2 - x1),2) + Math.pow((y2 - y1),2)));
		}
	}
	
}