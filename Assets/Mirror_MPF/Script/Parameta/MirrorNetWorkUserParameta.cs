using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Mirror���g�p�\�Ƃ���
using Mirror;

/// <summary>
/// Client�Ń��[�U�[��񂪋L�ڂ���Ă������
/// �L�����N�^�[�Ȃǂł��ǂ��̂ŁA�o�����ɓ���Ă����B
/// �A��1�݂̂Ɍ��肷�鎖
/// </summary>

//NetworkIdentity��ǉ�
[RequireComponent(typeof(NetworkIdentity))]
public class MirrorNetWorkUserParameta : NetworkBehaviour
{
    [SyncVar, Header("�v���C���[��[AutoServerLink]")]
    public string m_PlayerName;
    [SyncVar, Header("�O���[�v��[AutoServerLink]")]
    public string m_GroupName;
}
