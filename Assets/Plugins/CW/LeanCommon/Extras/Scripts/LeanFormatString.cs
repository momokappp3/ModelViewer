using UnityEngine;
using UnityEngine.Events;
using CW.Common;

namespace Lean.Common
{
	//intやfloatなどの値をフォーマットされたテキストに変換できるUIに表示できる
	//使用するには、SetString メソッドの1つを呼び出すだけ
	//フォーマットされた文字列を OnString イベントに出力このイベントは、UIテキストなどに接続できる
	[HelpURL(LeanCommon.HelpUrlPrefix + "LeanFormatString")]
	[AddComponentMenu(LeanCommon.ComponentPathPrefix + "Format String")]
	public class LeanFormatString : MonoBehaviour
	{
		[System.Serializable] public class StringEvent : UnityEvent<string> {}

		//最終テキストはこの文字列フォーマットを使用します。ここで、{0}は最初の値、{1}は2番目の値、
		//フォーマットは標準の<b>string.Format</b>スタイルを使用する
		public string Format { set { format = value; } get { return format; } } [SerializeField] [Multiline] private string format = "Current Value = {0}";

		//設定に基づいて、このイベントが呼び出されます。
		public StringEvent OnString { get { if (onString == null) onString = new StringEvent(); return onString;  } }
		[SerializeField] private StringEvent onString;

		//入力引数をフォーマットされた文字列に変換 OnString</b>イベントに出力
		public void SetString(string a)
		{
			SendString(a);
		}

		//入力引数をフォーマットされた文字列に変換 OnString イベントに出力
		public void SetString(string a, string b)
		{
			SendString(a, b);
		}

		//入力引数をフォーマットされた文字列に変換 OnString イベントに出力
		public void SetString(int a)
		{
			SendString(a);
		}

		//入力引数をフォーマットされた文字列に変換 OnString イベントに出力
		public void SetString(int a, int b)
		{
			SendString(a, b);
		}
		//入力引数をフォーマットされたものに変換 OnString イベントに出力
		public void SetString(float a)
		{
			SendString(a);
		}

		//入力引数をフォーマットされた文字列に変換し、出力 OnString イベントに送信
		public void SetString(float a, float b)
		{
			SendString(a, b);
		}

		//入力引数をフォーマットされたものに変換 OnString イベントに出力
		public void SetString(Vector2 a)
		{
			SendString(a);
		}

		//入力引数をフォーマットされたものに変換 OnString イベントに出力
		public void SetString(Vector2 a, Vector2 b)
		{
			SendString(a, b);
		}
		//入力引数をフォーマットされたものに変換 OnString イベントに出力
		public void SetString(Vector3 a)
		{
			SendString(a);
		}

		//入力引数をフォーマットされたものに変換 OnString イベントに出力
		public void SetString(Vector3 a, Vector3 b)
		{
			SendString(a, b);
		}

		//入力引数をフォーマットされたものに変換 OnString イベントに出力
		public void SetString(Vector4 a)
		{
			SendString(a);
		}

		//入力引数をフォーマットされたものに変換 OnString イベントに出力
		public void SetString(Vector4 a, Vector4 b)
		{
			SendString(a, b);
		}

		//入力引数をフォーマットされたものに変換 OnString イベントに出力
		public void SetString(float a, int b)
		{
			SendString(a, b);
		}
		//入力引数をフォーマットされたものに変換 OnString イベントに出力
		public void SetString(int a, float b)
		{
			SendString(a, b);
		}

		private void SendString(object a)
		{
			if (onString != null)
			{
				onString.Invoke(string.Format(format, a));
			}
		}

		private void SendString(object a, object b)
		{
			if (onString != null)
			{
				onString.Invoke(string.Format(format, a, b));
			}
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Common.Editor
{
	using UnityEditor;
	using TARGET = LeanFormatString;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class LeanFormatString_Editor : CwEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			BeginError(Any(tgts, t => string.IsNullOrEmpty(t.Format)));
				Draw("format", "The final text will use this string formatting, where {0} is the first value, {1} is the second, etc. Formatting uses standard <b>string.Format</b> style.");
			EndError();

			Separator();

			Draw("onString");
		}
	}
}
#endif