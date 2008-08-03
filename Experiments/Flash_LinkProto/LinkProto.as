package 
{
	import flash.display.MovieClip;
	import flash.events.Event;
	import flash.events.MouseEvent;
	import flash.geom.Point;
	import flash.text.TextField;
	
	import APoint;
	/**
	* ...
	* @author Anthony Massingham
	*/
	public class LinkProto extends MovieClip
	{
		public var thePoints:Array = new Array();
		public var mouseDown:Boolean = false;
		public var detailsTextField:TextField = new TextField();
		
		public function LinkProto():void
		{
			addChild(detailsTextField);
			detailsTextField.height = 200;
			detailsTextField.width = 400;
			detailsTextField.multiline = true;
			detailsTextField.scaleX = 0.8;
			detailsTextField.scaleY = 0.8;
			
			var point1:APoint = new APoint(3);
			point1.x = 200;
			point1.y = 200;
			point1.init();
			addChild(point1);
			point1.name = "POINT 1";
			thePoints.push(point1);
			
			var point2:APoint = new APoint(1);
			point2.x = 250;
			point2.y = 200;
			point2.init();
			addChild(point2);
			point2.name = "POINT 2";
			thePoints.push(point2);
			
			var point3:APoint = new APoint(2);
			point3.x = 300;
			point3.y = 200;
			point3.init();
			addChild(point3);
			point3.name = "POINT 3";
			thePoints.push(point3);
			
			// Add Links
			
			point1.addNextPart(point2);
			
			point2.addPrevPart(point1);
			point2.addNextPart(point3);
			
			point3.addPrevPart(point2);
			
			
			addEventListener(Event.ENTER_FRAME, update);
			this.stage.addEventListener(MouseEvent.MOUSE_UP, releaseMouse);
		}
		
		public function releaseMouse(event:MouseEvent):void
		{
			for each( var pointObject:APoint in thePoints)
			{
				pointObject.releaseMouse(event);
			}
		}
		
		public function update(event:Event):void
		{
			
			detailsTextField.text = "";
			
			for each (var pointObject:APoint in thePoints)
			{
				pointObject.updatePoint();
				detailsTextField.appendText(pointObject.detailsText + "\n");
			}
			
			
		}
	}
	
}