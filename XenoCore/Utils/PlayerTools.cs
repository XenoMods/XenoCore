using System;
using HarmonyLib;

namespace XenoCore.Utils {
	[HarmonyPatch]
	public static class PlayerTools {
		public static PlayerControl ClosestPlayer;
		public static double ClosestDistance;

		public static PlayerControl GetPlayerById(byte Id) {
			foreach (var player in PlayerControl.AllPlayerControls) {
				if (player.PlayerId == Id) {
					return player;
				}
			}

			return null;
		}

		public static void CalculateClosest(PlayerControl From) {
			CalculateClosest(From, out ClosestPlayer, out ClosestDistance, Player => true);
		}

		public static void CalculateClosest(PlayerControl From, out PlayerControl OutClosestPlayer,
			out double OutClosestDistance, Func<PlayerControl, bool> Filter) {
			OutClosestDistance = double.MaxValue;
			OutClosestPlayer = null;
			
			foreach (var Player in PlayerControl.AllPlayerControls) {
				if (Player.Data.IsDead || Player == From) continue;
				if (!Filter.Invoke(Player)) continue;
				
				var Distance = GetDistance(Player, From);
				if (Distance < OutClosestDistance) {
					OutClosestDistance = Distance;
					OutClosestPlayer = Player;
				}
			}
		}

		public static double GetDistance(PlayerControl First, PlayerControl Second) {
			return (First.GetTruePosition() - Second.GetTruePosition()).magnitude;
		}
		
		public static bool IsInKillRange(this double Distance) {
			return Distance < GameOptionsData.KillDistances[PlayerControl.GameOptions.KillDistance];
		}
	}
}