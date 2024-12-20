using UnityEngine;
using Mirror;
using System.Linq; // �v���C���[���X�g����ɕ֗�

public class ServerChatSystemTester : MonoBehaviour
{
    // Singleton�C���X�^���X��ݒ�i���̃X�N���v�g����A�N�Z�X�\�j
    public static ServerChatSystemTester Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this; // Singleton�̏�����
        else
            Destroy(gameObject); // �d���C���X�^���X���폜
    }

    // �N���C�A���g���瑗�M���ꂽ���b�Z�[�W����������
    [Server]
    public void HandleMessage(
        ClientChatSystemTester sender,
        string message,
        string targetName,
        string targetGroup,
        int targetIndex)
    {
        if (targetName != null)
        {
            // ����̃v���C���[�i���O�w��j
            foreach (var conn in NetworkServer.connections.Values)
            {
                var player = conn.identity.GetComponent<ClientChatSystemTester>();
                if (player.m_PlayerName == targetName)
                {
                    player.RpcDisplayMessage($"�N���C�A���g:{message}"); // ���b�Z�[�W���M
                }
            }
        }
        else if (targetGroup != null)
        {
            // ����̃O���[�v�i�O���[�v���w��j
            foreach (var conn in NetworkServer.connections.Values)
            {
                var player = conn.identity.GetComponent<ClientChatSystemTester>();
                if (player.m_GroupName == targetGroup)
                {
                    player.RpcDisplayMessage($"{message}"); // ���b�Z�[�W���M
                }
            }
        }
        else if (targetIndex >= 0)
        {
            // �w�肳�ꂽ�C���f�b�N�X�̃v���C���[
            var playerList = NetworkServer.connections.Values.Select(conn => conn.identity.GetComponent<ClientChatSystemTester>()).ToList();
            if (targetIndex < playerList.Count)
            {
                var targetPlayer = playerList[targetIndex];
                targetPlayer.RpcDisplayMessage($"{message}"); // ���b�Z�[�W���M
            }
        }
        else
        {
            // �S�v���C���[�ɑ��M
            foreach (var conn in NetworkServer.connections.Values)
            {
                var player = conn.identity.GetComponent<ClientChatSystemTester>();
                player.RpcDisplayMessage($"{message}"); // ���b�Z�[�W���M
            }
        }
    }
}
