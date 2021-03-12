using System;
using UnityEngine;
using XenoCore.Buttons.Strategy;
using XenoCore.Utils;
using Object = UnityEngine.Object;

namespace XenoCore.Buttons {
	public class UniversalButton : IDisposable {
		public readonly bool IsStandard;
		private readonly KillButtonManager Button;
		private readonly Transform ButtonTransform;
		private readonly GameObject ButtonObject;
		private readonly SpriteRenderer ButtonRenderer;
		private readonly Vector2 Offset;
		private readonly Sprite OriginalIcon;

		public Sprite Icon {
			get => Button.renderer.sprite;
			set => Button.renderer.sprite = value;
		}
		public bool Visible;
		public bool Active = true;
		public CooldownController Cooldown;
		public ButtonTargeter Targeter = NoneTargeter.INSTANCE;
		public ButtonSaturator Saturator = ActiveSaturator.INSTANCE;
		public CooldownStrategy CooldownStrategy = NoneCooldownStrategy.INSTANCE;

		public PlayerControl CurrentTarget;
		public Color TargetOutlineColor = Color.red;

		public static UniversalButton Create(Vector2 Offset, Action OnClick) {
			var Hud = HudManager.Instance;
			return new UniversalButton(null, Offset, Hud, OnClick, false);
		}

		public static UniversalButton FromKillButton() {
			var Hud = HudManager.Instance;
			return new UniversalButton(Hud.KillButton, Vector2.zero, Hud, null, true);
		}

		private UniversalButton(KillButtonManager Button, Vector2 Offset,
			HudManager HudManager, Action OnClick, bool IsStandard) {
			this.Offset = Offset;
			this.IsStandard = IsStandard;

			if (Button == null) {
				Button = Object.Instantiate(HudManager.KillButton, HudManager.transform);
			}

			this.Button = Button;
			ButtonTransform = Button.transform;
			ButtonObject = Button.gameObject;
			ButtonRenderer = Button.renderer;

			OriginalIcon = Button.renderer.sprite;

			if (IsStandard) return;
			var ButtonComponent = Button.GetComponent<PassiveButton>();
			ButtonComponent.OnClick.RemoveAllListeners();
			ButtonComponent.OnClick.AddListener(new Action(OnClick));
		}

		public void ResetIcon() {
			Icon = OriginalIcon;
		}

		public void Reset() {
			ResetIcon();
			Visible = false;
			Active = true;
			Cooldown = null;
			Targeter = NoneTargeter.INSTANCE;
			Saturator = NoneSaturator.INSTANCE;
			CooldownStrategy = NoneCooldownStrategy.INSTANCE;
			TargetOutlineColor = Color.red;
		}
		
		private void SetTarget(PlayerControl Target) {
			if (Button.CurrentTarget != null) {
				Button.CurrentTarget.GetComponent<SpriteRenderer>().material
					.SetFloat(Globals.OUTLINE, 0f);
			}
			
			Button.CurrentTarget = Target;
			
			if (Target == null) return;
			var Renderer = Target.myRend;
			if (Renderer == null) return;
			
			Renderer.material.SetFloat(Globals.OUTLINE, Button.isActive ? 1f : 0f);
			Renderer.material.SetColor(Globals.OUTLINE_COLOR, TargetOutlineColor);
		}

		public void Update() {
			if (!IsStandard && ButtonTransform.localPosition.x > 0F) {
				var Vector = ButtonTransform.localPosition;
				Vector.x = -(Vector.x + 1.3f);
				Vector += new Vector3(Offset.x, Offset.y);

				ButtonTransform.localPosition = Vector;
			}
			
			ButtonObject.SetActive(Visible);

			CurrentTarget = Targeter.GetTarget(Active);
			SetTarget(CurrentTarget);

			var KD = Cooldown?.GetKD() ?? 0f;
			var CooldownType = CooldownStrategy.GetType(KD, Active, CurrentTarget);

			if (Cooldown == null || CooldownType == CooldownType.NO_COOLDOWN) {
				KD = 0f;
				Button.SetCoolDown(KD, 1f);
			} else if (CooldownType == CooldownType.STANDARD) {
				Button.SetCoolDown(KD, Cooldown.Cooldown);				
			}

			if (Saturator.Saturated(KD, Active, CurrentTarget)) {
				ButtonRenderer.color = Palette.EnabledColor;
				ButtonRenderer.material.SetFloat(Globals.DESAT, 0f);
				Button.isActive = true;
			} else {
				ButtonRenderer.color = Palette.DisabledColor;
				ButtonRenderer.material.SetFloat(Globals.DESAT, 1f);
				Button.isActive = false;
			}
		}

		private bool IsDisposed;

		public void Dispose() {
			if (!IsDisposed) {
				try {
					Button.renderer.enabled = false;
					Button.TimerText.enabled = false;

					Object.Destroy(Button);
				} catch {
					// ignored
				}

				IsDisposed = true;
			}

			GC.SuppressFinalize(this);
		}
	}
}