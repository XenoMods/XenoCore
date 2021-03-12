using System.Collections.Generic;
using XenoCore.Utils;

namespace XenoCore.Commands.Impl {
	public class VanillaWinCommand : ICommand {
		public string Name() => "vwin";
		public string Usage() => "/vwin";

		public void Run(ChatCallback Callback, List<string> Args) {
			DebugTools.DebugWin(PlayerControl.LocalPlayer);
		}
	}
}