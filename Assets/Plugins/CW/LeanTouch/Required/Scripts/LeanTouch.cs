//一部のプラットフォームでは、誤った指IDデータが報告されたり、タップ間の指の距離が厳しすぎたりする場合がある
//このようなプラットフォームまたはデバイスで開発している場合は、コメントを外してIDの手動オーバーライドを有効にすることができる
//＃define LEAN_ALLOW_RECLAIM
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using CW.Common;

namespace Lean.Touch
{
	//このコンポーネントをシーンに追加すると、すべてのマウスとタッチのデータが使いやすいデータに変換
	//Lean.Touch.LeanTouch.Instance.Fingersを介してこのデータにアクセスするか、Lean.Touch.LeanTouch.On___イベントにフックすることができる
	//注：1フレームの入力遅延が発生した場合は、ScriptExecutionOrderを編集して、このスクリプトを他のスクリプトの前に強制的に更新する必要がある
	[ExecuteInEditMode]
	[DefaultExecutionOrder(-100)]
	[DisallowMultipleComponent]
	[HelpURL(HelpUrlPrefix + "LeanTouch")]
	[AddComponentMenu(ComponentPathPrefix + "Touch")]
	public partial class LeanTouch : MonoBehaviour
	{
		public const string ComponentPathPrefix = "Lean/Touch/Lean ";

		public const string HelpUrlPrefix = "https://carloswilkes.com/Documentation/LeanTouch#";

		public const string PlusHelpUrlPrefix = "https://carloswilkes.com/Documentation/LeanTouchPlus#";

		public const int MOUSE_FINGER_INDEX = -1;

		public const int HOVER_FINGER_INDEX = -42;

		private const int DEFAULT_REFERENCE_DPI = 200;

		private const int DEFAULT_GUI_LAYERS = 1 << 5;

		private const float DEFAULT_TAP_THRESHOLD = 0.2f;

		private const float DEFAULT_SWIPE_THRESHOLD = 100.0f;

		private const float DEFAULT_RECORD_LIMIT = 10.0f;

		//全てのアクティブで有効なLeanTouchインスタンスが含まれまれる
		public static List<LeanTouch> Instances = new List<LeanTouch>();

		//このリストには、現在画面に触れている（または画面に触れなくなったばかりの）すべての指が含まれている
		//注：このリストには、シミュレートされた指と、マウスのホバー指が含まれている
		public static List<LeanFinger> Fingers = new List<LeanFinger>(10);

		//このリストには、かつて画面に触れていたすべての指が含まれている これは、指でのタッピングや
		//古くてタッピングの対象ではなくなった「非アクティブ」な指を管理するために使用される
		public static List<LeanFinger> InactiveFingers = new List<LeanFinger>(10);

		//これは、指が画面に触れ始めると発生する（LeanFinger =現在の指）
		public static event System.Action<LeanFinger> OnFingerDown;

		//これは、指が画面に触れているフレームごとに発生する（LeanFinger = 現在の指）
		public static event System.Action<LeanFinger> OnFingerUpdate;

		//これは、指が画面に触れなくなると発生する（LeanFinger = 現在の指）
		public static event System.Action<LeanFinger> OnFingerUp;

		//これは、指が<b> TapThreshold </ b>秒より長く画面に触れている場合に発生し、タップおよびスワイプイベントの対象外になる
		public static event System.Action<LeanFinger> OnFingerOld;

		//これは、指が画面をタップしたときに発生する（これは、指が「TapThreshold」時間内に画面へのタッチを開始および停止したときです）
		public static event System.Action<LeanFinger> OnFingerTap;

		//これは、指が画面をスワイプしたときに発生します（これは、指が「TapThreshold」時間内に画面へのタッチを開始および停止し
		//「SwipeThreshold」距離を超えて移動した場合です）（LeanFinger=現在の指)
		public static event System.Action<LeanFinger> OnFingerSwipe;

		//これは、少なくとも1本の指が画面に触れているフレームごとに発生する（リスト=指）
		public static event System.Action<List<LeanFinger>> OnGesture;

		//これは、指が<b> TapThreshold </ b>秒より長く画面に触れた後に発生し、スワイプの対象外になります
		public static event System.Action<LeanFinger> OnFingerExpired;

		//これは、指が上がった後にフレームが発射されます
		public static event System.Action<LeanFinger> OnFingerInactive;

