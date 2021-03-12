namespace XenoCore.Buttons.Strategy {
	public enum CooldownType {
		/**
		 * Кулдаун отображается как обычно<br/>
		 * Виден только при времени != 0
		 */
		STANDARD,
		
		/**
		 * Кулдаун не отображается на кнопке
		 */
		NO_COOLDOWN
	}

	/**
	 * Стратегия отображения кулдауна на кнопке
	 */
	public interface CooldownStrategy {
		CooldownType GetType(float KD, bool Active, PlayerControl Target);
	}
	
	/**
	 * Кулдаун отображается как обычно
	 */
	public class NoneCooldownStrategy : CooldownStrategy {
		public static readonly CooldownStrategy INSTANCE = new NoneCooldownStrategy();
		
		private NoneCooldownStrategy() {
		}

		public CooldownType GetType(float KD, bool Active, PlayerControl Target) {
			return CooldownType.STANDARD;
		}
	}

	/**
	 * Кулдаун отображается в зависимости от Active флага
	 */
	public class ActiveDependentCooldownStrategy : CooldownStrategy {
		public static readonly CooldownStrategy INSTANCE = new ActiveDependentCooldownStrategy();
		
		private ActiveDependentCooldownStrategy() {
		}

		public CooldownType GetType(float KD, bool Active, PlayerControl Target) {
			return Active ? CooldownType.STANDARD : CooldownType.NO_COOLDOWN;
		}
	}
}