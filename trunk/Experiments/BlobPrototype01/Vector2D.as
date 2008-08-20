package  
{
	
	/**
	* Vector2D
	* Attempt at Conversion of NeHe's Vector3D Class from Java to Actionscript
	* * @author Anthony Massingham
	*/
	public class Vector2D 
	{
		
		public var x:Number;
		public var y:Number;
		
		public function Vector2D(x:Number, y:Number):void
		{
			this.x = x;
			this.y = y;
		}
		
		public function set(x:Number, y:Number):void
		{
			this.x = x;
			this.y = y;
		}
		
		public function add(v:Vector2D):Vector2D
		{
			return new Vector2D(x + v.x, y + v.y);
		}
		
		public function subtract(v:Vector2D):Vector2D
		{
			return new Vector2D(x - v.x, y - v.y);
		}
		
		public static function subtract(v1:Vector2D, v2:Vector2D):Vector2D
		{
			return new Vector2D(v1.x - v2.x, v1.y - v2.y);
		}		
		
		public function multiply(value:Number):Vector2D
		{
			return new Vector2D(x * value, y * value);
		}
		
		public function divide(value:Number):Vector2D
		{
			return new Vector2D(x / value,y / value);
		}
		
		public function addTo(v:Vector2D):Vector2D
		{
			x += v.x;
			y += v.y;
			return this;
		}
		
		public function subtractTo(v:Vector2D):Vector2D
		{
			x -= v.x;
			y -= v.y;
			return this;
		}
		
		public function multiplyTo(v:Number):Vector2D
		{
			x *= v;
			y *= v;
			return this;
		}
		
		public function divideTo(v:Number):Vector2D
		{
			x /= v;
			y /= v;
			return this;
		}
		
		public function negative():Vector2D
		{
			return new Vector2D( -x, -y);
		}
		
		public function length():Number
		{
			return Math.sqrt(x * x + y * y);
		}
		
		public function normalize():void
		{
			var length:Number = length();
			if (length == 0)
			{
				return;
			}
			x /= length;
			y /= length;
		}
		
		public function toString():String
		{
			return x + ", " + y;
		}
		
	}
	
}