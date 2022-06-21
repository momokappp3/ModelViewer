using UnityEngine;
using System.Collections.Generic;
using Lean.Common;
using CW.Common;

namespace Lean.Touch
{
	//このクラスは、シングルタッチ（またはシミュレートされたタッチ）に関する情報を格納する
	public class LeanFinger
	{
		//これは指のハードウェアIDです。
		//注：シミュレートされた指はハードウェアID-1および-2を使用する
		public int Index;

		//この指がアクティブ（または非アクティブ）であった時間を秒単位で示す
		public float Age;

		//この指は現在画面に触れているか
		public bool Set;

		//これは最後のフレームの「Set」値を示す
		public bool LastSet;

		//この指は画面をタップしただけか
		public bool Tap;

		//この指がタップされた回数を示す
		public int TapCount;

		//この指は画面をスワイプしただけか
		public bool Swipe;

		//この指がTapThresholdを超えて画面に触れている場合
		public bool Old;

		//この指がTapThresholdを超えて非アクティブになっている場合
		public bool Expired;

		//最後のフレームの圧力値を示す
		public float LastPressure;

		//この指の現在の圧力を示しす注：これをサポートしているのは一部のデバイスのみです）
		public float Pressure;

		//この指が画面に触れ始めたときの「ScreenPosition」値を示しす
		public Vector2 StartScreenPosition;

		//これは指の最後の画面位置を示しす
		public Vector2 LastScreenPosition;

		//指の現在の画面位置をピクセル単位で示す。ここで、0,0=左下です。
		public Vector2 ScreenPosition;

		//現在の指が画面に触れ始めたときに「IsOverGui」がtrueに設定されているかどうかを示す
		public bool StartedOverGui;

		//位置のスナップショットを保存するために使用され、LeanTouchのRecordFingersがこれを使用できるようにする
		public List<LeanSnapshot> Snapshots = new List<LeanSnapshot>(1000);

		//この指が現在画面に触れている場合、これはtrueを返す
		public bool IsActive
		{
			get
			{
				return LeanTouch.Fingers.Contains(this);
			}
		}

		//この指に保存されているスナップショット映像の秒数がわかる
		public float SnapshotDuration
		{
			get
			{
				if (Snapshots.Count > 0)
				{
					return Age - Snapshots[0].Age;
				}

				return 0.0f;
			}
		}

		//現在の指がUnityGUI要素の上にある場合、これはtrueを返す
		public bool IsOverGui
		{
			get
			{
				return LeanTouch.PointOverGui(ScreenPosition);
			}
		}

		//この指はこのフレームの画面に触れ始めましたか？
		public bool Down
		{
			get
			{
				return Set == true && LastSet == false;
			}
		}

		//このフレームで指が画面に触れるのを止めたか
		public bool Up
		{
			get
			{
				return Set == false && LastSet == true;
			}
		}

		//最後に記録されたスナップショットから指が移動した距離をピクセル単位で返す
		public Vector2 LastSnapshotScreenDelta
		{
			get
			{
				var snapshotCount = Snapshots.Count;

				if (snapshotCount > 0)
				{
					var snapshot = Snapshots[snapshotCount - 1];

					if (snapshot != null)
					{
						return ScreenPosition - snapshot.ScreenPosition;
					}
				}

				return Vector2.zero;
			}
		}

		//解像度に依存しない'LastSnapshotScreenDelta'値を返す
		public Vector2 LastSnapshotScaledDelta
		{
			get
			{
				return LastSnapshotScreenDelta * LeanTouch.ScalingFactor;
			}
		}

		//最後のフレームから指が移動した距離をピクセル単位で返す
		public Vector2 ScreenDelta
		{
			get
			{
				return ScreenPosition - LastScreenPosition;
			}
		}

		//解像度に依存しない'ScreenDelta'値を返す
		public Vector2 ScaledDelta
		{
			get
			{
				return ScreenDelta * LeanTouch.ScalingFactor;
			}
		}

		//この指が画面に触れ始めてからどれだけ移動したかを示す
		public Vector2 SwipeScreenDelta
		{
			get
			{
				return ScreenPosition - StartScreenPosition;
			}
		}

		//解像度に依存しない'SwipeScreenDelta'値を返す
		public Vector2 SwipeScaledDelta
		{
			get
			{
				return SwipeScreenDelta * LeanTouch.ScalingFactor;
			}
		}

		//0..1の進行値に基づいて、前の画面位置と現在の画面位置の間の滑らかなポイントを返す
		public Vector2 GetSmoothScreenPosition(float t)
		{
			if (Snapshots.Count > 0 && Set == true)
			{
				var d = Snapshots[Mathf.Max(0, Snapshots.Count - 4)].ScreenPosition;
				var c = Snapshots[Mathf.Max(0, Snapshots.Count - 3)].ScreenPosition;
				var b = Snapshots[Mathf.Max(0, Snapshots.Count - 2)].ScreenPosition;
				var a = Snapshots[Mathf.Max(0, Snapshots.Count - 1)].ScreenPosition;

				return LeanCommon.Hermite(d, c, b, a, t);
			}

			return Vector2.LerpUnclamped(LastScreenPosition, ScreenPosition, t);
		}

