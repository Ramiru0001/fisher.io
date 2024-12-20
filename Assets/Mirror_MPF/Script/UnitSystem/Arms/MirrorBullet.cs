using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(NetworkIdentity))]             //NetworkIdentity��ǉ�
public class MirrorBullet : NetworkBehaviour
{
    [Header("�e�̈ړ����x")]
    public float m_Speed = 20f;
    [Header("�e�̃_���[�W�l")]
    public int m_Damage = 10;
    [Header("����"),SerializeField]
    private Rigidbody m_Rigidbody;
    [Header("�j������܂ł̎���")]
    public float m_DestroyTime = 1.0f;

    void Start()
    {
        // �e�ۂ���莞�Ԍ�ɔj�󂷂�R���[�`�����J�n
        StartCoroutine(DestroyAfterTime(m_DestroyTime));

        //�����l��
        m_Rigidbody = GetComponent<Rigidbody>();

        //�e�̌����Ă�������ֈړ�����
        m_Rigidbody.velocity = transform.forward * m_Speed;
    }

    /// <summary>
    /// �ڐG�����ꍇ
    /// </summary>
    /// <param name="other">���������Ώ�</param>
    void OnTriggerEnter(Collider other)
    {
        //�T�[�o�[���ł���ꍇ
        if (isServer)
        {
            //�p�����[�^�[������ꍇ���
            MirrorParameta target = other.GetComponent<MirrorParameta>();
            //�p�����[�^�[������
            if (target != null)
            {
                //�_���[�W��^����
                target.Damage(m_Damage);
            }
            //�e�͏���
            DestroyBullet();
        }
    }
    /// <summary>
    /// �T�[�o�[���Œe��j�����A�����ŃN���C�A���g�����j������
    /// </summary>
    [Server]
    void DestroyBullet()
    {
        // �T�[�o�[���ŃI�u�W�F�N�g��j��
        NetworkServer.Destroy(gameObject);
    }
    /// <summary>
    /// �w�肵�����ԂɒB������A�T�[�o�[�o�R�Œe��j�󂵁A�N���C�A���g�����j�󂷂�
    /// </summary>
    /// <param name="time">�w�莞��(�b)</param>
    /// <returns></returns>
    IEnumerator DestroyAfterTime(float time)
    {
        //��莞�Ԃ܂őҋ@
        yield return new WaitForSeconds(time);
        //�T�[�o�[�ł���΁A�I�u�W�F�N�g�j������s
        if (isServer)
            DestroyBullet();
    }
}
