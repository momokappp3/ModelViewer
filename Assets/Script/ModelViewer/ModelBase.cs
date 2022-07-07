using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelBase : MonoBehaviour
{
    [SerializeField] public Transform _top;
    [SerializeField] public Transform　_buttom;
    [SerializeField] private GameObject _enableToViewer;

    private Camera _mainCamera;
    public bool _isUpMove = false;

    enum Sequence
    {
        MenuMove,
        Viewer,
        Max
    }

    private Sequence _sequence = Sequence.Max;

    public bool _isSelect = false;

    void Start()
    {
        _sequence = Sequence.Max;
        _isSelect = false;
        _mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        _enableToViewer.SetActive(false);
    }

    void Update()
    {

        // オブジェクトの上と下のスクリーン座標
        Debug.Log(_mainCamera.WorldToScreenPoint(_top.transform.position));
        Debug.Log(_mainCamera.WorldToScreenPoint(_buttom.transform.position));

        if (_sequence == Sequence.MenuMove && !_isSelect)
        {
            //範囲外まで動かす
            if (this.gameObject.transform.position.y > Screen.height / 2)
            {

            }
            else
            {

            }

            if (_mainCamera.WorldToScreenPoint(_buttom.transform.position).y + 10f > Screen.height)
            {
                //範囲外(上)に除外する
                Destroy(this.gameObject);
            }
            
            if(_mainCamera.WorldToScreenPoint(_top.transform.position).y - 10f < 0)
            {
                //範囲外(下)に除外する
                Destroy(this.gameObject);
            }
        }
        else
        {
            switch (_sequence)
            {
                case Sequence.MenuMove:
                    //真ん中に移動させる
                    //大きくする
                    //_sequence = Sequence.Viewer;
                    break;
                case Sequence.Viewer:
                    _enableToViewer.SetActive(false);
                    break;
                case Sequence.Max:
                    break;
                default:
                    break;
            }
        }
    }
}
