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
    private Vector3 _modelOffset = Vector3.zero; 
    //topから生成されているモデルtopまで, 0から生成されているbuttomまで
    private Vector2 _screenBlank;  

    void Start()
    {
        //position Xがおかしい　もう既に　_camera.ScreenToWorldPointが上手くいっていない
        var position = _camera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height,_camera.nearClipPlane));

        _screenSize = new Vector2(position.x, position.y);
        _modelOffset = _camera.ScreenToWorldPoint(new Vector3(0f, 20f, 0f));

        Debug.Log(_screenSize);
        //とりあえず真中に生成
        if (_modelInstance.Count == 0)
        {
            _modelInstance.Add(Instantiate(_model[_modelIndex], new Vector3(_screenSize.x, _screenSize.y, 3f), Quaternion.identity));
            _modelIndex++;
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
