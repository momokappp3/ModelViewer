using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Touch;

public class Crown : MonoBehaviour
{
    //素結合大事
    //タッチした時の座標指が離れている場合前タッチした時の座標
    public Vector2 ScreenPosition { set; private get; }

    public bool _isRotateModel = false;
    private float _targetAngle = 0f;
    public float _rotationSpeed = 4f;

    [SerializeField] private LeanFingerDown _leanFingerDown;

    void Start()
    {
        _isRotateModel = false;
        _targetAngle = 0f;
        _rotationSpeed = 4f;

        LeanTouch.OnFingerDown += OnFingerDown;
    }

    void OnFingerDown(LeanFinger finger)
    {
        Debug.Log("タッチされた");
        ScreenPosition = finger.StartScreenPosition;
        _targetAngle = GetRotationAngleByTargetPosition(new Vector3(ScreenPosition.x, ScreenPosition.y, 0));
        Debug.Log(ScreenPosition.x + "," + ScreenPosition.y);
    }

    void Update()
    {

        if (_isRotateModel)
        {
            transform.eulerAngles
            = new Vector3(0f,0f, Mathf.LerpAngle(this.transform.eulerAngles.z, _targetAngle, Time.deltaTime * _rotationSpeed));

            transform.Rotate(new Vector3(0f, 0.5f, 0f));
        }
    }

    public void OnClick()
    {
        _isRotateModel = _isRotateModel ? false : true;
    }

        float GetRotationAngleByTargetPosition(Vector3 mousePosition)
    {

        //自身の位置をワールド座標からスクリーン座標へ変換する
        Vector3 selfScreenPoint = Camera.main.WorldToScreenPoint(this.transform.position);
        //カプセル位置とマウスクリック位置の座標の差分を計算
        //diff x = 隣辺 y = 対辺 
        Vector3 diff = mousePosition - selfScreenPoint;

        //Mathf.Atan2 = タンジェントを算出できずAtanメソッドに引数を与えられない場合でも適切な角度を返す
        //Mathf.Rad2Deg 度からラジアンに変換する定数（読み取り専用） (PI * 2) / 360
        float angle = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        //90度引くことで動かす角度
        float finalAngle = angle - 90f;

        return finalAngle;
    }
}
