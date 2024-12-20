using UnityEngine;
//Mirror���g�p�\�Ƃ���
using Mirror;
using UnityEngine.UIElements;

/// <summary>
/// �ȒP��Mirror�ړ��������@
/// ��{�I�Ƀv���C���[���̓��͂��T�[�o�[���ɓn���ď������s���܂��B
/// 1.�N���C�A���g(�ړ���)���T�[�o�[�֓]��
/// 2.�T�[�o�[���A�ړ��ʂ��󂯎��A�L�����N�^�[���ړ�
/// 3.�T�[�o�[������ANetworkTransformReliable���o�R���āA�N���C�A���g�̃L�����N�^�[���ړ�
/// 
/// ����ɂ͗��R������A�N���C�A���g����������̈ړ��������s���ꍇ�A�N���C�A���g�̃f�[�^��
/// �������ƁA�`�[�g��������ł���ׁB
/// �T�[�o�[���Ɉړ��ʂ�n�����ہA�T�[�o�[���u�ړ��͂���������?�v�Ɣ��f�ł��邽�߁A
/// �T�[�o�[����N���C�A���g�ֈړ����䂪�����Ă���B
/// </summary>

///NetworkIdentity��ǉ�(�ʐM�ɕK�v)
[RequireComponent(typeof(NetworkIdentity))]

///NetworkTransformReliable��ǉ�(���W�E���������T�[�o�[����N���C�A���g�֗���)
///�������̐ݒ��[ServerToClient]�ɂ��鎖
[RequireComponent(typeof(NetworkTransformReliable))]

///NetworkAnimator��ǉ�(�A�j���[�V���������T�[�o�[����N���C�A���g�֗���)
///�������̐ݒ��[ServerToClient]�ɂ��鎖
[RequireComponent(typeof(NetworkAnimator))]

public class MirrorPlayerMoves : NetworkBehaviour
{
    [Header("�A�j���[�^�[�����N")]
    public Animator m_Animator;
    [Header("�L�����N�^�[�̈ړ����x")]
    public float m_MoveSpeed = 10f;
    [Header("�L�����N�^�[�̍ő呬�x")]
    public float m_MaxSpeed = 5f;
    [Header("�ړ��͌����l[0.5�b�ő��x��0�ɂȂ�悤�ɂ���]")]
    public float m_DecelerationTime = 0.5f;
    [Header("����"), SerializeField]
    private Rigidbody m_Rigidbody;
    [Header("�T�[�o�[�֓n���ړ���"), SerializeField]
    private Vector3 m_InputVector;
    [Header("�ړ��t���O"),SerializeField]
    private bool m_IsMoving = false;
    [Header("�ړ��A�j���[�V�����X�s�[�h"), SerializeField]
    private float m_AnimeMoveSpeed = 0;

    [Header("�O���J���������N"), SerializeField]
    private GameObject m_CameraLink;

    [Header("�L�����N�^�[�̐����"), SerializeField]
    private float m_RotationSpeed = 10.0f;

    [Header("[Shadow]�V�����ʒu���"), SerializeField,SyncVar]
    private Vector3 m_NewPosition;

    [Header("[Shadow]�V�����������"), SerializeField, SyncVar]
    private Quaternion m_NewRotation;

    [Header("[Shadow]���͏��"), SerializeField, SyncVar]
    private Vector3 m_NewDirection;



    void Start()
    {
        //�A�j���[�^�[�l��
        m_Animator = GetComponent<Animator>();
        //�������擾
        m_Rigidbody = GetComponent<Rigidbody>();
        //NetworkTransformReliable�Ɏ��g��transform�������N������
        GetComponent<NetworkTransformReliable>().target = this.transform;
        //NetworkAnimator�Ɏ��g��Animator�������N������
        GetComponent<NetworkAnimator>().animator = m_Animator;

        if (!isLocalPlayer)
        {
            m_Rigidbody.useGravity = false;
        }
    }

