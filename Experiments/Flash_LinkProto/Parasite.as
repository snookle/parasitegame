package  
{
	import flash.display.MovieClip;
	import flash.display.Sprite;
	import flash.geom.Point;
	
	import flash.events.KeyboardEvent;
	import flash.events.MouseEvent;
	import flash.events.Event;
	
	import flash.ui.Keyboard;
	
	/**
	* ...
	* @author Anthony Massingham
	*/
	public class Parasite extends MovieClip
	{
		// Parasite Part Variables
		private var theHead:ParasiteHead;									// The Head
		private var theBodyParts:Array = new Array();						// The Body Parts
		private var theTail:ParasiteTail;									// The Tail
		
		private var springLength:Number = 0.05;
		private var springConstant:Number = 10000;
		private var springFrictionConstant:Number = 0.2;
		private var airFrictionConstant:Number = 0.02;
		private var gravitation:Vector2D = new Vector2D(0, 9.81);
		private var groundFrictionConstant:Number = 0.2;
		private var groundAbsorptionConstant:Number = 2;
		private var groundRepulsionConstant:Number = 100;
		
		private var groundlevel:Number = 300;
		
		private var ropeConnectionPos:Vector2D = new Vector2D(0, 0);
		private var ropeConnectionVel:Vector2D = new Vector2D(0, 0);
		
		private var springs:Array = new Array();
		
		private var theParasite:Array = new Array();
		
		// Movement Variables
		private var leftMovement:Boolean = false;
		private var rightMovement:Boolean = false;
		
		public function Parasite() 
		{
			
			/**
			 * Parasite made up of 4 parts.
			 * -Head
			 * -BPart01
			 * -BPart02
			 * -BPart03
			 * -Tail
			 */
			
			createParasite(3);
		}
		
		public function createParasite(numParts:int):void
		{
			// Creating the Parasite : Draw Order is Important ( this will be different in C#, as it's not creation order, but render order )
			
			// Create the Tail
			var tailSprite:Sprite = new Sprite();
			tailSprite.graphics.clear();
			tailSprite.graphics.beginFill(0xC6B7A8, 0.5);
			tailSprite.graphics.drawCircle(0, 0, 10);
			tailSprite.graphics.endFill();
			tailSprite.graphics.lineStyle(0, 0, 0.2);
			tailSprite.graphics.moveTo(0, 0);
			tailSprite.graphics.lineTo(10, 0);
			
			theTail = new ParasiteTail(tailSprite, 0.05);
			this.addChild(theTail);
						
			// Create the Body Parts
			for (var i = 0; i < numParts; i++)
			{
				var bodySprite:Sprite = new Sprite();
				bodySprite.graphics.clear();
				bodySprite.graphics.beginFill(0x92B189, 0.5);
				bodySprite.graphics.drawCircle(0, 0, 10);
				bodySprite.graphics.endFill();
				bodySprite.graphics.lineStyle(0, 0, 0.2);
				bodySprite.graphics.moveTo(0, 0);
				bodySprite.graphics.lineTo(10, 0);
				
				var theBPart:ParasiteBodyPart = new ParasiteBodyPart(bodySprite, 0.05);
				theBodyParts.push(theBPart);
				this.addChild(theBPart);
			}
			
			// Create the Head
			var headSprite:Sprite = new Sprite();
			headSprite.graphics.clear();
			headSprite.graphics.beginFill(0x6A9160, 0.5);
			headSprite.graphics.drawCircle(0, 0, 10);
			headSprite.graphics.endFill();
			headSprite.graphics.lineStyle(0, 0, 0.2);
			headSprite.graphics.moveTo(0, 0);
			headSprite.graphics.lineTo(10, 0);
			
			theHead = new ParasiteHead(headSprite, 0.05);
			this.addChild(theHead);
			
			theParasite.push(theHead);
			for (i = 0; i < theBodyParts.length; i++)
			{
				theParasite.push(theBodyParts[i]);
			}
			theParasite.push(theTail);
			
			initParasitePosition();
		}
		
		/**
		 * This has to be a seprate method, otherwise the stage. variable doesn't get recognised.
		 */
		public function initParasite():void
		{
			initParasiteMovement();
		}
		
		public function initParasiteMovement():void
		{
			// Add Keyboard Listeners.
			// Have to add them to the stage, or the keys wont get noticed.
			stage.addEventListener(KeyboardEvent.KEY_UP, keyReleased);
			stage.addEventListener(KeyboardEvent.KEY_DOWN, keyPressed);
			
			this.addEventListener(Event.ENTER_FRAME, update);
		}
		
		// Initialises the position of each of the parasite segments
		private function initParasitePosition():void
		{
			for (var i = 0; i < theParasite.length; i++)
			{
				theParasite[i].x = i * springLength;
				theParasite[i].y = 0;
			}
			
			// create springs
			for (i = 0; i < theParasite.length - 1; i++)
			{
				springs.push(new Spring(theParasite[i], theParasite[i + 1], springConstant, springLength, springFrictionConstant));
			}
		}
		
		public function solve():void
		{
			// Apply force to all springs 
			for (var i = 0; i < springs.length; i++)
			{
				springs[i].solve();			// Apply Force
			}
			
			/*for (i = 0; i < theParasite.length; i++)
			{
				//var tempPoint:Point = theParasite[i].localToGlobal(theParasite[i].x, theParasite[i].y);
				//var globalPos:Vector2D = new Vector2D(tempPoint.x, tempPoint.y);
				
				// Apply Gravity
				//theParasite[i].applyForce(gravitation.multiply(theParasite[i].weight));
				// Air Friction
				//theParasite[i].applyForce(theParasite[i].velocity.multiply( -airFrictionConstant));
				// trace(theParasite[i].y);
				if (theParasite[i].y > groundlevel)
				{
					var v:Vector2D = new Vector2D(0, theParasite[i].velocity.y);
					
					trace(v.y);
					
					if (v.y > 0)
					{
						theParasite[i].applyForce(v.multiply(groundAbsorptionConstant));
					}
					
					var force:Vector2D = new Vector2D(0,groundRepulsionConstant).multiply(groundlevel - theParasite[i].y);
				}
			}*/
		}
		
		private function initParts():void
		{
			for (var i = 0; i < theParasite.length; i++)
			{
				theParasite[i].init();
			}
		}
		
		public function update(event:Event):void
		{
			initParts();
			solve();
			for (var i = 0; i < theParasite.length; i++)
			{
				theParasite[i].updatePoint();
			}
			// Order is important...
			
			/*ropeConnectionPos.addTo(ropeConnectionVel);
			if (ropeConnectionPos.y < 500)
			{
				ropeConnectionPos.y = 500;
				ropeConnectionVel.y = 0;
			}*/
			
			//ropeConnectionPos.x = mouseX;
			//ropeConnectionPos.y = mouseY;
			
			//theParasite[0].x = ropeConnectionPos.x;
			//theParasite[0].y = ropeConnectionPos.y;
			//theParasite[0].velocity.set(ropeConnectionVel.x,ropeConnectionVel.y);
			
			// * Update Keyboard Movement
			if (leftMovement)
			{
				theHead.velocity.x -= 0.5;
			} else if (rightMovement)
			{
				theHead.velocity.x += 0.5;
			}
			
			
			
			// * Update Velocities
			//theHead.updatePoint();
			//for each (var bodyPart:ParasiteBodyPart in theBodyParts)
			//{
			//	bodyPart.updatePoint();
			//}
			//theTail.updatePoint();
		}
		
		/**
		 * MOVEMENT METHODS
		 * Unsure if this should be in the parasite class, or in the stage class ? 
		 */
		
		// Keyboard Movement
		private function keyPressed(event:KeyboardEvent):void
		{
			if (event.keyCode == Keyboard.LEFT)
			{
				rightMovement = false;
				leftMovement = true;
				theHead.sprite.rotation = 0;
			} else if (event.keyCode == Keyboard.RIGHT)
			{
				leftMovement = false;
				rightMovement = true;
				theHead.sprite.rotation = 180;
			}
		}
		
		private function keyReleased(event:KeyboardEvent):void
		{
			leftMovement = false;
			rightMovement = false;
		}
		
		// Mouse Movement
		private function mouseDown(event:MouseEvent):void
		{
			
		}
		
		private function mouseUp(event:MouseEvent):void
		{
			
		}
		
	}
	
}