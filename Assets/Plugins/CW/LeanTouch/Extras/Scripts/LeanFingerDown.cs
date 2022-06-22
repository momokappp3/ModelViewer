using UnityEngine;
using UnityEngine.Events;
using Lean.Common;
using CW.Common;

namespace Lean.Touch
{
	//このコンポーネントは、指定された条件を満たす限り、指が画面に触れ始めたときに通知する
	[HelpURL(LeanTouch.HelpUrlPrefix + "LeanFingerDown")]
	[AddComponentMenu(LeanTouch.ComponentPathPrefix + "Finger Down")]
	public class LeanFingerDown : MonoBehaviour
	{
		[System.Serializable] public class LeanFingerEvent : UnityEvent<LeanFinger> {}
		[System.Serializable] public class Vector3Event : UnityEvent<Vector3> {}
		[System.Serializable] public class Vector2Event : UnityEvent<Vector2> {}

		[System.Flags]
		public enum ButtonTypes
		{
			LeftMouse   = 1 << 0,
			RightMouse  = 1 << 1,
			MiddleMouse = 1 << 2,
			Touch       = 1 << 5
		}

		//StartedOverGuiで指を無視しますか？
		public bool IgnoreStartedOverGui { set { ignoreStartedOverGui = value; } get { return ignoreStartedOverGui; } } [SerializeField] private bool ignoreStartedOverGui = true;

		//このコンポーネントはどの入力に反応する必要がありますか？
		public ButtonTypes RequiredButtons { set { requiredButtons = value; } get { return requiredButtons; } } [SerializeField] private ButtonTypes requiredButtons = (ButtonTypes)~0;

		//指定されたオブジェクトが設定されていて選択されていない場合、このコンポーネントは何もしない
		public LeanSelectable RequiredSelectable { set { requiredSelectable = value; } get { return requiredSelectable; } } [SerializeField] private LeanSelectable requiredSelectable;

		//このイベントは、指が画面に触れ始めたときに上記の条件が満たされた場合に呼び出される
		public LeanFingerEvent OnFinger { get { if (onFinger == null) onFinger = new LeanFingerEvent(); return onFinger; } } [SerializeField] private LeanFingerEvent onFinger;

		//指から世界座標を見つけるために使用されるメソッド。 詳細については、LeanScreenDepthのドキュメントを参照してください
		public LeanScreenDepth ScreenDepth = new LeanScreenDepth(LeanScreenDepth.ConversionType.DepthIntercept);

		//このイベントは、指が画面に触れ始めたときに上記の条件が満たされた場合に呼び出される
		//Vector3=ScreenDepth設定に基づく開始点
		public Vector3Event OnWorld { get { if (onWorld == null) onWorld = new Vector3Event(); return onWorld; } } [SerializeField] private Vector3Event onWorld;

		//このイベントは、指が画面に触れ始めたときに上記の条件が満たされた場合に呼び出される
		//Vector2=画面スペースでの指の位置
		public Vector2Event OnScreen { 
			get 
			{
				if (onScreen == null) onScreen = new Vector2Event();
				return onScreen; 
			} 
		}
		//[SerializeField] private Vector2Event onScreen;
		[SerializeField] public Vector2Event onScreen;

#if UNITY_EDITOR
		protected virtual void Reset()
		{
			requiredSelectable = GetComponentInParent<LeanSelectable>();
		}
#endif

		protected virtual void Awake()
		{
			if (requiredSelectable == null)
			{
				requiredSelectable = GetComponentInParent<LeanSelectable>();
			}
		}

		protected virtual void OnEnable()
		{
			LeanTouch.OnFingerDown += HandleFingerDown;
		}

		protected virtual void OnDisable()
		{
			LeanTouch.OnFingerDown -= HandleFingerDown;
		}

		protected virtual bool UseFinger(LeanFinger finger)
		{
			if (ignoreStartedOverGui == true && finger.IsOverGui == true)
			{
				return false;
			}

			if (RequiredButtonPressed(finger) == false)
			{
				return false;
			}

			if (requiredSelectable != null && requiredSelectable.IsSelected == false)
			{
				return false;
			}

			if (finger.Index == LeanTouch.HOVER_FINGER_INDEX)
			{
				return false;
			}

			return true;
		}

		protected void InvokeFinger(LeanFinger finger)
		{
			if (onFinger != null)
			{
				onFinger.Invoke(finger);
			}

			if (onWorld != null)
			{
				var position = ScreenDepth.Convert(finger.ScreenPosition, gameObject);

				onWorld.Invoke(position);
			}

			if (onScreen != null)
			{
				onScreen.Invoke(finger.ScreenPosition);
			}
		}

		protected virtual void HandleFingerDown(LeanFinger finger)
		{
			if (UseFinger(finger) == true)
			{
				InvokeFinger(finger);
			}
		}

		private bool RequiredButtonPressed(LeanFinger finger)
		{
			if (finger.Index < 0)
			{
				if (CwInput.GetMouseExists() == true)
				{
					if ((requiredButtons & ButtonTypes.LeftMouse) != 0 && CwInput.GetMouseIsHeld(0) == true)
					{
						return true;
					}

					if ((requiredButtons & ButtonTypes.RightMouse) != 0 && CwInput.GetMouseIsHeld(1) == true)
					{
						return true;
					}

					if ((requiredButtons & ButtonTypes.MiddleMouse) != 0 && CwInput.GetMouseIsHeld(2) == true)
					{
						return true;
					}
				}
			}
			else if ((requiredButtons & ButtonTypes.Touch) != 0)
			{
				return true;
			}

			return false;
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Touch.Editor
{
	using UnityEditor;
	using TARGET = LeanFingerDown;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET), true)]
	public class LeanFingerDown_Editor : CwEditor
	{
		protected void DrawIgnore()
		{
			Draw("ignoreStartedOverGui", "Ignore fingers with StartedOverGui?");
		}

		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			DrawIgnore();
			Draw("requiredButtons", "Which inputs should this component react to?");
			Draw("requiredSelectable", "If the specified object is set and isn't selected, then this component will do nothing.");

			Separator();

			var showUnusedEvents = DrawFoldout("Show Unused Events", "Show all events?");

			Separator();

			if (Any(tgts, t => t.OnFinger.GetPersistentEventCount() > 0) == true || showUnusedEvents == true)
			{
				Draw("onFinger");
			}

			if (Any(tgts, t => t.OnWorld.GetPersistentEventCount() > 0) == true || showUnusedEvents == true)
			{
				Draw("ScreenDepth");
				Draw("onWorld");
			}

			if (Any(tgts, t => t.OnScreen.GetPersistentEventCount() > 0) == true || showUnusedEvents == true)
			{
				Draw("onScreen");
			}
		}
	}
}
#endif