		//これは、指をシミュレートするときに呼び出される<b> AddFinger</b>メソッドを呼び出してそれらをシミュレートできる
		public event System.Action OnSimulateFingers;

		//これにより、タップを登録するために指を上下に動かすまでに必要な秒数を設定できる
		public float TapThreshold { set { tapThreshold = value; } get { return tapThreshold; } } [SerializeField] private float tapThreshold = DEFAULT_TAP_THRESHOLD;

		public static float CurrentTapThreshold
		{
			get
			{
				return Instances.Count > 0 ? Instances[0].tapThreshold : DEFAULT_TAP_THRESHOLD;
			}
		}

		//これにより、スワイプがトリガーされるためにTapThreshold内で必要な移動のピクセル数（ReferenceDpiに対して）を設定できる
		public float SwipeThreshold { set { swipeThreshold = value; } get { return swipeThreshold; } } [SerializeField] private float swipeThreshold = DEFAULT_SWIPE_THRESHOLD;


		public static float CurrentSwipeThreshold
		{
			get
			{
				return Instances.Count > 0 ? Instances[0].swipeThreshold : DEFAULT_SWIPE_THRESHOLD;
			}
		}

#if LEAN_ALLOW_RECLAIM
		/// <summary>This allows you to set how many pixels (relative to the ReferenceDpi) away from a previous finger the new touching finger must be for it to be reclaimed. This is useful on platforms that give incorrect finger ID data.</summary>
		public float ReclaimThreshold { set { reclaimThreshold = value; } get { return reclaimThreshold; } } [SerializeField] private float reclaimThreshold = DEFAULT_RECLAIM_THRESHOLD;

		public const float DEFAULT_RECLAIM_THRESHOLD = 10.0f;

		public static float CurrentReclaimThreshold
		{
			get
			{
				return Instances.Count > 0 ? Instances[0].reclaimThreshold : DEFAULT_RECLAIM_THRESHOLD;
			}
		}
#endif

		//これにより、入力スケーリングの基にするデフォルトのDPIを設定できる。たとえば、これを200に設定し、ディスプレイのDPIが400の場合
		//<b> ScaledDelta</b>の指の値はピクセルスペースの<b>ScreenDelta</b>値の半分の距離になる
		public int ReferenceDpi { set { referenceDpi = value; } get { return referenceDpi; } } [SerializeField] private int referenceDpi = DEFAULT_REFERENCE_DPI;

		public static int CurrentReferenceDpi
		{
			get
			{
				return Instances.Count > 0 ? Instances[0].referenceDpi : DEFAULT_REFERENCE_DPI;
			}
		}

		//これにより、GUIを配置するレイヤーを設定できるため、各指で無視できる
		public LayerMask GuiLayers { set { guiLayers = value; } get { return guiLayers; } } [SerializeField] private LayerMask guiLayers = (LayerMask)DEFAULT_GUI_LAYERS;

		public static LayerMask CurrentGuiLayers
		{
			get
			{
				return Instances.Count > 0 ? Instances[0].guiLayers : (LayerMask)DEFAULT_GUI_LAYERS;
			}
		}

		//これを無効にすると、リーンタッチは画面へのタッチを停止したかのように機能する
		public bool UseTouch { set { useTouch = value; } get { return useTouch; } } [SerializeField] private bool useTouch = true;

		//マウスのホバー位置を指として保存する必要がありますか？
		//注：HOVER_FINGER_INDEX=-42の指<b>インデックス</b>が与えられる
		public bool UseHover { set { useHover = value; } get { return useHover; } } [SerializeField] private bool useHover = true;

		//マウスボタンの押下を指として保存する必要がありますか？
		///注：MOUSE_FINGER_INDEX=-1の指<b>インデックス</b>が与えられる
		public bool UseMouse { set { useMouse = value; } get { return useMouse; } } [SerializeField] private bool useMouse = true;

		//<b> OnSimulateFingers </ b>イベントにフックされたコンポーネントを使用する必要がありますか？ （例：LeanTouchSimulator）
		public bool UseSimulator { set { useSimulator = value; } get { return useSimulator; } } [SerializeField] private bool useSimulator = true;

		//古い/レガシー入力システムを使用する場合、デフォルトでは、マウスがない場合でも、タッチデータがマウスデータに変換される
		//この設定を有効にすると、この動作が無効になりる
		public bool DisableMouseEmulation { set { disableMouseEmulation = value; UpdateMouseEmulation(); } get { return disableMouseEmulation; } } [SerializeField] private bool disableMouseEmulation = true;

