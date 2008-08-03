package  
{
	import flash.events.MouseEvent;
	/**
	* PARASITETAIL
	* Class used to look after the tail of the parasite
	* @author Anthony Massingham
	*/
	public class ParasiteTail extends ParasiteElement
	{
		
		public function ParasiteTail()
		{
			theColour = 0xCDC2B4;
			super(10);
			
			this.addEventListener(MouseEvent.MOUSE_DOWN, grabTail);
		}
		
		private function grabTail(event:MouseEvent):void
		{
			trace("Tail Grabbed");
		}
		
	}
	
}