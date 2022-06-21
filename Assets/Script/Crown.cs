using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Touch;

public class Crown : MonoBehaviour
{
    //�f�����厖
    //�^�b�`�������̍��W�w������Ă���ꍇ�O�^�b�`�������̍��W
    public Vector2 ScreenPosition { set; private get; }

    void Start()
    {

    }

    void Update()
    {
        transform.Rotate(new Vector3(0, 0, 0.05f));

        if (LeanTouch.Fingers[0].Set)
        {
            ScreenPosition = LeanTouch.Fingers[0].StartScreenPosition;
        }
    }
}
