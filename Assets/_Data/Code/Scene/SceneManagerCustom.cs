using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneManagerCustom : MonoBehaviour
{
    public void RunGame()
    {
        StartCoroutine(LoadSceneSmooth("SampleScene"));
    }

    private IEnumerator LoadSceneSmooth(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false; // chưa kích hoạt ngay

        // Chờ load xong
        while (!asyncLoad.isDone)
        {
            // Kiểm tra nếu load >= 90% (Unity giữ lại 10% để chờ kích hoạt)
            if (asyncLoad.progress >= 0.9f)
            {
                // Cho phép chuyển cảnh
                asyncLoad.allowSceneActivation = true;
            }
            yield return null;
        }
    }
}
