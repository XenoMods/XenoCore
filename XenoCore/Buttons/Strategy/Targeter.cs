using XenoCore.Utils;

namespace XenoCore.Buttons.Strategy {
	/**
	 * Стратегия определения текущей цели кнопки
	 */
	public interface ButtonTargeter {
		PlayerControl GetTarget(bool Active);
	}

	/**
	 * Кнопка не ищет цель
	 */
	public class NoneTargeter : ButtonTargeter {
		public static readonly ButtonTargeter INSTANCE = new NoneTargeter();
		
		private NoneTargeter() {
		}
		
		public PlayerControl GetTarget(bool Active) {
			return null;
		}
	}

	/**
	 * Кнопка ищет ближайшего игрока
	 */
	public class ClosestTargeter : ButtonTargeter {
		public static readonly ButtonTargeter INSTANCE = new ClosestTargeter();
		
		private ClosestTargeter() {
		}

		public PlayerControl GetTarget(bool Active) {
			return PlayerTools.ClosestDistance.IsInKillRange() ? PlayerTools.ClosestPlayer : null;
		}
	}

	/**
	 * Кнопка ищет ближайшего игрока при флаге Active
	 */
	public class ClosestActiveTargeter : ButtonTargeter {
		public static readonly ButtonTargeter INSTANCE = new ClosestActiveTargeter();
		
		private ClosestActiveTargeter() {
		}

		public PlayerControl GetTarget(bool Active) {
			return Active && PlayerTools.ClosestDistance.IsInKillRange()
				? PlayerTools.ClosestPlayer
				: null;
		}
	}

	/**
	 * Кнопка ищет ближайшего игрока, не проверяя дальность убийства
	 */
	public class ClosestIgnoreRangeTargeter : ButtonTargeter {
		public static readonly ButtonTargeter INSTANCE = new ClosestIgnoreRangeTargeter();
		
		private ClosestIgnoreRangeTargeter() {
		}

		public PlayerControl GetTarget(bool Active) {
			return PlayerTools.ClosestPlayer;
		}
	}
	
	/**
	 * Кнопка ищет ближайшего игрока, не проверяя дальность убийства, но только при флаге Active
	 */
	public class ClosestIgnoreRangeActiveTargeter : ButtonTargeter {
		public static readonly ButtonTargeter INSTANCE = new ClosestIgnoreRangeActiveTargeter();
		
		private ClosestIgnoreRangeActiveTargeter() {
		}

		public PlayerControl GetTarget(bool Active) {
			return Active ? PlayerTools.ClosestPlayer : null;
		}
	}
}