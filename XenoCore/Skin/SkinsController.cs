using System;
using HarmonyLib;

namespace XenoCore.Skin {
	[HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
	internal static class SkinsOverride {
		private static bool SkinsAdded;

		internal static void CheckSkinsAdded() {
			if (SkinsAdded) {
				throw new Exception("[XenoCore] Custom skins was already added");
			}
		}
		
		public static void Postfix() {
			if (SkinsAdded) return;
			SkinsAdded = true;

			PetsController.Inject();
			HatsController.Inject();
		}
	}
}