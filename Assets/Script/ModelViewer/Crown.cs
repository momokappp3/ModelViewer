using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Touch;

public class Crown : MonoBehaviour
{
    [SerializeField] Transform _crown;  //�������I�u�W�F�N�g
    private Camera _camera = null;

    //�^�b�`�������̍��W�w������Ă���ꍇ�O�^�b�`�������̍��W
    public Vector2 ScreenPosition { set; private get; }

    public bool _isRotateModel = false;
    private float _targetAngle = 0f;
    public float _rotationSpeed = 4f;

    private bool _isLeapAngle = false;
    

    void Start()
    {
        _isRotateModel = false;
        _targetAngle = 0f;
        _rotationSpeed = 4f;

        _camera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        LeanTouch.OnFingerDown += OnFingerDown;
    }

    void OnFingerDown(LeanFinger finger)
    {
        Debug.Log("�^�b�`���ꂽ");
        ScreenPosition = finger.StartScreenPosition;
        _targetAngle = GetRotationAngleByTargetPosition(new Vector3(ScreenPosition.x, ScreenPosition.y, 0));

        if(_targetAngle < 0)
        {
            _targetAngle += 360.0f;
        }

        Debug.Log(ScreenPosition.x + "," + ScreenPosition.y);
        _isLeapAngle = true;
        
    }

    void Update()
    {
        if (_isLeapAngle)
        {
            _crown.transform.eulerAngles
                = new Vector3(0f, 0f, Mathf.LerpAngle(_crown.transform.eulerAngles.z, _targetAngle, Time.deltaTime * _rotationSpeed));

            var zPlus = _crown.transform.eulerAngles.z + 1f;
            var zMinus = _crown.transform.eulerAngles.z - 1f;

            //Debug.Log($"Target({_targetAngle}) : Plus({zPlus}) : Minus({zMinus})");

            if(_targetAngle >= zMinus && _targetAngle <= zPlus)
            {
                _isLeapAngle = false;
            }
        }
        if (_isRotateModel)
        {
            //transform.Rotate(new Vector3(0f, 0.2f, 0f));
            _crown.transform.RotateAround(transform.position, transform.up, 50f * Time.deltaTime);
        }
    }

    public void OnClick()
    {
        _isRotateModel = _isRotateModel ? false : true;
        Debug.Log("�N���b�N");
    }

    float GetRotationAngleByTargetPosition(Vector3 mousePosition)
    {

        //���g�̈ʒu�����[���h���W����X�N���[�����W�֕ϊ�����
        Vector3 selfScreenPoint = _camera.WorldToScreenPoint(_crown.transform.position);
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
