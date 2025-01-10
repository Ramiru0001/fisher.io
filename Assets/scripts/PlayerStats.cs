using UnityEngine;
using Mirror;

public class PlayerStats : NetworkBehaviour
{
    [SyncVar] public int experience = 0; // �o���l�i���������j
    public float sizeIncreasePerExperience = 0.01f; // �o���l1�ɂ�������T�C�Y

    [Server]
    public void AddExperience(int amount)
    {
        experience += amount;

        // �v���C���[�̃X�P�[����ύX
        float newScale = 1 + experience * sizeIncreasePerExperience;
        //Debug.Log("sizeIncreasePerExperience:" + sizeIncreasePerExperience);
        transform.localScale = Vector3.one * newScale;

        // �N���C�A���g�ɂ��T�C�Y�ύX��ʒm
        RpcUpdateScale(transform.localScale);
    }
    [Server]
    public void ResetExperience()
    {
        experience = 0; // �o���l�����Z�b�g
    }

    [ClientRpc]
    void RpcUpdateScale(Vector3 newScale)
    {
        transform.localScale = newScale; // �N���C�A���g���ŃX�P�[�����X�V
    }
}
