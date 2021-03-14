using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;
using Reactor.Patches;
using XenoCore.Core;
using XenoCore.Events;

namespace XenoCore.Utils {
	/**
	 * Управляет выводом названий и версий загруженных модов
	 */
	internal static class VersionsList {
		private const string CHECK_URL = "http://msys.northeurope.cloudapp.azure.com/public/repo/check.php";
		private static TextRenderer VersionRenderer;
		private static string OriginalText;

		private static bool VersionsChecked;

		internal static void CheckVersions() {
			if (VersionsChecked) {
				return;
			}

			VersionsChecked = true;
			
			var Errors = new List<string>();
			
			foreach (var Mod in XenoMods.GetMods()) {
				if (!Mod.CheckVersion) continue;

				var Client = new WebClient();
				Client.QueryString.Add("id", Mod.Id);
				Client.QueryString.Add("v", Mod.Version);

				var ResultText = Client.DownloadString(CHECK_URL);
				var Lines = ResultText.Split('\n');

				if (Lines.Length < 2) {
					Errors.Add("Ошибка при обращении к сервису обновления модов");
					break;
				}

				if (Lines[0].Trim() == "0") {
					Errors.Add(Lines[1]);
				}
			}

			if (Errors.Count > 0) {
				MessageUtils.DisconnectShow(string.Join("\n", Errors));
			}
		}

		internal static void Init() {
			EventsController.HUD_INIT.Register(CheckVersions);
			ReactorVersionShower.TextUpdated += Text => {
				VersionRenderer = Text;
				OriginalText = Text.Text;
				Refresh();
			};
		}

		internal static void Refresh() {
			if (VersionRenderer == null) {
				return;
			}

			VersionRenderer.Text = string.Join('\n', XenoMods.GetMods())
			                       + '\n' + Globals.FORMAT_WHITE + OriginalText;
		}

		// ReSharper disable once ClassNeverInstantiated.Global
		[DataContract]
		internal class CheckVersionResult {
			[DataMember] public bool success { get; set; }
			[DataMember] public string message { get; set; }
		}
	}
}