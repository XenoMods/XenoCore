namespace XenoCore.Utils {
	public static class Game {
		public static bool IsHost() {
			return AmongUsClient.Instance.AmHost;
		}
	}
}