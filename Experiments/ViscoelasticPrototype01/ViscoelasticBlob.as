package  
{
	import flash.display.MovieClip;
	import flash.events.Event;
	import flash.geom.Point;
	
	/**
	* Test Class for Viscoelastic Particles
	* @author Anthony Massingham & Ben Kenny
	*/
	public class ViscoelasticBlob extends MovieClip
	{
		public var numParticles:Number;
		public var theParticles:Array;
		public var particleSize:Number;
		
		public var springs:Array;
		public var connected:Array;
		
		// Spatial Grid
		public var theGrid:SpatialGrid;
		
		// Forces
		public var gravity:Vector2D = new Vector2D(0, 0.5);
		
		// Variables
		public var threshold:Number = 20;
		public var restDensity:Number = 10;
		public var stiffness:Number = 0.004;
		public var nearStiffness:Number = 0.01;
		public var springStiffness:Number = 0.3;
		public var plasticityConstant:Number = 0.3;
		public var friction:Number = 1;
		public var unknownVariableO:Number = 0.2;
		public var unknownVariableB:Number = 0.05;
		
		public var yeildRatio:Number = 0.2;
		public var restLengthConstant:Number = 10;
		
		public function ViscoelasticBlob():void
		{
			initSimulation();
			
			addEventListener(Event.ENTER_FRAME, doSimulation);			
		}
		
		public function initSimulation():void {
			theParticles = new Array();
			springs = new Array();
			connected = new Array();
			numParticles = 50;
			particleSize = 5;
			
			theGrid = new SpatialGrid(500, 400, threshold);
			addChild(theGrid);
			theGrid.alpha = 0.5;
			
			// Create Particles
			for (var i = 0; i < numParticles; i++)
			{
				var theParticle:Particle = new Particle(100,100,particleSize);
				theParticles.push(theParticle);
				
				this.addChild(theParticle);
				
				connected.push(new Array());
				springs.push(new Array());
				
				theParticle.x += Math.random() * 300;
				theParticle.x += 50;
				
				theParticle.y += Math.random() * 200;
				theParticle.y += 50;
			}
		}
		
		/**
		 * ALGORITHM 1
		 * @param	event
		 */
		public function doSimulation(event:Event) {
			//theGrid = new SpatialGrid(500, 400, threshold);
			//theGrid.drawGrid();
			
			for (var i = 0; i < theParticles.length; i++) {
				// Apply Gravity
				theParticles[i].applyForce(gravity);
			}
			
			applyViscosity();
			
			for (i = 0; i < theParticles.length; i++) {
				theGrid.RemoveParticle(theParticles[i]);
				// Save Previous Pos				
				theParticles[i].prevPos = new Vector2D(theParticles[i].x, theParticles[i].y);
				
				// Move to Predicted Position
				theParticles[i].x += theParticles[i].velocity.x;
				theParticles[i].y += theParticles[i].velocity.y;
				
				theParticles[i].position = new Vector2D(theParticles[i].x, theParticles[i].y);
				
				// Spatial Grid :
				theGrid.AddParticle(theParticles[i]);
			}
			theGrid.showCurrentParticles();
			
			adjustSprings();
			applySpringDisplacements();
			doubleDensityRelaxation();
			resolveCollisions();
			
			for (i = 0; i < theParticles.length; i++) {			
				theParticles[i].velocity = (theParticles[i].position.subtract(theParticles[i].prevPos));
			}			
		}
		
		/**
		 * ALGORITHM 5
		 */
		public function applyViscosity():void {
			for (var i = 0; i < theParticles.length; i++) {
				theParticles[i].position = new Vector2D(theParticles[i].x, theParticles[i].y);
				var theNeighbours:Array = theGrid.GetNeighbors(theParticles[i]);
				for (var j = 0; j < theNeighbours.length; j++) {
					theNeighbours[j].position = new Vector2D(theNeighbours[j].x, theNeighbours[j].y);
					var r:Vector2D = theNeighbours[j].position.subtract(theParticles[i].position);
					var theDistance:Number = r.length();					
					var unitR:Vector2D = r.getUnitVector();

					var q:Number = theDistance / threshold;
					
					if (q < 1) {
						var u:Number = unitR.scalarVectorProduct(theNeighbours[j].velocity.subtract(theParticles[i].velocity));
						if (u > 0) {
							//Linear and Quadratic Impulses ... or something
							var impulseValue:Number = (1 - q) * (unknownVariableO * u + unknownVariableB * Math.pow(u, 2));
							var impulse:Vector2D = unitR.multiply(impulseValue);
							
							theParticles[i].velocity.subtract(impulse.divide(2));
							theNeighbours[j].velocity.add(impulse.divide(2));
						}
					}
				}
			}
		}
		
		/**
		 * ALGORITHM 4
		 */
		public function adjustSprings():void {			
			for (var i = 0; i < theParticles.length; i++) {
				theParticles[i].position = new Vector2D(theParticles[i].x, theParticles[i].y);
				var theNeighbours:Array = theGrid.GetNeighbors(theParticles[i]);
				
				//trace("The Particle " + i + " has " + theNeighbours.length + " neighbours");
				for (var j = 0; j < theNeighbours.length; j++) {
					theNeighbours[j].position = new Vector2D(theNeighbours[j].x, theNeighbours[j].y);
					var r:Vector2D = theNeighbours[j].position.subtract(theParticles[i].position);
					var theDistance:Number = r.length();
					
					//trace(theDistance);
					
					var q:Number = theDistance / threshold;
					
					if (q < 1) {
						if (springs[i][j] == null) {
							// create spring
							//trace("Creating Spring");
							var newSpring:Spring = new Spring(threshold);
							springs[i][j] = newSpring;
						}
					}
					
					if (springs[i][j] != null) {
						var deformation:Number = yeildRatio * springs[i][j].springLength;
						if (theDistance > restLengthConstant + deformation) {
							//// Stretch
							//trace("S");
							//trace(springs[i][j].springLength);
							springs[i][j].springLength = springs[i][j].springLength + plasticityConstant * (theDistance - restLengthConstant - deformation);
							//trace(springs[i][j].springLength);
							//trace();
						} else if (theDistance < restLengthConstant - deformation) {
							// Compress
							//trace("C");
							//trace(springs[i][j].springLength);
							springs[i][j].springLength = springs[i][j].springLength - plasticityConstant * (restLengthConstant - deformation - theDistance);
							//trace(springs[i][j].springLength);
							//trace();
						}
					}
					
				}
			}
			
			// For each spring, check to see if it needs removing
			for (i = 0; i < theParticles.length; i++) {
				theNeighbours = theGrid.GetNeighbors(theParticles[i]);
				for (j = 0; j < theNeighbours.length; j++) {
					if (springs[i][j] != null) {
						if (springs[i][j].springLength > threshold) {
							//trace("removing Spring");
							springs[i][j] = null;
							springs[i].splice(j, 1);
							//trace(springs[i].length+" springs left");
						}
					}
				}
			}
			
		}
		
		/**
		 * ALGORITHM 3
		 */
		public function applySpringDisplacements():void {
			for (var i = 0; i < theParticles.length; i++) {
				theParticles[i].position = new Vector2D(theParticles[i].x, theParticles[i].y);
				var theNeighbours:Array = theGrid.GetNeighbors(theParticles[i]);
				for (var j = 0; j < theNeighbours.length; j++) {
					theNeighbours[j].position = new Vector2D(theNeighbours[j].x, theNeighbours[j].y);
					var r:Vector2D = theNeighbours[j].position.subtract(theParticles[i].position);
					var theDistance:Number = r.length();	
					var unitR:Vector2D = r.getUnitVector();
				
					if (springs[i][j] != null) {						
						var displacementValue:Number = springStiffness * (1 - springs[i][j].springLength / threshold) * (springs[i][j].springLength - theDistance);
						var displacement:Vector2D = unitR.multiply(displacementValue);
						
						theParticles[i].position = new Vector2D(theParticles[i].x, theParticles[i].y);
						theParticles[i].position = theParticles[i].position.subtract(displacement.divide(2));

						theParticles[i].x = theParticles[i].position.x;
						theParticles[i].y = theParticles[i].position.y;
						
						theNeighbours[j].position = new Vector2D(theNeighbours[j].x, theNeighbours[j].y);
						theNeighbours[j].position = theNeighbours[j].position.add(displacement.divide(2))
						
						theNeighbours[j].x = theNeighbours[j].position.x;
						theNeighbours[j].y = theNeighbours[j].position.y;
					}
				}
			}
		}
		
		/**
		 * ALGORITHM 2
		 */
		public function doubleDensityRelaxation():void {
			for (var i = 0; i < theParticles.length; i++) {
				var density:Number = 0;
				var nearDensity:Number = 0;
				
				var pPosV:Vector2D = new Vector2D(theParticles[i].x, theParticles[i].y);
				
				var theNeighbours:Array = theGrid.GetNeighbors(theParticles[i]);
				//trace("Particle "+i+" has "+theNeighbours.length+" neighbors");
				for (var j = 0; j < theNeighbours.length; j++) {
					var nPosV:Vector2D = new Vector2D(theNeighbours[j].x,theNeighbours[j].y);

					var r:Vector2D = nPosV.subtract(pPosV);
					var theDistance:Number = r.length();					
					
					var q:Number = theDistance / threshold;
					
					if (q < 1) {
						density += Math.pow((1 - q), 2);
						nearDensity += Math.pow((1 - q),3);
					}
				}
				
				// Compute Pressure and Near Pressure
				var pressure:Number = stiffness * (density - nearDensity);
				var nearPressure:Number = nearStiffness * nearDensity;
				var dx:Vector2D = new Vector2D(0, 0);
				
				for (j = 0; j < theNeighbours.length; j++) {
					nPosV = new Vector2D(theNeighbours[j].x,theNeighbours[j].y);
					
					r = nPosV.subtract(pPosV);
					var unitR:Vector2D = r.getUnitVector();
					theDistance = r.length();
					
					q = theDistance / threshold;
					if (q < 1) {
						// Apply Displacements
						var displacementValue:Number = (pressure * (1 - q)) + (nearPressure * (1 - q))
						var displacement:Vector2D = unitR.multiply(displacementValue);

						theNeighbours[j].position = new Vector2D(theNeighbours[j].x, theNeighbours[j].y);
						theNeighbours[j].position = theNeighbours[j].position.add(displacement.divide(2));
						
						theNeighbours[j].x = theNeighbours[j].position.x;
						theNeighbours[j].y = theNeighbours[j].position.y;
						
						dx.subtract(displacement.divide(2));
					}
				}
				theParticles[i].position = new Vector2D(theParticles[i].x, theParticles[i].y);
				theParticles[i].position = theParticles[i].position.add(dx);
				
				theParticles[i].x = theParticles[i].position.x;
				theParticles[i].y = theParticles[i].position.y;
			}
		}
		
		/**
		 * Simple Collision Detection
		 */
		public function resolveCollisions():void {
			
			for (var i = 0; i < theParticles.length; i++) {
				// For now, simple boundary detection :				
				/*if (theParticles[i].x > 499)
				{
					theParticles[i].velocity.x *= -0.5;
					theParticles[i].x = 499;
				} else if (theParticles[i].x < 0)
				{
					theParticles[i].velocity.x *= -0.5;
					theParticles[i].x = 0;
				}
				
				if (theParticles[i].y > 399)
				{
					//theParticles[i].applyForce(new Vector2D(0, -5));
					theParticles[i].velocity.y *= -0.5;
					theParticles[i].y = 399;
				} else if (theParticles[i].y < 0)
				{
					theParticles[i].velocity.y *= -0.5;
					theParticles[i].y = 0;
				}*/
				
				//theParticles[i].x += theParticles[i].velocity.x;
				//theParticles[i].y += theParticles[i].velocity.y;
				//theParticles[i].position = new Vector2D(theParticles[i].x, theParticles[i].y);
				
				if (theParticles[i].y > 399) {
					var nVector:Vector2D = new Vector2D(0, 1);
					var vNormal:Vector2D = nVector.multiply((theParticles[i].velocity.scalarVectorProduct(nVector)));
					var vTangent:Vector2D = theParticles[i].velocity.subtract(vNormal);
					var impulse:Vector2D = vNormal.subtract(vTangent.multiply(friction));
					
					// Apply 
					theParticles[i].x -= impulse.x;
					theParticles[i].y -= impulse.y;
					
					//trace(impulse.y);
					
					if (theParticles[i].y > 399) {
						theParticles[i].y = 399;
					}
				}
				
				theParticles[i].position = new Vector2D(theParticles[i].x, theParticles[i].y);
			}
			
			
		}
		
	}
	
}