		//各指は画面位置のスナップショットを記録する必要がありますか？
		public bool RecordFingers { set { recordFingers = value; } get { return recordFingers; } } [SerializeField] private bool recordFingers = true;

		//これにより、別のスナップショットを保存するために指が移動する必要のあるピクセル数を設定できる
		public float RecordThreshold { set { recordThreshold = value; } get { return recordThreshold; } } [SerializeField] private float recordThreshold = 5.0f;

		//これにより、記録できる最大秒数を設定できます。0 = 無制限
		public float RecordLimit { set { recordLimit = value; } get { return recordLimit; } } [SerializeField] private float recordLimit = DEFAULT_RECORD_LIMIT;

		//GUIが使用されているかどうかを確認するために使用されます
		private static List<RaycastResult> tempRaycastResults = new List<RaycastResult>(10);

		//GUI以外の指を返すために使用されます
		private static List<LeanFinger> filteredFingers = new List<LeanFinger>(10);

		//RaycastGuiによって使用されます
		private static PointerEventData tempPointerEventData;

		//RaycastGuiによって使用されます
		private static EventSystem tempEventSystem;

		//最初のアクティブで有効なLeanTouchインスタンス
		public static LeanTouch Instance
		{
			get
			{
				return Instances.Count > 0 ? Instances[0] : null;
			}
		}

		//この値に他のピクセルデルタ（ScreenDeltaなど）を掛けると、デバイスのDPIに対してデバイスの解像度に依存しなくなります。
		public static float ScalingFactor
		{
			get
			{
				//現在の画面のDPIを取得
				var dpi = Screen.dpi;

				// 0以下の場合は無効なので、デフォルトのスケール1.0を返す
				if (dpi <= 0)
				{
					return 1.0f;
				}

				// DPIは有効と思われるため、参照DPIに対してスケーリングする
				return CurrentReferenceDpi / dpi;
			}
		}

		//この値に他のピクセルデルタ（ScreenDeltaなど）を掛けると、画面のピクセルサイズに対してデバイスの解像度に依存しなくなる
		public static float ScreenFactor
		{
			get
			{
				//最短サイズを取得
				var size = Mathf.Min(Screen.width, Screen.height);

				// 0以下の場合は無効なので、デフォルトのスケール1.0を返す
				if (size <= 0)
				{
					return 1.0f;
				}

				//乗算を簡単にするために逆数を返す
				return 1.0f / size;
			}
		}

		//これは、マウスまたは任意の指が現在GUIを使用している場合にtrueを返す
		public static bool GuiInUse
		{
			get
			{
				//レガシーGUIを使用していますか？
				if (GUIUtility.hotControl > 0)
				{
					return true;
				}

				//新しいGUIを使用していますか？
				foreach (var finger in Fingers)
				{
					if (finger.StartedOverGui == true)
					{
						return true;
					}
				}

				return false;
			}
		}

		public static bool ElementOverlapped(GameObject element, Vector2 screenPosition)
		{
			var results = RaycastGui(screenPosition, -1);

			if (results != null && results.Count > 0)
			{
				if (results[0].gameObject == element)
				{
					return true;
				}
			}

			return false;
		}

		public static EventSystem GetEventSystem()
		{
			var currentEventSystem = EventSystem.current;

			if (currentEventSystem == null)
			{
				currentEventSystem = FindObjectOfType<EventSystem>();
			}

			return currentEventSystem;
		}

		//これは、指定された画面ポイントがGUI要素の上にある場合にtrueを返す
		public static bool PointOverGui(Vector2 screenPosition)
		{
			return RaycastGui(screenPosition).Count > 0;
		}

		//これにより、現在のlayerMaskを使用して、指定された画面ポイントの下にあるすべてのRaycastResultsが返される
		//：最初の結果（0）は、最初にヒットした最上位のUI要素になる
		public static List<RaycastResult> RaycastGui(Vector2 screenPosition)
		{
			return RaycastGui(screenPosition, CurrentGuiLayers);
		}

