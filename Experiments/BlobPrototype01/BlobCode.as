package 
{
	import flash.display.MovieClip;
	import flash.geom.Point;
	import Particle;
	import flash.events.Event;
	import fl.events.SliderEvent;
	import flash.text.TextField;

	
	/**
	* ...
	* @author Anthony Massingham for BlueDog Training
	*/
	public class BlobCode extends MovieClip 
	{
		public var numParticles:Number = 100;
		public var theParticles:Array = new Array();
		public var connected:Array = new Array();
		
		public var threshold:Number = 20;
		public var numLinks:Number = 6;
		
		public var drawLinks:Boolean = false;
		
		public function BlobCode()
		{
			// Create Particles
			for (var i = 0; i < numParticles; i++)
			{
				var theParticle:Particle = new Particle(10 * i,100);
				theParticles.push(theParticle);
				this.addChild(theParticle);
				connected.push(new Array())
			}
			
			addEventListener(Event.ENTER_FRAME, update);
			threshLevel.text = "Threshold : "+threshold;
			thresholdLevel.addEventListener(SliderEvent.CHANGE, changeSlider);
		}
		
		private function changeSlider(event:Event):void
		{
			threshold = event.target.value;
			threshLevel.text = "Threshold : "+threshold;
		}
		
		private function update(event:Event):void
		{
			moveParticles();
			checkParticles();
			
			this.graphics.clear();
		}
		
		private function checkParticles():void
		{
			for (var i = 0; i < theParticles.length; i++)
			{
				var thisParticle:Particle = theParticles[i];
				thisParticle.clearLinks();
				for (var j = i+1; j < theParticles.length; j++)
				{
					var thatParticle:Particle = theParticles[j];
					
					var thisPosition:Point = localToGlobal(new Point(thisParticle.x, thisParticle.y));
					var thatPosition:Point = localToGlobal(new Point(thatParticle.x, thatParticle.y));
					
					this.graphics.lineStyle(0);
					this.graphics.moveTo(thisPosition.x, thisPosition.y);
					this.graphics.lineTo(thatPosition.x, thatPosition.y);
					
					var distance:Number = calculateDistance(thisPosition, thatPosition);
					distance = Math.round(distance);
					
					if (distance <= threshold && !connected[i][j])
					{
						if (thisParticle.neighbors.length != numLinks && thatParticle.neighbors.length != numLinks)
						{
							// room for connection
							//trace("Connect");
							
							connected[i][j] = true;
							
							thisParticle.addNeighbour(thatParticle);
							thatParticle.addNeighbour(thisParticle);
							// Create Sring
						}
					} else if(distance>threshold && connected[i][j]){
						connected[i][j] = false;
						thisParticle.removeNeighbour(thatParticle);
						thatParticle.removeNeighbour(thisParticle);
					} 
					//trace("i:" + i + ", j:" + j);
				}
				
				//trace(thisParticle.neighbors.length);
				
				if (thisParticle.neighbors.length > 0)
				{
					thisParticle.highlight(true);
					if (linksButton.selected)
					{
						thisParticle.drawLinks();
					}
				} else {
					thisParticle.highlight(false);
				}
			}
		}
		
		private function moveParticles():void
		{
			for (var i = 0; i < theParticles.length; i++)
			{
				theParticles[i].x += theParticles[i].velocity.x;
				theParticles[i].y += theParticles[i].velocity.y;
				
				var globalPosition:Point = localToGlobal(new Point(theParticles[i].x, theParticles[i].y));
				
				if (globalPosition.x > 500 || globalPosition.x < 0)
				{
					theParticles[i].velocity.x *= -1;
				}
				
				if (globalPosition.y > 500 || globalPosition.y < 0)
				{
					theParticles[i].velocity.y *= -1;
				}
			}
		}
		
		private function calculateDistance(point1:Point,point2:Point):Number
		{
			var xd:Number = point1.x - point2.x;
			var yd:Number = point1.y - point2.y;
			
			var td:Number = Math.sqrt(xd * xd + yd * yd);
			return td;
		}
		
		
	}
	
}