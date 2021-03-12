using System.Collections.Generic;
using HarmonyLib;
using Reactor.Extensions;
using UnityEngine;
using XenoCore.Events;

namespace XenoCore.Override.Vents {
	public abstract class VentSkin {
		public readonly string Id;
		internal Vent Prefab;

		public AnimationClip ExitAnimation {
			set => Prefab.ExitVentAnim = value;
		}

		public AnimationClip EnterAnimation {
			set => Prefab.EnterVentAnim = value;
		}

		public bool PlaySound;

		public Vector3 Offset {
			set => Prefab.Offset = value;
			get => Prefab.Offset;
		}

		protected VentSkin(string Id) {
			this.Id = Id;
		}

		protected static void PreparePrefab(Vent Vent) {
			Vent.Left = null;
			Vent.Right = null;
			Vent.Center = null;
			foreach (var Button in Vent.Buttons) {
				Object.Destroy(Button.gameObject);
			}

			Vent.gameObject.SetActive(false);
		}

		protected void SetPrefab(GameObject GameObject) {
			Prefab = Object.Instantiate(GameObject).GetComponent<Vent>().DontDestroy();
			PreparePrefab(Prefab);
		}

		internal Vent Instantiate(Vector2 Position) {
			var Vent = Object.Instantiate(Prefab.gameObject);
			Vent.transform.position = Position;
			return Vent.GetComponent<Vent>();
		}
	}

	public class CustomVentSkin : VentSkin {
		private static GameObject CommonVentTemplate;

		public CustomVentSkin(string Id, Sprite VentSprite) : base(Id) {
			SetPrefab(CommonVentTemplate);
			ExitAnimation = null;
			EnterAnimation = null;
			PlaySound = true;
			Prefab.myRend.sprite = VentSprite;
		}

		internal static void SetupCommonPrefab() {
			var Common = Object.Instantiate(AmongUsClient.Instance.ShipPrefabs[0]
				.GetComponentsInChildren<Vent>()[0].gameObject).DontDestroy().GetComponent<Vent>();

			CustomVent.VentButtonTemplate = Object.Instantiate(Common.Buttons[0]
				.gameObject).DontDestroy();
			CustomVent.VentButtonTemplate.gameObject.SetActive(false);

			PreparePrefab(Common);
			CommonVentTemplate = Common.gameObject;
		}
	}

	internal sealed class StandardVentSkin : VentSkin {
		public StandardVentSkin(string Id, ShipStatus FromMap) : base(Id) {
			SetPrefab(FromMap.GetComponentsInChildren<Vent>()[0].gameObject);
		}
	}

	public static class VentSkins {
		public static readonly string SKELD = "Skeld";
		public static readonly string POLUS = "Polus";
		private static readonly Dictionary<string, VentSkin> Skins = new Dictionary<string, VentSkin>();

		internal static void Init() {
			EventsController.GAME_INIT.Register(() => {
				CustomVentSkin.SetupCommonPrefab();
				RegisterStandardVent("Skeld", AmongUsClient.Instance.ShipPrefabs[0], true);
				RegisterStandardVent("Polus", AmongUsClient.Instance.ShipPrefabs[2], false);
			});
		}

		private static void RegisterStandardVent(string Id, ShipStatus FromMap, bool PlaySound) {
			Register(new StandardVentSkin(Id, FromMap) {PlaySound = PlaySound});
		}

		public static void Register(VentSkin Skin) {
			Skins.Add(Skin.Id, Skin);
		}

		public static VentSkin GetById(string Id) {
			return Skins[Id];
		}
	}
}