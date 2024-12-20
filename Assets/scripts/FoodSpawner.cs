using UnityEngine;
using Mirror;

public class FoodSpawner : NetworkBehaviour
{
    public GameObject foodPrefab; // �a��Prefab
    public int maxFoodCount = 50; // �V�[�����ɑ��݂���a�̍ő吔
    public float spawnInterval = 2f; // �a���[����Ԋu�i�b�j

    [SerializeField]
    private Transform parentObject; // �e�I�u�W�F�N�g���w�肷�邽�߂�Transform
    [SerializeField]
    private Transform floor;  // ����Cube
    [SerializeField]
    private Transform ceiling;  // �V���Cube
    [SerializeField]
    private Transform leftWall;  // �����̕�
    [SerializeField]
    private Transform rightWall; // �E���̕�
    [SerializeField]
    private Transform frontWall; // �O���̕�
    [SerializeField]
    private Transform backWall;  // �㑤�̕�

    private int currentFoodCount = 0; // ���݂̉a�̐�

    public override void OnStartServer()
    {
        Debug.Log("Server started, invoking repeating food spawn...");
        // ��������
        for (int i = 0; i < maxFoodCount; i++)
        {
            SpawnFood();
        }

        // ���Ԋu�ŉa���[���鏈�����J�n
        InvokeRepeating(nameof(CheckAndSpawnFood), spawnInterval, spawnInterval);
    }

    [Server]
    private void CheckAndSpawnFood()
    {
        // �K�v�ȉa�̐����v�Z
        int foodToSpawn = maxFoodCount - currentFoodCount;

        for (int i = 0; i < foodToSpawn; i++)
        {
            SpawnFood();
        }
    }

    [Server]
    private void SpawnFood()
    {
        // 6��Cube�͈͓̔��Ń����_���Ȉʒu�𐶐�
        Vector3 spawnPosition = GetRandomPositionWithinCubes();

        //Debug.Log($"Food spawned at {spawnPosition}");

        GameObject food = Instantiate(foodPrefab, spawnPosition, Quaternion.identity);
        // �e�I�u�W�F�N�g���w�肳��Ă���΁A���������a�����̎q�Ƃ��Đݒ�
        if (parentObject != null && parentObject.TryGetComponent<NetworkIdentity>(out NetworkIdentity parentIdentity))
        {
            /// �e�̃l�b�g���[�NID��ݒ�
            food.GetComponent<Food>().parentNetId = parentIdentity.netId;
            //Debug.Log($"Assigned parentNetId: {parentIdentity.netId}");
        }
        else
        {
            //Debug.LogWarning("Parent object is not set!");
        }
        
        NetworkServer.Spawn(food); // �N���C�A���g�ɃI�u�W�F�N�g�𓯊�
        currentFoodCount++;

        // �a���j�󂳂ꂽ�Ƃ��ɃJ�E���g�����炷�R�[���o�b�N��ݒ�
        food.GetComponent<Food>().OnFoodDestroyed += () => currentFoodCount--;
    }
    private Vector3 GetRandomPositionWithinCubes()
    {
        // �e�ǂ̃��[���h�X�P�[�����擾�ilossyScale�j
        Vector3 floorScale = floor.lossyScale;
        Vector3 ceilingScale = ceiling.lossyScale;
        Vector3 leftWallScale = leftWall.lossyScale;
        Vector3 rightWallScale = rightWall.lossyScale;
        Vector3 frontWallScale = frontWall.lossyScale;
        Vector3 backWallScale = backWall.lossyScale;

        // ���E�V���Y���͈�
        float minY = floor.position.y + (floorScale.y / 2);
        float maxY = ceiling.position.y - (ceilingScale.y / 2);

        // ���E��X���͈�
        float minX = leftWall.position.x + (leftWallScale.x / 2);
        float maxX = rightWall.position.x - (rightWallScale.x / 2);

        // �O���Z���͈�
        float minZ = backWall.position.z + (backWallScale.z / 2);
        float maxZ = frontWall.position.z - (frontWallScale.z / 2);

        // Cube�����ɏo�����Ȃ��悤�ɁA���������Ƀ}�[�W����݂���
        float margin = 0.1f;  // �K�v�ɉ����Ē���
        minX += margin; maxX -= margin;
        minY += margin; maxY -= margin;
        minZ += margin; maxZ -= margin;

        // �����_���Ȉʒu���v�Z
        return new Vector3(
            Random.Range(minX, maxX),
            Random.Range(minY, maxY),
            Random.Range(minZ, maxZ)
        );
    }
}
