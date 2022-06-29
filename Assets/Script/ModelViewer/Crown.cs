using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Touch;

public class Crown : MonoBehaviour
{
    //�f�����厖
    //�^�b�`�������̍��W�w������Ă���ꍇ�O�^�b�`�������̍��W
    public Vector2 ScreenPosition { set; private get; }

    public bool _isRotateModel = false;
    private float _targetAngle = 0f;
    public float _rotationSpeed = 4f;

    [SerializeField] private LeanFingerDown _leanFingerDown;

    void Start()
    {
        _isRotateModel = false;
        _targetAngle = 0f;
        _rotationSpeed = 4f;

        LeanTouch.OnFingerDown += OnFingerDown;
    }

    void OnFingerDown(LeanFinger finger)
    {
        Debug.Log("�^�b�`���ꂽ");
        ScreenPosition = finger.StartScreenPosition;
        _targetAngle = GetRotationAngleByTargetPosition(new Vector3(ScreenPosition.x, ScreenPosition.y, 0));
        Debug.Log(ScreenPosition.x + "," + ScreenPosition.y);
    }

    void Update()
    {

        if (_isRotateModel)
        {
            transform.eulerAngles
            = new Vector3(0f,0f, Mathf.LerpAngle(this.transform.eulerAngles.z, _targetAngle, Time.deltaTime * _rotationSpeed));

            transform.Rotate(new Vector3(0f, 0.5f, 0f));
        }
    }

    public void OnClick()
    {
        _isRotateModel = _isRotateModel ? false : true;
    }

        float GetRotationAngleByTargetPosition(Vector3 mousePosition)
    {

        //���g�̈ʒu�����[���h���W����X�N���[�����W�֕ϊ�����
        Vector3 selfScreenPoint = Camera.main.WorldToScreenPoint(this.transform.position);
        //�J�v�Z���ʒu�ƃ}�E�X�N���b�N�ʒu�̍��W�̍������v�Z
        //diff x = �ו� y = �Ε� 
        Vector3 diff = mousePosition - selfScreenPoint;

        //Mathf.Atan2 = �^���W�F���g���Z�o�ł���Atan���\�b�h�Ɉ�����^�����Ȃ��ꍇ�ł��K�؂Ȋp�x��Ԃ�
        //Mathf.Rad2Deg �x���烉�W�A���ɕϊ�����萔�i�ǂݎ���p�j (PI * 2) / 360
        float angle = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        //90�x�������Ƃœ������p�x
        float finalAngle = angle - 90f;

        return finalAngle;
    }
}