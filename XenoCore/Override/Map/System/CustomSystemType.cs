using Hazel;

namespace XenoCore.Override.Map.System {
	public abstract class CustomSystemType {
		private static int CurrentIndex = 548_254;
		internal readonly int Index;
		
		protected CustomSystemType() {
			Index = CurrentIndex;
			CurrentIndex++;
		}

		public abstract bool IsActive { get; }
		public abstract bool Detoriorate(float deltaTime);
		public abstract void RepairDamage(PlayerControl player, byte amount);
		public abstract void Serialize(MessageWriter writer, bool initialState);
		public abstract void Deserialize(MessageReader reader, bool initialState);

		public ISystemType ToSystemType() {
			return new HqHudSystemType {
				TargetNumber = Index
			}.Cast<ISystemType>();
		}
	}
}