package  
{
	import flash.display.Sprite;
	import flash.geom.Rectangle;
	
	/**
	* ...
	* @author DefaultUser (Tools -> Custom Arguments...)
	*/
	public class CollisionManager extends Sprite
	{
		var axisSprite:Sprite = new Sprite();
		
		public function CollisionManager() 
		{
			addChild(axisSprite);
		}
		
		public function doCollision(shape1:SimpleShape, shape2:SimpleShape):void {
			// 1. Determine type of shape, and therefore type of collision
			quadtoquad(shape1, shape2);
		}
		
		public function quadtoquad(quad1:SimpleShape, quad2:SimpleShape):void {
			// Get Axis Limits
			var boundingBoxRect:Rectangle = quad1.Bounds.union(quad2.Bounds);
			boundingBoxRect.width += 50;
			boundingBoxRect.height += 50;
			
			axisSprite.graphics.clear();
			axisSprite.graphics.lineStyle(0, 0, 0.2);
			axisSprite.graphics.drawRect(boundingBoxRect.x, boundingBoxRect.y, boundingBoxRect.width, boundingBoxRect.height);
			
			// Angle Projection
			
			// Check X
			// Quad 1X
			var xRect1:Rectangle = new Rectangle(quad1.Position.x + quad1.Vertices[0].x, boundingBoxRect.y + boundingBoxRect.height, quad1.Position.x + quad1.Vertices[1].x, boundingBoxRect.y + boundingBoxRect.height);
			axisSprite.graphics.lineStyle(0,0xC04B4B,0.8);
			axisSprite.graphics.moveTo(xRect1.x,xRect1.y);
			axisSprite.graphics.lineTo(xRect1.width,xRect1.height);
			
			// Quad 2X
			var xRect2:Rectangle = new Rectangle(quad2.Position.x + quad2.Vertices[0].x, boundingBoxRect.y + boundingBoxRect.height, quad2.Position.x + quad2.Vertices[1].x, boundingBoxRect.y + boundingBoxRect.height);
			axisSprite.graphics.lineStyle(0,0x5CBD4F,0.8);
			axisSprite.graphics.moveTo(xRect2.x, xRect2.y);
			axisSprite.graphics.lineTo(xRect2.width, xRect2.height);
			
			if (xRect1.intersects(xRect2)) {
				// X AXIS INTERSECTS, CONTINUE
				var yRect1:Rectangle = new Rectangle(boundingBoxRect.x + boundingBoxRect.width, quad1.Position.y + quad1.Vertices[0].y, boundingBoxRect.x + boundingBoxRect.width, quad1.Position.y + quad1.Vertices[3].y);
				axisSprite.graphics.lineStyle(0,0xC04B4B,0.8);
				axisSprite.graphics.moveTo(yRect1.x, yRect1.y);
				axisSprite.graphics.lineTo(yRect1.width,yRect1.height);
				
				var yRect2:Rectangle = new Rectangle(boundingBoxRect.x + boundingBoxRect.width, quad2.Position.y + quad2.Vertices[0].y, boundingBoxRect.x + boundingBoxRect.width, quad2.Position.y + quad2.Vertices[3].y);
				axisSprite.graphics.lineStyle(0,0x5CBD4F,0.8);
				axisSprite.graphics.moveTo(yRect2.x,yRect2.y);
				axisSprite.graphics.lineTo(yRect2.width, yRect2.height);
				
				if (yRect1.intersects(yRect2)) {
					trace("COLLISION!!!!!");
				}
			} else {
				return;
			}
			
			// Y Axis overlap
		}
		
		public function clearSprite():void {
			axisSprite.graphics.clear();
		}
		
	}
	
}