package  
{
	import flash.display.DisplayObject;
	import flash.display.MovieClip;
	
	/**
	* Spring - Used for Parasite Connections (hopefully)
	* Based on Java conversion of NeHe's "Spring" class
	* 
	* * @author Anthony Massingham
	*/
	public class Spring 
	{
		
		public var object1:Particle;							// The First Mass @ the Tip of the spring
		public var object2:Particle;							// The Second Mass @ the Other Tip
		
		public var springConstant:Number;						// Stiffness of Spring
		public var springLength:Number;							// Length of the Spring
		public var frictionConstant:Number;						// Inner Friction
		
		public function Spring(springLength:Number) 
		{
			this.springLength = springLength
		}
		
	}
	
}