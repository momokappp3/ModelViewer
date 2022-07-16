using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelBase : MonoBehaviour
{
    [SerializeField] private Transform _topObject;
    [SerializeField] private Transform _buttomObject;
    [SerializeField] private GameObject _enableToViewer;

    public Vector3 _top = Vector3.zero;
    public Vector3 _buttom = Vector3.zero;

    private Camera _mainCamera;
    public bool _isUpMove = false;
    public float _diffToMiddle = 0f;  //上の位置から真ん中までの差

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

        _diffToMiddle = _top.y - this.transform.position.y;

        _top = _topObject.transform.position;
        _buttom = _buttomObject.transform.position;
    }

    public float GetButtomY()
    {
        _buttom = _buttomObject.transform.position;
        return _buttom.y;
    }

    public float GetDiffToMiddle()
    {
        _diffToMiddle = _topObject.position.y - this.transform.position.y;
        return _diffToMiddle;
    }

    void Update()
    {
        if (_sequence == Sequence.MenuMove && !_isSelect)
        {
            //範囲外まで動かす
            if (this.gameObject.transform.position.y > Screen.height / 2)
            {

            }
            else
            {

            }

            if (_mainCamera.WorldToScreenPoint(_buttom).y + 10f > Screen.height)
            {
                //範囲外(上)に除外する
                Destroy(this.gameObject);
            }

            if (_mainCamera.WorldToScreenPoint(_top).y - 10f < 0)
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
