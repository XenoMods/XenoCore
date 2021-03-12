using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using XenoCore.Events;
using XenoCore.Utils;

namespace XenoCore.Override.Usable {
	/**
	 * Управляет всеми "используемыми" объектами на карте, позволяет
	 * создавать свой
	 */
	public static class UsableController {
		private static readonly Dictionary<int, CustomUsable> Map = new Dictionary<int, CustomUsable>();

		internal static void Init() {
			EventsController.RESET_ALL.Register(() => {
				Map.Clear();
			});
		}

		private static Console CreateNew(GameObject HolderObject, SpriteRenderer Renderer,
			CustomUsable Usable) {
			var Collider = HolderObject.AddComponent<CircleCollider2D>();
			Collider.radius = Usable.UsableDistance;
			Collider.isTrigger = true;

			var Console = HolderObject.AddComponent<Console>();
			Usable.Console = Console;
			Usable.ImageRenderer = Renderer;

			if (ConsoleMaterialPatch.ImageMaterial != null) {
				SetUsableMaterial(Usable, ConsoleMaterialPatch.ImageMaterial);
			}
			
			Console.Image = Renderer;

			Map.Add(Usable.ConsoleId, Usable);
			Console.ConsoleId = Usable.ConsoleId;

			return Console;
		}

		public static Console CreateNew(GameObject HolderObject, CustomUsable Usable) {
			return CreateNew(HolderObject, HolderObject.GetComponent<SpriteRenderer>(),
				Usable);
		}

		public static Console CreateNew(Sprite Sprite, Vector3 Position, CustomUsable Usable) {
			var HolderObject = new GameObject();
			HolderObject.transform.position = Position;
			
			var Renderer = HolderObject.AddComponent<SpriteRenderer>();
			Renderer.sprite = Sprite;
			
			return CreateNew(HolderObject, Renderer, Usable);
		}

		private static void SetUsableMaterial(CustomUsable Usable, Material Material) {
			Material = new Material(Material);
			Usable.ImageRenderer.material = Material;
			Usable.ImageMaterial = Material;
			Usable.OnImageMaterialSet();
		}
		
		private static void UpdateImageMaterial() {
			var Material = ConsoleMaterialPatch.ImageMaterial;
			
			foreach (var Usable in Map.Values) {
				SetUsableMaterial(Usable, Material);
			}
		}
		
		[HarmonyPatch(typeof(SystemConsole), nameof(SystemConsole.Start))]
		public class ConsoleMaterialPatch {
			public static Material ImageMaterial;
			
			public static void Prefix(SystemConsole __instance) {
				if (ImageMaterial == null && __instance.Image != null) {
					ImageMaterial = __instance.Image.material;
					UpdateImageMaterial();
				}
			}
		}

		[HarmonyPatch(typeof(UseButtonManager), nameof(UseButtonManager.SetTarget))]
		public static class UseTargetPatch {
			public static bool Prefix(UseButtonManager __instance, IUsable CAKODNGLPDF) {
				if (CAKODNGLPDF == null) return true;

				var Console = CAKODNGLPDF.TryCast<Console>();
				if (Console == null) return true;
				
				if (!Map.ContainsKey(Console.ConsoleId)) return true;

				var CustomUsable = Map[Console.ConsoleId];
				__instance.currentTarget = CAKODNGLPDF;
				__instance.UseButton.sprite = CustomUsable.UseIcon != null
					? CustomUsable.UseIcon
					: __instance.UseImage;
				CooldownHelpers.SetCooldownNormalizedUvs(__instance.UseButton);
				__instance.UseButton.material.SetFloat(Globals.PERCENT,
					CustomUsable.PercentCooldown);
				__instance.UseButton.color = UseButtonManager.EnabledColor;
				
				return false;
			}
		}

		[HarmonyPatch(typeof(Console), nameof(Console.SetOutline))]
		public static class ConsoleSetOutlinePatch {
			public static bool Prefix(Console __instance, bool EHNJJAPKPMF, bool OPFNEGKJDLB) {
				if (!Map.ContainsKey(__instance.ConsoleId)) return true;

				Map[__instance.ConsoleId].SetOutline(EHNJJAPKPMF, OPFNEGKJDLB);
				return false;
			}
		}

		[HarmonyPatch(typeof(Console), nameof(Console.Use))]
		public static class ConsoleUsePatch {
			public static bool Prefix(Console __instance) {
				if (!Map.ContainsKey(__instance.ConsoleId)) return true;

				Map[__instance.ConsoleId].Use();
				return false;
			}
		}

		[HarmonyPatch(typeof(Console), nameof(Console.CanUse))]
		public static class ConsoleCanUsePatch {
			public static bool Prefix(Console __instance, ref float __result,
				GameData.PlayerInfo NMEAPOJFNKA,
				ref bool HPLJBLJOCKO,
				ref bool KBFCKGNGJED) {
				if (!Map.ContainsKey(__instance.ConsoleId)) return true;

				__result = Map[__instance.ConsoleId].CanUse(NMEAPOJFNKA,
					out HPLJBLJOCKO, out KBFCKGNGJED);
				return false;
			}
		}
	}
}