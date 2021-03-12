namespace XenoCore.Utils {
	public interface INumberProvider {
		float GetValue();
	}
	
	public sealed class ConstantNumberProvider : INumberProvider {
		private readonly float Value;

		public ConstantNumberProvider(float Value) {
			this.Value = Value;
		}

		public float GetValue() {
			return Value;
		}
	}
}