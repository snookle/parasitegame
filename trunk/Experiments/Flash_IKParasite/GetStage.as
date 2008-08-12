package {
	
	import flash.display.*;
	
	public class GetStage{
		
		private static var _stage:Stage
		
		public function GetStage(){
		}
		
		public static function get stage():Stage{
			return _stage
		}
		public static function set stage(s:Stage):void{
			_stage=s
		}
		
	}
}