package 
{
	import flash.display.MovieClip;
	import flash.display.Sprite;
	import flash.geom.Point;
	import flash.ui.Mouse;
	import Particle;
	import flash.events.Event;
	import fl.events.SliderEvent;
	import flash.text.TextField;
	
	import flash.events.MouseEvent;

	
	/**
	* ...
	* @author Anthony Massingham for BlueDog Training
	*/
	public class BlobCode extends MovieClip 
	{
		public var numParticles:Number = 100;
		public var theParticles:Array = new Array();
		public var connected:Array = new Array();
		public var particleSize:Number = 6;
		
		public var gravity:Vector2D = new Vector2D(0, 0.5);
		
		public var theSprings:Array = new Array();
		
		public var threshold:Number = 20;
		public var disconThresh:Number = 24;
		public var numLinks:Number = 6;
		
		// SPRING VALUES :
		
		public var springStiffness:Number	= 0.62;
		public var springLength:Number		= threshold;
		public var springFriction:Number	= 0.03;
		
		public var mouseHit:Sprite = new Sprite();
		
		public var drawLinks:Boolean = false;
		
		private var running:Boolean = false;
		
		public function BlobCode()
		{
			startbutton.addEventListener(MouseEvent.CLICK, clear);
			threshLevel.text = "Threshold : " + threshold;
			disconLevel.text = "Disconnect : "+disconThresh;
			thresholdLevel.addEventListener(SliderEvent.CHANGE, changeSlider);
			disconnectLevel.addEventListener(SliderEvent.CHANGE, disconSlider);
			
			springLengthText.text = "Spring Length : " + springLength;
			springLengthSlider.addEventListener(SliderEvent.CHANGE, updateSpringSliders);
			stiffnessText.text = "Spring Stiffness : " + springStiffness;
			stiffnessSlider.addEventListener(SliderEvent.CHANGE, updateSpringSliders);
			springFrictionText.text = "Spring Friction : " + springFriction;
			springFrictionSlider.addEventListener(SliderEvent.CHANGE, updateSpringSliders);
			numConnectionsText.text = "Num Connections : " + numLinks;
			numConnectionsSlider.addEventListener(SliderEvent.CHANGE, updateSpringSliders);
			numParticlesText.text = "Num Particles : " + numParticles;
			numParticlesSlider.addEventListener(SliderEvent.CHANGE, updateSlider);
			particleSizeText.text = "Particle Size : " + particleSize;
			particleSizeSlider.addEventListener(SliderEvent.CHANGE, updateSlider);
		}
		
		public function updateSlider(event:SliderEvent):void
		{
			switch(event.target.name)
			{
				case "numParticlesSlider":
					numParticles = event.target.value;
					numParticlesText.text = "Num Particles : " + numParticles;
				break;
				case "particleSizeSlider":
					particleSize = event.target.value;	
					particleSizeText.text = "Particle Size : " + particleSize;
				break;
			}
		}
		
		public function clear(event:MouseEvent):void
		{
			for (var i = theParticles.length-1; i >= 0; i--)
			{
				removeChild(theParticles[i]);
				theParticles.pop();
			}
			
			theParticles = new Array();
			theSprings = new Array();
			connected = new Array();
			mouseHit = new Sprite();
			
			startSimulation();
		}
		
		public function startSimulation():void
		{		
			// Create Particles
			for (var i = 0; i < numParticles; i++)
			{
				var theParticle:Particle = new Particle(100,100,particleSize);
				theParticles.push(theParticle);
				this.addChild(theParticle);
				connected.push(new Array())
				theSprings.push(new Array());
			}
			
			addEventListener(Event.ENTER_FRAME, update);
			
			mouseHit.graphics.beginFill(0x000000, 0.5);
			mouseHit.graphics.drawCircle(0, 0, 50);
			mouseHit.graphics.endFill();
			mouseHit.alpha = 0;
			addChild(mouseHit);
			
			stage.addEventListener(MouseEvent.CLICK, boostPoints);
			
			running = true;
		}
		
		private function updateSpringSliders(event:Event):void
		{
			switch(event.target.name)
			{
				case "springLengthSlider":
					springLength = event.target.value;
					springLengthText.text = "Spring Length : " + springLength;
				break;
				case "stiffnessSlider":
					springStiffness = event.target.value;	
					stiffnessText.text = "Spring Stiffness : " + springStiffness;
				break;
				case "springFrictionSlider":
					springFriction = event.target.value;
					springFrictionText.text = "Spring Friction : " + springFriction;
				break;
				case "numConnectionsSlider":
					numLinks = event.target.value;
					numConnectionsText.text = "Num Connections : " + numLinks;
				break;
			}
			
			updateSprings();
		}
		
		private function changeSlider(event:Event):void
		{
			threshold = event.target.value;
			threshLevel.text = "Threshold : "+threshold;
		}
		
		private function disconSlider(event:Event):void
		{
			disconThresh = event.target.value;
			disconLevel.text = "Disconnect : "+disconThresh;
		}
		
		private function boostPoints(event:MouseEvent):void
		{
			if(mouseX<500 && mouseY<450){
				mouseHit.alpha = 1;
				mouseHit.x = mouseX;
				mouseHit.y = mouseY;
				
				for (var i = 0; i < theParticles.length; i++)
				{
					var mousePos:Point = new Point(mouseX, mouseY);
					var thisPosition:Point = localToGlobal(new Point(theParticles[i].x, theParticles[i].y));
					var distance:Number = calculateDistance(mousePos, thisPosition);
					
					if (distance < 60)
					{
						theParticles[i].applyForce(new Vector2D(0, -15));
					}
				}
			}
		}
		
		private function update(event:Event):void
		{
			moveParticles();
			checkParticles();
			
			if (mouseHit.alpha > 0)
			{
				mouseHit.alpha -= 0.1;
			}
			
			//this.graphics.clear();
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
							// Create Spring
							
							var newSpring:Spring = new Spring(thisParticle, thatParticle, springStiffness, springLength, springFriction);
							theSprings[i][j] = newSpring;
						}
					} else if(distance>disconThresh && connected[i][j]){
						connected[i][j] = false;
						thisParticle.removeNeighbour(thatParticle);
						thatParticle.removeNeighbour(thisParticle);
						
						theSprings[i][j] = null;
					} 
					
					if (theSprings[i][j] != null)
					{
						theSprings[i][j].solve();
					}
					//trace("i:" + i + ", j:" + j);
				}
				
				//trace(thisParticle.neighbors.length);
				
				if (thisParticle.neighbors.length > 0)
				{
					//thisParticle.highlight(true);
					if (linksButton.selected)
					{
						thisParticle.drawLinks();
					}
					if (thisParticle.neighbors.length >= numLinks)
					{
						thisParticle.highlight(0x92C479);
					}  else {
						thisParticle.highlight(0x2E2E2E);
					}
				} else {
					thisParticle.highlight(0xDA7C50);
				}
			}
		}
		
		private function updateSprings():void
		{
			for (var i = 0; i < theParticles.length; i++)
			{
				for (var j = i + 1; j < theParticles.length; j++)
				{
					if (theSprings[i][j] != null)
					{
						theSprings[i].springLength = springLength;
						theSprings[i].springConstant = springStiffness;
						theSprings[i].frictionConstant = springFriction;
					}
				}
			}
		}
		
		private function moveParticles():void
		{
			for (var i = 0; i < theParticles.length; i++)
			{
				theParticles[i].x += theParticles[i].velocity.x;
				theParticles[i].y += theParticles[i].velocity.y;
				
				if(gravityButton.selected){
					theParticles[i].applyForce(gravity);
				}
				
				var globalPosition:Point = localToGlobal(new Point(theParticles[i].x, theParticles[i].y));
				
				if (globalPosition.x > 500)
				{
					theParticles[i].velocity.x *= -0.5;
					theParticles[i].x = 500;
				} else if (globalPosition.x < 0)
				{
					theParticles[i].velocity.x *= -0.5;
					theParticles[i].x = 0;
				}
				
				if (globalPosition.y > 400)
				{
					//theParticles[i].applyForce(new Vector2D(0, -5));
					theParticles[i].velocity.y *= -0.5;
					theParticles[i].y = 400;
				} else if (globalPosition.y < 0)
				{
					theParticles[i].velocity.y *= -0.5;
					theParticles[i].y = 0;
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