		//0と1のスムーズな画面位置の間の画面スペース距離を返す
		public float SmoothScreenPositionDelta
		{
			get
			{
				if (Snapshots.Count > 0 && Set == true)
				{
					var c = Snapshots[Mathf.Max(0, Snapshots.Count - 3)].ScreenPosition;
					var b = Snapshots[Mathf.Max(0, Snapshots.Count - 2)].ScreenPosition;

					return Vector2.Distance(c, b);
				}

				return Vector2.Distance(LastScreenPosition, ScreenPosition);
			}
		}

		//指定されたカメラ（none / null =メインカメラ）に対する指の現在の位置の光線を返す
		public Ray GetRay(Camera camera = null)
		{
			//カメラが存在することを確認
			camera = CwHelper.GetCamera(camera);

			if (camera != null)
			{
				return camera.ScreenPointToRay(ScreenPosition);
			}
			else
			{
				Debug.LogError("Failed to find camera. Either tag your cameras MainCamera, or set one in this component.");
			}

			return default(Ray);
		}

		//指定されたカメラ（none / null =メインカメラ）を基準にした指の開始位置の光線を返す
		public Ray GetStartRay(Camera camera = null)
		{
			//カメラが存在することを確認
			camera = CwHelper.GetCamera(camera);

			if (camera != null)
			{
				return camera.ScreenPointToRay(StartScreenPosition);
			}
			else
			{
				Debug.LogError("Failed to find camera. Either tag your cameras MainCamera, or set one in this component.");
			}

			return default(Ray);
		}

		//過去の「deltaTime」秒で指がどれだけ移動したか
		public Vector2 GetSnapshotScreenDelta(float deltaTime)
		{
			return ScreenPosition - GetSnapshotScreenPosition(Age - deltaTime);
		}

		//解像度に依存しない'GetSnapshotScreenDelta'値を返す
		public Vector2 GetSnapshotScaledDelta(float deltaTime)
		{
			return GetSnapshotScreenDelta(deltaTime) * LeanTouch.ScalingFactor;
		}

		//現在の指が「targetAge」にあったときの記録された位置が返される
		public Vector2 GetSnapshotScreenPosition(float targetAge)
		{
			var screenPosition = ScreenPosition;

			LeanSnapshot.TryGetScreenPosition(Snapshots, targetAge, ref screenPosition);

			return screenPosition;
		}

		//現在の指が「targetAge」にあったときに記録された世界が返される
		public Vector3 GetSnapshotWorldPosition(float targetAge, float distance, Camera camera = null)
		{
			//カメラが存在することを確認
			camera = CwHelper.GetCamera(camera);

			if (camera != null)
			{
				var screenPosition = GetSnapshotScreenPosition(targetAge);
				var point          = new Vector3(screenPosition.x, screenPosition.y, distance);

				return camera.ScreenToWorldPoint(point);
			}
			else
			{
				Debug.LogError("Failed to find camera. Either tag your cameras MainCamera, or set one in this component.");
			}

			return default(Vector3);
		}

		//画面を基準にした指と参照点の間の角度が返される
		public float GetRadians(Vector2 referencePoint)
		{
			return Mathf.Atan2(ScreenPosition.x - referencePoint.x, ScreenPosition.y - referencePoint.y);
		}

		//画面を基準にした指と参照点の間の角度が返される
		public float GetDegrees(Vector2 referencePoint)
		{
			return GetRadians(referencePoint) * Mathf.Rad2Deg;
		}

		//画面を基準にした、最後の指の位置と参照点の間の角度が返される
		public float GetLastRadians(Vector2 referencePoint)
		{
			return Mathf.Atan2(LastScreenPosition.x - referencePoint.x, LastScreenPosition.y - referencePoint.y);
		}

		//画面を基準にした、最後の指の位置と参照点の間の角度が返される
		public float GetLastDegrees(Vector2 referencePoint)
		{
			return GetLastRadians(referencePoint) * Mathf.Rad2Deg;
		}

		//基準点を基準にした最後の指の位置と現在の指の位置の間のデルタ角度が返される
		public float GetDeltaRadians(Vector2 referencePoint)
		{
			return GetDeltaRadians(referencePoint, referencePoint);
		}

		//基準点と最後の基準点を基準にした最後の指の位置と現在の指の位置の間のデルタ角度が返される
		public float GetDeltaRadians(Vector2 referencePoint, Vector2 lastReferencePoint)
		{
			var a = GetLastRadians(lastReferencePoint);
			var b = GetRadians(referencePoint);
			var d = Mathf.Repeat(a - b, Mathf.PI * 2.0f);

			if (d > Mathf.PI)
			{
				d -= Mathf.PI * 2.0f;
			}

			return d;
		}

		//基準点を基準にした最後の指の位置と現在の指の位置の間のデルタ角度が返される
		public float GetDeltaDegrees(Vector2 referencePoint)
		{
			return GetDeltaRadians(referencePoint, referencePoint) * Mathf.Rad2Deg;
		}

