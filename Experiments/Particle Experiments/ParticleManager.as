package { 

	import flash.display.MovieClip;
	import flash.display.BitmapData;
	
	import flash.events.Event;
	
	import flash.geom.Point;
	import flash.geom.Rectangle;
	import flash.geom.ColorTransform;
	import flash.geom.Matrix;
	
	import coreyoneil.collision.*;

	public class ParticleManager extends MovieClip{
		
		private var numParticles:Number;					// The Max Number of Particles
		private var theParticles:Array = new Array();		// The Array to hold the particles
		
		private var theBoundary:MovieClip;					// The Movie Clip used for Hit Detection
		
		private var collisionGroup:CollisionGroup = new CollisionGroup();
		
		public function ParticleManager(_numParticles:Number, theHitMask:MovieClip):void{
			
			numParticles = _numParticles;
			theBoundary = theHitMask;
			
			trace("Particle Manager Started");
			this.addEventListener(Event.ENTER_FRAME,manageParticles);
			
			collisionGroup.returnAngle = true;
			collisionGroup.addItem(theHitMask);
		}
		
		private function manageParticles(event:Event):void{
			// See if any more particles need to be added 
			if(theParticles.length<numParticles){
				addParticle();
			}
			
			for(var i=0;i<theParticles.length;i++){
				theParticles[i].doMovement();
				theParticles[i].drawParticle();
			}
			
			collisionManagement();
		}
		
		private function addParticle():void{
			var theParticle:Particle = new Particle(Math.random()*500,0,20);
			
			this.addChild(theParticle);
			collisionGroup.addItem(theParticle);
			
			theParticles.push(theParticle);
		}
		
		private function removeParticles():void{
		}
		
		private function collisionManagement():void{
			var theCollisions:Array = collisionGroup.checkCollisions();
			if(theCollisions.length != 0){
				// object 1 smaller
				// object 2 larger
				// angle
				// overlap
				for(var i=0;i<theCollisions.length;i++){
					trace(theCollisions[i].angle);
					theCollisions[i].object1.flipY(theCollisions[i].overlap);
					//theCollisions[i].object1.changeDirection(theCollisions[i].overlap/100,0);
					//theCollisions[i].object2.changeDirection(0,0)
				}
			}
		}
		
	}
	
}