using UnityEngine;
using UnityEngine.Events;
using Lean.Common;
using CW.Common;

namespace Lean.Touch
{
	//���̃R���|�[�l���g�́A�w�肳�ꂽ�����𖞂�������A�w����ʂɐG��n�߂��Ƃ��ɒʒm����
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

		//StartedOverGui�Ŏw�𖳎����܂����H
		public bool IgnoreStartedOverGui { set { ignoreStartedOverGui = value; } get { return ignoreStartedOverGui; } } [SerializeField] private bool ignoreStartedOverGui = true;

		//���̃R���|�[�l���g�͂ǂ̓��͂ɔ�������K�v������܂����H
		public ButtonTypes RequiredButtons { set { requiredButtons = value; } get { return requiredButtons; } } [SerializeField] private ButtonTypes requiredButtons = (ButtonTypes)~0;

		//�w�肳�ꂽ�I�u�W�F�N�g���ݒ肳��Ă��đI������Ă��Ȃ��ꍇ�A���̃R���|�[�l���g�͉������Ȃ�
		public LeanSelectable RequiredSelectable { set { requiredSelectable = value; } get { return requiredSelectable; } } [SerializeField] private LeanSelectable requiredSelectable;

		//���̃C�x���g�́A�w����ʂɐG��n�߂��Ƃ��ɏ�L�̏������������ꂽ�ꍇ�ɌĂяo�����
		public LeanFingerEvent OnFinger { get { if (onFinger == null) onFinger = new LeanFingerEvent(); return onFinger; } } [SerializeField] private LeanFingerEvent onFinger;

		//�w���琢�E���W�������邽�߂Ɏg�p����郁�\�b�h�B �ڍׂɂ��ẮALeanScreenDepth�̃h�L�������g���Q�Ƃ��Ă�������
		public LeanScreenDepth ScreenDepth = new LeanScreenDepth(LeanScreenDepth.ConversionType.DepthIntercept);

		//���̃C�x���g�́A�w����ʂɐG��n�߂��Ƃ��ɏ�L�̏������������ꂽ�ꍇ�ɌĂяo�����
		//Vector3=ScreenDepth�ݒ�Ɋ�Â��J�n�_
		public Vector3Event OnWorld { get { if (onWorld == null) onWorld = new Vector3Event(); return onWorld; } } [SerializeField] private Vector3Event onWorld;

		//���̃C�x���g�́A�w����ʂɐG��n�߂��Ƃ��ɏ�L�̏������������ꂽ�ꍇ�ɌĂяo�����
		//Vector2=��ʃX�y�[�X�ł̎w�̈ʒu
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