    void Update()
    {
        //�N���C�A���g/�v���C���[�ł���ꍇ�A�v���C���[���͂�����
        if (isLocalPlayer)
        {
            PlayerMove();
        }
    }
    private void LateUpdate()
    {
        //�N���C�A���g/�v���C���[�ł���ꍇ�A�v���C���[���͂�����
        if (isLocalPlayer)
        {
            //�J���������N���Ȃ��ꍇ�A�J���������N��T���Čq���A����ꍇ�́A�v���C���[�ɒǏ]������B
            if (!m_CameraLink)
                m_CameraLink = GameObject.Find("�J���������N");
            else
            {
                m_CameraLink.transform.position = this.transform.position;
                m_CameraLink.transform.GetChild(0).Rotate(new Vector3(0, Input.GetAxis("Mouse X"), 0));
            }
        }
        else
        {
            // ���̃N���C�A���g�̃f�[�^���󂯎���ăL�����N�^�[�ɓK�p
            transform.position = Vector3.Lerp(transform.position, m_NewPosition, Time.deltaTime * 10);
            transform.rotation = Quaternion.Lerp(transform.rotation, m_NewRotation, Time.deltaTime * 10);
            //�ړ��A�j���[�V��������
            MoveAnimator(m_NewDirection);
        }

    }
    /// <summary>
    /// �N���C�A���g��/�v���C���[���ł���ꍇ�A�ړ����͂������A�T�[�o�[�ֈړ��ʂ�n��
    /// </summary>
    public void PlayerMove()
    {
        if (!m_CameraLink)
            return;

        // ���͂��擾
        m_InputVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        //���͒l��0�ȊO�ł���΁Am_IsMoveing��true�Ƃ���B
        m_IsMoving = m_InputVector != Vector3.zero;

        //�v���C���[�̌������Đݒ肷��
        PlayerRotation(
            m_InputVector, 
            m_CameraLink.transform.GetChild(0).forward,
            m_CameraLink.transform.GetChild(0).right,
            ref m_NewPosition,
            ref m_NewRotation);

        //�T�[�o�[�ցA�ړ��ʒu�ƌ�������]��
        ServerMove(this.transform.position, m_NewRotation, m_InputVector);

    //�T�[�o�[�ֈړ��ʂ�]��
    /*
    ShadowMove(m_InputVector, m_CameraLink.transform.GetChild(0).forward, m_CameraLink.transform.GetChild(0).right);
    */
    //���g�֓]��

}

/// <summary>
/// �T�[�o�[�����󂯎�鏈��[�ړ�����]
/// </summary>
/// <param name="direction">�ړ���</param>
[Command]
    void ServerMove(Vector3 N_Position,Quaternion N_Rotation,Vector3 N_InputVector)
    {
        if (!isLocalPlayer)
        {
            //���W�X�V
            m_NewPosition = N_Position;
            //�����X�V
            m_NewRotation = N_Rotation;

            m_NewDirection = N_InputVector;

            // ���̃N���C�A���g�ɒʒm
            //ShadowMove(N_Position, N_Rotation,N_InputVector);
        }

        //�v���C���[�̌������Đݒ肷��
        //PlayerRotation(direction, CameraForward, CameraRight);
        /*
        //�ړ��ʂ̐��l��1�𒴂��Ă���ꍇ�A���̐��l�𐳋K������
        if (direction.magnitude > 1)
            direction.Normalize();

        //�ړ��́~�ړ��X�s�[�h�Ő��K�̈ړ��ʂƂ���
        Vector3 force = direction * m_MoveSpeed;

        // ���݂̑��x���`�F�b�N
        if (m_Rigidbody.velocity.magnitude < m_MaxSpeed)
            m_Rigidbody.AddForce(force, ForceMode.Force);

        // ���x���ő呬�x�𒴂��Ȃ��悤�ɂ���
        if (m_Rigidbody.velocity.magnitude > m_MaxSpeed)
            m_Rigidbody.velocity = m_Rigidbody.velocity.normalized * m_MaxSpeed;

        // �ړ����Ă��Ȃ��ꍇ�A�����������s��
        if (!m_IsMoving)
            m_Rigidbody.velocity =
                Vector3.Lerp(m_Rigidbody.velocity, Vector3.zero, Time.fixedDeltaTime / m_DecelerationTime);
        //�ړ��A�j���[�V��������
        MoveAnimator(direction);
        */


    }

