package  
{
	import flash.geom.Point;
	import flash.text.GridFitType;
	import flash.display.MovieClip
	import flash.events.Event;
	
	/**
	* Spatial Grid Class based on Ben's C# code.
	* @author Anthony Massingham & Ben Kenny
	*/
	public class SpatialGrid extends MovieClip
	{
		private var grid:Array;
		
		private var theWidth:int;
		private var theHeight:int;
		private var gridSize:int;
		
		public function SpatialGrid(gridWidth:int, gridHeight:int, gridSize:int) 
		{
			this.theWidth = Math.floor(gridWidth/gridSize);
			this.theHeight = Math.floor(gridHeight/gridSize);
			this.gridSize = gridSize;
			
			grid = new Array();
			
			for (var i = 0; i < theWidth+1; i++) {
				grid[i] = new Array();
				for (var j = 0; j < theHeight+1; j++) {
					grid[i][j] = new Array();
				}
			}
			drawGrid();
			showCurrentParticles();
		}
		
		public function showCurrentParticles():void {
			this.graphics.clear();
			drawGrid();			
			for (var i = 0; i < theWidth+1; i++) {
				for (var j = 0; j < theHeight+1; j++) {
					if (grid[i][j].length > 1) {
						//trace("hit in :" + i + "," + j);
						this.graphics.lineStyle();
						this.graphics.beginFill(0xF1B4B4);
						this.graphics.drawRect(i * gridSize, j * gridSize, gridSize, gridSize);
						this.graphics.endFill();
					} else if(grid[i][j].length == 1){
						if (grid[i][j].length > 0) {
						//trace("hit in :" + i + "," + j);
						this.graphics.lineStyle();
						this.graphics.beginFill(0xBACDEB);
						this.graphics.drawRect(i * gridSize, j * gridSize, gridSize, gridSize);
						this.graphics.endFill();
					}
					}
				}
			}
		}
		
		public function drawGrid():void {
			this.graphics.lineStyle(0,0,0.1);
			for (var i = 1; i < theWidth; i++) {
				this.graphics.moveTo(gridSize*i, 0);
				this.graphics.lineTo(gridSize*i, theHeight * gridSize);
			}
			
			for (i = 1; i < theHeight; i++) {
				this.graphics.moveTo(0, gridSize*i);
				this.graphics.lineTo(theWidth*gridSize,gridSize*i);
			}		
		}
		
		public function RemoveParticle(theParticle:Particle):void {
			var x:int = Math.floor(theParticle.position.x / gridSize);
			var y:int = Math.floor(theParticle.position.y / gridSize);
			
			if (x < 0) {
				x = 0;
			} else if (x > theWidth) {
				x = theWidth;
			}
			
			if (y < 0) {
				y = 0;
			} else if (y > theHeight) {
				y = theHeight;
			}
			
			var particlePos:int = -1;
			
			for (var i = 0; i < grid[x][y].length; i++) {
				if (grid[x][y][i] == theParticle) {
					particlePos = i;
					grid[x][y].splice(i, 1);
					
					i = grid[x][y].length + 10;				
				}
			}
		}
		
		public function AddParticle(theParticle:Particle):void {			
			var x:int = int(Math.floor(theParticle.position.x / gridSize));
			var y:int = int(Math.floor(theParticle.position.y / gridSize));
			
			if (x < 0) {
				x = 0;
			} else if (x > theWidth) {
				x = theWidth;
			}
			
			if (y < 0) {
				y = 0;
			} else if (y > theHeight) {
				y = theHeight;
			}
			
			grid[x][y].push(theParticle);			
		}
		
		public function GetNeighbors(theParticle:Particle):Array {
			var x:int = int(Math.floor(theParticle.position.x / gridSize));
			var y:int = int(Math.floor(theParticle.position.y / gridSize));
			
			if (x < 0) {
				x = 0;
			} else if (x > theWidth) {
				x = theWidth;
			}
			
			if (y < 0) {
				y = 0;
			} else if (y > theHeight) {
				y = theHeight;
			}
			
			var returnList:Array = new Array();

			for (var i = x - (x == 0 ? 0 : 1); i < x + (x == theWidth ? 0 : 1); i++) {
				for (var j = y - (y == 0 ? 0 : 1); j < y + (y == theHeight ? 0 : 1); j++) {
					returnList = returnList.concat(grid[i][j]);
				}
			}
			
			for (i = 0; i < returnList.length; i++) {
				if (returnList[i] == theParticle) {					
					returnList.splice(i, 1);
				}
			}
			
			return returnList;
		}
		
	}
	
}