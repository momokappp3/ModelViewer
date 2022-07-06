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
                //��ɏ��O����
                //���O���ꂽ��Destroy List������폜����
            }
            else
            {
                //���ɏ��O����
                //���O���ꂽ��Destoroy List������폜����
            }
        }
        else
        {
            switch (_sequence)
            {
                case Sequence.MenuMove:
                    //�^�񒆂Ɉړ�������
                    //�傫������
                    break;
                case Sequence.Viewer:
                    //���̃I�u�W�F�N�g�̃}�E�X���͂������悤�ɂȂ�
                    //���̃I�u�W�F�N�g��UI���\�������悤�ɂȂ�
                    break;
                default:
                    break;
            }
        }
    }
}
