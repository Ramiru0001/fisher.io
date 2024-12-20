using UnityEngine;
using Mirror;
using System.Collections;

public class Food : NetworkBehaviour
{
    public int experiencePoints = 10; // ���̉a���v���C���[�ɗ^����o���l
    // �a���j�󂳂ꂽ�Ƃ��ɌĂ΂��C�x���g
    public event System.Action OnFoodDestroyed;
    [SyncVar]
    public uint parentNetId; // �T�[�o�[����n���ꂽ�e�̃l�b�g���[�NID
    public override void OnStartClient()
    {
        base.OnStartClient();

        // �N���C�A���g���Őe���������čĐݒ�
        StartCoroutine(WaitForParentAndSet());
    }
    private IEnumerator WaitForParentAndSet()
    {
        NetworkIdentity parentIdentity = null;

        // �e�I�u�W�F�N�g���X�|�[�������܂őҋ@
        while ((parentIdentity = GetParentNetworkIdentity()) == null)
        {
            //Debug.Log($"Waiting for parentNetId: {parentNetId}");
            yield return null; // ���̃t���[���܂őҋ@
        }

        // �e�q�֌W��ݒ�
        transform.SetParent(parentIdentity.transform);
        //Debug.Log($"Parent set to {parentIdentity.name} on client");
    }
    private NetworkIdentity GetParentNetworkIdentity()
    {
        // NetworkClient���g���Đe��NetworkIdentity������
        foreach (var identity in NetworkClient.spawned.Values)
        {
            if (identity.netId == parentNetId)
            {
                return identity;
            }
        }
        return null;
    }
    [ServerCallback]
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // �v���C���[�Ɍo���l��t�^
            other.GetComponent<PlayerStats>()?.AddExperience(experiencePoints);

            // �a��j��
            OnFoodDestroyed?.Invoke();
            NetworkServer.Destroy(gameObject);
        }
    }
}
