package  
{
	
	import flash.display.MovieClip;
	import flash.events.Event;
	/**
	* PARASITEHEAD
	* Class used to look after the *head* of the parasite.
	* 
	* @author Anthony Massingham
	*/
	public class ParasiteHead extends ParasiteElement
	{
		
		public function ParasiteHead() 
		{
			theColour = 0x729164;
			super(15);
			
			this.addEventListener(Event.ENTER_FRAME, blink);
		}
		
		private function blink(event:Event):void
		{
			var randomChance:Number = Math.round(Math.random() * 30);
			if (randomChance == 30)
			{
				this.eyes.scaleY = 0.2;
			} else {
				this.eyes.scaleY = 1;
			}
		
		}
		
	}
	
}