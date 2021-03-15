using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

		private static string Prepare(string Source) {
			return Source.Replace('[', '{')
				.Replace(']', '}');
		}

		public void Usage(string UsageText) {
			Send(XenoLang.USAGE.Get().Replace("%1", Prepare(UsageText)));
		}
		
		public void Usage(IEnumerable<string> Usages) {
			Send(XenoLang.USAGE_ALL.Get().Replace("%1",
				string.Join("\n", Usages.Select(Prepare))));
		}
	}
}