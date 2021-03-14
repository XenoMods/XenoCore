using Hazel;
using XenoCore.Utils;

namespace XenoCore.Core {
	public static class NetworkUtils {
		public static void WriteNullablePlayer(this MessageWriter Writer, PlayerControl Player) {
			if (Player != null) {
				Writer.Write((int) Player.PlayerId);
			} else {
				Writer.Write(-1);
			}
		}

		public static PlayerControl ReadNullablePlayer(this MessageReader Reader) {
			var PlayerId = Reader.ReadInt32();

			return PlayerId == -1 ? null : PlayerTools.GetPlayerById((byte) PlayerId);
		}
	}
}