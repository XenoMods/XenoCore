using XenoCore.Locale;
using XenoCore.Utils;

namespace XenoCore.Commands {
	public class ChatCallback {
		private readonly ChatController Controller;

		public ChatCallback(ChatController Controller) {
			this.Controller = Controller;
		}

		public void Send(string Message) {
			Controller.AddChat(Message);
		}

		public void Usage(string UsageText) {
			Send(XenoLang.USAGE.Get().Replace("%1", UsageText));
		}
	}
}