using System;
using Hazel;

namespace XenoCore.Utils {
	public static class Network {
		private static MessageWriter Start(byte RPCId) {
			return AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
				RPCId, SendOption.Reliable, -1);
		}

		private static void End(this MessageWriter Writer) {
			AmongUsClient.Instance.FinishRpcImmediately(Writer);
		}

		public static void Send(byte RPCId) {
			End(Start(RPCId));
		}
		
		public static void Send(byte RPCId, Action<MessageWriter> WriteData) {
			var Writer = Start(RPCId);
			WriteData(Writer);
			End(Writer);
		}
	}
}