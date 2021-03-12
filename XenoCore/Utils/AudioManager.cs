using System;
using XenoCore.Events;

namespace XenoCore.Utils {
	public static class AudioManager {
		public static float Effects { get; private set; }
		public static float Music { get; private set; }

		internal static void Init() {
			EventsController.PREFERENCES_CHANGED.Register(Reload);
			Reload();
		}

		private static void Reload() {
			Effects = SaveManager.SfxVolume;
			Music = SaveManager.MusicVolume;
		}

		public static float EffectsScale(float Multiplier) {
			return Math.Min(1.0f, Effects * Multiplier);
		}

		public static float MusicScale(float Multiplier) {
			return Math.Min(1.0f, Music * Multiplier);
		}
	}
}