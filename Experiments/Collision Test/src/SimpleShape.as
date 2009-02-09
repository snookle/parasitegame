package  
{
	import flash.display.Sprite;
	import flash.events.Event;
	import flash.events.MouseEvent;
	import flash.geom.Rectangle;
	import Vector2;
	
	/**
	* Simple Draggable Sprite
	*/
	public class SimpleShape extends Sprite
	{
		public var Vertices:Array;
		public var Position:Vector2 = new Vector2(0, 0);
		public var Bounds:Rectangle;
		private var theSprite:Sprite;
		private var BackgroundColour:uint = 0x668FA6;
		
		public function SimpleShape(vertices:Array, colour:uint = 0x668FA6) 
		{
			this.Vertices = vertices;
			this.BackgroundColour = colour;
			
			theSprite = new Sprite();
			addChild(theSprite);
			theSprite.addEventListener(MouseEvent.MOUSE_DOWN, dragSprite);
			theSprite.buttonMode = true;
			theSprite.mouseEnabled = true;
		}
		
		private function dragSprite(event:MouseEvent):void {
			this.stage.addEventListener(MouseEvent.MOUSE_UP, releaseSprite);
			this.addEventListener(Event.ENTER_FRAME, dragging);
		}
		
		private function dragging(event:Event):void {
			this.Position = new Vector2(mouseX, mouseY);
		}
		
		private function releaseSprite(event:MouseEvent) {
			this.removeEventListener(Event.ENTER_FRAME, dragging);
		}
		
		public function update():void {
		}
		
		public function draw():void {
			theSprite.graphics.clear();
			theSprite.graphics.lineStyle(0, BackgroundColour, 0.8);
			theSprite.graphics.beginFill(BackgroundColour, 0.5);
			
			theSprite.graphics.moveTo(Position.x + Vertices[0].x, Position.y + Vertices[0].y);
			for (var i = 1; i < Vertices.length; i++) {
				theSprite.graphics.lineTo(Position.x + Vertices[i].x, Position.y + Vertices[i].y);
			}
			theSprite.graphics.lineTo(Position.x + Vertices[0].x, Position.y + Vertices[0].y);
			
			theSprite.graphics.endFill();
			Bounds = theSprite.getBounds(this);
		}
		
	}
	
}