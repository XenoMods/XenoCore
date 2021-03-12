using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using Reactor.Extensions;
using UnhollowerBaseLib;
using UnityEngine;
using XenoCore.Locale;
using XenoCore.Utils;

namespace XenoCore.CustomOptions {
	public partial class CustomOption {
		public static readonly OptionGroup DEFAULT_GROUP = new OptionGroup(0);

		public static int GetOptionOrder(CustomOption Option) {
			return Option.Group?.Order ?? 0;
		}
		
		private static IEnumerable<OptionBehaviour> GetGameOptions(float lowestY) {
			var options = new List<OptionBehaviour>();

			var toggleOption = Object.FindObjectsOfType<ToggleOption>().FirstOrDefault();
			var numberOption = Object.FindObjectsOfType<NumberOption>().FirstOrDefault();
			var stringOption = Object.FindObjectsOfType<StringOption>().FirstOrDefault();

			var i = 0;
			foreach (var option in Options.OrderBy(GetOptionOrder)) {
				if (option.GameSetting != null) {
					options.Add(option.GameSetting);

					continue;
				}

				if (option.Type == CustomOptionType.Toggle) {
					if (toggleOption == null) continue;

					var toggle = Object.Instantiate(toggleOption, toggleOption.transform.parent).DontDestroy();

					var transform = toggle.transform;
					transform.localPosition = new Vector3(transform.localPosition.x, lowestY - ++i * 0.5F,
						transform.localPosition.z);

					option.OnGameOptionCreated(toggle);

					options.Add(toggle);
				} else if (option.Type == CustomOptionType.Number) {
					if (numberOption == null) continue;

					var number = Object.Instantiate(numberOption, numberOption.transform.parent).DontDestroy();

					var transform = number.transform;
					transform.localPosition = new Vector3(transform.localPosition.x, lowestY - ++i * 0.5F,
						transform.localPosition.z);

					option.OnGameOptionCreated(number);

					options.Add(number);
				} else if (option.Type == CustomOptionType.String) {
					if (stringOption == null) continue;

					var str = Object.Instantiate(stringOption, stringOption.transform.parent).DontDestroy();

					var transform = str.transform;
					transform.localPosition = new Vector3(transform.localPosition.x, lowestY - ++i * 0.5F,
						transform.localPosition.z);

					option.OnGameOptionCreated(str);

					options.Add(str);
				} else if (option.Type == CustomOptionType.Title) {
					if (toggleOption == null) continue;

					var toggle = Object.Instantiate(toggleOption, toggleOption.transform.parent).DontDestroy();

					var transform = toggle.transform;
					transform.localPosition = new Vector3(transform.localPosition.x, lowestY - ++i * 0.5F,
						transform.localPosition.z);

					option.OnGameOptionCreated(toggle);
					toggle.CheckMark.transform.parent.gameObject.SetActive(false);

					options.Add(toggle);
				}
			}

			return options;
		}

