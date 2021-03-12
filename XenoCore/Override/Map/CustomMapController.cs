using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;
using XenoCore.CustomOptions;
using XenoCore.Events;
using XenoCore.Locale;
using XenoCore.Override.Vents;
using XenoCore.Utils;
using Object = UnityEngine.Object;

namespace XenoCore.Override.Map {
	public static class CustomMapController {
		private static readonly Dictionary<string, CustomMapType> Maps
			= new Dictionary<string, CustomMapType>();

		public static readonly CustomStringOption CustomMapOption = CustomOption
			.AddString(XenoLang.MAPS_CUSTOM.Id, true, CustomMapType.NONE_MAP);

		public static CustomMapType SelectedMap;

		internal static void Init() {
			CustomMapOption.Group = XenoPlugin.XENO_GROUP;
			Maps.Clear();
			EventsController.HOST_START_GAME.Register(() => {
				UpdateSelectedMap();

				if (SelectedMap == null) return;

				PlayerControl.GameOptions.MapId = 0;
				PlayerControl.LocalPlayer.RpcSyncSettings(PlayerControl.GameOptions);
			});
		}

		private static void UpdateSelectedMap() {
			var Key = CustomMapOption.GetText();
			SelectedMap = (Key == CustomMapType.NONE_MAP || Key == null)
				? null
				: Maps[Key];
		}

		public static void Register(CustomMapType MapType) {
			Maps.Add(MapType.Key, MapType);
			RegenerateOption();
		}

		public static void AddSpawnModifier(ISpawnLocationModifier Modifier) {
			SpawnPatch.AddModifier(Modifier);
		}

		private static void RegenerateOption() {
			var Names = new List<string> {CustomMapType.NONE_MAP};
			Names.AddRange(Maps.Keys);
			CustomMapOption.SetValues(Names.ToArray());
		}

		[HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.GetSpawnLocation))]
		public static class SpawnPatch {
			private static readonly List<ISpawnLocationModifier> LocationModifiers
				= new List<ISpawnLocationModifier>();

			internal static void AddModifier(ISpawnLocationModifier Modifier) {
				LocationModifiers.Add(Modifier);
			}

			private static Vector2 ApplyModifiers(Vector2 Source, PlayerControl Player) {
				return LocationModifiers.Aggregate(Source, (Current, Modifier)
					=> Modifier.Modify(Current, Player));
			}

			public static bool Prefix(ShipStatus __instance, ref Vector2 __result, int NHOCGFDHKKK) {
				if (SelectedMap == null) return true;

				var Player = PlayerTools.GetPlayerById((byte) NHOCGFDHKKK);
				if (Player == null) return true;

				__result = SelectedMap.SpawnPoints.RandomItem().Position;
				__result = ApplyModifiers(__result, Player);
				return false;
			}

			public static void Postfix(ShipStatus __instance, ref Vector2 __result, int NHOCGFDHKKK) {
				var Player = PlayerTools.GetPlayerById((byte) NHOCGFDHKKK);
				if (Player == null) return;

				__result = ApplyModifiers(__result, Player);
			}
		}

		[HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Awake))]
		public static class ShipAwake {
			public static void Prefix(ShipStatus __instance) {
				CustomVentsController.ResetAll();
				UpdateSelectedMap();
				if (SelectedMap == null) return;
				
				foreach (var TransformObject in __instance.transform) {
					Object.Destroy(TransformObject.Cast<Transform>().gameObject);
				}

				SelectedMap.RuntimeMap = Object.Instantiate(
					SelectedMap.MapPrefab, __instance.transform);
				
				SelectedMap.RecalculateRuntime();
				SelectedMap.ComponentsAwake();
				CustomVentsController.MoveAllVents();
			}
		}

		[HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.Awake))]
		public static class MiniMapPatch {
			public static void Postfix(MapBehaviour __instance) {
				if (SelectedMap != null && SelectedMap.MiniMap != null) {
					var Map = __instance;
					Map.ColorControl.GetComponent<SpriteRenderer>().sprite = SelectedMap.MiniMap;
					Map.transform.Find("RoomNames").gameObject.SetActive(false);
					ShipStatus.Instance.MapScale = SelectedMap.MapScale;
					Map.ColorControl.transform.localPosition = SelectedMap.MapOffset;
				}
			}
		}

		[HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.Method_4))]
		public static class MiniMapPatchShow {
			public static void Postfix(MapBehaviour __instance) {
				if (SelectedMap != null && SelectedMap.MiniMap != null) {
					__instance.transform.localPosition = SelectedMap.MapOffset * -1;
				}
			}
		}

		[HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.OnEnable))]
		public static class ShipOnEnable {
			public static void Postfix(ShipStatus __instance) {
				if (__instance.Systems == null) {
					return;
				}

				var Type = SelectedMap;
				if (Type == null) return;

				__instance.Systems.Clear();
				foreach (var (SystemTypes, SystemType) in Type.Systems) {
					__instance.Systems.Add(SystemTypes, SystemType);
				}
				
				__instance.Systems.Add(SystemTypes.Electrical, new SwitchSystem().Cast<ISystemType>());
				__instance.Systems.Add(SystemTypes.MedBay, new MedScanSystem().Cast<ISystemType>());

				__instance.Systems.Add(SystemTypes.Sabotage,
					new SabotageSystemType(Type.Systems
							.Select(Pair => Pair.Value.TryCast<IActivatable>())
							.Where(Activatable => Activatable != null).ToArray())
						.Cast<ISystemType>());

				var main = Camera.main;
				main.backgroundColor = SelectedMap?.BackgroundClor ?? __instance.CameraColor;
				var component = main.GetComponent<FollowerCamera>();

				if (component != null) {
					component.shakeAmount = Type.CameraShakeAmount;
					component.shakePeriod = Type.CameraShakePeriod;
				}
			}
		}
	}
}