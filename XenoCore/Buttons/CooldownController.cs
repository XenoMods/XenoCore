using System;
using XenoCore.CustomOptions;
using XenoCore.Utils;

namespace XenoCore.Buttons {
	/**
	 * Вспомогательный класс для хранения данных о кулдауне и
	 * вычисления его параметров
	 */
	public class CooldownController {
		// TimeProvider
		private readonly INumberProvider _Cooldown;
		
		// Runtime
		public float Cooldown => _Cooldown.GetValue();

		/**
		 * Время последнего использования кулдауна
		 */
		public DateTime? LastUsed { get; set; }

		public CooldownController(INumberProvider Option) {
			_Cooldown = Option;
		}

		public static CooldownController FromConstant(float ConstantCooldown) {
			return new CooldownController(new ConstantProvider(ConstantCooldown));
		}

		public static CooldownController FromOption(string Prefix, string OptionName = "cooldown",
			float Value = 30f, float Min = 5f, float Max = 60f, float Increment = 2.5f) {
			return new CooldownController(CustomOption.AddNumber(
				$"{Prefix}.{OptionName}", Value, Min, Max, Increment));
		}

		public float GetKD(bool DefaultIsCooldown = true) {
			if (LastUsed == null) {
				return DefaultIsCooldown ? Cooldown : 0;
			}

			var Now = DateTime.UtcNow;
			var Diff = (TimeSpan) (Now - LastUsed);

			var CooldownMs = Cooldown * 1000.0f;
			if (CooldownMs - (float) Diff.TotalMilliseconds < 0) {
				return 0;
			} else {
				return (CooldownMs - (float) Diff.TotalMilliseconds) / 1000.0f;
			}
		}

		public bool IsReady() {
			return GetKD() == 0;
		}

		public void ForceSetLastUsedFrom(CooldownController AnotherController) {
			LastUsed = AnotherController.LastUsed;
		}

		public void Reset() {
			LastUsed = null;
		}

		public void LastUsedFromNow(float Seconds) {
			LastUsed = DateTime.UtcNow.AddSeconds(Seconds);
		}

		public void Use() {
			LastUsedFromNow(0);
		}

		public void UpdateForIntro(IntroCutscene.CoBegin__d Cutscene, float IntroCooldown = 10f) {
			LastUsedFromNow(Cutscene.timer + IntroCooldown - Cooldown);
		}

		public void UpdateForExile(ExileController Exile) {
			LastUsed = DateTime.UtcNow.AddMilliseconds(Exile.Duration);
		}

		private class ConstantProvider : INumberProvider {
			private readonly float Value;

			public ConstantProvider(float Value) {
				this.Value = Value;
			}

			public float GetValue() {
				return Value;
			}
		}
	}
}