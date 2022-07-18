using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Touch;

public class SelectMenu : MonoBehaviour
{
    [SerializeField] List<GameObject> _model;
    [SerializeField] Camera _camera;
    
    private int _modelIndex = 0;  //Listのどのモデルを生成するか
    private List<GameObject> _modelInstance = new List<GameObject>();  //生成されているModel(3個か4個) 

    private Vector2 _screenWorldRightUp = Vector2.zero;
    private Vector2 _screenWorldLeftDown = Vector2.zero;
    //生成する位置どれだけ空けるか(World座標)
    private float _modelOffsetY = 0f;

   // private float 

    void Start()
    {
        var rightUp = _camera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 3));  //右上
        var leftDown = _camera.ScreenToWorldPoint(new Vector3(0, 0, 3));  //左下

        _screenWorldRightUp = new Vector2(rightUp.x, rightUp.y);
        _screenWorldLeftDown = new Vector2(leftDown.x, leftDown.y);
        _modelOffsetY = 1f;

        //1個モデル生成(モデルの真ん中を一番上に)
        _modelInstance.Add(Instantiate(_model[_modelIndex], new Vector3(0, _screenWorldRightUp.y, 3f), Quaternion.identity));
        _modelIndex++;

        //前に生成したモデルの下に生成
        // 引数1 基準のモデル 引数2 上に生成するか下に生成するか
        ModelGenerate();
        ModelGenerate();

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








    }

    //引数は生成するモデルのindex
    void ModelGenerate()
    {
        //前に生成されたモデルの一番下の座標
        var buttomPosi = _modelInstance[_modelIndex - 1].GetComponent<ModelBase>().GetButtomY();
        //モデルサイズがスクリーン縦の範囲外ならreturn
        if (buttomPosi < _screenWorldLeftDown.y )
        {
            return;
        }
        _modelInstance.Add(Instantiate(_model[_modelIndex],
            new Vector3(0, buttomPosi - _modelOffsetY - _model[_modelIndex - 1].GetComponent<ModelBase>().GetDiffToMiddle(), 3f),
            Quaternion.identity));


        _modelInstance[_modelInstance.Count].GetComponent<ModelBase>().SetScreenWorldHeightPosi
            (new Vector2(_screenWorldRightUp.y, _screenWorldLeftDown.y));
        _modelIndex++;
    }
}
