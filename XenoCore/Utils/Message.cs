namespace XenoCore.Utils {
	public static class Message {
		public static void DisconnectShow(string Message) {
			var Client = AmongUsClient.Instance;

			Client.LastDisconnectReason = DisconnectReasons.Custom;
			Client.LastCustomDisconnect = Message;
			Client.HandleDisconnect(Client.LastDisconnectReason, Client.LastCustomDisconnect);
		}
		
		public static void Show(string Message) {
			DisconnectPopup.Instance.ShowCustom(Message);
		}
	}
}