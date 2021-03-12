using System.Collections.Generic;
using PowerTools;
using UnityEngine;
using XenoCore.Utils;

namespace XenoCore.Skin {
	public class PetDefinition {
		public readonly string Name;
		public readonly float YOffset;
		public readonly GameObject Prefab;
		public readonly AnimationClip Idle;
		public readonly AnimationClip Sad;
		public readonly AnimationClip Scared;
		public readonly AnimationClip Walk;

		public PetDefinition(BundleDefinition Bundle, string Name, float YOffset = -0.1f)
			: this(Bundle.Object(Name),
				Bundle.Animation($"{Name}Idle"),
				Bundle.Animation($"{Name}Sad"),
				Bundle.Animation($"{Name}Scared"),
				Bundle.Animation($"{Name}Walk"),
				Name, YOffset) {
		}
		
		public PetDefinition(GameObject Prefab, AnimationClip Idle, AnimationClip Sad,
			AnimationClip Scared, AnimationClip Walk, string Name, float YOffset = -0.1f) {
			this.Name = Name;
			this.YOffset = YOffset;
			this.Prefab = Prefab;
			this.Idle = Idle;
			this.Sad = Sad;
			this.Scared = Scared;
			this.Walk = Walk;
		}
	}
	
	public static class PetsController {
		private static readonly List<PetDefinition> Definitions = new List<PetDefinition>();

		public static void Add(PetDefinition Definition) {
			Definitions.Add(Definition);
		}

		internal static void Inject() {
			foreach (var Definition in Definitions) {
				Inject(Definition);
			}
		}
		
		private static void Inject(PetDefinition PetDefinition) {
			var Manager = HatManager.Instance;
			
			var Pet = PetDefinition.Prefab;
			var Controller = Pet.AddComponent<PetBehaviour>();
			Controller.Free = true;
			Controller.NotInStore = true;
			Controller.ProductId = $"xeno_pet_{PetDefinition.Name.ToLowerInvariant()}";
			Controller.StoreName = $"Xeno Pet {PetDefinition.Name}";
			
			Controller.body = Pet.GetComponent<Rigidbody2D>();
			Controller.Collider = Pet.GetComponent<Collider2D>();
			Controller.idleClip = PetDefinition.Idle;
			Controller.sadClip = PetDefinition.Sad;
			Controller.scaredClip = PetDefinition.Scared;
			Controller.walkClip = PetDefinition.Walk;
			Controller.rend = Pet.GetComponent<SpriteRenderer>();

			var Shadow = Pet.transform.GetChild(0).gameObject;
			Controller.shadowRend = Shadow.GetComponent<SpriteRenderer>();

			Controller.animator = Pet.AddComponent<SpriteAnim>();
			Controller.Visible = true;
			Controller.YOffset = PetDefinition.YOffset;

			Pet.layer = Shadow.layer = 8;

			Manager.AllPets.Add(Controller);
		}
	}
}