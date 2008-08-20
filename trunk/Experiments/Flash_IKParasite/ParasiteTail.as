package  
{
	import flash.events.MouseEvent;
	
	/**
	* Parasite Tail Class
	* @author Anthony Massingham
	*/
	public class ParasiteTail extends ParasiteBodyPart
	{
		public var isMoving:Boolean = false;
		public var isClicked:Boolean = false;
		
		public function ParasiteTail(_theSprite, _theWeight):void
		{
			super(_theSprite, _theWeight);
		}
		
		public function initTail():void
		{
			this.buttonMode = true;
			this.mouseEnabled = true;
			this.addEventListener(MouseEvent.MOUSE_OVER, hover);
			
			if(IKPoint!=null){
				//this.IKPoint.enableDrag();
				
				addEventListener(MouseEvent.MOUSE_DOWN, clickTail);
				GetStage.stage.addEventListener(MouseEvent.MOUSE_UP, stopclickTail);
			}
		}
		
		public override function specialMovement():void
		{
			var xDist:Number = (GetStage.stage.mouseX - this.x);
			var yDist:Number = (GetStage.stage.mouseY - this.y);
			
			if (xDist < 25 && yDist < 25)
			{
				dragTail(null);
			} else if (isMoving)
			{
				releaseTail(null);
			}
		}
		
		private function clickTail(event:MouseEvent):void
		{
			isClicked = true;
		}
		
		private function stopclickTail(event:MouseEvent):void
		{
			isClicked = false;
		}
		
		private function dragTail(event:MouseEvent):void
		{
			isMoving = true;
			this.IKPoint.startMove(event);
		}
		
		private function releaseTail(event:MouseEvent):void
		{
			isMoving = false;
			this.IKPoint.stopMove(event);
		}
		
		private function hover(event:MouseEvent):void
		{
			
		}
	}
	
}