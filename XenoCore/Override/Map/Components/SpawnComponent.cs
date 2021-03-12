using System.Collections.Generic;

namespace XenoCore.Override.Map.Components {
	public class SpawnComponent : PseudoComponent {
	}

	public class SpawnComponentBuilder : PseudoComponentBuilder {
		public override string TypeId => "spawn";
		
		public override PseudoComponent Build(Dictionary<string, string> Options) {
			return new SpawnComponent();
		}
	}
}