		[HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.Start))]
		private class GameOptionsMenuPatchStart {
			public static void Postfix(GameOptionsMenu __instance) {
				var customOptions = GetGameOptions(__instance
					.GetComponentsInChildren<OptionBehaviour>().Min(option => option.transform.localPosition.y));
				var defaultOptions = __instance.Children;

				var options = defaultOptions.Concat(customOptions).ToArray();

				__instance.Children = new Il2CppReferenceArray<OptionBehaviour>(options);
			}
		}

		[HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.Update))]
		private class GameOptionsMenuPatchUpdate {
			public static void Postfix(GameOptionsMenu __instance) {
				__instance.GetComponentInParent<Scroller>().YBounds.max = __instance.Children.Length * 0.45F;
			}
		}

		[HarmonyPatch(typeof(GameOptionsData), nameof(GameOptionsData.Method_24))] //ToHudString
		private static class GameOptionsDataPatch {
			private static void Postfix(ref string __result) {
				var builder = new StringBuilder(__result);
				foreach (var option in Options.OrderBy(GetOptionOrder)) {
					builder.Append($"{Globals.FORMAT_WHITE}{LanguageManager.Get(option.Name)}"
						.Argumentize(option.LocalizationArguments));

					if (option.ValueShown) {
						builder.AppendLine($"{Globals.FORMAT_WHITE}: {option}");
					} else {
						builder.AppendLine();
					}
				}

				__result = builder.ToString();
			}
		}

		private static bool OnEnable(OptionBehaviour opt) {
			var customOption = Options.FirstOrDefault(option => option.GameSetting == opt);

			if (customOption == null) return true;

			customOption.OnGameOptionCreated(opt);

			return false;
		}

		[HarmonyPatch(typeof(ToggleOption), nameof(ToggleOption.OnEnable))]
		private static class ToggleOptionOnEnablePatch {
			private static bool Prefix(ToggleOption __instance) {
				return OnEnable(__instance);
			}
		}

		[HarmonyPatch(typeof(NumberOption), nameof(NumberOption.OnEnable))]
		private static class NumberOptionOnEnablePatch {
			private static bool Prefix(NumberOption __instance) {
				return OnEnable(__instance);
			}
		}

		[HarmonyPatch(typeof(StringOption), nameof(StringOption.OnEnable))]
		private static class StringOptionOnEnablePatch {
			private static bool Prefix(StringOption __instance) {
				return OnEnable(__instance);
			}
		}

		[HarmonyPatch(typeof(ToggleOption), nameof(ToggleOption.Toggle))]
		private class ToggleButtonPatch {
			public static bool Prefix(ToggleOption __instance) {
				var
					option = Options.FirstOrDefault(option =>
						option.GameSetting == __instance); // Works but may need to change to gameObject.name check
				if (option is CustomToggleOption toggle) {
					toggle.Toggle();

					return false;
				}

				return true;
			}
		}

		[HarmonyPatch(typeof(NumberOption), nameof(NumberOption.Increase))]
		private class NumberOptionPatchIncrease {
			public static bool Prefix(NumberOption __instance) {
				var option = Options.FirstOrDefault(option =>
						option.GameSetting == __instance); // Works but may need to change to gameObject.name check
				if (option is CustomNumberOption number) {
					number.Increase();

					return false;
				}

				return true;
			}
		}

		[HarmonyPatch(typeof(NumberOption), nameof(NumberOption.Decrease))]
		private class NumberOptionPatchDecrease {
			public static bool Prefix(NumberOption __instance) {
				var option = Options.FirstOrDefault(option =>
						option.GameSetting == __instance); // Works but may need to change to gameObject.name check
				if (option is CustomNumberOption number) {
					number.Decrease();

					return false;
				}

				return true;
			}
		}

		[HarmonyPatch(typeof(StringOption), nameof(StringOption.Increase))]
		private class StringOptionPatchIncrease {
			public static bool Prefix(StringOption __instance) {
				var option = Options.FirstOrDefault(option => option.GameSetting == __instance);
				if (option is CustomStringOption str) {
					str.Increase();

					return false;
				}

				return true;
			}
		}

		[HarmonyPatch(typeof(StringOption), nameof(StringOption.Decrease))]
		private class StringOptionPatchDecrease {
			public static bool Prefix(StringOption __instance) {
				var option = Options.FirstOrDefault(option => option.GameSetting == __instance);
				if (option is CustomStringOption str) {
					var oldValue = str.GetText();
					str.Decrease();

					return false;
				}

				return true;
			}
		}

		[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcSyncSettings))]
		private class PlayerControlPatch {
			public static void Postfix() {
				if (PlayerControl.AllPlayerControls.Count < 2 || !AmongUsClient.Instance ||
				    !PlayerControl.LocalPlayer || !AmongUsClient.Instance.AmHost) return;

				Rpc.Instance.Send(new Rpc.Data(Options));
			}
		}
	}
}