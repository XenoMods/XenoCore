using UnityEngine;

namespace XenoCore.Utils {
	public static class Globals {
		public const string PROCESS = "Among Us.exe";
		
		public static readonly int VISOR_COLOR = Shader.PropertyToID("_VisorColor");
		public static readonly int BACK_COLOR = Shader.PropertyToID("_BackColor");
		public static readonly int BODY_COLOR = Shader.PropertyToID("_BodyColor");
		public static readonly int OUTLINE = Shader.PropertyToID("_Outline");
		public static readonly int OUTLINE_COLOR = Shader.PropertyToID("_OutlineColor");
		public static readonly int DESAT = Shader.PropertyToID("_Desat");
		public static readonly int PERCENT = Shader.PropertyToID("_Percent");
		public static readonly int MASK = Shader.PropertyToID("_Mask");
		public static readonly int ADD_COLOR = Shader.PropertyToID("_AddColor");

		public static readonly byte MAP_SKELD = 0;
		public static readonly byte MAP_MIRA = 1;
		public static readonly byte MAP_POLUS = 2;
			
		public static readonly string FORMAT_WHITE = "[ffffffff]";
	}
}