		//これにより、指定されたlayerMaskを使用して、指定された画面ポイントの下にあるすべてのRaycastResultsが返される
		//注：最初の結果（0）は、最初にヒットした最上位のUI要素になる
		public static List<RaycastResult> RaycastGui(Vector2 screenPosition, LayerMask layerMask)
		{
			tempRaycastResults.Clear();

			var currentEventSystem = GetEventSystem();

			if (currentEventSystem != null)
			{
				//このイベントシステムのポイントイベントデータを作成しますか？
				if (currentEventSystem != tempEventSystem)
				{
					tempEventSystem = currentEventSystem;

					if (tempPointerEventData == null)
					{
						tempPointerEventData = new PointerEventData(tempEventSystem);
					}
					else
					{
						tempPointerEventData.Reset();
					}
				}

				//指定されたポイントでのレイキャストイベントシステム
				tempPointerEventData.position = screenPosition;

				currentEventSystem.RaycastAll(tempPointerEventData, tempRaycastResults);

				//すべての結果をループし、レイヤーマスクと一致しないものを削除する
				if (tempRaycastResults.Count > 0)
				{
					for (var i = tempRaycastResults.Count - 1; i >= 0; i--)
					{
						var raycastResult = tempRaycastResults[i];
						var raycastLayer  = 1 << raycastResult.gameObject.layer;

						if ((raycastLayer & layerMask) == 0)
						{
							tempRaycastResults.RemoveAt(i);
						}
					}
				}
			}
			else
			{
				Debug.LogError("Failed to RaycastGui because your scene doesn't have an event system! To add one, go to: GameObject/UI/EventSystem");
			}

			return tempRaycastResults;
		}

		//これにより、指定した要件に基づいてすべての指をフィルタリングできる
		///注：ignoreGuiFingersが設定されている場合、Fingersはフィルターされ、StartedOverGuiで削除されます。
		///注：requiredFingerCountが0より大きい場合、指数が一致しない場合、このメソッドはnullを返します。
		///注：requiredSelectableが設定されていて、そのSelectingFingerがnullでない場合は、その指だけが返されます
		public static List<LeanFinger> GetFingers(bool ignoreIfStartedOverGui, bool ignoreIfOverGui, int requiredFingerCount = 0, bool ignoreHoverFinger = true)
		{
			filteredFingers.Clear();

			foreach (var finger in Fingers)
			{
				// Ignore?
				if (ignoreIfStartedOverGui == true && finger.StartedOverGui == true)
				{
					continue;
				}

				if (ignoreIfOverGui == true && finger.IsOverGui == true)
				{
					continue;
				}

				if (ignoreHoverFinger == true && finger.Index == HOVER_FINGER_INDEX)
				{
					continue;
				}

				// Add
				filteredFingers.Add(finger);
			}

			if (requiredFingerCount > 0)
			{
				if (filteredFingers.Count != requiredFingerCount)
				{
					filteredFingers.Clear();

					return filteredFingers;
				}
			}

			return filteredFingers;
		}

		private static LeanFinger simulatedTapFinger = new LeanFinger();

		//これにより、指定した場所で画面をタップすることをシミュレートできる
		public static void SimulateTap(Vector2 screenPosition, float pressure = 1.0f, int tapCount = 1)
		{
			if (OnFingerTap != null)
			{
				simulatedTapFinger.Index               = -5;
				simulatedTapFinger.Age                 = 0.0f;
				simulatedTapFinger.Set                 = false;
				simulatedTapFinger.LastSet             = true;
				simulatedTapFinger.Tap                 = true;
				simulatedTapFinger.TapCount            = tapCount;
				simulatedTapFinger.Swipe               = false;
				simulatedTapFinger.Old                 = false;
				simulatedTapFinger.Expired             = false;
				simulatedTapFinger.LastPressure        = pressure;
				simulatedTapFinger.Pressure            = pressure;
				simulatedTapFinger.StartScreenPosition = screenPosition;
				simulatedTapFinger.LastScreenPosition  = screenPosition;
				simulatedTapFinger.ScreenPosition      = screenPosition;
				simulatedTapFinger.StartedOverGui      = simulatedTapFinger.IsOverGui;
				simulatedTapFinger.ClearSnapshots();
				simulatedTapFinger.RecordSnapshot();

				OnFingerTap(simulatedTapFinger);
			}
		}

		//すべてのfingerイベントを停止する場合は、このメソッドを呼び出すことができる
		//次に、このコンポーネントを無効にして、新しいコンポーネントが更新されないようにすることができる
		public void Clear()
		{
			UpdateFingers(0.001f, false);
			UpdateFingers(1.0f, false);
		}

