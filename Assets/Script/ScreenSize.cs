using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenSize : MonoBehaviour
{
    //�ŏ��ɍ������ʂ�width
    public float defaultWidth = 9.0f;

    //�ŏ��ɍ������ʂ�height
    public float defaultHeight = 16.0f;

    void Start()
    {
        //camera.main��ϐ��Ɋi�[
        Camera mainCamera = Camera.main;

        //�ŏ��ɍ������ʂ̃A�X�y�N�g�� 
        float defaultAspect = defaultWidth / defaultHeight;

        //���ۂ̉�ʂ̃A�X�y�N�g��
        float actualAspect = (float)Screen.width / (float)Screen.height;

        //���@��unity��ʂ̔䗦
        float ratio = actualAspect / defaultAspect;

        //�T�C�Y����
        mainCamera.orthographicSize /= ratio;

    }
}
