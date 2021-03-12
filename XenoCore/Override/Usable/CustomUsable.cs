using UnityEngine;
using XenoCore.Utils;

namespace XenoCore.Override.Usable {
	/**
	 * Основа для всех "юзаемых" объектов карты
	 */
	public abstract class CustomUsable {
		private static int CurrentConsoleId = 5482;
		public readonly int ConsoleId;
		public Console Console;
		public SpriteRenderer ImageRenderer;
		public Material ImageMaterial;

		public virtual float UsableDistance => 0.5f;
		public virtual float PercentCooldown => 0f;
		public virtual Sprite UseIcon => null;

		protected CustomUsable() {
			ConsoleId = CurrentConsoleId;
			CurrentConsoleId++;
		}

		public virtual void OnImageMaterialSet() {
		}

		public virtual float CanUse(GameData.PlayerInfo pc, out bool canUse, out bool couldUse) {
			var num = float.MaxValue;
			var @object = pc.Object;
			var truePosition = @object.GetTruePosition();
			couldUse = @object.CanMove && !pc.IsDead;
			canUse = couldUse;

			if (!canUse) return num;
			
			num = Vector2.Distance(truePosition, Console.transform.position);
			canUse &= num <= UsableDistance;
			return num;
		}

		public virtual void SetOutline(bool on, bool mainTarget) {
			Recolor(on ? 1f : 0f,
				Color.yellow,
				mainTarget ? Color.yellow : Color.clear);
		}

		protected void Recolor(float Outline, Color OutlineColor, Color AddColor) {
			if (ImageMaterial != null) {
				ImageMaterial.SetFloat(Globals.OUTLINE, Outline);
				ImageMaterial.SetColor(Globals.OUTLINE_COLOR, OutlineColor);
				ImageMaterial.SetColor(Globals.ADD_COLOR, AddColor);
			}
		}
		
		public abstract void Use();
	}
}