		//これにより、現在の<b> DisableMouseEmulation</b>設定に基づいてUnityが更新される
		public void UpdateMouseEmulation()
		{
			if (disableMouseEmulation == true)
			{
				Input.simulateMouseWithTouches = false;
			}
			else
			{
				Input.simulateMouseWithTouches = true;
			}
		}

		protected virtual void OnEnable()
		{
			Instances.Add(this);

			UpdateMouseEmulation();
		}

		protected virtual void OnDisable()
		{
			Instances.Remove(this);
		}

		protected virtual void Update()
		{
#if UNITY_EDITOR
			if (Application.isPlaying == false)
			{
				return;
			}
#endif

			//これが最初のインスタンスである場合にのみ、更新メソッドを実行する
			//（つまり、シーンに複数のLeanTouchコンポーネントがある場合は、最初のコンポーネントのみを使用します）
			if (Instances[0] == this)
			{
				UpdateFingers(Time.unscaledDeltaTime, true);
			}
		}

		private void UpdateFingers(float deltaTime, bool poll)
		{
			//新しい情報のために古い指のデータを準備します
			BeginFingers(deltaTime);

			//現在のタッチ+マウスデータをポーリングし、それを指に変換します
			if (poll == true)
			{
				PollFingers();
			}

			//使用されなくなった指を処理します
			EndFingers(deltaTime);

			//新しい指のデータに基づいてイベントを更新します
			UpdateEvents();
		}

		//すべてのFingersとInactiveFingersを更新して、新しいフレームの準備をします
		private void BeginFingers(float deltaTime)
		{
			//非アクティブな指の年齢
			for (var i = InactiveFingers.Count - 1; i >= 0; i--)
			{
				var inactiveFinger = InactiveFingers[i];

				inactiveFinger.Age += deltaTime;

				//期限切れですか？
				if (inactiveFinger.Expired == false && inactiveFinger.Age > tapThreshold)
				{
					inactiveFinger.Expired = true;

					if (OnFingerExpired != null) OnFingerExpired(inactiveFinger);
				}
			}

			//フィンガーデータをリセットする
			for (var i = Fingers.Count - 1; i >= 0; i--)
			{
				var finger = Fingers[i];

				//これは前回設定されましたか？ もしそうなら、それは現在非アクティブです
				if (finger.Up == true || finger.Set == false)
				{
					//指を非アクティブにします
					Fingers.RemoveAt(i); InactiveFingers.Add(finger);

					//年齢をリセットして、非アクティブになっている時間を計測できるようにします
					finger.Age = 0.0f;

					//古いスナップショットをプールします
					finger.ClearSnapshots();

					if (OnFingerInactive != null) OnFingerInactive(finger);
				}
				else
				{
					finger.LastSet            = finger.Set;
					finger.LastPressure       = finger.Pressure;
					finger.LastScreenPosition = finger.ScreenPosition;

					finger.Set   = false;
					finger.Tap   = false;
					finger.Swipe = false;
				}
			}

			//すべてのアクティブな指を保存します（これらは後でAddFingerで削除される）
			missingFingers.Clear();

			foreach (var finger in Fingers)
			{
				missingFingers.Add(finger);
			}
		}

		//新しい指のデータに基づいてすべての指を更新します
		private void EndFingers(float deltaTime)
		{
			//行方不明の指を強制的に上に上げます（通常は指がないはずです）
			tempFingers.Clear();

			tempFingers.AddRange(missingFingers);

			foreach (var finger in tempFingers)
			{
				AddFinger(finger.Index, finger.ScreenPosition, finger.Pressure, false);
			}

			//指を更新する
			foreach (var finger in Fingers)
			{
				// Up?
				if (finger.Up == true)
				{
					// Tap or Swipe?
					if (finger.Age <= tapThreshold)
					{
						if (finger.SwipeScreenDelta.magnitude * ScalingFactor < swipeThreshold)
						{
							finger.Tap       = true;
							finger.TapCount += 1;
						}
						else
						{
							finger.TapCount = 0;
							finger.Swipe    = true;
						}
					}
					else
					{
						finger.TapCount = 0;
					}
				}
				// Down?
				else if (finger.Down == false)
				{
					// Age it
					finger.Age += deltaTime;

					// Too old?
					if (finger.Age > tapThreshold && finger.Old == false)
					{
						finger.Old = true;

						if (OnFingerOld != null) OnFingerOld(finger);
					}
				}
			}
		}

