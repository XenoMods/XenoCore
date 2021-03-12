using System;
using Reactor;
using UnityEngine;

namespace XenoCore.Override.Vents.Button {
	[RegisterInIl2Cpp]
	public class CustomButtonBehavior : MonoBehaviour {
		public bool OnUp = true;
		public bool OnDown;
		public bool Repeat;
		public Action OnClick;
		private Controller myController = new Controller();
		private Collider2D[] colliders;
		private float downTime;

		public CustomButtonBehavior() {
		}

		public CustomButtonBehavior(IntPtr obj0) : base(obj0) {
		}

		public void OnEnable() {
			colliders = GetComponents<Collider2D>();
			myController.Reset();
		}

		public void Update() {
			myController.Update();
			var array = colliders;
			foreach (var coll in array) {
				switch (myController.CheckDrag(coll)) {
					case DragState.TouchStart:
						if (OnDown) {
							OnClick();
						}

						break;
					case DragState.Released:
						if (OnUp) {
							OnClick();
						}

						break;
					case DragState.Dragging:
						if (Repeat) {
							downTime += Time.fixedDeltaTime;
							if (downTime >= 0.3f) {
								downTime = 0f;
								OnClick();
							}
						} else {
							downTime = 0f;
						}

						break;
				}
			}
		}
	}

	public class Controller {
		public class TouchState {
			public Vector2 DownAt;
			public Vector2 Position;
			public bool WasDown;
			public bool IsDown;
			public bool TouchStart;
			public bool TouchEnd;
			public DragState dragState;
		}

		public readonly TouchState[] Touches = new TouchState[2];
		private Collider2D amTouching;
		private int touchId = -1;

		public bool AnyTouch => Touches[0].IsDown || Touches[1].IsDown;

		public bool AnyTouchDown {
			get {
				if (!Touches[0].TouchStart) {
					return Touches[1].TouchStart;
				}

				return true;
			}
		}

		public bool AnyTouchUp {
			get {
				if (!Touches[0].TouchEnd) {
					return Touches[1].TouchEnd;
				}

				return true;
			}
		}

		public bool FirstDown => Touches[0].TouchStart;
		public Vector2 DragPosition => touchId < 0 ? Vector2.zero : Touches[touchId].Position;
		public Vector2 DragStartPosition => Touches[touchId].DownAt;
		public Camera mainCam { get; set; }

		public Controller() {
			for (var i = 0; i < Touches.Length; i++) {
				Touches[i] = new TouchState();
			}
		}

		public DragState CheckDrag(Collider2D coll) {
			if (coll == null) {
				return DragState.NoTouch;
			}

			if (touchId > -1 && (amTouching == null || !amTouching.isActiveAndEnabled)) {
				touchId = -1;
				amTouching = null;
			}

			if (touchId > -1) {
				if (coll != amTouching) {
					return DragState.NoTouch;
				}

				TouchState touchState = Touches[touchId];
				if (touchState.IsDown) {
					if (Vector2.Distance(touchState.DownAt, touchState.Position) > 0.05f ||
					    touchState.dragState == DragState.Dragging) {
						touchState.dragState = DragState.Dragging;
						return DragState.Dragging;
					}

					touchState.dragState = DragState.Holding;
					return DragState.Holding;
				}

				amTouching = null;
				touchId = -1;
				touchState.dragState = DragState.Released;
				return DragState.Released;
			}

			for (int i = 0; i < Touches.Length; i++) {
				TouchState touchState2 = Touches[i];
				if (touchState2.TouchStart && coll.OverlapPoint(touchState2.Position)) {
					amTouching = coll;
					touchId = i;
					touchState2.dragState = DragState.TouchStart;
					return DragState.TouchStart;
				}
			}

			return DragState.NoTouch;
		}

		public void ResetDragPosition() {
			Touches[touchId].DownAt = Touches[touchId].Position;
		}

		public void ClearTouch() {
			if (touchId >= 0) {
				TouchState obj = Touches[touchId];
				obj.dragState = DragState.NoTouch;
				obj.TouchStart = true;
				amTouching = null;
				touchId = -1;
			}
		}

		public void Update() {
			if (mainCam == null) {
				mainCam = Camera.main;
			}

			var touchState = Touches[0];
			var mouseButton = Input.GetMouseButton(0);
			touchState.Position = mainCam.ScreenToWorldPoint(Input.mousePosition);
			touchState.TouchStart = !touchState.IsDown && mouseButton;
			if (touchState.TouchStart) {
				touchState.DownAt = touchState.Position;
			}

			touchState.TouchEnd = touchState.IsDown && !mouseButton;
			touchState.IsDown = mouseButton;
		}

		public void Reset() {
			for (int i = 0; i < Touches.Length; i++) {
				Touches[i] = new TouchState();
			}

			touchId = -1;
			amTouching = null;
		}

		public TouchState GetTouch(int i) {
			return Touches[i];
		}
	}
}