    /*
    // �N���C�A���g�Ƀu���[�h�L���X�g����RPC (Remote Procedure Call)
    [ClientRpc]
    void ShadowMove(Vector3 N_Position, Quaternion N_Rotation, Vector3 N_InputVector)
    {
        // �������g�̃N���C�A���g�ł͎��s���Ȃ�
        if (!isLocalPlayer)
        {
            m_NewPosition = N_Position;
            m_NewRotation = N_Rotation;
            m_NewDirection = N_InputVector;
        }
    }
    */




    /// <summary>
    /// �L�����N�^�[�̌������J���������ƁA���͂��犄��o��
    /// </summary>
    public void PlayerRotation(
        Vector3 direction,
        Vector3 CameraForward,
        Vector3 CameraRight,
        ref Vector3 N_Position,
        ref Quaternion N_Rotation)
    {
        // ���͕����̃x�N�g���𐶐�
        Vector3 MoveDirection = direction.normalized;

        // ���͂�����Ή�]���������s
        if (MoveDirection.magnitude > 0)
        {
            // �J�����̑O���������琅���ʂ̌������擾�iY���͖����j
            CameraForward.y = 0;  // �L�����N�^�[��Y����]�ɉe�����Ȃ��悤�ɂ���
            CameraForward.Normalize();

            // �J�����̉E�����x�N�g�����擾
            CameraRight.y = 0; // ���l��Y���̉e���𖳎�
            CameraRight.Normalize();

            // �J������̈ړ��������Z�o
            Vector3 DesiredDirection = CameraForward * direction.z + CameraRight * direction.x;

            // �L�����N�^�[�̌�������]������
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(DesiredDirection), Time.deltaTime * 10f);

            // �����ړ����͂�����Η͂�������
            if (DesiredDirection.magnitude > 0)
                m_Rigidbody.AddForce(DesiredDirection * m_MoveSpeed);

            // ���݂̑��x���m�F���A�ő呬�x�𒴂��Ȃ��悤�ɂ���
            // ���݂̕��ʑ��x�iY�������������x�j���v�Z
            Vector3 flatVelocity = new Vector3(m_Rigidbody.velocity.x, 0, m_Rigidbody.velocity.z);

            // �ő呬�x�𒴂��Ă���ꍇ�A���x�𐧌�
            if (flatVelocity.magnitude > m_MaxSpeed)
            {
                // �������ꂽ���x�x�N�g����K�p
                Vector3 limitedVelocity = flatVelocity.normalized * m_MaxSpeed;
                m_Rigidbody.velocity = new Vector3(limitedVelocity.x, m_Rigidbody.velocity.y, limitedVelocity.z);
            }
            //�ŏI�ʒu
            N_Position = this.transform.position;
            //�ŏI����
            N_Rotation = this.transform.rotation;
        }
        //�ړ��A�j���[�V��������
        MoveAnimator(direction);
    }

    /// <summary>
    /// �T�[�o�[���ł̈ړ��A�j���[�V��������
    /// </summary>
    /// <param name="direction"></param>
    public void MoveAnimator(Vector3 direction)
    {
        //�ړ��A�j���[�V�������s
        //�T�[�o�[���󂯂��v���C���[����̈ړ��͂�0�ł͂Ȃ��B
        if (direction != Vector3.zero)
        {
            //�ړ��A�j���[�V�����X�s�[�h����(2�{�b)
            m_AnimeMoveSpeed += 2 * Time.deltaTime;
            if (m_AnimeMoveSpeed > 1) m_AnimeMoveSpeed = 1;
        }
        else
        {
            //�ړ��A�j���[�V�����X�s�[�h����(2�{�b)
            m_AnimeMoveSpeed -= 2 * Time.deltaTime;
            if (m_AnimeMoveSpeed <= 0) m_AnimeMoveSpeed = 0;
        }
        //�T�[�o�[���̃A�j���[�V������ύX
        m_Animator.SetFloat("Speed", m_AnimeMoveSpeed);
    }
}
