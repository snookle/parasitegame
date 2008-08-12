package  
{
	import flash.display.MovieClip;
	import flash.display.Sprite;
	import flash.geom.Point;
	import flash.events.Event;
	
	/**
	* Default Class for Parasite body Parts.
	* @author Anthony Massingham
	*/
	public class ParasiteBodyPart extends MovieClip
	{
		public var sprite:Sprite;
		public var velocity:Vector2D;
		
		public var weight:Number;
		
		public var force:Vector2D;
		
		public var nextPart:ParasiteBodyPart;		// Possibly control this via 'Parasite' ? 
		public var prevPart:ParasiteBodyPart;
		
		public function ParasiteBodyPart(_sprite:Sprite,_weight:Number):void
		{
			sprite = _sprite;
			weight = _weight;
			
			// initialise velocity
			velocity = new Vector2D(0, 0);
			force = new Vector2D(0, 0);
			
			// Add the sprite to the display position - May have to realign so it is centrerd at x,y
			addChild(sprite);
			
			init();
		}
		
		public function init():void
		{
			force.x = 0;
			force.y = 0;
		}
		
		public function addNextPart(aPart:ParasiteBodyPart):void
		{
			nextPart = aPart;
		}
		
		public function addPrevPart(aPart:ParasiteBodyPart):void
		{
			prevPart = aPart;
		}
		
		public function applyForce(force:Vector2D):void
		{
			this.force.addTo(force);
			
			// trace("Adding :" + force.x + "," + force.y);
		}
		
		public function updatePoint():void
		{
			//var globalPos:Point = this.localToGlobal(new Point(this.x, this.y));
			
			//velocity.addTo(force.divide(weight).multiply(dt));
			velocity.addTo(force);
			
			//trace("Velocity :" + velocity.x + "," + velocity.y);
			
			this.x += velocity.x;
			this.y += velocity.y;
			
			//trace("Current Pos :" + this.x + "," + this.y);
			
			// X Pos
			/*this.velocity.x *= 0.7;
			this.x += this.velocity.x;
			
			
			// Y Pos
			if (this.y+300 < 480)
			{
				this.velocity.y += 9;
			} else {
				this.velocity.y = 0;
			}
			
			this.velocity.y *= 0.7;
			this.y += this.velocity.y;*/
		}
		
		public function globalX():Number
		{
			return this.localToGlobal(new Point(this.x, this.y)).x;
		}
		
		public function globalY():Number
		{
			return this.localToGlobal(new Point(this.x, this.y)).x;
		}
		
		
		
	}
	
}