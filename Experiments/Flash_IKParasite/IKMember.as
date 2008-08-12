package  
{
	import flash.events.MouseEvent;
	import flash.events.Event;
	
	import flash.display.MovieClip;
	
	import flash.display.Stage;
	
	/**
	* ...
	* @author Anthony Massingham
	*/
	public class IKMember 
	{
		
		public var skin:MovieClip;
		public var father:IKMember = null;
		public var distance:Number;
		public var name:String = "";
		
		private var nnb:Array;
		private var fpr:Array;
		
		public var rigid:Boolean = false;
		
		public function IKMember(sprite:MovieClip) 
		{
			distance = 20// Default Distance
			skin = sprite;
			nnb = new Array();
			
			//skin.addEventListener(MouseEvent.MOUSE_DOWN, startMove);
			//GetStage.stage.addEventListener(MouseEvent.MOUSE_UP, stopMove);
			
			// skin.addEventListener(Event.ENTER_FRAME, update);
		}
		
		public function enableDrag():void
		{
			skin.addEventListener(MouseEvent.MOUSE_DOWN, startMove);
			GetStage.stage.addEventListener(MouseEvent.MOUSE_UP, stopMove);
		}
		
		public function disableDrag():void
		{
			skin.removeEventListener(MouseEvent.MOUSE_DOWN, startMove);
			GetStage.stage.removeEventListener(MouseEvent.MOUSE_UP, stopMove);
		}
		
		public function addNeighbor(neighbor:IKMember):void
		{
			nnb.push(neighbor);
		}
		
		public function addAngleConstraint(n1:IKMember, angle1:Number, n2:Number, angle2:Number):void
		{
			fpr = new Array(n1, n2, angle1, angle2);
		}
		
		public function startMove(event:MouseEvent):void
		{
			GetStage.stage.addEventListener(MouseEvent.MOUSE_MOVE, fireMove);
		}
		
		public function stopMove(event:MouseEvent):void
		{
			GetStage.stage.removeEventListener(MouseEvent.MOUSE_MOVE, fireMove);
		}
		
		private function fireMove(evnet:MouseEvent):void
		{
			move(GetStage.stage.mouseX, GetStage.stage.mouseY);
		}
		
		public function update(event:Event):void
		{
			move(skin.x, skin.y, father);
		}
		
		public function move(_x:Number, _y:Number, father:IKMember = null):void
		{
			if (father == null)
			{
				skin.x = _x;
				skin.y = _y;
			} else {
				MakeMove(this, father);
			}
			
			setAngle(father);
			for (var i = 0; i < nnb.length; i++)
			{
				if (nnb[i] != father)
				{
					nnb[i].move(_x, _y, this);
				}
			}
		}
		
		private function MakeMove(child:IKMember, father:IKMember):void
		{
			var dx:Number = child.skin.x - father.skin.x;
			var dy:Number = child.skin.y - father.skin.y;
			var a1:Number = Math.atan2(dy, dx);
			
			child.skin.x = father.skin.x + Math.cos(a1) * distance;
			child.skin.y = father.skin.y + Math.sin(a1) * distance;
			child.skin.rotation = (Math.PI + a1) * 180 / Math.PI;
		}
		
		private function setAngle(father:IKMember):void
		{
			var angle:Number;
			var a:IKMember;
			var b:IKMember;
			if (fpr != null)
			{
				var node = this.fpr;
				if (node[1] == father)
				{
					a = node[1];
					b = node[0];
					angle = 2 * Math.PI - node[2];
				} else {
					a = node[0];
					b = node[1];
					angle = node[2];
				}
				var ax:Number = a.skin.x - skin.x;
				var ay:Number = a.skin.y - skin.y;
				var aangle:Number = Math.atan2(ay, ax);
				b.skin.x = skin.x + Math.cos(aangle + angle) * distance;
				b.skin.y = skin.y + Math.sin(aangle + angle) * distance;				
			}
		}
		
		
		
	}
	
}