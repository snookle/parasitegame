package  
{
	import flash.errors.IllegalOperationError;
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
		
		public var lockAngle:Boolean = false;
		public var lockedAngle:Number = 0;
		
		public var currentAngle:Number = 0;
		
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
		
		public function addAngleConstraint(n1:IKMember, angle1:Number, n2:IKMember):void
		{
			fpr = new Array(n1, n2, angle1);
		}
		
		public function removeAngleConstraint():void
		{
			fpr = null;
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
			if(!lockAngle){
				var dx:Number = child.skin.x - father.skin.x;
				var dy:Number = child.skin.y - father.skin.y;
				var a1:Number = Math.atan2(dy, dx);
				
				currentAngle = a1;

				child.skin.x = father.skin.x + Math.cos(a1) * distance;
				child.skin.y = father.skin.y + Math.sin(a1) * distance;
				child.skin.rotation = (Math.PI + a1) * 180 / Math.PI;
			} else {
				// Code for the Locked Angle ...
				
			}
			
			
		}
		
		private function setAngle(father:IKMember):void
		{
			var angle:Number = currentAngle;
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
					trace("other");
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
		
		private function setAngleold(father:IKMember):void
		{
			var angle:Number;
			var a:IKMember;
			var b:IKMember;
			var c:IKMember;
			
			if (fpr != null)
			{
				
				var border:Number = 0;
				var node = this.fpr;
				if (node[1] == father)
				{
					// move first neighbour and itself
					a = node[1];
					b = node[0];
					c  = this;
					angle = 2 * Math.PI - node[2] - node[3];
				} else if (node[0] == father)
				{
					// Move second neighbour and itself
					a = node[0];
					b = node[1];
					c = this;
					angle = node[2];
				} else {
					// Parent is not fixed member, just have to move second neighbour
					a = node[0];
					b = node[1];
					c = null;
					angle = node[2];
				}
				
				var ax:Number = a.skin.x - skin.x;
				var ay:Number = a.skin.y - skin.y;
				var aangle:Number = Math.atan2(ay, ax);
				var bx:Number = b.skin.x - skin.x;
				var by:Number = b.skin.y - skin.y;
				var bangle:Number = Math.atan2(by, bx);
				
				var maxAngle:Number = node[3];
				
				if (maxAngle == 0)
				{
					// No Limiting Angle ? 
					// Place Members to position defined by angles
					b.skin.x = skin.x + Math.cos(aangle + angle) * distance;
					b.skin.y = skin.y + Math.sin(aangle + angle) * distance;
				} else if (c!=null) {
					// Must not go under 0...
					if (bangle < aangle)
					{
						bangle += 2 * Math.PI;
					}
					
					// If there is a limiting Angle ( angle and angle+maxAngle)
					// Check if present position is under or over the limit, and sets the closest border.
					
					// If it's under the limit :
					if (bangle < aangle + angle)
					{
						border = angle;
					}
					
					// if it's over the highest limit
					if (bangle > aangle + angle + maxAngle)
					{
						border = angle + maxAngle;
					}
					
					if (border != 0)
					{
						var cx:Number = b.skin.x - a.skin.x;
						var cy:Number = b.skin.y - a.skin.y;
						var cangle:Number = Math.atan2(cy, cx);
						
						// Calculate ar and br sided limiting triangles
						var px:Number = distance - Math.cos(border) * distance;
						var py:Number = Math.sin(border) * distance;
						var mind:Number = Math.sqrt(px * px + py * py);
						var gamma:Number = Math.atan2(py, px);
						var deltha:Number = Math.PI - border - gamma;

						// The Third angle of the Tri
						b.skin.x = a.skin.x + Math.cos(cangle) * mind;
						b.skin.y = a.skin.y + Math.sin(cangle) * mind;
						
						var thisang:Number = cangle + deltha;
						
						c.skin.x = a.skin.x + Math.cos(thisang) * distance;
						c.skin.y = a.skin.y + Math.sin(thisang) * distance;
					}
				}
			}
		}		
	}
	
}