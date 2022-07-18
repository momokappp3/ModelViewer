using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using CW.Common;

namespace Lean.Common
{
	//���͈������t�H�[�}�b�g���ꂽ���̂ɕϊ� OnString �C�x���g�ɏo��
	[HelpURL(LeanCommon.HelpUrlPrefix + "LeanSelectable")]
	[AddComponentMenu(LeanCommon.ComponentPathPrefix + "Selectable")]
	public class LeanSelectable : MonoBehaviour
	{
		[System.Serializable] public class LeanSelectEvent : UnityEvent<LeanSelect> {}

		public static LinkedList<LeanSelectable> Instances = new LinkedList<LeanSelectable>(); [System.NonSerialized] private LinkedListNode<LeanSelectable> instancesNode;

		public bool SelfSelected { set { if (selfSelected != value) { selfSelected = value; if (value == true) InvokeOnSelected(null); else InvokeOnDeslected(null); } } get { return selfSelected; } } [SerializeField] private bool selfSelected;

		//���̃I�u�W�F�N�g���I������邽�тɌĂяo�����LeanSelect = �I���̌����ƂȂ����R���|�[�l���g
		//�inull =���ȑI���j���F����͕����񔭐�����\��������
		public LeanSelectEvent OnSelected { get { if (onSelected == null) onSelected = new LeanSelectEvent(); return onSelected; } }
		[SerializeField] private LeanSelectEvent onSelected;

		//���̃I�u�W�F�N�g���I������邽�тɌĂяo�����LeanSelect = �I���̌����ƂȂ����R���|�[�l���g
		//�inull =���ȑI���j���F����͕����񔭐�����\��������
		public LeanSelectEvent OnDeselected { get { if (onDeselected == null) onDeselected = new LeanSelectEvent(); return onDeselected; } }
		[SerializeField] private LeanSelectEvent onDeselected;

		public static event System.Action<LeanSelectable> OnAnyEnabled;

		public static event System.Action<LeanSelectable> OnAnyDisabled;

		public static event System.Action<LeanSelect, LeanSelectable> OnAnySelected;

		public static event System.Action<LeanSelect, LeanSelectable> OnAnyDeselected;

		protected static List<LeanSelectable> tempSelectables = new List<LeanSelectable>();

		//����ɂ��A�V�[�����Ō��݂��̃I�u�W�F�N�g���I������Ă��� LeanSelect �R���|�[�l���g�̐����킩��
		public int SelectedCount
		{
			get
			{
				var count = 0;

				if (selfSelected == true)
				{
					count += 1;
				}

				foreach (var select in LeanSelect.Instances)
				{
					if (select.IsSelected(this) == true)
					{
						count += 1;
					}
				}
				return count;
			}
		}

		//���̃I�u�W�F�N�g�����ȑI������Ă��邩�A�V�[������ LeanSelect�R���|�[�l���g�ɂ���đI������Ă��邩������
		public bool IsSelected
		{
			get
			{
				if (selfSelected == true)
				{
					return true;
				}

				foreach (var select in LeanSelect.Instances)
				{
					if (select.IsSelected(this) == true)
					{
						return true;
					}
				}
				return false;
			}
		}

		public static int IsSelectedCount
		{
			get
			{
				var count = 0;

				foreach (var selectable in Instances)
				{
					if (selectable.IsSelected == true)
					{
						count += 1;
					}
				}
				return count;
			}
		}

		[ContextMenu("Deselect")]
		public void Deselect()
		{
			SelfSelected = false;

			foreach (var select in LeanSelect.Instances)
			{
				select.Deselect(this);
			}
		}

		//����ɂ��A�V�[�����̂��ׂẴI�u�W�F�N�g�̑I�������������
		public static void DeselectAll()
		{
			foreach (var select in LeanSelect.Instances)
			{
				select.DeselectAll();
			}

			foreach (var selectable in LeanSelectable.Instances)
			{
				selectable.SelfSelected = false;
			}
		}

		public void InvokeOnSelected(LeanSelect select)
		{
			if (onSelected != null)
			{
				onSelected.Invoke(select);
			}

			if (OnAnySelected != null)
			{
				OnAnySelected.Invoke(select, this);
			}
		}

		public void InvokeOnDeslected(LeanSelect select)
		{
			if (onDeselected != null)
			{
				onDeselected.Invoke(select);
			}

			if (OnAnyDeselected != null)
			{
				OnAnyDeselected.Invoke(select, this);
			}
		}

		protected virtual void OnEnable()
		{
			instancesNode = Instances.AddLast(this);

			if (OnAnyEnabled != null)
			{
				OnAnyEnabled.Invoke(this);
			}
		}

		protected virtual void OnDisable()
		{
			Instances.Remove(instancesNode); instancesNode = null;

			if (OnAnyDisabled != null)
			{
				OnAnyDisabled.Invoke(this);
			}
		}

		protected virtual void OnDestroy()
		{
			Deselect();
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Common.Editor
{
	using UnityEditor;
	using TARGET = LeanSelectable;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class LeanSelectable_Editor : CwEditor
	{
		[System.NonSerialized] TARGET tgt; [System.NonSerialized] TARGET[] tgts;

		protected override void OnInspector()
		{
			GetTargets(out tgt, out tgts);

			DrawSelected();

			Separator();

			var showUnusedEvents = DrawFoldout("Show Unused Events", "Show all events?");

			DrawEvents(showUnusedEvents);
		}

		private void DrawSelected()
		{
			BeginDisabled();
				EditorGUILayout.Toggle(new GUIContent("Is Selected", "This will tell you if this object is self selected, or selected by any LeanSelect components in the scene."), tgt.IsSelected);
			EndDisabled();
			BeginIndent();
				if (Draw("selfSelected") == true)
				{
					Each(tgts, t => t.SelfSelected = serializedObject.FindProperty("selfSelected").boolValue, true);
				}
				BeginDisabled();
					foreach (var select in LeanSelect.Instances)
					{
						if (IsSelectedByAnyTgt(select) == true)
						{
							EditorGUILayout.ObjectField(new GUIContent("selectedBy"), select, typeof(LeanSelect), true);
						}
					}
				EndDisabled();
			EndIndent();
		}

		private bool IsSelectedByAnyTgt(LeanSelect select)
		{
			foreach (var tgt in tgts)
			{
				if (select.IsSelected(tgt) == true)
				{
					return true;
				}
			}
			return false;
		}

		protected virtual void DrawEvents(bool showUnusedEvents)
		{
			if (showUnusedEvents == true || Any(tgts, t => t.OnSelected.GetPersistentEventCount() > 0))
			{
				Draw("onSelected");
			}

			if (showUnusedEvents == true || Any(tgts, t => t.OnDeselected.GetPersistentEventCount() > 0))
			{
				Draw("onDeselected");
			}
		}
	}
}
#endif