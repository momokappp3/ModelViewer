using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectMenu : MonoBehaviour
{
    [SerializeField] List<GameObject> _model;
    [SerializeField] Camera _camera;
    
    private int _modelIndex = 0;  //Listのどのモデルを生成するか
    private List<GameObject> _modelInstance = new List<GameObject>();  //生成されているModel(3個か4個) 

    private Vector2 _screenSize = Vector2.zero;
    //生成する位置どれだけ空けるか(World座標)
    private float _modelOffsetY = 0f;
    //topから生成されているモデルtopまで, 0から生成されているbuttomまで
    private Vector2 _screenBlank;  

    void Start()
    {
        //position Xがおかしい　もう既に　_camera.ScreenToWorldPointが上手くいっていない
        var position = _camera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height,3));

        _screenSize = new Vector2(position.x, position.y);
        _modelOffsetY = 1f;

        Debug.Log(_screenSize);
        //とりあえずxは真中に生成
        if (_modelInstance.Count == 0)
        {
            _modelInstance.Add(Instantiate(_model[_modelIndex], new Vector3(0, _screenSize.y, 3f), Quaternion.identity));
            _modelIndex++;

            //生成されたオブジェクトの一番下
            var buttomPosi = _modelInstance[_modelIndex - 1].GetComponent<ModelBase>().GetButtomY();
            //前作った_buttomがスクリーン座標の縦(0)からはみ出ていなかったら生成する
            if (buttomPosi > 0)
            {
                _modelInstance.Add(Instantiate(_model[_modelIndex],
                    new Vector3(0, buttomPosi - _modelOffsetY - _model[_modelIndex -1].GetComponent<ModelBase>().GetDiffToMiddle(), 3f), Quaternion.identity));
                _modelIndex++;
            }
        }
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

        //とりあえず上から順番に生成


        //モデルサイズがスクリーン縦の範囲内なら生成させる
        //真ん中作って上下に置けるか判断する
        //if(Screen.height < )


        //スワイプでモデルの位置を移動させる
    }
}
