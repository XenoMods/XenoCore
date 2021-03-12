using System.Collections.Generic;

namespace XenoCore.Commands.Impl {
	public class HelpCommand : ICommand {
		public string Name() => "help";
		public string Usage() => "/help [Command Name]";

		public void Run(ChatCallback Callback, List<string> Args) {
			if (Args.Count == 0) {
				CommandsController.Help(Callback);
			} else {
				CommandsController.Help(Callback, Args[0]);
			}
		}
	}
}