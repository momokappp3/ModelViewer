using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelObject : MonoBehaviour
{

    enum ModelSequence
    {
        Title,
        Menu,
        Viewer
    }

    private ModelSequence _modelSequence;

    void Start()
    {
        _modelSequence = ModelSequence.Viewer;
    }

    void Update()
    {
        switch (_modelSequence)
        {
            case ModelSequence.Title:
                break;
            case ModelSequence.Menu:
                //* �����������^�b�`���ꂽ��
                /*
                if ()
                {
                    this.transform.position = new Vector3(Screen.width / 2, Screen.height / 2, 0);
                    //�傳��Viewer�̎��̑傫���ɕύX
                    _modelSequence = ModelSequence.Viewer;
                }
                */
                break;
            case ModelSequence.Viewer:
                break;
            default:
                break;




        }
    }
}
