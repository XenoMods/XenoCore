using System.Collections.Generic;
using System.Linq;
using PowerTools;
using UnityEngine;
using XenoCore.Override.Vents.Button;
using XenoCore.Utils;
using Il2CppType = UnhollowerRuntimeLib.Il2CppType;

namespace XenoCore.Override.Vents {
	public class CustomVent {
		internal static GameObject VentButtonTemplate;
		private static int CurrentId = 548_254;
		public readonly int Id;
		public readonly string[] Groups;
		public readonly VentSkin Skin;

		private readonly Vent Instance;

		private readonly List<VentButtonPair> Connections = new List<VentButtonPair>();

		public Vector2 Position {
			get => Instance.transform.position;
			set => Instance.transform.position = new Vector3(value.x, value.y, 1);
		}

		public CustomVent(VentSkin Skin, params string[] Groups) {
			Id = CurrentId;
			CurrentId++;
			this.Groups = Groups;
			this.Skin = Skin;

			Instance = Skin.Instantiate(Vector2.zero);
			Instance.Id = Id;
			Instance.gameObject.SetActive(true);
		}

		private CustomButtonBehavior InstantiateButton(CustomVent Vent) {
			var ButtonObject = Object.Instantiate(VentButtonTemplate, Instance.transform);
			Object.Destroy(ButtonObject.GetComponent<ButtonBehavior>());

			var ButtonBehavior = ButtonObject.AddComponent(Il2CppType
				.Of<CustomButtonBehavior>()).Cast<CustomButtonBehavior>();
			ButtonBehavior.OnClick = () => { ArrowClicked(Vent); };
			ButtonObject.SetActive(false);
			return ButtonBehavior;
		}

		internal void SetButtons(bool Enabled) {
			foreach (var Pair in Connections) {
				var Arrow = Pair.Arrow;
				Arrow.gameObject.SetActive(Enabled);
				if (!Enabled) continue;

				var Vent = Pair.Destination;
				var val = Vent.Instance.transform.position - Instance.transform.position;
				var localPosition = val.normalized * 0.7f;
				localPosition.x *= Mathf.Sign(ShipStatus.Instance.transform.localScale.x);
				localPosition.y -= 0.08f;
				localPosition.z = -10f;
				Arrow.transform.localPosition = localPosition;
				Arrow.transform.LookAt2d(Vent.Instance.transform);
			}
		}

		private static void DoMove(Vector3 pos) {
			pos -= (Vector3) PlayerControl.LocalPlayer.Collider.offset;
			PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(pos);
			ShipStatus.Instance.VentMoveSounds.RandomItem().PlayGlobally(AudioManager.Effects,
				Random.Range(0.8f, 1.2f));
		}

		private void ArrowClicked(CustomVent Vent) {
			if (!PlayerControl.LocalPlayer.inVent) return;

			DoMove(Vent.Instance.transform.position);
			SetButtons(false);
			Vent.SetButtons(true);
		}

		internal void Bake(List<CustomVent> VentsGroup) {
			foreach (var SomeTransform in Instance.transform) {
				ConsoleTools.Info($"{SomeTransform.GetType().FullName}");
				var Transform = (Transform) SomeTransform;
				Object.Destroy(Transform.gameObject);
			}
			Connections.Clear();

			foreach (var Vent in VentsGroup.Where(Vent => Vent.Id != Id)) {
				Connections.Add(new VentButtonPair {
					Destination = Vent,
					Arrow = InstantiateButton(Vent)
				});
			}
		}

		internal static void BakeAllVents(Dictionary<int, CustomVent>.ValueCollection Vents) {
			ShipStatus.Instance.AllVents = Vents
				.Select(Vent => Vent.Instance)
				.Concat(ShipStatus.Instance.AllVents)
				.ToArray();
		}

		private void AnimateVent(PlayerControl Player, AnimationClip Clip) {
			if (Clip != null) {
				var SpriteAnim = Instance.GetComponent<SpriteAnim>();
				SpriteAnim.Play(Clip);
				SpriteAnim.SetSpeed(1f);
				SpriteAnim.SetTime(0f);
			}

			if (!Player.AmOwner || !Skin.PlaySound) return;
			ShipStatus.Instance.VentEnterSound.PlayGlobally(AudioManager.Effects,
				Random.Range(0.8f, 1.2f));
		}

		public void ExitVent(PlayerControl Player) {
			AnimateVent(Player, Instance.ExitVentAnim);
		}

		public void EnterVent(PlayerControl Player) {
			AnimateVent(Player, Instance.EnterVentAnim);
		}
	}

	public class VentButtonPair {
		public CustomVent Destination;
		public CustomButtonBehavior Arrow;
	}
}