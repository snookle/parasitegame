package  
{
	
	/**
	* Parasite Head Class
	* @author Anthony Massingham
	*/
	public class ParasiteHead extends ParasiteBodyPart
	{
		
		public function ParasiteHead(_theSprite, _theWeight):void
		{
			super(_theSprite, _theWeight);
		}
		
		public override function updatePoint():void
		{
			velocity.addTo(force);
			
			this.IKPoint.update(null);
			
			// X Pos
			this.velocity.x *= 0.9;
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
		
	}
	
}