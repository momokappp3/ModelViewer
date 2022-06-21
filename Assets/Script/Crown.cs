using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Touch;

public class Crown : MonoBehaviour
{
    LeanFinger _leanFinger;  //???


    void Start()
    {

    }

    void Update()
    {
        transform.Rotate(new Vector3(0, 0, 0.05f));


    }
}
