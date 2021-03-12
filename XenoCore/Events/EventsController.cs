using System;
using System.Collections.Generic;
using HarmonyLib;
using Hazel;
using XenoCore.Network;
using XenoCore.Utils;

namespace XenoCore.Events {
	public class EventDefinition {
		private readonly string EventName;
		private readonly List<Action> Listeners = new List<Action>();

		public EventDefinition(string EventName) {
			this.EventName = EventName;
		}

		public void Register(Action Listener) {
			Listeners.Add(Listener);
		}

		internal void Post() {
			ConsoleTools.Info($"Event: {EventName}");
			
			foreach (var Listener in Listeners) {
				Listener();
			}
		}
	}
	
	public static class EventsController {
		public static readonly EventDefinition GAME_INIT = new EventDefinition("GAME_INIT");
		public static readonly EventDefinition RESET_ALL = new EventDefinition("RESET_ALL");
		public static readonly EventDefinition GAME_STARTED = new EventDefinition("GAME_STARTED");
		public static readonly EventDefinition MAP_INIT = new EventDefinition("MAP_INIT");
		public static readonly EventDefinition HOST_START_GAME = new EventDefinition("HOST_START_GAME");
		public static readonly EventDefinition PREFERENCES_CHANGED = new EventDefinition("PREFERENCES_CHANGED");

		internal static void Init() {
			HandleRpcPatch.AddListener(new EventsNetworkListener());
		}
		
		[HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
		private static class StartPatch {
			private static bool Initialized = false;
			
			public static void Postfix() {
				if (Initialized) return;
				
				GAME_INIT.Post();
			}
		}
		
		[HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.BeginGame))]
		private static class HostStartGamePatch {
			public static void Prefix() {
				if (!AmongUsClient.Instance.AmHost) return;
				HOST_START_GAME.Post();
			}
		}
		
		[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcSetInfected))]
		private static class ResetAllPatch {
			public static void Prefix() {
				RESET_ALL.Post();
				Utils.Network.Send((byte) EventsRPC.ResetAll);
			}
		}

		[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.Awake))]
		private static class GameStartedPatch {
			public static void Postfix(PlayerPhysics __instance) {
				if (!__instance.myPlayer.AmOwner) return;
				GAME_STARTED.Post();
				MAP_INIT.Post();
			}
		}
		
		[HarmonyPatch(typeof(SaveManager), nameof(SaveManager.Method_52))]
		internal static class ReloadPreferencesPatch {
			public static void Postfix() {
				PREFERENCES_CHANGED.Post();
			}
		}

		private enum EventsRPC : byte {
			ResetAll = 40,
		}

		private class EventsNetworkListener : RPCListener {
			public void Handle(byte PacketId, MessageReader Reader) {
				switch (PacketId) {
					case (byte) EventsRPC.ResetAll: {
						RESET_ALL.Post();
						break;
					}
				}
			}
		}
	}
}