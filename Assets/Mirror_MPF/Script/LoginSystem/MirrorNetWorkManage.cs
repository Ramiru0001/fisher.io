using Mirror;
using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace MirrorChatSystems
{

    /// NetworkManager���g�����āA�`���b�g�p�̃J�X�^���l�b�g���[�N�}�l�[�W���[����������
    /// <summary>
    /// Mirror�̃f�t�H���g�l�b�g���[�N�}�l�[�W���[���p�������V�X�e��
    /// �Ȍ�A�l�b�g���[�N�}�l�[�W���[�͂�������g�p���A�Ή�����
    /// </summary>
    [AddComponentMenu("")]

    public class MirrorNetWorkManage : NetworkManager
    {
        /*
        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            // �T�[�o�[���N���C�A���g�ڑ����󂯓���A�v���C���[�𐶐�����
            string playerName = conn.authenticationData as string;

            // �v���C���[�n�u�̐���
            GameObject player = Instantiate(playerPrefab);
            PlayerNetWorkSystem playerNetworkSystem = player.GetComponent<PlayerNetWorkSystem>();
            playerNetworkSystem.SetPlayerName(playerName); // �v���C���[����ݒ�

            // �v���C���[���Q�[���ɒǉ�
            NetworkServer.AddPlayerForConnection(conn, player);
        }
        */



        [Header("���O�C���V�X�e�������N")]
        public MirrorNewLoginSystem m_MirrorNewLoginSystem;

        // �N���C�A���g���瑗�M�����v���C���[���i�N���C�A���g�̐ڑ����ɐݒ肳���j
        public string m_PlayerName;


        #region �N���C�A���g���O�A�E�g�����ꎮ
        /// <summary>
        /// �N���C�A���g������A�T�[�o�[���ؒf���ꂽ�ꍇ�����Ŏ��s
        /// </summary>
        public override void OnStopClient()
        {
            //�N���C�A���g���~������
            base.OnStopClient();
            Debug.Log("�N���C�A���g���T�[�o�[����ؒf����܂����B");
            // �����Ń��O�A�E�g�������s��
            ClientLogout();
        }

        /// <summary>
        /// �N���C�A���g���O�A�E�g����
        /// </summary>
        void ClientLogout()
        {
            // ���O�A�E�g����������
            // ��: ���O�C����ʂɖ߂�A�Z�b�V���������N���A���铙
            Debug.Log("���O�A�E�g���������s���܂��B");

            //�E�B���h�D�����O�C���O��Ԃɖ߂��B
            if (!m_MirrorNewLoginSystem)
                m_MirrorNewLoginSystem = GetComponent<MirrorNewLoginSystem>();

            //UI�y���O�C���p�l���z���A�N�e�B�u�Ȃ�
            if (m_MirrorNewLoginSystem.m_LogInWindow.gameObject.activeSelf)
                m_MirrorNewLoginSystem.OnLogInWindows();

            //UI�y���O�A�E�g�p�l���z���A�N�e�B�u�Ȃ�
            if (m_MirrorNewLoginSystem.m_LogOutWindow.gameObject.activeSelf)
                m_MirrorNewLoginSystem.OnLogOutWindows();

            //UI�y�N���C�A���g�p�A�T�[�o�[���N�����Ă��Ȃ��x���p�l���z���A�N�e�B�u�Ȃ�
            if (!m_MirrorNewLoginSystem.m_NotServerWindow.gameObject.activeSelf)
                m_MirrorNewLoginSystem.OnNotServerWindows();

            //m_MPF_NewLoginSystem.OnLogOutButton();
        }
        #endregion

        // �T�[�o�[���Ńv���C���[���ǉ����ꂽ�ۂɌĂ΂�郁�\�b�h
        public override void OnServerAddPlayer(NetworkConnectionToClient NCTC)
        {
            // �f�t�H���g�̃v���C���[�n�u�𐶐�
            GameObject player = Instantiate(playerPrefab);

            // PlayerNetWorkSystem�R���|�[�l���g���擾
            PlayerNetWorkSystem playerNetSystem = player.GetComponent<PlayerNetWorkSystem>();

            // �v���C���[����ݒ�
            if (playerNetSystem != null)
            {
                // �ڑ����̔F�؃f�[�^����v���C���[����ݒ�
                if (NCTC.authenticationData != null)
                {
                    playerNetSystem.m_PlayerName = (string)NCTC.authenticationData;
                }
                else if (!string.IsNullOrEmpty(m_PlayerName))
                {
                    // ��ւƂ��āANetworkManager�ɐݒ肳�ꂽplayerName���g�p
                    playerNetSystem.m_PlayerName = m_PlayerName;
                }
            }

            // �v���C���[�I�u�W�F�N�g���Q�[���ɒǉ�
            NetworkServer.AddPlayerForConnection(NCTC, player);
        }
    }
}