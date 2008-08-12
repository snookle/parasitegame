package  
{
	import flash.events.MouseEvent;
	
	/**
	* Parasite Tail Class
	* @author Anthony Massingham
	*/
	public class ParasiteTail extends ParasiteBodyPart
	{
		public function ParasiteTail(_theSprite, _theWeight):void
		{
			super(_theSprite, _theWeight);
		}
		
		public override function init():void
		{
			this.buttonMode = true;
			this.addEventListener(MouseEvent.MOUSE_OVER, hover);
		}
		
		private function hover(event:MouseEvent):void
		{
			
		}
	}
	
}