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
		
		public var IKPoint:IKMember = null;
		
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
			
			this.mouseEnabled = false;
			this.mouseChildren = false;
			
			init();
		}
		
		public function init():void
		{
			force.x = 0;
			force.y = 0;
		}
		
		public function addIKPoint(thePoint:IKMember):void
		{
			IKPoint = thePoint;
		}
		
		public function applyForce(force:Vector2D):void
		{
			this.force.addTo(force);
		}
		
		public function updatePoint():void
		{
			specialMovement();
			
			velocity.addTo(force);
			
			this.IKPoint.update(null);
			// X Pos
			this.velocity.x *= 0.95;
			this.x += this.velocity.x;
			
			
			// Y Pos
			this.velocity.y *= 0.95;
			if (this.y+this.velocity.y > 480)
			{
				// Equal and Opposite force perhaps ?
				this.velocity.y = 0;
				this.y = 481;
			} else {	
				this.y += this.velocity.y;
			}

			
		}
		
		public function specialMovement():void
		{
			
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