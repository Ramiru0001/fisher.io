using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �_���[�W��^����I�u�W�F�N�g���͓̂��������������܂��B
/// �����������s�����ꍇ�A�]���f�[�^���c��ɂȂ�ׂł��B
/// ���̃R���|�[�l���g���R���W�����ɐڐG�����A
/// MirrorParameta���ڐG����̏ꍇ�̂݁A�_���[�W��^���܂��B
/// </summary>
public class MirrorDamageObject : MonoBehaviour
{
    [Header("�_���[�W�l")]
    public int m_Damage;
    [Header("���G���[�h")]
    public bool m_Invincibility;
    [Header("���ł���܂ł̎���")]
    public float m_DestroyTime = 1.0f;
    private void Start()
    {
        Destroy(gameObject, m_DestroyTime);
    }
}
