using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;
using XenoCore.Events;
using XenoCore.Override.Map;

namespace XenoCore.Override.Vents {
	public static class CustomVentsController {
		private static readonly Dictionary<int, CustomVent> VentMap = new Dictionary<int, CustomVent>();

		private static readonly Dictionary<string, List<CustomVent>> VentGroups
			= new Dictionary<string, List<CustomVent>>();
		
		public static void ResetAll() {
			VentMap.Clear();
			VentGroups.Clear();
		}
		
		public static CustomVent Add(string SkinId, params string[] Groups) {
			return Add(VentSkins.GetById(SkinId), Groups);
		}

		public static CustomVent Add(VentSkin Skin, params string[] Groups) {
			var Vent = new CustomVent(Skin, Groups);
			VentMap.Add(Vent.Id, Vent);

			foreach (var Group in Groups) {
				if (!VentGroups.ContainsKey(Group)) {
					VentGroups[Group] = new List<CustomVent>();
				}

				VentGroups[Group].Add(Vent);				
			}

			return Vent;
		}

		internal static void Bake() {
			foreach (var Vent in VentMap.Values) {
				var Group = new List<CustomVent>();

				foreach (var Vents in VentGroups
					.Select(GroupPair => GroupPair.Value)
					.Where(Vents => Vents.Contains(Vent))) {
					Group.AddRange(Vents);
				}
				
				Vent.Bake(Group);
			}

			CustomVent.BakeAllVents(VentMap.Values);
		}

		internal static void Init() {
			EventsController.GAME_STARTED.Register(Bake);
		}

		[HarmonyPatch(typeof(Vent), nameof(Vent.SetButtons))]
		private static class SetButtonsPatch {
			public static bool Prefix(Vent __instance) {
				if (!VentMap.ContainsKey(__instance.Id)) return true;

				return false;
			}
		}

		[HarmonyPatch(typeof(Vent), nameof(Vent.Use))]
		private static class CanUsePatch {
			public static bool Prefix(Vent __instance) {
				if (!VentMap.ContainsKey(__instance.Id)) return true;
				var Custom = VentMap[__instance.Id];
				
				__instance.CanUse(PlayerControl.LocalPlayer.Data, out var canUse, out _);
				if (!canUse) return false;
				
				var localPlayer = PlayerControl.LocalPlayer;
				if (localPlayer.inVent) {
					localPlayer.MyPhysics.RpcExitVent(Custom.Id);
					Custom.SetButtons( false);
				} else {
					localPlayer.MyPhysics.RpcEnterVent(Custom.Id);
					Custom.SetButtons(true);
				}

				return false;
			}
		}
		
		[HarmonyPatch(typeof(Vent), nameof(Vent.Method_1))]
		private static class ExitVentPatch {
			public static bool Prefix(Vent __instance, PlayerControl NMEAPOJFNKA) {
				if (!VentMap.ContainsKey(__instance.Id)) return true;
				var Custom = VentMap[__instance.Id];
				Custom.ExitVent(NMEAPOJFNKA);

				return false;
			}
		}

		[HarmonyPatch(typeof(Vent), nameof(Vent.Method_38))]
		private static class EnterVentPatch {
			public static bool Prefix(Vent __instance, PlayerControl NMEAPOJFNKA) {
				if (!VentMap.ContainsKey(__instance.Id)) return true;
				var Custom = VentMap[__instance.Id];
				Custom.EnterVent(NMEAPOJFNKA);

				return false;
			}
		}

		internal static void MoveAllVents() {
			foreach (var Vent in Object.FindObjectsOfType<Vent>()) {
				if (!VentMap.ContainsKey(Vent.Id)) continue;
				
				Vent.transform.SetParent(CustomMapController.SelectedMap.RuntimeMap.transform);
			}
		}
	}
}