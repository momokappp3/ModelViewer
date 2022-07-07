using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectMenu : MonoBehaviour
{
    [SerializeField] List<GameObject> _model;
    [SerializeField] Camera _camera;
    
    private int _modelIndex = 0;  //List�̂ǂ̃��f���𐶐����邩
    private List<GameObject> _modelInstance = new List<GameObject>();  //��������Ă���Model(3��4��) 

    private Vector2 _screenSize = Vector2.zero;
    //top���琶������Ă��郂�f��top�܂�, 0���琶������Ă���buttom�܂�
    private Vector2 _screenBlank;  

    void Start()
    {
        var position = _camera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height,0));

        _screenSize = new Vector2(position.x, position.y);

        Debug.Log(position);

        //�Ƃ肠�����^���ɐ���
        if (_modelInstance.Count == 0)
        {
            _modelInstance.Add(Instantiate(_model[0], new Vector3(0, 0, 3f), Quaternion.identity));
            //DrawFingetDown�𖳌��ɂ���

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

        //�Ƃ肠�����ォ�珇�Ԃɐ���


        //���f���T�C�Y���X�N���[���c�͈͓̔��Ȃ琶��������
        //�^�񒆍���ď㉺�ɒu���邩���f����
        //if(Screen.height < )


        //�X���C�v�Ń��f���̈ʒu���ړ�������
    }
}
