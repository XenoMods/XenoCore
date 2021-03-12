using BepInEx;
using BepInEx.IL2CPP;
using BepInEx.Logging;
using HarmonyLib;
using Reactor;
using XenoCore.CustomOptions;
using XenoCore.Events;
using XenoCore.Locale;
using XenoCore.Override.Map;
using XenoCore.Override.Map.Components;
using XenoCore.Override.Tasks;
using XenoCore.Override.Usable;
using XenoCore.Override.Vents;
using XenoCore.Skin;
using XenoCore.Utils;

namespace XenoCore {
	[BepInPlugin(Id)]
	[BepInProcess(Globals.PROCESS)]
	[BepInDependency(ReactorPlugin.Id)]
	[ReactorPluginSide(PluginSide.ClientOnly)]
	public class XenoPlugin : BasePlugin {
		public const string Id = "com.mishin870.xenocore";
		public static readonly string Version = "1.0.4";
		public static XenoPlugin Instance => PluginSingleton<XenoPlugin>.Instance;

		internal static readonly OptionGroup XENO_GROUP = CustomOption.DEFAULT_GROUP.Up(10_000);

		private Harmony Harmony { get; } = new Harmony(Id);

		internal static ManualLogSource Logger;

		public override void Load() {
			Logger = Log;
			Harmony.PatchAll();
			PluginSingleton<XenoPlugin>.Instance = this;
			RegisterInIl2CppAttribute.Register();
			RegisterCustomRpcAttribute.Register(this);
			
			EventsController.Init();

			AudioManager.Init();
			LanguageManager.Init();
			XenoLang.Init();

			UsableController.Init();
			TasksOverlay.Init();

			VentSkins.Init();
			CustomVentsController.Init();

			CustomMapController.Init();
			PseudoComponentsRegistry.Init();
			
			SkinsController.Init();

			VersionsList.Add("XenoCore", Version, true);
			VersionsList.Init();
		}
	}
}