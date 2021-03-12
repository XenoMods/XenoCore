using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using XenoCore.Events;
using XenoCore.Utils;

namespace XenoCore.Override.Tasks {
	/**
	 * <para>Переопределение иконок на карте</para>
	 * <para>Для включения вызвать функцию Enable()</para>
	 * <para>Добавленные иконки можно изменять в любой момент и это будет отражаться на карте</para>
	 */
	public static class TasksOverlay {
		private static bool Enabled;

		public static void Enable() {
			Enabled = true;
		}
		
		private static readonly List<MapIcon> Icons = new List<MapIcon>();

		private static readonly Dictionary<MapIcon, PooledMapIcon>
			PooledIcons = new Dictionary<MapIcon, PooledMapIcon>();

		internal static void Init() {
			EventsController.RESET_ALL.Register(ResetAll);
			ResetAll();
		}
		
		public static void ResetAll() {
			foreach (var (_, Pooled) in PooledIcons) {
				Object.Destroy(Pooled);
			}
			PooledIcons.Clear();
			Icons.Clear();
		}

		public static MapIcon Add(MapIcon Icon) {
			Icons.Add(Icon);

			return Icon;
		}

		private static void SetFrom(this PooledMapIcon Pooled, MapIcon MapIcon) {
			if (MapIcon.Icon != null) {
				Pooled.rend.sprite = MapIcon.Icon;
			}

			Pooled.rend.color = MapIcon.Color;
			Pooled.alphaPulse.enabled = MapIcon.Pulse;
			Pooled.rend.material.SetFloat(Globals.OUTLINE, MapIcon.Outline);
			
			var Temp = new Vector3(MapIcon.Position.x, MapIcon.Position.y);
			Temp /= ShipStatus.Instance.MapScale;
			Temp.z = -1f;
			Pooled.transform.localPosition = Temp;
		}

		[HarmonyPatch(typeof(MapTaskOverlay), nameof(MapTaskOverlay.Show))]
		private static class ShowPatch {
			public static bool Prefix(MapTaskOverlay __instance) {
				if (!Enabled) return true;
				
				__instance.gameObject.SetActive(true);

				foreach (var MapIcon in Icons) {
					var Pooled = Object.Instantiate(__instance.icons.Prefab, __instance.transform)
						.GetComponent<PooledMapIcon>();
					Pooled.transform.localScale = new Vector3(Pooled.NormalSize,
						Pooled.NormalSize, Pooled.NormalSize);

					MapIcon.Pooled = Pooled;
					MapIcon.Color = MapIcon.Color;
					Pooled.SetFrom(MapIcon);
					PooledIcons.Add(MapIcon, Pooled);
				}

				return false;
			}
		}
		
		[HarmonyPatch(typeof(MapTaskOverlay), nameof(MapTaskOverlay.Hide))]
		private static class HidePatch {
			public static void Prefix(MapTaskOverlay __instance) {
				if (!Enabled) return;
				
				foreach (var PooledIcon in PooledIcons.Values) {
					Object.Destroy(PooledIcon.gameObject);
				}
				
				PooledIcons.Clear();
			}
		}
		
		[HarmonyPatch(typeof(MapTaskOverlay), nameof(MapTaskOverlay.Update))]
		private static class UpdatePatch {
			public static bool Prefix(MapTaskOverlay __instance) {
				if (!Enabled) return true;
				
				foreach (var (MapIcon, Pooled) in PooledIcons) {
					Pooled.SetFrom(MapIcon);
				}
				
				return false;
			}
		}
	}

	public class MapIcon {
		public Sprite Icon;
		public Vector2 Position;

		public bool Pulse;
		public float Outline;
		
		private Color _Color = Color.yellow;

		public Color Color {
			get => _Color;
			set {
				_Color = value;
				if (Pooled != null) Pooled.alphaPulse.SetColor(value);
			}
		}

		internal PooledMapIcon Pooled;
	}
}