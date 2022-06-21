using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Touch;

public class Crown : MonoBehaviour
{
    //素結合大事
    //タッチした時の座標指が離れている場合前タッチした時の座標
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
