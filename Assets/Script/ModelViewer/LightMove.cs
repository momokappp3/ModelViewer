using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightMove: MonoBehaviour
{
    //�v���C���[��ϐ��Ɋi�[
    public GameObject Player;

    //��]������X�s�[�h
    public float rotateSpeed = 3.0f;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //�v���C���[�ʒu���
        Vector3 playerPos = Player.transform.position;

        //�J��������]������
        transform.RotateAround(playerPos, 5f * rotateSpeed);
    }
}
