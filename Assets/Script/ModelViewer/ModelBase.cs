using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelBase : MonoBehaviour
{
    [SerializeField] private Transform _topObject;
    [SerializeField] private Transform _buttomObject;
    [SerializeField] private GameObject _enableToViewer;

    private Vector2 _screenWorldHegitPosi = Vector2.zero;

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

        _diffToMiddle = _topObject.transform.position.y - this.transform.position.y;
    }

    public float GetButtomY()
    {
        return _buttomObject.transform.position.y;
    }

    public float GetTopY()
    {
        return _topObject.transform.position.y;
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
            /*
            //範囲外まで動かす
            if (this.gameObject.transform.position.y > Screen.height / 2)
            {


            }
            else
            {

            }*/

            if (_mainCamera.WorldToScreenPoint(_buttomObject.transform.position).y + 10f > _screenWorldHegitPosi.x)
            {
                //範囲外(上)に除外する
                Destroy(this.gameObject);
            }

            if (_mainCamera.WorldToScreenPoint(_topObject.transform.position).y - 10f < _screenWorldHegitPosi.y)
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
    public void SetScreenWorldHeightPosi(Vector2 posi)
    {
        _screenWorldHegitPosi = posi;
    }
}
