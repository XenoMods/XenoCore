using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;
using HarmonyLib;
using Reactor.Patches;
using UnityEngine;

namespace XenoCore.Utils {
	/**
	 * Управляет выводом названий и версий загруженных модов
	 */
	public static class VersionsList {
		private const string CHECK_URL = "http://msys.northeurope.cloudapp.azure.com/public/repo/check.php";
		private static readonly List<ModDefinition> Mods = new List<ModDefinition>();
		private static TextRenderer VersionRenderer;
		private static string OriginalText;

		private static bool VersionsChecked;

		internal static void CheckVersions() {
			if (VersionsChecked) {
				return;
			}

			VersionsChecked = true;
			
			var Errors = new List<string>();
			
			foreach (var Mod in Mods) {
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
				Message.DisconnectShow(string.Join("\n", Errors));
			}
		}

		internal static void Init() {
			ReactorVersionShower.TextUpdated += Text => {
				VersionRenderer = Text;
				OriginalText = Text.Text;
				Refresh();
			};
		}

		public static void Add(string Id, string Version, bool CheckVersion = false) {
			Mods.Add(new ModDefinition(Id, Version, CheckVersion));
			Refresh();
		}

		private static void Refresh() {
			if (VersionRenderer == null) {
				return;
			}

			VersionRenderer.Text = string.Join('\n', Mods)
			                       + '\n' + ModDefinition.CLEAR_COLOR + OriginalText;
		}

		internal class ModDefinition {
			public static readonly string CLEAR_COLOR = $"[{Color.white.ToHexRGBA()}]";
			public readonly string Id;
			public readonly string Version;
			public readonly bool CheckVersion;

			public ModDefinition(string id, string version, bool checkVersion) {
				Id = id;
				Version = version;
				CheckVersion = checkVersion;
			}

			public override string ToString() {
				return $"{CLEAR_COLOR}{Id} v{Version}";
			}
		}

		// ReSharper disable once ClassNeverInstantiated.Global
		[DataContract]
		internal class CheckVersionResult {
			[DataMember] public bool success { get; set; }
			[DataMember] public string message { get; set; }
		}
	}


	[HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
	public class VersionCheckPatch {
		public static void Postfix() {
			VersionsList.CheckVersions();
		}
	}
}