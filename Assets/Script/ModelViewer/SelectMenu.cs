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
    //topから生成されているモデルtopまで, 0から生成されているbuttomまで
    private Vector2 _screenBlank;  

    void Start()
    {
        var position = _camera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height,0));

        _screenSize = new Vector2(position.x, position.y);

        Debug.Log(position);

        //とりあえず真中に生成
        if (_modelInstance.Count == 0)
        {
            _modelInstance.Add(Instantiate(_model[0], new Vector3(0, 0, 3f), Quaternion.identity));
            //DrawFingetDownを無効にする

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
