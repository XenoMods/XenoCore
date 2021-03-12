using System.Collections.Generic;
using HarmonyLib;
using Hazel;

namespace XenoCore.Override.Map.System {
	public static class SystemTypeController {
		private static readonly Dictionary<int, CustomSystemType> Map =
			new Dictionary<int, CustomSystemType>();
		
		public static void Clear() {
			Map.Clear();
		}

		public static void Register(CustomSystemType Type) {
			Map.Add(Type.Index, Type);
		}
		
		[HarmonyPatch(typeof(HqHudSystemType), nameof(HqHudSystemType.IsActive), MethodType.Getter)]
		public static class PatchIsActive {
			public static bool Prefix(HqHudSystemType __instance, ref bool __result) {
				if (!Map.ContainsKey(__instance.TargetNumber)) return true;

				__result = Map[__instance.TargetNumber].IsActive;
				return false;
			}
		}
		
		[HarmonyPatch(typeof(HqHudSystemType), nameof(HqHudSystemType.Detoriorate))]
		public static class PatchDetoriorate {
			public static bool Prefix(HqHudSystemType __instance, ref bool __result, float INJIGFHIDCA) {
				if (!Map.ContainsKey(__instance.TargetNumber)) return true;

				__result = Map[__instance.TargetNumber].Detoriorate(INJIGFHIDCA);
				return false;
			}
		}
		
		[HarmonyPatch(typeof(HqHudSystemType), nameof(HqHudSystemType.RepairDamage))]
		public static class PatchRepairDamage {
			public static bool Prefix(HqHudSystemType __instance, PlayerControl IIEKJBMPELC, byte AMMANIHDKKA) {
				if (!Map.ContainsKey(__instance.TargetNumber)) return true;

				Map[__instance.TargetNumber].RepairDamage(IIEKJBMPELC, AMMANIHDKKA);
				return false;
			}
		}
		
		[HarmonyPatch(typeof(HqHudSystemType), nameof(HqHudSystemType.Serialize))]
		public static class PatchSerialize {
			public static bool Prefix(HqHudSystemType __instance, MessageWriter AGLJMGAODDG, bool BILBBBFMCOB) {
				if (!Map.ContainsKey(__instance.TargetNumber)) return true;

				Map[__instance.TargetNumber].Serialize(AGLJMGAODDG, BILBBBFMCOB);
				return false;
			}
		}
		
		[HarmonyPatch(typeof(HqHudSystemType), nameof(HqHudSystemType.Deserialize))]
		public static class PatchDeserialize {
			public static bool Prefix(HqHudSystemType __instance, MessageReader ALMCIJKELCP, bool BILBBBFMCOB) {
				if (!Map.ContainsKey(__instance.TargetNumber)) return true;

				Map[__instance.TargetNumber].Deserialize(ALMCIJKELCP, BILBBBFMCOB);
				return false;
			}
		}
	}
}