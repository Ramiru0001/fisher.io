using UnityEngine;
using UnityEngine.UI; // UI����p
using Mirror; // Mirror�̋@�\�𗘗p

/// <summary>
/// Server����Client���ւ̐�p���b�Z�[�W�̎�M
/// �����܂ŁAServer����̎w�����߂̎�M�p�Ƃ���
/// �K���Ӄv���C���[Prefab(�������́AMirror�ɂ�鐶��?)�ɓ������鎖������
/// </summary>
public class PlayerInMessageTester : NetworkBehaviour
{
    [Header("�v���C���[��UI��Text")]
    public Text m_PlayerMessageText;

    [Header("�v���C���[��[��������]"),SyncVar]
    public string m_PlayerName;

    [Header("�O���[�v��[��������]"), SyncVar]
    public string m_GroupName;

    /// <summary>
    /// ���g���v���C���[�ł���ꍇ�A�N������Message�e�L�X�g�ƘA������
    /// </summary>
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        //�g�p����Text��T���o��
        GameObject D = GameObject.Find("�V�X�e�����b�Z�[�W");
        //Text�ƃ����N����
        m_PlayerMessageText = D.GetComponent<Text>();
        Debug.Log($"�V�X�e�����b�Z�[�W�e�L�X�g�ƘA�����܂���: {m_PlayerName}");
    }

    /// <summary>
    /// Server����̃��b�Z�[�W��M
    /// Server����w�肳���
    /// </summary>
    /// <param name="message">��M���e</param>
    [ClientRpc]
    public void RpcDisplayMessage(string message)
    {
        Debug.Log("��M");
        if (m_PlayerMessageText != null)
        {
            // �w�肳�ꂽ���b�Z�[�W��UI�ɕ\��
            m_PlayerMessageText.text += message + "\n";
        }
    }
}
