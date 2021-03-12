using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using XenoCore.Locale;
using XenoCore.Override.Map.Components;
using XenoCore.Utils;
using NotImplementedException = System.NotImplementedException;

namespace XenoCore.Override.Map {
	public class CustomMapType {
		public static readonly string NONE_MAP = XenoLang.MAPS_NONE.Id;
		
		public string Name;
		public string Key => $"xeno.maps.impl.{Name}";

		public readonly Dictionary<SystemTypes, ISystemType> Systems
			= new Dictionary<SystemTypes, ISystemType>();

		public void AddSystem(SystemTypes Type, ISystemType IType) {
			Systems.Add(Type, IType);
		}

		public void AddSystem(SystemTypes Type, Il2CppSystem.Object IType) {
			Systems.Add(Type, IType.Cast<ISystemType>());
		}
		
		public Color BackgroundClor = Color.black;

		public float CameraShakeAmount; // = 0.02f;
		public float CameraShakePeriod; // = 0.3f;

		public Sprite MiniMap;
		public float MapScale = 1f;
		public Vector2 MapOffset = Vector2.zero;
		
		public GameObject RuntimeMap;
		public GameObject MapPrefab;

		public readonly List<PseudoComponent> Components = new List<PseudoComponent>();
		public readonly List<SpawnComponent> SpawnPoints = new List<SpawnComponent>();

		public IEnumerable<T> FindComponents<T>() where T : PseudoComponent {
			return Components.Where(Component => Component is T).Cast<T>();
		}
		
		internal void RecalculateRuntime() {
			Components.Clear();
			
			var AllComponents = RuntimeMap.transform.FindByTypeRecursive<Text>();
			foreach (var Component in AllComponents) {
				Components.Add(PseudoComponentsRegistry.Build(Component));
				Object.Destroy(Component);
			}
			
			SpawnPoints.Clear();
			SpawnPoints.AddRange(FindComponents<SpawnComponent>());
		}

		internal void ComponentsAwake() {
			foreach (var Component in Components) {
				Component.Awake();
			}
		}
	}
}