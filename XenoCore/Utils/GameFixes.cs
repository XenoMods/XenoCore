using HarmonyLib;

namespace XenoCore.Utils {
	[HarmonyPatch(typeof(BanMenu), nameof(BanMenu.Show))]
	public static class CheaterBanFixPatch {
		public static void Prefix(BanMenu __instance) {
			foreach (var Client in AmongUsClient.Instance.allClients) {
				var Data = Client.Character.Data;

				if (string.IsNullOrWhiteSpace(Data.PlayerName)) {
					Data.PlayerName = "???";
				}
			}
		}
	}
	
	[HarmonyPatch(typeof(StatsManager), nameof(StatsManager.AmBanned), MethodType.Getter)]
	public class DisableBanPatch {
		public static void Postfix(out bool __result) {
			__result = false;
		}
	}
	
	[HarmonyPatch(typeof(BanMenu), nameof(BanMenu.SetVisible))]
	public static class BanMenuPatch {
		private static bool IsDead;
		
		public static void Prefix() {
			var Data = PlayerControl.LocalPlayer.Data;
			IsDead = Data.IsDead;
			Data.IsDead = false;
		}

		public static void Postfix() {
			PlayerControl.LocalPlayer.Data.IsDead = IsDead;
		}
	}
	
	[HarmonyPatch(typeof(InnerNetClient), nameof(InnerNetClient.CanBan))]
	public static class BanAlwaysActivePatch {
		public static void Postfix(out bool __result, InnerNetClient __instance) {
			__result = __instance.AmHost;
		}
	}
	
	[HarmonyPatch(typeof(InnerNetClient), nameof(InnerNetClient.CanKick))]
	public static class KickAlwaysActivePatch {
		public static void Postfix(out bool __result, InnerNetClient __instance) {
			__result = __instance.AmHost;
		}
	}
	
	[HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.BeginGame))]
	public static class BeginGamePatch {
		public static void Postfix(GameStartManager __instance) {
			__instance.countDownTimer = 0;
		}
	}
	
	[HarmonyPatch(typeof(ShhhBehaviour), nameof(ShhhBehaviour.Update))]
	public class ShhhPatch {
		public static void Prefix(ShhhBehaviour __instance) {
			__instance.Delay = 0;
			__instance.Duration = 0;
			__instance.HoldDuration = 0;
			__instance.PulseDuration = 0;
			__instance.TextDuration = 0;
		}
	}
	
	[HarmonyPatch(typeof(SaveManager), nameof(SaveManager.GetPurchase))]
	public class ShopPatch {
		public static void Postfix(out bool __result) {
			__result = true;
		}
	}
}