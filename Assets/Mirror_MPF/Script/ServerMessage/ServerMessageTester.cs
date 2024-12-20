using UnityEngine;
using Mirror;
using System.Linq;
using System.Collections.Generic; // �v���C���[���X�g����ɕ֗�

/// <summary>
/// Server������̃��b�Z�[�W���M
/// PlayerInMessageTester��RpcDisplayMessage�ƘA������
/// </summary>
public class ServerMessageTester : NetworkBehaviour
{

    public void SendAllPlayersName()
    {
        Debug.Log("�S�v���C���[���M");
        string PlayerNames = "";
        foreach (var conn in NetworkServer.connections.Values)
        {
            // �v���C���[���擾
            PlayerInMessageTester player = conn.identity.GetComponent<PlayerInMessageTester>();
            PlayerNames += player.m_PlayerName + "\n";
            // �S�v���C���[�Ƀ��b�Z�[�W���M
            player.RpcDisplayMessage(player.m_PlayerName);
        }
        foreach (var conn in NetworkServer.connections.Values)
        {
            // �v���C���[���擾
            PlayerInMessageTester player = conn.identity.GetComponent<PlayerInMessageTester>();
            // �S�v���C���[�Ƀ��b�Z�[�W���M
            player.RpcDisplayMessage(player.m_PlayerName);
        }
    }


    /// <summary>
    /// �S�v���C���[�Ƀ��b�Z�[�W�𑗐M����
    /// </summary>
    /// <param name="message">���M���e</param>
    [Server] // �T�[�o�[���݂̂Ŏ��s�����
    public void SendMessageToAll(string message)
    {
        Debug.Log("�S�v���C���[���M");
        foreach (var conn in NetworkServer.connections.Values)
        {
            // �v���C���[���擾
            PlayerInMessageTester player = conn.identity.GetComponent<PlayerInMessageTester>();
            // �S�v���C���[�Ƀ��b�Z�[�W���M
            player.RpcDisplayMessage(message);
        }
    }

    /// <summary>
    /// ����̃v���C���[�ɑ��M����
    /// </summary>
    /// <param name="message">���M���e</param>
    /// <param name="targetName">���M�Ώۂ̃v���C���[��</param>
    [Server]
    public void SendMessageToSpecificPlayer(string message, string targetName)
    {
        Debug.Log("����v���C���[���M");
        //�S�Ă̐ڑ��Ώێ҂𑖍�
        foreach (var conn in NetworkServer.connections.Values)
        {
            //���葤�̃f�[�^��M�R���|�[�l���g���擾
            PlayerInMessageTester player = conn.identity.GetComponent<PlayerInMessageTester>();
            //���葤�̃v���C���[���Ɣ�r
            if (player.m_PlayerName == targetName)
            {
                // �w��v���C���[�̓��b�Z�[�W���M
                player.RpcDisplayMessage(message);
            }
        }
    }

    /// <summary>
    /// ����̃O���[�v�ɑ��M����
    /// </summary>
    /// <param name="message">���M���e</param>
    /// <param name="targetGroup">���M�Ώۂ̃O���[�v��</param>
    [Server]
    public void SendMessageToGroup(string message, string targetGroup)
    {
        Debug.Log("����O���[�v���M");
        //�S�Ă̐ڑ��Ώێ҂𑖍�
        foreach (var conn in NetworkServer.connections.Values)
        {
            //���葤�̃f�[�^��M�R���|�[�l���g���擾
            PlayerInMessageTester player = conn.identity.GetComponent<PlayerInMessageTester>();
            //���葤�̃O���[�v���Ɣ�r
            if (player.m_GroupName == targetGroup)
            {
                // �w��O���[�v�ɏ������Ă���v���C���[�̓��b�Z�[�W���M
                player.RpcDisplayMessage(message);
            }
        }
    }

    /// <summary>
    /// �ڑ��ԍ�1�Ԃ̃v���C���[���M(�e�X�g)
    /// </summary>
    /// <param name="message">���M���e</param>
    [Server]
    public void SendMessageToFirstPlayer(string message)
    {
        Debug.Log("1�ԃv���C���[���M");
        //���݂̐ڑ��҂��P�l�ł�����
        if (NetworkServer.connections.Count > 0)
        {
            //�ڑ����ꂽ�ŏ��̃v���C���[���擾
            NetworkConnectionToClient firstConn = NetworkServer.connections.Values.First();
            //���葤�̃f�[�^��M�R���|�[�l���g���擾
            PlayerInMessageTester player = firstConn.identity.GetComponent<PlayerInMessageTester>();
            //����Ƀ��b�Z�[�W���M
            player.RpcDisplayMessage(message);
        }
    }

    private void Update()
    {
        //���g�̃T�[�o�[�ł���
        if (isServer)
        {
            //�e��Button�ŁAServer�����瑗�M
            //1�L�[��������
            if (Input.GetKeyDown(KeyCode.Alpha1))
                SendMessageToAll("�S����Message�𑗂�܂��B");

            //2�L�[��������
            if (Input.GetKeyDown(KeyCode.Alpha2))
                SendMessageToSpecificPlayer("�u����v�̃v���C���[�̂ݑ���܂��B", "����");

            //3�L�[��������
            if (Input.GetKeyDown(KeyCode.Alpha3))
                SendMessageToGroup("�uWolfTeam�v�ɏ������Ă���l��������܂��B", "WolfTeam");

            //4�L�[��������
            if (Input.GetKeyDown(KeyCode.Alpha4))
                SendMessageToFirstPlayer("��ԍŏ��Ƀ��O�C�������l�ɑ���܂��B");
        }
    }
}
