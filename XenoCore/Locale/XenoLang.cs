namespace XenoCore.Locale {
	public static class XenoLang {
		public static readonly LanguageEntry ON = new LanguageEntry {
			Id = "xeno.on",
			English = "On",
			Russian = "Вкл."
		};

		public static readonly LanguageEntry OFF = new LanguageEntry {
			Id = "xeno.off",
			English = "Off",
			Russian = "Выкл."
		};

		public static readonly LanguageEntry DEFEAT = new LanguageEntry {
			Id = "xeno.defeat",
			English = "Defeat",
			Russian = "Поражение"
		};

		public static readonly LanguageEntry VICTORY = new LanguageEntry {
			Id = "xeno.victory",
			English = "Victory",
			Russian = "Победа"
		};
		
		public static readonly LanguageEntry CREWMATE = new LanguageEntry {
			Id = "xeno.crewmate",
			English = "Crewmate",
			Russian = "Член экипажа"
		};
		
		public static readonly LanguageEntry IMPOSTOR = new LanguageEntry {
			Id = "xeno.impostor",
			English = "Impostor",
			Russian = "Предатель"
		};
		
		public static readonly LanguageEntry COMMAND_NOT_FOUND = new LanguageEntry {
			Id = "xeno.commands.not_found",
			English = "Command \"%1\" not found",
			Russian = "Команды \"%1\" не существует"
		};
		
		public static readonly LanguageEntry AVAILABLE_COMMANDS = new LanguageEntry {
			Id = "xeno.commands.available",
			English = "Available commands: %1",
			Russian = "Доступные команды: %1"
		};
		
		public static readonly LanguageEntry USAGE = new LanguageEntry {
			Id = "xeno.commands.usage",
			English = "Usage: %1",
			Russian = "Использование: %1"
		};
		
		public static readonly LanguageEntry USAGE_ALL = new LanguageEntry {
			Id = "xeno.commands.usage_all",
			English = "All commands:\n%1",
			Russian = "Все команды:\n%1"
		};
		
		public static readonly LanguageEntry MAPS_CUSTOM = new LanguageEntry {
			Id = "xeno.maps.custom",
			English = "Map Override",
			Russian = "Переопределение карты"
		};
		public static readonly LanguageEntry MAPS_NONE = new LanguageEntry {
			Id = "xeno.maps.none",
			English = "None",
			Russian = "Пусто"
		};
		
		internal static void Init() {
			LanguageManager.Add(ON);
			LanguageManager.Add(OFF);
			LanguageManager.Add(DEFEAT);
			LanguageManager.Add(VICTORY);
			LanguageManager.Add(CREWMATE);
			LanguageManager.Add(IMPOSTOR);
			
			LanguageManager.Add(COMMAND_NOT_FOUND);
			LanguageManager.Add(AVAILABLE_COMMANDS);
			LanguageManager.Add(USAGE);
			LanguageManager.Add(USAGE_ALL);
			
			LanguageManager.Add(MAPS_CUSTOM);
			LanguageManager.Add(MAPS_NONE);
		}
	}
}