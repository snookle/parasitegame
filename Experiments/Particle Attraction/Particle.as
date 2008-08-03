package {
	
	// Individual awesome particle
	
	import flash.display.MovieClip;
	import flash.display.Sprite;
	import flash.geom.Point;
	import flash.events.Event;
	
	public class Particle extends MovieClip{
		
		private var theParticle:Sprite = new Sprite();
		
		private var xPos:Number;
		private var yPos:Number;
		private var pointOrigin:Point;
		protected var targetPoint:Point = new Point();
		
		private var gravity:Number = 1.1;
		
		private var theRadius:Number = 10;
		
		public function Particle(_xPos,_yPos):void{
			xPos = _xPos;
			yPos = _yPos;

			drawParticle();
			//drawRadius();
			
			this.addEventListener(Event.ENTER_FRAME, update);
		}
		
		private function update(event:Event):void{
			//if (findDistance(xPos,yPos,targetPoint.x,targetPoint.y)<50){
				moveParticles();
				//drawParticle();
				//drawRadius();
			//}
		}
		
		public function moveTo(newX:Number,newY:Number):void{
		}
		
		private function drawParticle():void{
			this.graphics.clear();
			this.graphics.beginFill(0x000000,0.1);
			this.graphics.drawCircle(xPos,yPos,theRadius);
			this.graphics.endFill();
		}
		
		private function drawRadius():void{
			//this.graphics.beginFill(0x000000,0.2);
			//this.graphics.drawCircle(xPos,yPos,theRadius*2);
			//this.graphics.endFill();
			
			this.graphics.beginFill(0x000000,0.2);
			this.graphics.drawCircle(xPos,yPos,theRadius*3);
			this.graphics.endFill();
		}
		
		public function moveParticles():void{
			var ax:Number = (targetPoint.x - this.x) * 0.008;
			this.x += ax;
			//xPos -= xSpeed;
			var ay:Number = (targetPoint.y - this.y) * 0.008;
				
			this.y += ay;
			
			//this.y *= gravity;
			
			//if(this.y>200){
				//targetPoint.y *= -1;
				//this.y = 200;
			//} else if (this.y == 200){
				///trace("200");
			//}
			//yPos -= ySpeed;
		}
		public function modifyTarget(newX:Number,newY:Number):void{
			targetPoint.x += newX;
			targetPoint.y += newY;
		}
		
		public function setTarget(newX:Number,newY:Number):void{
			targetPoint.x = newX;
			targetPoint.y = newY;
		}
	}
	
}