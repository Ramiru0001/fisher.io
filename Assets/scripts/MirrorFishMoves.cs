//MirrorFishMoves.cs
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
//[RequireComponent(typeof(NetworkAnimator))]

public class MirrorFishMoves : NetworkBehaviour
{
    //[Header("�A�j���[�^�[�����N")]
    //public Animator m_Animator;
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
    [Header("�ړ��t���O"), SerializeField]
    private bool m_IsMoving = false;
    //[Header("�ړ��A�j���[�V�����X�s�[�h"), SerializeField]
    //private float m_AnimeMoveSpeed = 0;

    [Header("�O���J���������N"), SerializeField]
    private GameObject m_CameraLink;

    [Header("�L�����N�^�[�̐����"), SerializeField]
    private float m_RotationSpeed = 10.0f;

    [Header("[Shadow]�V�����ʒu���"), SerializeField, SyncVar]
    private Vector3 m_NewPosition;

    [Header("[Shadow]�V�����������"), SerializeField, SyncVar]
    private Quaternion m_NewRotation;

    [Header("[Shadow]���͏��"), SerializeField, SyncVar]
    private Vector3 m_NewDirection;

    [Header("�J�����̐����"), SerializeField]
    private float cam_RotationSpeed = 3.0f;


    void Start()
    {
        //�A�j���[�^�[�l��
        //m_Animator = GetComponent<Animator>();
        //�������擾
        m_Rigidbody = GetComponent<Rigidbody>();
        if (m_Rigidbody == null)
        {
            Debug.LogError("Rigidbody���A�^�b�`����Ă��܂���I");
            return;
        }
        //NetworkTransformReliable�Ɏ��g��transform�������N������
        GetComponent<NetworkTransformReliable>().target = this.transform;
        //NetworkAnimator�Ɏ��g��Animator�������N������
        //GetComponent<NetworkAnimator>().animator = m_Animator;

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
        CameraControl();
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
            //MoveAnimator(m_NewDirection);
        }

    }

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        // �������T�[�o�[�łȂ��ꍇ�͉������Ȃ�
        if (!isServer) return;

        // �Փ˂������肪�v���C���[���m�F
        MirrorFishMoves otherPlayer = other.GetComponent<MirrorFishMoves>();
        if (otherPlayer == null) return; // ���肪�v���C���[�łȂ��ꍇ�͉������Ȃ�

        // �����Ƒ���̑傫���i�X�P�[���j���r
        float thisSize = transform.localScale.magnitude;
        float otherSize = otherPlayer.transform.localScale.magnitude;

        if (thisSize > otherSize)
        {
            // �������傫���ꍇ�A�o���l���l����������폜
            AddExperience(50); // �l���o���l�͓K�X����
            otherPlayer.GameOver();
        }
        else if (thisSize < otherSize)
        {
            // ���肪�傫���ꍇ�A�������폜�����
            otherPlayer.AddExperience(50);
            GameOver();
        }
        else
        {
            // �����T�C�Y�̏ꍇ�̏����i����͉������Ȃ��j
        }
    }

    [Server]
    private void AddExperience(int amount)
    {
        PlayerStats stats = GetComponent<PlayerStats>();
        if (stats != null)
        {
            stats.AddExperience(amount); // �v���C���[�̌o���l�����Z
        }
    }

    [Server]
    private void GameOver()
    {
        // �������Q�[������폜
        NetworkServer.Destroy(gameObject);
    }

    private void CameraControl()
    {
        if (m_CameraLink)
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            // Y���i������]�j�̓��[���h��Y������ɉ�]
            m_CameraLink.transform.rotation = Quaternion.Euler(
                m_CameraLink.transform.eulerAngles.x,
                m_CameraLink.transform.eulerAngles.y + mouseX * m_RotationSpeed,
                0
            );

            // X���i������]�j�̓��[���h��X������ɉ�]
            m_CameraLink.transform.GetChild(0).rotation = Quaternion.Euler(
                m_CameraLink.transform.GetChild(0).eulerAngles.x - mouseY * m_RotationSpeed,
                m_CameraLink.transform.GetChild(0).eulerAngles.y,
                0
            );

            // ���I�u�W�F�N�g�̃J����������ύX
            Transform cameraTransform = m_CameraLink.transform.GetChild(0).GetChild(0); // ���I�u�W�F�N�g�̃J����
            // �}�E�X�z�C�[���ŃJ���������𒲐�
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");
            if (scrollInput != 0)
            {
                Vector3 localPos = cameraTransform.localPosition;
                localPos.z += scrollInput * 5f; // �����ύX�̑��x
                localPos.z = Mathf.Clamp(localPos.z, -20f, -2f); // �ŏ��E�ő勗����ݒ�
                cameraTransform.localPosition = localPos;
            }
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
        m_InputVector = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Height"), Input.GetAxis("Vertical"));
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
    void ServerMove(Vector3 N_Position, Quaternion N_Rotation, Vector3 N_InputVector)
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

        //�ړ��A�j���[�V��������
        /*MoveAnimator(direction);
        */
    }

    // �N���C�A���g�Ƀu���[�h�L���X�g����RPC (Remote Procedure Call)
    //[ClientRpc]
    //void ShadowMove(Vector3 N_Position, Quaternion N_Rotation, Vector3 N_InputVector)
    //{
    //    // �������g�̃N���C�A���g�ł͎��s���Ȃ�
    //    if (!isLocalPlayer)
    //    {
    //        m_NewPosition = N_Position;
    //        m_NewRotation = N_Rotation;
    //        m_NewDirection = N_InputVector;
    //    }
    //}

    /// <summary>
    /// �L�����N�^�[�̌������J���������ƁA���͂��犄��o��
    /// </summary>
    public void PlayerRotation(
        //direction = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Height"), Input.GetAxis("Vertical"));
        Vector3 direction,
        Vector3 CameraForward,
        Vector3 CameraRight,
        ref Vector3 N_Position,
        ref Quaternion N_Rotation)
    {
        // �J�����̑O�㍶�E��������Ɉړ��������v�Z
        Vector3 moveDirection = (CameraForward * direction.z) + (CameraRight * direction.x) + (Vector3.up * direction.y);

        // �������͂��Ȃ��ꍇ�A��������return
        if (moveDirection.magnitude < 0.01f)
        {
            // ���͂��Ȃ��ꍇ�͑��x������������
            m_Rigidbody.velocity = Vector3.Lerp(m_Rigidbody.velocity, Vector3.zero, Time.deltaTime / m_DecelerationTime);
            // ���݂̈ʒu�E��]���T�[�o�[�ɍX�V
            N_Position = transform.position;
            N_Rotation = transform.rotation;
            return;
        }

        // �v���C���[�̌������v�Z�iY����]�j
        Vector3 flatDirection = new Vector3(moveDirection.x, 0, moveDirection.z); // ���������̈ړ��̂�
        // Z���̉�]�i�㉺��]�j���v�Z
        float tiltZ = Input.GetAxis("Height") * 30f; // ��]�p�x�𒲐����邽�߂̔{���i30f�͓K�X�����\�j
        Quaternion currentRotation = transform.rotation;
        Quaternion tiltRotationZ = Quaternion.Euler(0, 0, tiltZ);

        Quaternion finalRotation;

        if (flatDirection.magnitude > 0.01f)
        {
            // �����ړ�������ꍇ�AY����]���X�V
            Quaternion targetRotationY = Quaternion.LookRotation(flatDirection) * Quaternion.Euler(0, -90, 0);
            finalRotation = targetRotationY * tiltRotationZ;
        }
        else
        {
            // �����ړ����Ȃ��ꍇ�́A���݂�Y��]���ێ�����Z���̉�]�̂ݓK�p
            float currentY = currentRotation.eulerAngles.y;
            Quaternion yRotationPreserved = Quaternion.Euler(0, currentY, 0);
            finalRotation = yRotationPreserved * tiltRotationZ;
        }

        // ��]��K�p
        transform.rotation = Quaternion.Slerp(
        transform.rotation,
        finalRotation,
        Time.deltaTime * m_RotationSpeed
    );

        // Rigidbody���g���ăv���C���[���ړ�������
        m_Rigidbody.AddForce(moveDirection.normalized * m_MoveSpeed, ForceMode.Force);

            // ���݂̑��x���擾
            Vector3 velocity = m_Rigidbody.velocity;

            // �ő呬�x�𒴂��Ȃ��悤�ɑ��x�𐧌�
            if (velocity.magnitude > m_MaxSpeed)
            {
                m_Rigidbody.velocity = velocity.normalized * m_MaxSpeed;
            }

            // �T�[�o�[�ɑ��邽�߂̐V�����ʒu�Ɖ�]��ݒ�
            N_Position = this.transform.position;
            N_Rotation = this.transform.rotation;

        //�ړ��A�j���[�V��������
        //MoveAnimator(direction);

    }

    /// <summary>
    /// �T�[�o�[���ł̈ړ��A�j���[�V��������
    /// </summary>
    /// <param name="direction"></param>
    //public void MoveAnimator(Vector3 direction)
    //{
    //    //�ړ��A�j���[�V�������s
    //    //�T�[�o�[���󂯂��v���C���[����̈ړ��͂�0�ł͂Ȃ��B
    //    if (direction != Vector3.zero)
    //    {
    //        //�ړ��A�j���[�V�����X�s�[�h����(2�{�b)
    //        m_AnimeMoveSpeed += 2 * Time.deltaTime;
    //        if (m_AnimeMoveSpeed > 1) m_AnimeMoveSpeed = 1;
    //    }
    //    else
    //    {
    //        //�ړ��A�j���[�V�����X�s�[�h����(2�{�b)
    //        m_AnimeMoveSpeed -= 2 * Time.deltaTime;
    //        if (m_AnimeMoveSpeed <= 0) m_AnimeMoveSpeed = 0;
    //    }
    //    //�T�[�o�[���̃A�j���[�V������ύX
    //    m_Animator.SetFloat("Speed", m_AnimeMoveSpeed);
    //}
}