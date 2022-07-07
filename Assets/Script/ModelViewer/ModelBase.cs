using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelBase : MonoBehaviour
{
    [SerializeField] public Transform _top;
    [SerializeField] public Transform�@_buttom;
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

        // �I�u�W�F�N�g�̏�Ɖ��̃X�N���[�����W
        Debug.Log(_mainCamera.WorldToScreenPoint(_top.transform.position));
        Debug.Log(_mainCamera.WorldToScreenPoint(_buttom.transform.position));

        if (_sequence == Sequence.MenuMove && !_isSelect)
        {
            //�͈͊O�܂œ�����
            if (this.gameObject.transform.position.y > Screen.height / 2)
            {

            }
            else
            {

            }

            if (_mainCamera.WorldToScreenPoint(_buttom.transform.position).y + 10f > Screen.height)
            {
                //�͈͊O(��)�ɏ��O����
                Destroy(this.gameObject);
            }
            
            if(_mainCamera.WorldToScreenPoint(_top.transform.position).y - 10f < 0)
            {
                //�͈͊O(��)�ɏ��O����
                Destroy(this.gameObject);
            }
        }
        else
        {
            switch (_sequence)
            {
                case Sequence.MenuMove:
                    //�^�񒆂Ɉړ�������
                    //�傫������
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
