using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Touch;

using System.Collections.Specialized;

public class SelectMenu : MonoBehaviour
{
    [SerializeField] List<GameObject> _model;
    [SerializeField] Camera _camera;
    [SerializeField] LeanFingerSwipe _leanFingerSwipe;
    
    private List<GameObject> _modelInstance = new List<GameObject>();  //生成されているModel(3個か4個) 
    private List<int> _modelInsIndex = new List<int>();  //どのモデルを生成するか
    //private Dictionary<int,GameObject> _d_modelInstance = new Dictionary<int, GameObject>();

    private Vector2 _screenWorldRightUp = Vector2.zero;
    private Vector2 _screenWorldLeftDown = Vector2.zero;
    
    private float _modelOffsetY = 0f;  //生成する位置どれだけ空けるか(World座標)


    //スワイプ中に呼ばれる
    public void OnDelta(Vector2 delta)
    {
        //delta分動かす
    }

    void Start()
    {
        var rightUp = _camera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 3));  //右上
        var leftDown = _camera.ScreenToWorldPoint(new Vector3(0, 0, 3));  //左下

        _screenWorldRightUp = new Vector2(rightUp.x, rightUp.y);
        _screenWorldLeftDown = new Vector2(leftDown.x, leftDown.y);
        _modelOffsetY = 1f;

        //1個モデル生成(モデルの真ん中を一番上に)
        //_modelInstance.Add(Instantiate(_model[0], new Vector3(0, _screenWorldRightUp.y, 3f), Quaternion.identity));

        _modelInstance.Add(Instantiate(_model[0], new Vector3(0, _screenWorldRightUp.y, 3f), Quaternion.identity));
        _modelInsIndex.Add(0);

        //前に生成したモデルの下に生成
        // 引数1 基準にするモデルIndex 引数2 上に生成するか
        ModelGenerate(0,true);
        ModelGenerate(1, true);

    }

    void Update()
    {
        for (int i = 0; i < _modelInstance.Count; i++)
        {
            if (_modelInstance[i] == null)
            {
                _modelInstance.Remove(_modelInstance[i]);
                _modelInsIndex.Remove(_modelInsIndex[i]);
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
        var modelInstance = _modelInstance[preIndex];
        var preModelIndex = _modelInsIndex[preIndex];
        var modelBase = modelInstance.GetComponent<ModelBase>();
        var modelIndex = -1;

        if (up)
        {
            modelIndex = 0 == preModelIndex ? _model.Count - 1 : preModelIndex - 1;
            var topPos = modelBase.GetTopY();
            var pos = new Vector3(0, topPos + _modelOffsetY + modelBase.GetDiffToMiddle(), 3f);

            _modelInstance.Add(Instantiate(_model[modelIndex], pos, Quaternion.identity));
        }
        else
        {
            modelIndex = _model.Count == preModelIndex ? 0 : preModelIndex + 1;
            var buttomPos = modelBase.GetButtomY();
            var pos = new Vector3(0, buttomPos - _modelOffsetY - modelBase.GetDiffToMiddle(), 3f);

            _modelInstance.Add(Instantiate(_model[modelIndex],pos,Quaternion.identity));
        }

        _modelInsIndex.Add(modelIndex);

        var addModelBace = _modelInstance[_modelInstance.Count - 1].GetComponent<ModelBase>();

        addModelBace.SetScreenWorldHeightPosi(new Vector2(_screenWorldRightUp.y, _screenWorldLeftDown.y));
    }
}
