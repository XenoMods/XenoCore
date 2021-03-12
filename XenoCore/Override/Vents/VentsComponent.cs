using System.Collections.Generic;
using XenoCore.Override.Map.Components;

namespace XenoCore.Override.Vents {
	public class VentComponent : PseudoComponent {
		public string[] Groups;
		public string Skin;

		public override void Awake() {
			var Vent = CustomVentsController.Add(Skin, Groups);
			Vent.Position = Position;
		}
	}

	public class VentComponentBuilder : PseudoComponentBuilder {
		public override string TypeId => "vent";
		
		public override PseudoComponent Build(Dictionary<string, string> Options) {
			return new VentComponent {
				Groups = Options["group"].Split(','),
				Skin = Options["skin"],
			};
		}
	}
}