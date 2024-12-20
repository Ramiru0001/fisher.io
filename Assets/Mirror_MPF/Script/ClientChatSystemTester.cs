using UnityEngine;
using UnityEngine.UI; // UI����p
using Mirror; // Mirror�̋@�\�𗘗p

public class ClientChatSystemTester : NetworkBehaviour
{
    [Header("�`���b�g���b�Z�[�WText�ƘA��")]
    public Text m_PlayerMessageText;

    [Header("�v���C���[��[��������]"),SyncVar]
    public string m_PlayerName;

    [Header("�O���[�v����[��������]"), SyncVar]
    public string m_GroupName;

    /// <summary>
    /// ���g���v���C���[�ł���ꍇ�A�N������Message�e�L�X�g�ƘA������
    /// </summary>
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        //�g�p����Text��T���o��
        GameObject D = GameObject.Find("�`���b�g�E�B���h�D");
        //Text�ƃ����N����
        m_PlayerMessageText = D.GetComponent<Text>();
        Debug.Log($"�`���b�g�e�L�X�g�ƘA�����܂���: {m_PlayerName}");
    }

    /// <summary>
    /// ��M���b�Z�[�W��\��
    /// Server���瑗�M���ꂽ�l�`���b�gMessage���󂯎��
    /// </summary>
    /// <param name="message">��M���e</param>
    [ClientRpc]
    public void RpcDisplayMessage(string message)
    {
        //�w�肳�ꂽ���b�Z�[�W��UI�ɕ\��
        if (m_PlayerMessageText != null)
            m_PlayerMessageText.text += message + "\n";
    }

    /// <summary>
    /// �N���C�A���g���瑼�̃N���C�A���g�փ��b�Z�[�W�𑗐M
    /// �N���C�A���g���T�[�o�[���N���C�A���g�o�R�փ��b�Z�[�W�������
    /// </summary>
    /// <param name="message">���M���e</param>
    /// <param name="targetName">���M���������薼[�Ȃ��ꍇ��null]</param>
    /// <param name="targetGroup">���M�������O���[�v��[�Ȃ��ꍇ��null]</param>
    /// <param name="targetIndex">���M����������ԍ�[�Ȃ��ꍇ��-1]</param>
    [Command] // �N���C�A���g����T�[�o�[�ւ̃��N�G�X�g
    public void CmdSendMessage(
        string message,
        string targetName,
        string targetGroup,
        int targetIndex)
    {
        // �T�[�o�[�Ń��b�Z�[�W�������iServerChatSystemTester�ɏ������ϑ��j
        ServerChatSystemTester.Instance.HandleMessage(
            this, message,
            targetName,
            targetGroup,
            targetIndex);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            // �S�v���C���[�Ɂu�҂������ȁA�݂�Ȃ�!!�v�𑗐M
            CmdSendMessage(m_PlayerName + ": �҂������ȁA�݂�Ȃ�!!",null,null,-1);
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            // �v���C���[���u����v�Ɂu��͔C���āA��ɍs��! ���₟��!!�v�𑗐M
            CmdSendMessage(m_PlayerName + ": ��͔C���āA��ɍs��! ���₟��!!", "����", null, -1);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            // �O���[�v�uWolfTeam�v�Ɂu�������c�~�܂�񂶂�˂����c�B�v�𑗐M
            CmdSendMessage(m_PlayerName + ": �������c�~�܂�񂶂�˂����c�B", null, "WolfTeam", -1);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            // 1�Ԗڂ̃v���C���[�Ɂu���O���c�i���o�[������!�v�𑗐M
            CmdSendMessage(m_PlayerName + ": ���O���c�i���o�[������!", null, null, 0);
        }
    }
}