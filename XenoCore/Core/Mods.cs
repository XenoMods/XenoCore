using System.Collections.Generic;
using System.Linq;
using Hazel;
using XenoCore.Utils;

namespace XenoCore.Core {
	public class XenoMod {
		private readonly NetworkRegistry Network;
		
		public readonly string Id;
		public readonly string Name;
		public readonly string Version;
		public readonly bool CheckVersion;

		internal int InternalId;

		public XenoMod(string Id, string Name, string Version, bool CheckVersion = false,
			bool Register = true) {
			this.Id = Id;
			this.Name = Name;
			this.Version = Version;
			this.CheckVersion = CheckVersion;
			
			if (Register) {
				DoRegister();
			}

			Network = new NetworkRegistry(this);
		}

		public void DoRegister() {
			XenoMods.Register(this);
		}

		public void RegisterMessage(Message Message) {
			Network.Register(Message);
		}

		internal Message MessageById(int MessageId) {
			return Network.ById(MessageId);
		}

		public override string ToString() {
			return $"{Globals.FORMAT_WHITE}{Name} v{Version}";
		}
	}
	
	internal static class XenoMods {
		private static readonly List<XenoMod> ModsList = new List<XenoMod>();
		private static readonly Dictionary<int, XenoMod> Mods = new Dictionary<int, XenoMod>();
		private static int CurrentId;
		
		internal static void Register(XenoMod Mod) {
			Mod.InternalId = CurrentId;
			CurrentId++;
			
			ModsList.Add(Mod);
			Mods.Add(Mod.InternalId, Mod);
			VersionsList.Refresh();
		}

		internal static XenoMod ById(int InternalId) {
			return Mods[InternalId];
		}

		internal static IEnumerable<XenoMod> GetMods() {
			return Mods.Values;
		}

		internal static void Synchronize() {
			SynchronizeXenoModsMessage.INSTANCE.Send(Mods);
		}

		internal static void ReadSync(MessageReader Reader) {
			Mods.Clear();
			var Count = Reader.ReadInt32();

			for (var i = 0; i < Count; i++) {
				var InternalId = Reader.ReadInt32();
				var ModId = Reader.ReadString();
				var Mod = Mods.Values.First(SomeMod => SomeMod.Id == ModId);

				Mod.InternalId = InternalId;
				Mods.Add(InternalId, Mod);
			}
		}
	}
	
	internal class SynchronizeXenoModsMessage : Message {
		public static readonly SynchronizeXenoModsMessage INSTANCE = new SynchronizeXenoModsMessage();

		private SynchronizeXenoModsMessage() {
		}
		
		protected override void Handle() {
			XenoMods.ReadSync(Reader);
		}

		public void Send(Dictionary<int, XenoMod> Mods) {
			Write(Writer => {
				Writer.Write(Mods.Count);

				foreach (var (InternalId, Mod) in Mods) {
					Writer.Write(InternalId);
					Writer.Write(Mod.Id);
				}
			});
		}
	}
}