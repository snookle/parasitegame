package 
{
	import flash.display.Sprite;
	import flash.events.Event;
	import flash.geom.Rectangle;
	
	/**
	 * Attempt at getting collisions working for Paresightz
	 */
	public class Main extends Sprite
	{
		
		var shapes:Array = new Array();
		var collisionManager:CollisionManager;
		
		public function Main():void
		{
			if (stage) init();
			else addEventListener(Event.ADDED_TO_STAGE, init);
		}
		
		private function init(e:Event = null):void 
		{
			removeEventListener(Event.ADDED_TO_STAGE, init);
			addEventListener(Event.ENTER_FRAME, draw);
			addEventListener(Event.ENTER_FRAME, update);
			
			collisionManager = new CollisionManager();
			addChild(collisionManager);
			
			// Entry Point
			trace("STARTED");
			
			// Init Collision Manager
			
			// Create Simple Shape 1
			var simpleShape1:SimpleShape = new SimpleShape(new Array(new Vector2(0, 0), new Vector2(50, 0), new Vector2(50, 50), new Vector2(0, 50)));
			addChild(simpleShape1);
			shapes.push(simpleShape1);
			
			// Create Simple Shape 2
			var simpleShape2:SimpleShape = new SimpleShape(new Array(new Vector2(0, 0), new Vector2(50, 0), new Vector2(50, 50), new Vector2(0, 50)), 0xC04B4B);
			simpleShape2.Position = new Vector2(100, 100);
			addChild(simpleShape2);
			shapes.push(simpleShape2);
		}
		
		private function draw(event:Event):void {
			for (var i = 0; i < shapes.length; i++) {
				shapes[i].draw();
			}
		}
		
		private function update(event:Event):void {
			for (var i = 0; i < shapes.length; i++) {
				shapes[i].update();
			}
			
			// we're assuming that the two shapes are both in the same spatial grid ranges
			
			// 1. Check Bounding Boxes
			// Not Implemented Currently
			if (shapes[0].Bounds.intersects(shapes[1].Bounds)) {
				collisionManager.doCollision(shapes[0], shapes[1]);
			} else {
				collisionManager.clearSprite();
			}
		}
	}
}