using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectMenu : MonoBehaviour
{
    [SerializeField] List<GameObject> _model;
    [SerializeField] GameObject _test;
    [SerializeField] Camera _testCamera;

    private Vector2 _screenSize = new Vector2(Screen.width, Screen.height);
    private int _modelIndex = 0;  //Listのどのモデルを生成するか
    private List<GameObject> _generateModel;  //生成されているModel(3個か4個) 

    void Start()
    {

    }

    void Update()
    {

        //モデルサイズがスクリーン縦の範囲内なら生成させる
        //真ん中作って上下に置けるか判断する
        //if(Screen.height < )
        

        //スワイプでモデルの位置を移動させる

        // オブジェクトの真中のスクリーン座標
        Debug.Log(_testCamera.WorldToScreenPoint(_test.transform.position));
    }
}