		//これにより、Unityが指が上がったことを最初に示さずに、指が誤って削除されたかどうかを追跡できます
		private static HashSet<LeanFinger> missingFingers = new HashSet<LeanFinger>();

		private static List<LeanFinger> tempFingers = new List<LeanFinger>();

		//新しいハードウェアフィンガーデータを読み取ります
		private void PollFingers()
		{
			//本物の指を提出しますか？
			if (useTouch == true && CwInput.GetTouchCount() > 0)
			{
				for (var i = 0; i < CwInput.GetTouchCount(); i++)
				{
					int id; Vector2 position; float pressure; bool set;

					CwInput.GetTouch(i, out id, out position, out pressure, out set);

					AddFinger(id, position, pressure, set);
				}
			}

			//マウスホバーを指として送信しますか？
			if (useHover == true && CwInput.GetMouseExists() == true)
			{
				var mousePosition = CwInput.GetMousePosition();
				var hoverFinger   = AddFinger(HOVER_FINGER_INDEX, mousePosition, 0.0f, true);

				hoverFinger.StartedOverGui = false;
				hoverFinger.LastSet        = true;
			}

			//マウスボタンを指として送信しますか？
			if (useMouse == true && CwInput.GetMouseExists() == true)
			{
				var mouseSet = false;
				var mouseUp  = false;

				for (var i = 0; i < 5; i++)
				{
					mouseSet |= CwInput.GetMouseIsHeld(i);
					mouseUp  |= CwInput.GetMouseWentUp(i);
				}

				if (mouseSet == true || mouseUp == true)
				{
					var mousePosition = CwInput.GetMousePosition();

					//マウスは画面内にありますか？
					// if（new Rect（0、0、Screen.width、Screen.height）.Contains（mousePosition）== true）
					{
						AddFinger(MOUSE_FINGER_INDEX, mousePosition, 1.0f, mouseSet);
					}
				}
			}

			//他の指をシミュレートしますか？
			if (useSimulator == true)
			{
				if (OnSimulateFingers != null) OnSimulateFingers.Invoke();
			}
		}

		private void UpdateEvents()
		{
			var fingerCount = Fingers.Count;

			if (fingerCount > 0)
			{
				for (var i = 0; i < fingerCount; i++)
				{
					var finger = Fingers[i];

					if (finger.Tap   == true && OnFingerTap    != null) OnFingerTap(finger);
					if (finger.Swipe == true && OnFingerSwipe  != null) OnFingerSwipe(finger);
					if (finger.Down  == true && OnFingerDown   != null) OnFingerDown(finger);
					if (                        OnFingerUpdate != null) OnFingerUpdate(finger);
					if (finger.Up    == true && OnFingerUp     != null) OnFingerUp(finger);
				}

				if (OnGesture != null)
				{
					filteredFingers.Clear();
					filteredFingers.AddRange(Fingers);

					OnGesture(filteredFingers);
				}
			}
		}

