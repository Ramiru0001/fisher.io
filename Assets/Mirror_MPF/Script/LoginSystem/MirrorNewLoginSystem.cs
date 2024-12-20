using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class MirrorNewLoginSystem : MonoBehaviour
{
    #region �z��ꎮ
    [Header("�T�[�o�[��IP")]
    public string m_NetWorkIP = "127.0.0.1";

    [Header("�l�b�g���[�N�}�l�[�W���[�����N")]
    public NetworkManager m_NetworkManager;

    [SerializeField, Header("�`�F�b�N����T�[�o�[�|�[�g�̓������g�����X�|�[�g")]
    private kcp2k.KcpTransport m_KCP;

    [Header("���O�C���p�l��")]
    public GameObject m_LogInWindow;

    [Header("���O�A�E�g�p�l��")]
    public GameObject m_LogOutWindow;

    [Header("�N���C�A���g�p�A�T�[�o�[���N�����Ă��Ȃ��x���p�l��")]
    public GameObject m_NotServerWindow;

    [Header("�v���C���[�����̓t�B�[���h")]
    public InputField m_UserNameField;

    public enum MirrorSystemMode
    {
        ������,
        �N���C�A���g,
        �T�[�o�[,
    }
    [Header("[�������]�T�[�o�[�ɂ���ƁA�����ŃT�[�o�[�@�Ƃ��ċ@�\����")]
    public MirrorSystemMode m_MirrorSystemMode = MirrorSystemMode.������;

    [SerializeField, Header("�e�X�g�p�f�o�b�N���[�h")]
    private bool m_DebugMode = false;


    #endregion

    #region �����N������
    private void Awake()
    {

#if UNITY_EDITOR
        if (m_DebugMode)
            m_MirrorSystemMode = MirrorSystemMode.������;
#else
        if (m_DebugMode)
            m_MirrorSystemMode = MirrorSystemMode.�T�[�o�[;
#endif

        //�l�b�g���[�N�}�l�[�W���[���Ȃ��ꍇ�A�l�b�g���[�N�}�l�[�W���[����
        if (!m_NetworkManager)
            m_NetworkManager = this.GetComponent<NetworkManager>();

        //KCP���Ȃ��ꍇ�AKCP��������
        if (!m_KCP)
            m_KCP = GetComponent<kcp2k.KcpTransport>();

        // �T�[�o�[�̃A�h���X�ݒ�
        //NetworkManager.singleton.networkAddress = m_NetWorkIP;

        //��������T�[�o�[�N���w�肵�Ă���ꍇ�A�S�ẴE�B���h�D(�`���b�g�ȊO)��Off
        if (m_MirrorSystemMode == MirrorSystemMode.�T�[�o�[)
        {
            //�S�Ă͔�\��
            m_LogInWindow.gameObject.SetActive(false);
            m_LogOutWindow.gameObject.SetActive(false);
            m_NotServerWindow.gameObject.SetActive(false);
            //�T�[�o�[�ŋN��
            OnServerButton();
        }
        else
        {
            //�T�[�o�[�ł͂Ȃ��ꍇ�́A���O�C����ʂ�\����Ԃɂ���

            //���O�C���E�B���h�D��\���A����ȊO�͔�\��
            if (m_LogInWindow.gameObject.activeSelf == false)
                OnLogInWindows();
            if (m_LogOutWindow.gameObject.activeSelf == true)
                OnLogOutWindows();
            if (m_NotServerWindow.gameObject.activeSelf == true)
                OnNotServerWindows();

            //���������[�h�ɂ��đI���o����悤�ɂ���
            m_MirrorSystemMode = MirrorSystemMode.������;
        }
    }
#endregion

    #region ���O�C���p�l���\����
    #region  [�T�[�o�[�{�^���������ɌĂ΂��]DedicatedServer���g�p����ׁA�T�[�o�[��Auto�B�ȉ��̋@�\�̓I�~�b�g���܂��B
    public void OnServerButton()
    {
        //�T�[�o�[���N��
        m_NetworkManager.StartServer();
        //NetworkManager.singleton.StartServer();
        //���݂̃��[�h�̓T�[�o�[�ł���
        m_MirrorSystemMode = MirrorSystemMode.�T�[�o�[;
    }
    #endregion

    // �N���C�A���g�{�^���������ɌĂ΂��
    public void OnClientButton()
    {
        //�N���C�A���g���N�����ĂȂ��ꍇ�̂ݎ��s
        if (!NetworkClient.active)
        {
            //�N���C�A���g�́A�T�[�o�[���N�����Ă��邩�ǂ����m�F����܂ł́A�S�ăI�t
            if (m_LogInWindow.gameObject.activeSelf)
                OnLogInWindows();
            if (!m_LogOutWindow.gameObject.activeSelf)
                OnLogOutWindows();
            if (m_NotServerWindow.gameObject.activeSelf)
                OnNotServerWindows();


            //���݂̃��[�h
            m_MirrorSystemMode = MirrorSystemMode.�N���C�A���g;
            //�T�[�o�[����IP���w�肷��ꍇ�͂��������
            //m_NetworkManager.networkAddress = m_NetWorkIP;

            // �T�[�o�[�̃A�h���X�ݒ�
            m_NetworkManager.networkAddress = m_NetWorkIP;

            //�V�K�N���C�A���g���O�C��
            //m_NetworkManager.StartClient();
            NetworkManager.singleton.StartClient();
        }
    }

    // �T�[�o�[�����݂��Ă��Ȃ��x���{�^���������ɌĂ΂��
    public void OnNotServerButton()
    {
        //�N���C�A���g�́A�T�[�o�[���N�����Ă��邩�ǂ����m�F����܂ł́A�S�ăI�t
        if (!m_LogInWindow.gameObject.activeSelf)
            OnLogInWindows();
        if (m_LogOutWindow.gameObject.activeSelf)
            OnLogOutWindows();
        if (m_NotServerWindow.gameObject.activeSelf)
            OnNotServerWindows();

        //���݂̃��[�h
        m_MirrorSystemMode = MirrorSystemMode.������;
    }
    #endregion

    #region ���O�A�E�g����
    public void OnLogOutButton()
    {
        //�T�[�o�[�A�N���C�A���g�e�탂�[�h�ł̏����؂�ւ�
        switch (m_MirrorSystemMode)
        {
            //���g��Server�ł���ꍇ
            case MirrorSystemMode.�T�[�o�[:
                //�T�[�o�[���~����
                m_NetworkManager.StopServer();
                break;
            //���g��Client�ł���ꍇ
            case MirrorSystemMode.�N���C�A���g:
                //�N���C�A���g���~����
                m_NetworkManager.StopClient();
                break;
            //����ȊO�̏ꍇ�͊�{�G���[�ł���
            default:
                Debug.LogWarning("�G���[:�{���ł͎��s����Ȃ����������s���ꂽ!!");
                break;
        }

        //�T�[�o�[�����݂���̂ŁA���O�A�E�g��ʂ��ĕ\��
        if (!m_LogInWindow.gameObject.activeSelf)
            OnLogInWindows();
        if (m_LogOutWindow.gameObject.activeSelf)
            OnLogOutWindows();
        if (m_NotServerWindow.gameObject.activeSelf)
            OnNotServerWindows();
        //���O�A�E�g�����̂ŁA�����������ɐ؂�ւ��
        m_MirrorSystemMode = MirrorSystemMode.������;
    }
    #endregion

    #region ���O�C���E�A�E�g�E�B���h�D����
    /// <summary>
    /// ���O�C���E�B���h�D����(���]����)
    /// </summary>
    public void OnLogInWindows()
    {
        //���O�C���E�B���h�D�Ɋ���
        m_LogInWindow.gameObject.SetActive(!m_LogInWindow.gameObject.activeSelf);
    }
    /// <summary>
    /// ���O�A�E�g�E�B���h�D����(���]����)
    /// </summary>
    public void OnLogOutWindows()
    {
        //���O�A�E�g�E�B���h�D�Ɋ���
        m_LogOutWindow.gameObject.SetActive(!m_LogOutWindow.gameObject.activeSelf);
    }
    /// <summary>
    /// �T�[�o�[���N���x���E�B���h�D����(���]����)
    /// </summary>
    public void OnNotServerWindows()
    {
        //�x���E�B���h�D�Ɋ���
        m_NotServerWindow.gameObject.SetActive(!m_NotServerWindow.gameObject.activeSelf);
    }
    #endregion
}
