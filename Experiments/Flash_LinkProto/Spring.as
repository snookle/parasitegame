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
		
		public var object1:MovieClip;							// The First Mass @ the Tip of the spring
		public var object2:MovieClip;							// The Second Mass @ the Other Tip
		
		public var springConstant:Number;						// Stiffness of Spring
		public var springLength:Number;							// Length of the Spring
		public var frictionConstant:Number;						// Inner Friction
		
		public function Spring(object1:MovieClip, object2:MovieClip, springConstant:Number, springLength:Number,frictionConstant:Number) 
		{
			this.springConstant = springConstant;
			this.springLength = springLength;
			this.frictionConstant = frictionConstant;
			
			this.object1 = object1;
			this.object2 = object2;			
		}
		
		public function solve():void
		{
			// Workaround for flash, as objects have inbuilt x and y.
			var object1Pos:Vector2D = new Vector2D(object1.x, object1.y);
			var object2Pos:Vector2D = new Vector2D(object2.x, object2.y);
						
			var springVector:Vector2D = object1Pos.subtract(object2Pos);		// Vector between the Two object			
			
			var r:Number = springVector.length();								// Distance between the object
			
			var force:Vector2D = new Vector2D(0, 0);							// Force is initially 0
			
			// To Avoid / by 0
			if (r != 0)
			{
				// Spring force is added
				//var temp:Vector2D = springVector.divide(r).multiply((r - springLength) * ( -springConstant));
				
				//var forceVal:Number = ((-springConstant) * (r - springLength));

				//var tempX:Number = (springVector.x / r) * ((r - springLength) * -springConstant);
				//var tempY:Number = (springVector.y / r) * ((r - springLength) * -springConstant);
				
				//var tempX:Number = (springVector.x/r) * forceVal;
				//var tempY:Number = (springVector.y/r) * forceVal;
				
				//trace(forceVal);
								
				//var temp:Vector2D = new Vector2D(tempX, tempY);
				force.addTo(temp);
			}
			
			//var frictionForce:Vector2D = object1.velocity.subtract(object2.velocity);
			//frictionForce.multiplyTo(-frictionConstant);
			//force.addTo(frictionForce);		// Friction force is added
			
			object1.applyForce(force);
			object2.applyForce(force.negative());
		}
		
	}
	
}