		//インデックスに基づいて指を追加するか、既存の指を返します
		public LeanFinger AddFinger(int index, Vector2 screenPosition, float pressure, bool set)
		{
			var finger = FindFinger(index);

			// Fingerはすでにアクティブになっているため、不足しているコレクションから削除します
			if (finger != null)
			{
				missingFingers.Remove(finger);
			}
			//指が見つかりませんか？ それを見つけるか作成する
			else
			{
				//指が上がったがまだ登録されていない場合は、イベントフローが混乱するため、スキップします（通常は発生しないはずです）。
				if (set == false)
				{
					return null;
				}

				var inactiveIndex = FindInactiveFingerIndex(index);

				//非アクティブな指を使用しますか？
				if (inactiveIndex >= 0)
				{
					finger = InactiveFingers[inactiveIndex]; InactiveFingers.RemoveAt(inactiveIndex);

					//非アクティブが長すぎますか？
					if (finger.Age > tapThreshold)
					{
						finger.TapCount = 0;
					}

					//値をリセット
					finger.Age     = 0.0f;
					finger.Old     = false;
					finger.Set     = false;
					finger.LastSet = false;
					finger.Tap     = false;
					finger.Swipe   = false;
					finger.Expired = false;
				}
				else
				{
#if LEAN_ALLOW_RECLAIM
					// Before we create a new finger, try reclaiming one in case the finger ID was given incorrectly
					finger = ReclaimFinger(index, screenPosition);
#endif

					//新しい指を作成しますか？
					if (finger == null)
					{
						finger = new LeanFinger();

						finger.Index = index;
					}
				}

				finger.StartScreenPosition = screenPosition;
				finger.LastScreenPosition  = screenPosition;
				finger.LastPressure        = pressure;
				finger.StartedOverGui      = PointOverGui(screenPosition);

				Fingers.Add(finger);
			}

			finger.Set            = set;
			finger.ScreenPosition = screenPosition;
			finger.Pressure       = pressure;

			// 記録？
			if (recordFingers == true)
			{
				//スナップショットが多すぎますか？
				if (recordLimit > 0.0f)
				{
					if (finger.SnapshotDuration > recordLimit)
					{
						var removeCount = LeanSnapshot.GetLowerIndex(finger.Snapshots, finger.Age - recordLimit);

						finger.ClearSnapshots(removeCount);
					}
				}
				//ホバーフィンガーが永久に記録されないようにします
				else if (finger.Index == HOVER_FINGER_INDEX)
				{
					if (finger.SnapshotDuration > DEFAULT_RECORD_LIMIT)
					{
						var removeCount = LeanSnapshot.GetLowerIndex(finger.Snapshots, finger.Age - DEFAULT_RECORD_LIMIT);

						finger.ClearSnapshots(removeCount);
					}
				}

				//スナップショットを記録しますか？
				if (recordThreshold > 0.0f)
				{
					if (finger.Snapshots.Count == 0 || finger.LastSnapshotScreenDelta.magnitude >= recordThreshold)
					{
						finger.RecordSnapshot();
					}
				}
				else
				{
					finger.RecordSnapshot();
				}
			}

			return finger;
		}

		//指定されたインデックスを持つ指を見つけるか、nullを返します
		private LeanFinger FindFinger(int index)
		{
			foreach (var finger in Fingers)
			{
				if (finger.Index == index)
				{
					return finger;
				}
			}

			return null;
		}

#if LEAN_ALLOW_RECLAIM
		// Some platforms may give unexpected finger ID information, override it?
		private LeanFinger ReclaimFinger(int index, Vector2 screenPosition)
		{
			for (var i = InactiveFingers.Count - 1; i>= 0; i--)
			{
				var finger = InactiveFingers[i];

				if (finger.Expired == false && Vector2.Distance(finger.ScreenPosition, screenPosition) * ScalingFactor < reclaimThreshold)
				{
					finger.Index = index;

					InactiveFingers.RemoveAt(i);

					Fingers.Add(finger);

					return finger;
				}
			}

			return null;
		}
#endif

