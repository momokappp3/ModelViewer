using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectMenu : MonoBehaviour
{
    [SerializeField] List<GameObject> _model;
    [SerializeField] GameObject _test;
    [SerializeField] Camera _testCamera;

    private Vector2 _screenSize = new Vector2(Screen.width, Screen.height);
    private int _modelIndex = 0;  //List�̂ǂ̃��f���𐶐����邩
    private List<GameObject> _generateModel;  //��������Ă���Model(3��4��) 

    void Start()
    {

    }

    void Update()
    {

        //���f���T�C�Y���X�N���[���c�͈͓̔��Ȃ琶��������
        //�^�񒆍���ď㉺�ɒu���邩���f����
        //if(Screen.height < )
        

        //�X���C�v�Ń��f���̈ʒu���ړ�������

        // �I�u�W�F�N�g�̐^���̃X�N���[�����W
        Debug.Log(_testCamera.WorldToScreenPoint(_test.transform.position));
    }
}
