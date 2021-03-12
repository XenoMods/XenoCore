namespace XenoCore.Override {
	public static class DoorsController {
		public static void DestroyDoors() {
			Utils.Extensions.DestroyAllOfType<SomeKindaDoor>();
			Utils.Extensions.DestroyAllOfType<DoorConsole>();
			Utils.Extensions.DestroyAllOfType<DeconControl>();
		}
	}
}