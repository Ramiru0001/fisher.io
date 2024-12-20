using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target; // �J�������Ǐ]����^�[�Q�b�g�i��: �v���C���[�j
    public float distance = 10f; // ��������
    public float zoomSpeed = 2f; // �Y�[�����x
    public float minDistance = 5f; // �ŏ�����
    public float maxDistance = 20f; // �ő勗��
    public Vector3 offset = Vector3.zero; // �^�[�Q�b�g����̃I�t�Z�b�g

    void LateUpdate()
    {
        if (target == null) return;

        // �}�E�X�z�C�[�����͂��擾
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        // �����𒲐�
        distance -= scrollInput * zoomSpeed;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);

        // �J�����̈ʒu���X�V
        Vector3 direction = (transform.position - target.position).normalized; // �^�[�Q�b�g����J�����ւ̕���
        transform.position = target.position + direction * distance + offset;

        // �^�[�Q�b�g������
        transform.LookAt(target);
    }
}
