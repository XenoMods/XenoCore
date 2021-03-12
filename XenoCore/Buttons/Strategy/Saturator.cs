namespace XenoCore.Buttons.Strategy {
	/**
	 * Стратегия подсветки кнопки активным цветом
	 */
	public interface ButtonSaturator {
		bool Saturated(float KD, bool Active, PlayerControl Target);
	}
	
	/**
	 * Кнопка всегда неактивна
	 */
	public class NoneSaturator : ButtonSaturator {
		public static readonly ButtonSaturator INSTANCE = new NoneSaturator();
		
		private NoneSaturator() {
		}

		public bool Saturated(float KD, bool Active, PlayerControl Target) {
			return false;
		}
	}

	/**
	 * Кнопка активна при флаге Active и нулевом кулдауне
	 */
	public class ActiveAndKDSaturator : ButtonSaturator {
		public static readonly ButtonSaturator INSTANCE = new ActiveAndKDSaturator();
		
		private ActiveAndKDSaturator() {
		}

		public bool Saturated(float KD, bool Active, PlayerControl Target) {
			return Active && KD == 0;
		}
	}
	
	/**
	 * Кнопка активна при флаге Active
	 */
	public class ActiveSaturator : ButtonSaturator {
		public static readonly ButtonSaturator INSTANCE = new ActiveSaturator();
		
		private ActiveSaturator() {
		}

		public bool Saturated(float KD, bool Active, PlayerControl Target) {
			return Active;
		}
	}
	
	/**
	 * Кнопка активна при флаге Active и не пустой цели
	 */
	public class ActiveAndTargetSaturator : ButtonSaturator {
		public static readonly ButtonSaturator INSTANCE = new ActiveAndTargetSaturator();
		
		private ActiveAndTargetSaturator() {
		}

		public bool Saturated(float KD, bool Active, PlayerControl Target) {
			return Active && Target != null;
		}
	}
	
	/**
	 * Кнопка активна при флаге Active, нулевом кулдауне и не пустой цели
	 */
	public class ActiveKDAndTargetSaturator : ButtonSaturator {
		public static readonly ButtonSaturator INSTANCE = new ActiveKDAndTargetSaturator();
		
		private ActiveKDAndTargetSaturator() {
		}

		public bool Saturated(float KD, bool Active, PlayerControl Target) {
			return Active && KD == 0 && Target != null;
		}
	}
}