		//基準点と最後の基準点を基準にした最後の指の位置と現在の指の位置の間のデルタ角度が返される
		public float GetDeltaDegrees(Vector2 referencePoint, Vector2 lastReferencePoint)
		{
			return GetDeltaRadians(referencePoint, lastReferencePoint) * Mathf.Rad2Deg;
		}

		//指と参照点の間の距離が返される
		public float GetScreenDistance(Vector2 point)
		{
			return Vector2.Distance(ScreenPosition, point);
		}

		//これは解像度に依存しない'GetScreenDistance'値を返す
		public float GetScaledDistance(Vector2 point)
		{
			return GetScreenDistance(point) * LeanTouch.ScalingFactor;
		}

		//小指と参照点の間の距離を返す
		public float GetLastScreenDistance(Vector2 point)
		{
			return Vector2.Distance(LastScreenPosition, point);
		}

		//解像度に依存しない'GetLastScreenDistance'値を返す
		public float GetLastScaledDistance(Vector2 point)
		{
			return GetLastScreenDistance(point) * LeanTouch.ScalingFactor;
		}

		//開始指と参照点の間の距離を返す
		public float GetStartScreenDistance(Vector2 point)
		{
			return Vector2.Distance(StartScreenPosition, point);
		}

		//解像度に依存しない'GetStartScreenDistance'値を返す
		public float GetStartScaledDistance(Vector2 point)
		{
			return GetStartScreenDistance(point) * LeanTouch.ScalingFactor;
		}

		//カメラからの距離に基づいて、この指の開始ワールド位置を返す
		public Vector3 GetStartWorldPosition(float distance, Camera camera = null)
		{
			//カメラが存在することを確認
			camera = CwHelper.GetCamera(camera);

			if (camera != null)
			{
				var point = new Vector3(StartScreenPosition.x, StartScreenPosition.y, distance);

				return camera.ScreenToWorldPoint(point);
			}
			else
			{
				Debug.LogError("Failed to find camera. Either tag your cameras MainCamera, or set one in this component.");
			}

			return default(Vector3);
		}

		//カメラからの距離に基づいて、この指の最後のワールド位置を返す
		public Vector3 GetLastWorldPosition(float distance, Camera camera = null)
		{
			//カメラが存在することを確認
			camera = CwHelper.GetCamera(camera);

			if (camera != null)
			{
				var point = new Vector3(LastScreenPosition.x, LastScreenPosition.y, distance);

				return camera.ScreenToWorldPoint(point);
			}
			else
			{
				Debug.LogError("Failed to find camera. Either tag your cameras MainCamera, or set one in this component.");
			}

			return default(Vector3);
		}

		//カメラからの距離に基づいて、この指のワールド位置が返す
		public Vector3 GetWorldPosition(float distance, Camera camera = null)
		{
			//カメラが存在することを確認
			camera = CwHelper.GetCamera(camera);

			if (camera != null)
			{
				var point = new Vector3(ScreenPosition.x, ScreenPosition.y, distance);

				return camera.ScreenToWorldPoint(point);
			}
			else
			{
				Debug.LogError("Failed to find camera. Either tag your cameras MainCamera, or set one in this component.");
			}

			return default(Vector3);
		}

		//カメラからの距離に基づいたこの指のワールド位置の変化が返す
		public Vector3 GetWorldDelta(float distance, Camera camera = null)
		{
			return GetWorldDelta(distance, distance, camera);
		}

		//カメラからの最後の現在の距離に基づいて、この指のワールド位置の変化が返す
		public Vector3 GetWorldDelta(float lastDistance, float distance, Camera camera = null)
		{
			//カメラが存在することを確認
			camera = CwHelper.GetCamera(camera);

			if (camera != null)
			{
				return GetWorldPosition(distance, camera) - GetLastWorldPosition(lastDistance, camera);
			}
			else
			{
				Debug.LogError("Failed to find camera. Either tag your cameras MainCamera, or set one in this component.");
			}

			return default(Vector3);
		}

		//この指のすべてのスナップショットがクリアされ、プールされる
		//すべてのスナップショットはカウント=-1
		public void ClearSnapshots(int count = -1)
		{
			//古いものだけをクリアしますか？
			if (count > 0 && count <= Snapshots.Count)
			{
				for (var i = 0; i < count; i++)
				{
					LeanSnapshot.InactiveSnapshots.Add(Snapshots[i]);
				}

				Snapshots.RemoveRange(0, count);
			}
			// すべてクリア？
			else if (count < 0)
			{
				LeanSnapshot.InactiveSnapshots.AddRange(Snapshots);

				Snapshots.Clear();
			}
		}

		//現在の指の位置のスナップショットが即座に保存される
		public void RecordSnapshot()
		{
			//未使用のスナップショットを取得して設定
			var snapshot = LeanSnapshot.Pop();

			snapshot.Age            = Age;
			snapshot.ScreenPosition = ScreenPosition;

			// リストに追加する
			Snapshots.Add(snapshot);
		}
	}
}