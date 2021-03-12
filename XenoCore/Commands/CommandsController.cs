using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using XenoCore.Commands.Impl;
using XenoCore.Locale;

namespace XenoCore.Commands {
	[HarmonyPatch(typeof(ChatController), nameof(ChatController.SendChat))]
	public class ChatPatch {
		public static bool Prefix(ChatController __instance) {
			if (__instance.TextArea.text.StartsWith("/")) {
				CommandsController.Run(new ChatCallback(__instance),
					__instance.TextArea.text);
				__instance.TextArea.Clear();
				return false;
			}

			return true;
		}
	}
	
	public static class CommandsController {
		private static readonly Dictionary<string, ICommand> Commands
			= new Dictionary<string, ICommand>();

		static CommandsController() {
			Register(new HelpCommand());
			Register(new VanillaWinCommand());
		}
		
		public static void Register(ICommand Command) {
			Commands.Add(Command.Name(), Command);
		}

		public static void Help(ChatCallback Callback, string CommandName = null) {
			if (CommandName == null) {
				foreach (var Command in Commands.Values) {
					Callback.Usage(Command.Usage());
				}
			} else {
				if (Commands.ContainsKey(CommandName)) {
					Callback.Usage(Commands[CommandName].Usage());
				} else {
					NotFound(Callback, CommandName);
				}
			}
		}

		private static void AllCommands(ChatCallback Callback) {
			Callback.Send(XenoLang.AVAILABLE_COMMANDS.Get()
				.Replace("%1", string.Join(", ", Commands.Keys)));
		}

		private static void NotFound(ChatCallback Callback, string CommandName) {
			Callback.Send(XenoLang.COMMAND_NOT_FOUND.Get()
				.Replace("%1", CommandName));
		}

		public static void Run(ChatCallback Callback, string ChatText) {
			if (ChatText.StartsWith("/")) {
				ChatText = ChatText.Substring(1);
			}

			if (ChatText.Length == 0) {
				AllCommands(Callback);
				return;
			}

			var Parts = ChatText.Split(' ')
				.Where(Part => Part.Trim().Length > 0)
				.ToList();

			if (Commands.ContainsKey(Parts[0])) {
				Commands[Parts[0]].Run(Callback, Parts.Skip(1).ToList());
			} else {
				NotFound(Callback, Parts[0]);
				AllCommands(Callback);
			}
		}
	}
}