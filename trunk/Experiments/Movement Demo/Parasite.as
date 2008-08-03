package  
{
	import flash.display.MovieClip;
	
	// Key logging
	import flash.events.*;
	import flash.ui.Keyboard;
	
	/**
	* PARASITE
	* Test Movement Class for Parasite.   To be used with 'Movement_Demo.fla' and eventually with 'Artery Adventure'
	* 
	* Possible Extensions : Dynamic Shading of Body Parts
	* 
	* @author Anthony Massingham
	*/
	public class Parasite extends MovieClip
	{
		
		private var theHead:ParasiteHead;									// The Head
		private var theBodyParts:Array = new Array();						// The Body Parts
		private var theTail:ParasiteTail;									// The Tail
		
		// Movement Variables
		
		private var leftMovement:Boolean = false;
		private var rightMovement:Boolean = false;
		
		private var movementCap:Number = 5;
		private var movementAmount:Number = 0;
		private var catchUp:Number = 5;
		
		// Direction
		private var direction:String = "LEFT";
		
		// Parsaite 
		public function Parasite() 
		{
			/**
			 * 
			 * Parasite made up of 4 parts.
			 * -Head ( Which in turn = Head, Hat, Eyes )
			 * -BPart01
			 * -BPart02
			 * -BPart03
			 * -Tail
			 * 
			 * For the Purposes of this demo, only One Body part will be used.
			 * 
			 */
			
			createParasite(3);
			
			this.x = 200;
			this.y = 200;
		}
		
		public function initParasite():void
		{
			initParasiteMovement();
		}
		
		// Creates a parasite of Head, X parts, and Tail
		private function createParasite(numParts:Number):void
		{			
			// Create the Tail
			theTail = new ParasiteTail;
			this.addChild(theTail);
						
			// Create the Body Parts
			for (var i = 0; i < numParts; i++)
			{
				var theBPart:ParasiteBPart = new ParasiteBPart;
				theBodyParts.push(theBPart);
				this.addChild(theBPart);
			}
			
			// Create the Head
			theHead = new ParasiteHead();
			this.addChild(theHead);
			
			initParasitePosition();
		}
		
		// Initialises the position of each of the parasite segments
		private function initParasitePosition():void
		{
			// Move the first part to it's correct location
			theBodyParts[0].x = theHead.x + theHead.theWeight;
			theBodyParts[0].y = theHead.y;
			
			// Do the rest for the remaining body parts
			for (var i = 1; i < theBodyParts.length; i++)
			{
				theBodyParts[i].x = theBodyParts[i-1].x + theBodyParts[i-1].theWeight;
				theBodyParts[i].y = theBodyParts[i-1].y;
			}
			
			// Finally, Position the Tail
			theTail.x = theBodyParts[theBodyParts.length-1].x + theBodyParts[theBodyParts.length-1].theWeight;
			theTail.y = theBodyParts[theBodyParts.length-1].y;
		}
		
		// Initialises all the movement methods for the parasite
		private function initParasiteMovement():void
		{
			// Add Keyboard Listeners.
			// Have to add them to the stage, or the keys wont get noticed.
			stage.addEventListener(KeyboardEvent.KEY_UP, keyReleased);
			stage.addEventListener(KeyboardEvent.KEY_DOWN, keyPressed);
			
			this.addEventListener(Event.ENTER_FRAME, update);
		}
		
		private function update(event:Event):void
		{
			if (leftMovement)
			{
				theHead.xVel = -1*movementAmount;
			} else if (rightMovement)
			{
				theHead.xVel = movementAmount;
			}
			
			if (movementAmount > movementCap)
			{
				movementAmount = 0;
			} else {
				movementAmount += 0.25;
			}
			
			if(direction == "LEFT"){
				theBodyParts[0].targetX = theHead.x + theHead.theWeight;
			} else {
				theBodyParts[0].targetX = theHead.x - theHead.theWeight;
			}
			
			for (var i = 1; i < theBodyParts.length; i++)
			{
				// update body parts
				if(direction == "LEFT"){
					theBodyParts[i].targetX = theBodyParts[i - 1].x + theBodyParts[i - 1].theWeight;
				} else {
					theBodyParts[i].targetX = theBodyParts[i - 1].x - theBodyParts[i - 1].theWeight;
				}
			}
			
			if (direction == "LEFT")
			{
				theTail.targetX = theBodyParts[theBodyParts.length-1].x + theBodyParts[theBodyParts.length-1].theWeight;			
			} else {
				theTail.targetX = theBodyParts[theBodyParts.length-1].x - theBodyParts[theBodyParts.length-1].theWeight;			
			}
			
		}
		
		private function keyPressed(event:KeyboardEvent):void
		{
			if (event.keyCode == Keyboard.LEFT)
			{
				if (direction == "RIGHT")
				{
					theHead.flip();
					for (var i = 0; i < theBodyParts.length; i++)
					{
						theBodyParts[i].flip();
						movementAmount = 0;
					}
					theTail.flip();
				}
				
				direction = "LEFT";
				leftMovement = true;
			} else if (event.keyCode == Keyboard.RIGHT)
			{
				if (direction == "LEFT")
				{
					theHead.flip();
					for (i = 0; i < theBodyParts.length; i++)
					{
						theBodyParts[i].flip();
						movementAmount = 0;
					}
					theTail.flip();
				}
				direction = "RIGHT";
				rightMovement = true;
			}
		}
		
		private function keyReleased(event:KeyboardEvent):void
		{
			leftMovement = false;
			rightMovement = false;
		}
	}
	
}