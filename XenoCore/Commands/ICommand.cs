using System.Collections.Generic;

namespace XenoCore.Commands {
	public interface ICommand {
		string Name();
		string Usage() => $"/{Name()}";
		void Run(ChatCallback Callback, List<string> Args);
	}
}