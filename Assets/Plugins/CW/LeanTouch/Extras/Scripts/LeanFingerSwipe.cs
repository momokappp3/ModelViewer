using UnityEngine;
using Lean.Common;

namespace Lean.Touch
{
	//指が画面上をスワイプした場合にイベントを発生
	//スワイプ = LeanTouch.TapThreshold時間内に開始又は終了
	//LeanTouch.SwipeThreshold距離を超えて移動したタッチとして定義される
	[HelpURL(LeanTouch.HelpUrlPrefix + "LeanFingerSwipe")]
	[AddComponentMenu(LeanTouch.ComponentPathPrefix + "Finger Swipe")]
	public class LeanFingerSwipe : LeanSwipeBase
	{
		//StartedOverGuiで指を無視しするか
		public bool IgnoreStartedOverGui { set { ignoreStartedOverGui = value; } get { return ignoreStartedOverGui; } }
		[SerializeField] private bool ignoreStartedOverGui = true;

		//OverGuiで指を無視するか
		public bool IgnoreIsOverGui { set { ignoreIsOverGui = value; } get { return ignoreIsOverGui; } }
		[SerializeField] private bool ignoreIsOverGui;

		//指定されたオブジェクトが設定されていて選択されていない場合、このコンポーネントは何もしない
		public LeanSelectable RequiredSelectable { set { requiredSelectable = value; } get { return requiredSelectable; } }
		[SerializeField] private LeanSelectable requiredSelectable;

#if UNITY_EDITOR
		protected virtual void Reset()
		{
			requiredSelectable = GetComponentInParent<LeanSelectable>();
		}
#endif

		protected virtual void Start()
		{
			if (requiredSelectable == null)
			{
				requiredSelectable = GetComponentInParent<LeanSelectable>();
			}
		}

		protected virtual void OnEnable()
		{
			LeanTouch.OnFingerSwipe += HandleFingerSwipe;
		}

		protected virtual void OnDisable()
		{
			LeanTouch.OnFingerSwipe -= HandleFingerSwipe;
		}

		private void HandleFingerSwipe(LeanFinger finger)
		{
			if (ignoreStartedOverGui == true && finger.StartedOverGui == true)
			{
				return;
			}

			if (ignoreIsOverGui == true && finger.IsOverGui == true)
			{
				return;
			}

			if (requiredSelectable != null && requiredSelectable.IsSelected == false)
			{
				return;
			}
			HandleFingerSwipe(finger, finger.StartScreenPosition, finger.ScreenPosition);
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Touch.Editor
{
	using UnityEditor;
	using TARGET = LeanFingerSwipe;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class LeanFingerSwipe_Editor : LeanSwipeBase_Editor
	{
		protected override void OnInspector()
		{
			TARGET tgt;
			TARGET[] tgts;

			GetTargets(out tgt, out tgts);
			Draw("ignoreStartedOverGui", "Ignore fingers with StartedOverGui?");
			Draw("ignoreIsOverGui", "Ignore fingers with OverGui?");
			Draw("requiredSelectable", "If the specified object is set and isn't selected, then this component will do nothing.");
			base.OnInspector();
		}
	}
}
#endif