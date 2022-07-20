using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Touch;

public class SelectMenu : MonoBehaviour
{
    [SerializeField] List<GameObject> _model;
    //private int _modelIndex = 0;  //どのモデルを生成するか
    [SerializeField] Camera _camera;
    
    private List<GameObject> _modelInstance = new List<GameObject>();  //生成されているModel(3個か4個) 

    private Vector2 _screenWorldRightUp = Vector2.zero;
    private Vector2 _screenWorldLeftDown = Vector2.zero;
    
    private float _modelOffsetY = 0f;  //生成する位置どれだけ空けるか(World座標)

    void Start()
    {
        var rightUp = _camera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 3));  //右上
        var leftDown = _camera.ScreenToWorldPoint(new Vector3(0, 0, 3));  //左下

        _screenWorldRightUp = new Vector2(rightUp.x, rightUp.y);
        _screenWorldLeftDown = new Vector2(leftDown.x, leftDown.y);
        _modelOffsetY = 1f;

        //1個モデル生成(モデルの真ん中を一番上に)
        _modelInstance.Add(Instantiate(_model[0], new Vector3(0, _screenWorldRightUp.y, 3f), Quaternion.identity));

        //前に生成したモデルの下に生成
        // 引数1 基準にするモデルIndex 引数2 上に生成するか
        ModelGenerate(0,true);

        //ここの引数が0としてわたっている
        ModelGenerate(1, true);

        //swipeの登録
        LeanTouch.OnFingerSwipe += HandleFingerSwipe;
    }
    
    private void HandleFingerSwipe(LeanFinger finger)
    {
        /*
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
        */

        //HandleFingerSwipe(finger, finger.StartScreenPosition, finger.ScreenPosition);
    }
    

    void Update()
    {
        for (int i = 0; i < _modelInstance.Count; i++)
        {
            if (_modelInstance[i] == null)
            {
                _modelInstance.Remove(_modelInstance[i]);
            }
        }
#if UNITY_EDITOR
        //ModelBaseにスクリーンサイズを渡す
        var rightUp = _camera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 3));  //右上
        var leftDown = _camera.ScreenToWorldPoint(new Vector3(0, 0, 3));  //左下

        _screenWorldRightUp = new Vector2(rightUp.x, rightUp.y);
        _screenWorldLeftDown = new Vector2(leftDown.x, leftDown.y);

        for (int i = 0; i < _modelInstance.Count; i++)
        {
            if (_modelInstance[i] != null)
            {
                _modelInstance[i].GetComponent<ModelBase>().SetScreenWorldHeightPosi
                    (new Vector2(_screenWorldRightUp.y, _screenWorldLeftDown.y));
            }
        }
#endif

        //スワイプでモデルの位置を移動させる
        //生成削除の制御
        //二次元配列にする Instanceとどのモデルを生成するかの番号



    }

    //引数 = 生成する基準のモデルのindex,上か下か
    void ModelGenerate(int preIndex,bool up)
    {
        //基準のモデルの下に作る
        if (!up)
        {
            var modelIndex = _model.Count == preIndex ? 0 : preIndex + 1;
            var buttomPosi = _modelInstance[preIndex].GetComponent<ModelBase>().GetButtomY();

            _modelInstance.Add(Instantiate(_model[modelIndex],
                new Vector3(0, buttomPosi - _modelOffsetY - _modelInstance[preIndex].GetComponent<ModelBase>().GetDiffToMiddle(), 3f),
                Quaternion.identity));

            //ここで落ちる
            _modelInstance[modelIndex].GetComponent<ModelBase>().SetScreenWorldHeightPosi
                (new Vector2(_screenWorldRightUp.y, _screenWorldLeftDown.y));
            //_modelIndex++;
        }
        else
        {
            var modelIndex = 0 == preIndex ? _model.Count - 1 : preIndex -= 1;
            var topPosi = _modelInstance[preIndex].GetComponent<ModelBase>().GetTopY();

            _modelInstance.Add(Instantiate(_model[modelIndex],
                new Vector3(0, topPosi + _modelOffsetY + _modelInstance[preIndex].GetComponent<ModelBase>().GetDiffToMiddle(), 3f),
                Quaternion.identity));
            //_modelIndex++;
        }
    }
}
