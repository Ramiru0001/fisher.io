using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MirrorChatSystems
{
    /// <summary>
    /// Mirror�̃v���C���[�I�u�W�F�N�g���ɃZ�b�g�����
    /// �`���b�g�Ȃǂ̃v���C���[���Ȃǂ��m�ۂ��A���L������V�X�e��
    /// </summary>
    public class FishNetWorkSystem : NetworkBehaviour
    {

        [SyncVar(hook = nameof(OnNameChanged)), Header("�v���C���[��")]
        public string m_PlayerName;


        // �v���C���[����ݒ肷�郁�\�b�h�i�T�[�o�[���ł̂݌Ă΂��j
        [Server]
        public void SetPlayerName(string name)
        {
            m_PlayerName = name;
        }

        // �v���C���[�����ύX���ꂽ�Ƃ��̃N���C�A���g���ł̏���
        private void OnNameChanged(string oldName, string newName)
        {
            Debug.Log($"�v���C���[�����ύX����܂���: {newName}");
            // �����Ŗ��O�̕ύX�Ɋ֘A����UI��Q�[�����\�����X�V
        }

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();
            Debug.Log($"���[�J���v���C���[���J�n����܂���: {m_PlayerName}");
        }
        /*
        /// <summary>
        /// Server���N����
        /// </summary>
        public override void OnStartServer()
        {
            m_PlayerName = (string)connectionToClient.authenticationData;
        }

        /// <summary>
        /// ���[�J���v���C���[���N��
        /// </summary>
        public override void OnStartLocalPlayer()
        {
            //�v���C���[������
            ChatUISystem.m_LocalPlayerName = m_PlayerName;
        }
        */
    }
}
