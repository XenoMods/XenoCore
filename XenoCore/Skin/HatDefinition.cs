using System.Collections.Generic;
using UnityEngine;
using XenoCore.Utils;

namespace XenoCore.Skin {
	public class HatDefinition {
		public readonly string Name;
		public readonly Vector2 ChipOffset;
		public readonly bool InFront;
		public readonly Sprite Main;
		public readonly Sprite Back;
		public readonly Sprite Floor;

		public HatDefinition(BundleDefinition Bundle, string Name, Vector2 ChipOffset, bool InFront = true) 
			: this(Bundle.Sprite(Name),
				Bundle.Sprite($"{Name}Back"),
				Bundle.Sprite($"{Name}Floor"),
				Name, ChipOffset, InFront) {
		}
		
		public HatDefinition(Sprite Main, Sprite Back, Sprite Floor, string Name, Vector2 ChipOffset,
			bool InFront = true) {
			this.Name = Name;
			this.ChipOffset = ChipOffset;
			this.InFront = InFront;
			this.Main = Main;
			this.Back = Back;
			this.Floor = Floor;
		}
	}

	public class HatDefinitionSimple : HatDefinition {
		public HatDefinitionSimple(BundleDefinition Bundle, string Name, Vector2 ChipOffset, bool InFront = true)
			: this(Bundle.Sprite(Name), Name, ChipOffset, InFront) {
		}

		public HatDefinitionSimple(Sprite Main, string Name, Vector2 ChipOffset, bool InFront = true) 
			: base(Main, Main, Main, Name, ChipOffset, InFront) {
		}
	}
	
	public static class HatsController {
		private static readonly List<HatDefinition> Definitions = new List<HatDefinition>();

		public static void Add(HatDefinition Definition) {
			Definitions.Add(Definition);
		}

		internal static void Inject() {
			foreach (var Definition in Definitions) {
				Inject(Definition);
			}
		}
		
		private static void Inject(HatDefinition HatDefinition) {
			var Manager = HatManager.Instance;
			var Hat = ScriptableObject.CreateInstance<HatBehaviour>();
			Hat.Free = true;
			Hat.NotInStore = true;
			Hat.ProductId = $"xeno_hat_{HatDefinition.Name.ToLowerInvariant()}";
			Hat.StoreName = $"Xeno Hat {HatDefinition.Name}";
			
			Hat.MainImage = HatDefinition.Main;
			Hat.FloorImage = HatDefinition.Floor;
			Hat.BackImage = HatDefinition.Back;
			Hat.ChipOffset = HatDefinition.ChipOffset;
			Hat.AltShader = null;
			Hat.RelatedSkin = null;
			Hat.InFront = HatDefinition.InFront;

			Manager.AllHats.Add(Hat);
		}
	}
}