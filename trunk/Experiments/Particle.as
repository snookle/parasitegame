package { 

	import flash.display.MovieClip;
	import flash.display.Shape;

	public class Particle extends MovieClip {
		
		protected var theSize:Number;
		protected var theColour:uint = 0xff5522;
		
		protected var xPos:Number = 0;
		protected var xVel:Number = 0;
		protected var yPos:Number = 0;
		protected var yVel:Number = 0;
		
		protected var gravity:Number = 0.5;
		protected var drag:Number = 0.98;
		
		public function Particle(_x,_y,_theSize:Number):void{
			trace("Particle Created");
			
			theSize = _theSize;
			xPos = _x;
			yPos = _y;
			
			drawParticle();
		}
		
		public function drawParticle():void{
			this.graphics.clear();
			this.graphics.beginFill(theColour);
			this.graphics.drawEllipse(xPos-(theSize/2),yPos-(theSize/2),theSize,theSize);
			this.graphics.endFill();
		}
		
		public function doMovement():void{			
			yVel=yVel*drag+gravity;
			
			yPos += yVel;
			xPos += xVel;			
					
			// Initial velocity changes at any given time
			//yVel = yVel=yVel*drag+gravity;
			//xVel = xVel;
			
			// New x,y positions after this iteration
			//yPos = this.y * (yVel);
			//xPos = this.x * (xVel);
		}
		
		public function changeDirection(xV:Number, yV:Number):void{
			xVel += xV;
			yVel += yV;
		}
		
		public function flipY(overlap:Number):void{
			yPos -= overlap/10;
			yVel *= -1;
		}
		public function flipX():void{
			xVel *= -1;
		}
		
	}
	
}