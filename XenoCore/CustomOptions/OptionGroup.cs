namespace XenoCore.CustomOptions {
	public sealed class OptionGroup {
		public readonly int Order;

		public OptionGroup(int Order) {
			this.Order = Order;
		}

		public OptionGroup Up(int Distance = 1) {
			return new OptionGroup(Order - Distance);
		}
			
		public OptionGroup Down(int Distance = 1) {
			return new OptionGroup(Order + Distance);
		}
	}
}