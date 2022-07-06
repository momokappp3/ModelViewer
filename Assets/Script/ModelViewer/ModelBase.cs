using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelBase : MonoBehaviour
{
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
    }

    void Update()
    {
        if (_sequence == Sequence.MenuMove && !_isSelect)
        {
            if (this.transform.position.y < Screen.height / 2)
            {
                //上に除外する
                //除外されたらDestroy Listからも削除する
            }
            else
            {
                //下に除外する
                //除外されたらDestoroy Listからも削除する
            }
        }
        else
        {
            switch (_sequence)
            {
                case Sequence.MenuMove:
                    //真ん中に移動させる
                    //大きくする
                    break;
                case Sequence.Viewer:
                    //このオブジェクトのマウス入力が効くようになる
                    //このオブジェクトのUIが表示されるようになる
                    break;
                default:
                    break;
            }
        }
    }
}
