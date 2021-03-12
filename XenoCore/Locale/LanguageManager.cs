using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using XenoCore.Events;

namespace XenoCore.Locale {
	public static class LanguageManager {
		public static Language CurrentLanguage { get; private set; }
		private static int CurrentIndex;

		private static readonly Dictionary<string, string>[] Texts = new Dictionary<string, string>[5];
		private static readonly Dictionary<string, string> DEFAULT;
		
		static LanguageManager() {
			for (var Index = 0; Index < 5; Index++) {
				Texts[Index] = new Dictionary<string, string>();
			}

			DEFAULT = Texts[(int) Language.ENGLISH];
		}

		internal static void Init() {
			EventsController.PREFERENCES_CHANGED.Register(Reload);
			Reload();
		}

		private static void Reload() {
			CurrentIndex = (int) SaveManager.LastLanguage;
			CurrentLanguage = (Language) CurrentIndex;
		}

		public static string Get(string Key) {
			var Target = Texts[CurrentIndex];

			if (Target.ContainsKey(Key)) {
				return Target[Key];
			}

			return DEFAULT.ContainsKey(Key) ? DEFAULT[Key] : Key;
		}

		public static void Add(string Key, Language In, string Value) {
			Texts[(int) In].Add(Key, Value);
		}

		public static void Add(string Key, string[] Values) {
			for (var Index = 0; Index < Values.Length; Index++) {
				Texts[Index].Add(Key, Values[Index]);
			}
		}

		private static void Assign(Language In, string Id, string Value) {
			if (Value != null) {
				Texts[(int) In].Add(Id, Value);
			}
		}

		public static void Add(LanguageEntry Entry) {
			Assign(Language.ENGLISH, Entry.Id, Entry.English);
			Assign(Language.SPANISH, Entry.Id, Entry.Spanish);
			Assign(Language.PORTUGUESE, Entry.Id, Entry.Portuguese);
			Assign(Language.KOREAN, Entry.Id, Entry.Korean);
			Assign(Language.RUSSIAN, Entry.Id, Entry.Russian);
		}

		public static void Load(Assembly From, string BasePath = "",
			Dictionary<string, string> Variables = null) {
			LoadInner(From, Language.ENGLISH, "en", BasePath, Variables);
			LoadInner(From, Language.SPANISH, "es", BasePath, Variables);
			LoadInner(From, Language.PORTUGUESE, "pt", BasePath, Variables);
			LoadInner(From, Language.KOREAN, "ko", BasePath, Variables);
			LoadInner(From, Language.RUSSIAN, "ru", BasePath, Variables);
		}

		private static void LoadInner(Assembly From, Language In, string LangCode, string BasePath,
			Dictionary<string, string> Variables) {
			using var Stream = From.GetManifestResourceStream($"{BasePath}{LangCode}.lang");
			if (Stream == null) {
				return;
			}
			
			using var Reader = new StreamReader(Stream);
			var Index = (int) In;

			while (!Reader.EndOfStream) {
				var Line = Reader.ReadLine();
				if (Line == null) {
					break;
				}

				if (Line.Length == 0) {
					continue;
				}

				var Parts = Line.Split('=');

				var Value = string.Join("=", Parts.Skip(1));
				if (Variables != null) {
					foreach (var (PairKey, PairValue) in Variables) {
						Value = Value.Replace(PairKey, PairValue);
					}
				}

				Texts[Index].Add(Parts[0], Value);
			}
		}
	}
}