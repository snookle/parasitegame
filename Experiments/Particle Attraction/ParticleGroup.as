package {
	
	import flash.display.MovieClip;
	
	import flash.events.Event;
	
	import flash.geom.Point;
	import flash.geom.Rectangle;
	
	import coreyoneil.collision.*;
	
	// ParticleGroups : Collection of Particles
	//  - Handles inter-particle relationships
	//  - Draws 'tension line
	
	import Particle;
	
	public class ParticleGroup extends MovieClip{
		
		private var theParticles:Array = new Array();
		
		private var collisionGroup:CollisionGroup = new CollisionGroup();
		
		private var particleRadius:Number = 10;
		
		private var centreGroup:Point = new Point(0,0);
		
		public function ParticleGroup():void{
			// To start with, each particle is it's own 'particle group'
			collisionGroup.returnAngle = true;
		}
		
		public function initGroup():void{
			addParticles(20);
			centreGroup.x = 0;
			centreGroup.y = 0;
			
			addEventListener(Event.ENTER_FRAME, checkSeparation);
		}
		
		private function drawCentre():void{
			this.graphics.beginFill(0x00ffaa,0.01);
			this.graphics.drawCircle(0,0,2);
			this.graphics.endFill();
		}
		
		private function checkSeparation(event:Event):void{
			this.graphics.clear();
			this.graphics.lineStyle(0,0x55FF22);
			var newRect:Rectangle = getBounds(this);
			this.graphics.drawRect(newRect.x,newRect.y,newRect.width,newRect.height);
			drawCentre();
			if(theParticles.length>1){
				var collisions:Array = collisionGroup.checkCollisions();
				
				for(var i=0;i<theParticles.length;i++){
					for(var n=0;n<theParticles.length;n++){
						// Draw Connecting Lines :
						this.graphics.lineStyle(0,0xFF4444);
						this.graphics.moveTo(theParticles[i].x,theParticles[i].y);
						this.graphics.lineTo(theParticles[n].x,theParticles[n].y);
					}
					
					var localPoint:Point = new Point(theParticles[0].x-theParticles[i].x,theParticles[0].y-theParticles[i].y);
					theParticles[i].setTarget(localPoint.x,localPoint.y);
				}
				
				
				for(i=0;i<collisions.length;i++){
					// object1 is the smallest
					// object2 is the larger
					// angle of the collision
					// overlap - number of pixels overlapping
					
					var inverseAngle = collisions[i].angle;
					var newY:Number = Math.sin(inverseAngle)*(particleRadius*10);
					var newX:Number = Math.cos(inverseAngle)*(particleRadius*10);
					
					// Possibility of 3 collision levels - determined by alpha
					// Level 1 : HIT 
					// Level 2 : Strong Attraction
					// Level 3 : Weak Attraction / Repulsion
					
					collisions[i].object1.setTarget(newX,newY);
				}

			}
			
		}
		
		private function checkDistance(x1:Number,y1:Number,x2:Number,y2:Number):Number{
			var xd:Number = x2-x1
			var yd:Number = y2-y1
			return Math.sqrt(xd*xd + yd*yd);
		}
		
		private function removeParticle():void{
		}
		
		private function addParticle():void{
			var localPoint:Point = localToGlobal(centreGroup);
			var theParticle:Particle = new Particle(0,0);
			
			theParticle.y = Math.random()*100;
			theParticle.x = Math.random()*100;
			this.addChild(theParticle);
			
			collisionGroup.addItem(theParticle);
			
			theParticles.push(theParticle);
		}
		
		private function addParticles(numParticles:Number):void{
			for(var i=0;i<numParticles;i++){
				addParticle();
			}
			
		}
		
	}
	
}