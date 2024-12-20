using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

public class AssetBundleManager : MonoBehaviour
{
    // AssetBundle�̃o�[�W�����Ǘ��Ɏg�p����ϐ�
    private string m_AssetBundleURL = "https://github.com/user/repository/assetbundle";
    private string m_VersionURL = "https://github.com/user/repository/version.txt"; // �o�[�W�����Ǘ��t�@�C����URL
    private int m_LocalVersion = 1;  // ���[�J���ɕێ�����o�[�W����

    // AssetBundle�Ƃ��̈ˑ��֌W�̊Ǘ��p
    private AssetBundle m_AssetBundle;

    // ������AssetBundle�̃o�[�W��������ێ�����
    private Dictionary<string, int> m_AssetBundleVersions = new Dictionary<string, int>();

    // �o�[�W���������_�E�����[�h���ăo�[�W�����`�F�b�N���s��
    IEnumerator Start()
    {
        // GitHub��̃o�[�W�����t�@�C�����_�E�����[�h
        UnityWebRequest versionRequest = UnityWebRequest.Get(m_VersionURL);
        yield return versionRequest.SendWebRequest();

        if (versionRequest.result == UnityWebRequest.Result.Success)
        {
            // �T�[�o�[����擾�����ŐV�o�[�W����
            int latestVersion = int.Parse(versionRequest.downloadHandler.text);

            // �o�[�W�����`�F�b�N
            if (latestVersion > m_LocalVersion)
            {
                // �V�����o�[�W����������ꍇ�̓_�E�����[�h���J�n
                Debug.Log("�V�����o�[�W������������܂����BAssetBundle���_�E�����[�h���܂��B");
                StartCoroutine(DownloadAssetBundle(latestVersion));
            }
            else
            {
                // ���[�J���o�[�W�������ŐV
                Debug.Log("AssetBundle�͍ŐV�ł��B");
            }
        }
        else
        {
            Debug.LogError("�o�[�W�������̎擾�Ɏ��s���܂���: " + versionRequest.error);
        }
    }

    // AssetBundle�̃_�E�����[�h����
    IEnumerator DownloadAssetBundle(int latestVersion)
    {
        // AssetBundle��URL�Ƀo�[�W�����p�����[�^��ǉ����ă_�E�����[�h
        string bundleURL = m_AssetBundleURL + "?v=" + latestVersion;
        UnityWebRequest assetBundleRequest = UnityWebRequestAssetBundle.GetAssetBundle(bundleURL);
        yield return assetBundleRequest.SendWebRequest();

        if (assetBundleRequest.result == UnityWebRequest.Result.Success)
        {
            // AssetBundle�̓ǂݍ���
            m_AssetBundle = DownloadHandlerAssetBundle.GetContent(assetBundleRequest);
            m_LocalVersion = latestVersion;  // ���[�J���̃o�[�W���������X�V
            Debug.Log("AssetBundle�̃_�E�����[�h�ɐ������܂����B");
        }
        else
        {
            Debug.LogError("AssetBundle�̃_�E�����[�h�Ɏ��s���܂���: " + assetBundleRequest.error);
        }
    }
}
