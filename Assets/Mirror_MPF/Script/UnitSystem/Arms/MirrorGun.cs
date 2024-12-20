using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �N���C�A���g���̂ݎ���
/// �e�𔭎˂���v���O����
/// </summary>
public class MirrorGun : NetworkBehaviour
{
    [Header("�e")]
    public GameObject m_BulletPrefab;
    [Header("�e��")]
    public Transform m_Muzzle;

    void Update()
    {
        //�v���C���[���A�e���ˎ��s����
        if (isLocalPlayer && Input.GetButtonDown("Fire1"))
        {
            //�T�[�o�[���ɔ��C����
            CmdShoot();
        }
    }

    /// <summary>
    /// �T�[�o�[���̔��C����
    /// </summary>
    [Command]
    void CmdShoot()
    {
        //�T�[�o�[���̒e�o��
        GameObject bullet = Instantiate(m_BulletPrefab, m_Muzzle.position, m_Muzzle.rotation);
        //�T�[�o�[�o�R�ŃN���C�A���g�ɒe�o�����s
        //�v���n�u�� NetworkManager �� Registered Spawnable Prefabs ���X�g�ɓo�^����Ă���K�v������܂��B
        //����ɂ��A�T�[�o�[���I�u�W�F�N�g���X�|�[�������ۂɃN���C�A���g���ł��F������܂��B
        NetworkServer.Spawn(bullet);
    }
}
