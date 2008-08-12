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
		
		private var theParasite:Array = new Array();
		
		// Movement Variables
		private var leftMovement:Boolean = false;
		private var rightMovement:Boolean = false;
		
		private var tailMoving:Boolean = false;
		
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
			var headIK:IKMember = new IKMember(theHead);
			var lastIK = headIK;
			theHead.addIKPoint(headIK);
			
			for (i = 0; i < theBodyParts.length; i++)
			{
				theParasite.push(theBodyParts[i]);
				var ik:IKMember = new IKMember(theBodyParts[i]);
				
				if (i != 0)
				{
					// To Ensure that Dragging does not affect the Head
					ik.addNeighbor(lastIK);	
				}
				lastIK.addNeighbor(ik);
				
				theBodyParts[i].addIKPoint(ik);
				
				lastIK = ik;
			}
			theParasite.push(theTail);
			
			var tailIK:IKMember = new IKMember(theTail);
			tailIK.addNeighbor(lastIK);
			lastIK.addNeighbor(tailIK);
			
			theTail.addIKPoint(tailIK);
			theTail.initTail();
			
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
				theParasite[i].x = 400+i * 20;
				theParasite[i].y = 300+0;
			}
		}
	
		
		public function update(event:Event):void
		{
			
			// * Update Keyboard Movement
			if (leftMovement)
			{
				theHead.velocity.x -= 0.5;
			} else if (rightMovement)
			{
				theHead.velocity.x += 0.5;
			}
			
			//theHead.updatePoint();
			for (var i = 0; i < theParasite.length; i++)
			{
				theParasite[i].init();
				var gravitation:Vector2D = new Vector2D(0, 9.8);
				if (!theTail.isMoving)
				{
					theParasite[i].applyForce(gravitation);	
				}
				theParasite[i].updatePoint();
			}
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