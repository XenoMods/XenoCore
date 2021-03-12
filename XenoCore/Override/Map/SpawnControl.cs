using UnityEngine;

namespace XenoCore.Override.Map {
	public interface ISpawnLocationModifier {
		Vector2 Modify(Vector2 Source, PlayerControl Player);
	}
}