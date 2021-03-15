using System;
using System.Collections.Generic;
using HarmonyLib;
using XenoCore.Core;
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
			ConsoleTools.Light($"Event: {EventName}");
			
			foreach (var Listener in Listeners) {
				Listener();
			}
		}
	}
	
	public static class EventsController {
		// Инициализация всей игры (главное меню)
		public static readonly EventDefinition GAME_INIT = new EventDefinition("GAME_INIT");
		
		// Вызывается перед стартом игровой сессии, нужно для очистки временных переменных
		public static readonly EventDefinition RESET_ALL = new EventDefinition("RESET_ALL");
		
		// Вызывается после полной инициализации игровой сессии
		public static readonly EventDefinition GAME_STARTED = new EventDefinition("GAME_STARTED");
		
		// Вызывается при инициализации карты после GAME_STARTED
		public static readonly EventDefinition MAP_INIT = new EventDefinition("MAP_INIT");
		
		// Вызывается при старте игры хостом
		public static readonly EventDefinition HOST_START_GAME = new EventDefinition("HOST_START_GAME");
		
		// Вызывается при изменении настроек игры
		public static readonly EventDefinition PREFERENCES_CHANGED = new EventDefinition("PREFERENCES_CHANGED");
		
		// Вызывается при первом старте HudManager
		public static readonly EventDefinition HUD_INIT = new EventDefinition("HUD_INIT");
		
		// Вызывается при каждом чистом старте HudManager
		public static readonly EventDefinition HUD_START = new EventDefinition("HUD_START");

		[HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
		private static class HudStartPatch {
			public static bool Initialized;

			public static bool Prefix(HudManager __instance) {
				if (Initialized) {
					HUD_START.Post();
					return true;
				}
				
				HUD_INIT.Post();
				HUD_START.Post();
				Initialized = true;

				__instance.SetTouchType(SaveManager.ControlMode);
				
				return false;
			}
		}
		
		[HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
		private static class HudUpdatePatch {
			public static bool Prefix() {
				return HudStartPatch.Initialized;
			}
		}
		
		[HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
		private static class StartPatch {
			private static bool Initialized;
			
			public static void Postfix() {
				if (Initialized) return;
				
				GAME_INIT.Post();
				Initialized = true;
			}
		}
		
		[HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.BeginGame))]
		private static class HostStartGamePatch {
			public static void Prefix() {
				if (!Game.IsHost()) return;
				HOST_START_GAME.Post();
			}
		}
		
		[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcSetInfected))]
		private static class ResetAllPatch {
			public static void Prefix() {
				RESET_ALL.Post();
				ResetAllMessage.INSTANCE.Send();
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
	}
}