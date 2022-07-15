using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectMenu : MonoBehaviour
{
    [SerializeField] List<GameObject> _model;
    [SerializeField] Camera _camera;
    
    private int _modelIndex = 0;  //List�̂ǂ̃��f���𐶐����邩
    private List<GameObject> _modelInstance = new List<GameObject>();  //��������Ă���Model(3��4��) 

    private Vector2 _screenWorldRightUp = Vector2.zero;
    private Vector2 _screenWorldLeftDown = Vector2.zero;
    //��������ʒu�ǂꂾ���󂯂邩(World���W)
    private float _modelOffsetY = 0f;

    void Start()
    {
        var rightUp = _camera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 3));  //�E��
        var leftDown = _camera.ScreenToWorldPoint(new Vector3(0, 0, 3));  //����

        _screenWorldRightUp = new Vector2(rightUp.x, rightUp.y);
        _screenWorldLeftDown = new Vector2(leftDown.x, leftDown.y);
        _modelOffsetY = 1f;

        //1���f������(���f���̐^�񒆂���ԏ��)
        _modelInstance.Add(Instantiate(_model[_modelIndex], new Vector3(0, _screenWorldRightUp.y, 3f), Quaternion.identity));
        _modelIndex++;

        //�O�ɐ����������f���̉��ɐ���
        // ����1 ��̃��f�� ����2 ��ɐ������邩���ɐ������邩
        ModelGenerate();
        ModelGenerate();
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
#if UNITY_EDITOR
        var rightUp = _camera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 3));  //�E��
        var leftDown = _camera.ScreenToWorldPoint(new Vector3(0, 0, 3));  //����

        _screenWorldRightUp = new Vector2(rightUp.x, rightUp.y);
        _screenWorldLeftDown = new Vector2(leftDown.x, leftDown.y);
#endif

        //�X���C�v�Ń��f���̈ʒu���ړ�������








    }

    //�����͐������郂�f����index
    void ModelGenerate()
    {
        //�O�ɐ������ꂽ���f���̈�ԉ��̍��W
        var buttomPosi = _modelInstance[_modelIndex - 1].GetComponent<ModelBase>().GetButtomY();
        //���f���T�C�Y���X�N���[���c�͈̔͊O�Ȃ�return
        if (buttomPosi < _screenWorldLeftDown.y )
        {
            return;
        }
        _modelInstance.Add(Instantiate(_model[_modelIndex],
            new Vector3(0, buttomPosi - _modelOffsetY - _model[_modelIndex - 1].GetComponent<ModelBase>().GetDiffToMiddle(), 3f), Quaternion.identity));
        _modelIndex++;
    }
}
