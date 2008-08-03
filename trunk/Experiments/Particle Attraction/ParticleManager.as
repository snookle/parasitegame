package {
	
	import flash.display.MovieClip;
	
	// ParticleManager handles collections of ParticleGroups
	
	public class ParticleManager extends MovieClip{
		
		private var theParticleGroups:Array = new Array();			// Array to hold the groups
		
		public function ParticleManager():void{
			// To start with, let's create 1 particle group
		}
		
		public function startManager(numGroups:Number):void{
			addGroup(numGroups);
		}
		
		private function addGroup(number:Number):void{
			for(var i=0;i<number;i++){
				var theParticleGroup:ParticleGroup = new ParticleGroup();
				this.addChild(theParticleGroup);
				
				theParticleGroup.x = 300;
				theParticleGroup.y = 300;
				
				theParticleGroup.initGroup();
				theParticleGroups.push(theParticleGroup);
			}
		}
		
		
		
	}
	
}