		//指定されたインデックスを持つ非アクティブな指のインデックスを検索するか、-1を返します
		private int FindInactiveFingerIndex(int index)
		{
			for (var i = InactiveFingers.Count - 1; i>= 0; i--)
			{
				if (InactiveFingers[i].Index == index)
				{
					return i;
				}
			}

			return -1;
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Touch.Editor
{
	using UnityEditor;
	using TARGET = LeanTouch;

	[CustomEditor(typeof(TARGET))]
	public class LeanTouch_Editor : CwEditor
	{
		private static List<LeanFinger> allFingers = new List<LeanFinger>();

		private static GUIStyle fadingLabel;

		public static event System.Action<TARGET> OnExtendInspector;

		[System.NonSerialized] TARGET tgt; [System.NonSerialized] TARGET[] tgts;

		[MenuItem("GameObject/Lean/Touch", false, 1)]
		public static void CreateTouch()
		{
			var gameObject = new GameObject(typeof(LeanTouch).Name);

			Undo.RegisterCreatedObjectUndo(gameObject, "Create Touch");

			gameObject.AddComponent<LeanTouch>();

			Selection.activeGameObject = gameObject;
		}

		//インスペクター全体を描画します
		protected override void OnInspector()
		{
			GetTargets(out tgt, out tgts);

			if (LeanTouch.Instances.Count > 1)
			{
				Warning("There is more than one active and enabled LeanTouch...");

				Separator();
			}

			var touch = (LeanTouch)target;

			Separator();

			DrawSettings(touch);

			Separator();

			if (OnExtendInspector != null)
			{
				OnExtendInspector.Invoke(tgt);
			}

			Separator();

			DrawFingers(touch);

			Separator();

			Repaint();
		}

		private void DrawSettings(LeanTouch touch)
		{
			var updateMouseEmulation = false;

			Draw("tapThreshold", "This allows you to set how many seconds are required between a finger down/up for a tap to be registered.");
			Draw("swipeThreshold", "This allows you to set how many pixels of movement (relative to the ReferenceDpi) are required within the TapThreshold for a swipe to be triggered.");
#if LEAN_ALLOW_RECLAIM
			Draw("reclaimThreshold", "This allows you to set how many pixels (relative to the ReferenceDpi) away from a previous finger the new touching finger must be for it to be reclaimed. This is useful on platforms that give incorrect finger ID data.");
#endif
			Draw("referenceDpi", "This allows you to set the default DPI you want the input scaling to be based on. For example, if you set this to 200 and your display has a DPI of 400, then the <b>ScaledDelta</b> finger value will be half the distance of the pixel space <b>ScreenDelta</b> value.");
			Draw("guiLayers", "This allows you to set which layers your GUI is on, so it can be ignored by each finger.");

			Separator();

			Draw("useTouch", "If you disable this then lean touch will act as if you stopped touching the screen.");
			Draw("useHover", "Should the mouse hover position be stored as a finger?\n\nNOTE: It will be given a finger <b>Index</b> of HOVER_FINGER_INDEX = -42.");
			Draw("useMouse", "Should any mouse button press be stored as a finger?\n\nNOTE: It will be given a finger <b>Index</b> of MOUSE_FINGER_INDEX = -1.");
			Draw("useSimulator", "Should components hooked into the <b>OnSimulateFingers</b> event be used? (e.g. LeanTouchSimulator)");

			Separator();

			Draw("disableMouseEmulation", ref updateMouseEmulation, "When using the old/legacy input system, by default it will convert touch data into mouse data, even if there is no mouse. Enabling this setting will disable this behavior.");
			Draw("recordFingers", "Should each finger record snapshots of their screen positions?");

			if (touch.RecordFingers == true)
			{
				BeginIndent();
					Draw("recordThreshold", "This allows you to set the amount of pixels a finger must move for another snapshot to be stored.");
					Draw("recordLimit", "This allows you to set the maximum amount of seconds that can be recorded, 0 = unlimited.");
				EndIndent();
			}

			if (updateMouseEmulation == true)
			{
				Each(tgts, t => t.UpdateMouseEmulation(), true);
			}
		}

		private void DrawFingers(LeanTouch touch)
		{
			EditorGUILayout.LabelField(new GUIContent("Fingers", "Index - State - Taps - X, Y - Age"), EditorStyles.boldLabel);

			allFingers.Clear();
			allFingers.AddRange(LeanTouch.Fingers);
			allFingers.AddRange(LeanTouch.InactiveFingers);
			allFingers.Sort((a, b) => a.Index.CompareTo(b.Index));

			for (var i = 0; i < allFingers.Count; i++)
			{
				var finger   = allFingers[i];
				var progress = touch.TapThreshold > 0.0f ? finger.Age / touch.TapThreshold : 0.0f;
				var style    = GetFadingLabel(finger.Set, progress);

				if (style.normal.textColor.a > 0.0f)
				{
					var screenPosition = finger.ScreenPosition;
					var state          = "UPDATE";

					if (finger.Down     == true ) state = "DOWN";
					if (finger.Up       == true ) state = "UP";
					if (finger.IsActive == false) state = "INACTIVE";
					if (finger.Expired  == true ) state = "EXPIRED";

					//Mathf.FloorToInt = 切り捨ての整数を返す
					EditorGUILayout.LabelField(finger.Index + " - " + state + " - " + finger.TapCount + "  + " + Mathf.FloorToInt(screenPosition.x) + ", " + Mathf.FloorToInt(screenPosition.y) + ") - " + finger.Age.ToString("0.0"), style);
				}
			}
		}

		private static GUIStyle GetFadingLabel(bool active, float progress)
		{
			if (fadingLabel == null)
			{
				fadingLabel = new GUIStyle(EditorStyles.label);
			}

			var a = EditorStyles.label.normal.textColor;
			var b = a; b.a = active == true ? 0.5f : 0.0f;

			fadingLabel.normal.textColor = Color.Lerp(a, b, progress);

			return fadingLabel;
		}
	}
}
#endif