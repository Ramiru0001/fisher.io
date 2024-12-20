using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Mirror���g�p�\�Ƃ���
using Mirror;

/// <summary>
/// ��{�I�ɁA�T�[�o�[�����f�[�^���������A�N���C�A���g����̐\�����󂯂�`�ŏ��������s����
/// </summary>
public class MirrorParameta : NetworkBehaviour
{
    [Header("�L�����N�^�[�ő�Hp")]
    public int m_MaxHp = 100;

    [SyncVar,Header("[����]�L�����N�^�[Hp"),SerializeField]
    private int m_Hp;

    void Start()
    {
        //Hp���X�V
        m_Hp = m_MaxHp;
    }

    /// <summary>
    /// �T�[�o�[������
    /// </summary>
    /// <param name="amount">�_���[�W��</param>
    [Server]
    public void Damage(int amount)
    {
        //����Hp���s���Ă���ꍇ�́A�_���[�W����
        if (m_Hp <= 0) return;

        //�_���[�W�����Z
        m_Hp -= amount;
        //Hp��0�ȉ��̏ꍇ�AHp=0�Ƃ��A���S������s��
        if (m_Hp <= 0)
        {
            m_Hp = 0;
            Die();
        }
    }

    /// <summary>
    /// �T�[�o�[���͎��S���������s
    /// </summary>
    [Server]
    void Die()
    {
        //PRC�o�R�ŃN���C�A���g�Ɏ��S���������s
        RpcOnDeath();
    }

    /// <summary>
    /// �N���C�A���g���Ŏ��S�������s
    /// </summary>
    [ClientRpc]
    void RpcOnDeath()
    {
        // �N���C�A���g���̎��S����
        gameObject.SetActive(false);
    }
}
