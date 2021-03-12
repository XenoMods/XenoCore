using XenoCore.Events;

namespace XenoCore.Skin {
	internal static class SkinsController {
		internal static void Init() {
			EventsController.HUD_INIT.Register(() => {
				PetsController.Inject();
				HatsController.Inject();
			});
		}
	}
}