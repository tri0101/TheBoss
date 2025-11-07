using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class LoadingScene : MonoBehaviour
{
    public GameObject loadingScreen;
    public static LoadingScene instance;
    public ProgressBar bar;

    public AssetReference environment;
    [SerializeField] private Transform mainCamera;
    private AsyncOperationHandle<GameObject> environmentHandle;
    private GameObject environmentInstance;
    [SerializeField] private GameObject eventSystem;
    [SerializeField] private Transform teran;

    private void Awake()
    {
        instance = this;
        SceneManager.LoadSceneAsync("TitleScreen", LoadSceneMode.Additive);
    }

    List<AsyncOperation> scenesLoading = new List<AsyncOperation>();
    float totalSceneProgress;

    //public void LoadGame()
    //{
    //    scenesLoading.Add(SceneManager.UnloadSceneAsync("TitleScreen"));
    //    if (environmentInstance != null)
    //    {
    //        Addressables.ReleaseInstance(environmentInstance);
    //        environmentInstance = null;
    //    }

    //    // Bật loading screen
    //    loadingScreen.SetActive(true);
    //    eventSystem.gameObject.SetActive(false);
    //    // 👉 Đầu tiên chạy thanh progress từ 1 -> 100
    //    StartCoroutine(FakeProgressAndLoadScene());
    //}
    public void LoadGame()
    {
        StartCoroutine(LoadGameCoroutine());
    }

    private IEnumerator LoadGameCoroutine()
    {
        mainCamera.GetComponent<AudioListener>().enabled = false;
        // Unload TitleScreen
        AsyncOperation unloadTitle = SceneManager.UnloadSceneAsync("TitleScreen");
        if (unloadTitle != null)
        {
            while (!unloadTitle.isDone)
                yield return null;
        }

        // Giải phóng environment
        if (environmentInstance != null)
        {
            Addressables.ReleaseInstance(environmentInstance);
            environmentInstance = null;
        }

        // Bật loading
        loadingScreen.SetActive(true);
        eventSystem.gameObject.SetActive(false);
        bar.current = 0;

        // Chạy progress giả
        yield return StartCoroutine(FakeProgressAndLoadScene());
    }
    //private IEnumerator FakeProgressAndLoadScene()
    //{
    //    bar.current = 1;

    //    // 🟢 Giả lập progress 1 → 90
    //    while (bar.current < 90)
    //    {
    //        bar.current += 1;
    //        yield return new WaitForSeconds(0.02f);
    //    }

    //    // 🟡 Bắt đầu load thật
    //    AsyncOperation async = SceneManager.LoadSceneAsync("SampleScene", LoadSceneMode.Additive);
    //    async.allowSceneActivation = false;
    //    scenesLoading.Add(async);

    //    // ⏳ Chờ scene load nội dung đến 90%
    //    while (async.progress < 0.9f)
    //        yield return null;

    //    // 🟣 Khi load xong, spawn environment trước
    //    DestroyOldEnvironment();
    //    yield return StartCoroutine(SpawnEnvironmentCoroutine());

    //    // 🟢 Giả lập progress 90 → 100
    //    while (bar.current < 100)
    //    {
    //        bar.current += 1;
    //        yield return new WaitForSeconds(0.02f);
    //    }

    //    // 🕐 Cho CPU/GPU nghỉ nhẹ
    //    yield return new WaitForSeconds(0.5f);

    //    // ✅ Cho phép scene hiển thị
    //    async.allowSceneActivation = true;

    //    // Đợi scene hoàn tất load (thực sự loaded)
    //    while (!async.isDone)
    //        yield return null;

    //    // 🔵 Bây giờ mới được phép SetActiveScene
    //    Scene sampleScene = SceneManager.GetSceneByName("SampleScene");
    //    if (sampleScene.IsValid() && sampleScene.isLoaded)
    //    {
    //        SceneManager.SetActiveScene(sampleScene);
    //    }
    //    else
    //    {
    //        Debug.LogError("Scene SampleScene chưa được load đầy đủ!");
    //    }

    //    // 🔚 Tắt màn hình loading
    //    loadingScreen.SetActive(false);
    //}
    private IEnumerator FakeProgressAndLoadScene()
    {
        bar.current = 1;
        loadingScreen.SetActive(true);
        eventSystem.SetActive(false);

        // 🟢 Bắt đầu load SampleScene (chưa active)
        AsyncOperation asyncScene = SceneManager.LoadSceneAsync("SampleScene", LoadSceneMode.Additive);
        asyncScene.allowSceneActivation = false;

        // 🟣 Tăng progress giả mượt
        while (bar.current < 80)
        {
            bar.current += 2;
            yield return new WaitForSeconds(0.01f);
        }

        // 🔸 Chờ scene load xong đến 90%
        yield return new WaitUntil(() => asyncScene.progress >= 0.9f);

        // 🟢 Cho phép scene hiển thị
        asyncScene.allowSceneActivation = true;
        yield return new WaitUntil(() => asyncScene.isDone);

        // 🔹 Set SampleScene làm scene active
        Scene sampleScene = SceneManager.GetSceneByName("SampleScene");
        SceneManager.SetActiveScene(sampleScene);

        // 🧹 Dọn environment cũ
        DestroyOldEnvironment();

        // 🟢 Spawn environment sau khi SampleScene đã active
        yield return StartCoroutine(SpawnEnvironmentCoroutine());

        // 🔹 Giả lập progress 80 → 100
        while (bar.current < 100)
        {
            bar.current += 2;
            yield return new WaitForSeconds(0.005f);
        }

        // 🔚 Tắt loading UI, bật scene chính
        mainCamera.gameObject.SetActive(false);
        loadingScreen.SetActive(false);
    }
    //private IEnumerator SpawnEnvironmentCoroutine()
    //{


    //    var handle = environment.InstantiateAsync(Vector3.zero, Quaternion.identity);
    //    yield return handle;

    //    if (handle.Status == AsyncOperationStatus.Succeeded)
    //    {
    //        environmentInstance = handle.Result;
    //        Debug.Log("Environment loaded thành công trong SampleScene!");
    //    }
    //    else
    //    {
    //        Debug.LogError("Environment load thất bại!");
    //    }
    //    mainCamera.gameObject.SetActive(false);
    //}
    private IEnumerator SpawnEnvironmentCoroutine()
    {
        // Đảm bảo SampleScene đã active
        Scene sampleScene = SceneManager.GetSceneByName("SampleScene");
        while (!sampleScene.isLoaded)
            yield return null;

        SceneManager.SetActiveScene(sampleScene);

        // Load environment qua Addressables
        var handle = environment.InstantiateAsync(Vector3.zero, Quaternion.identity);
        yield return handle;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            environmentInstance = handle.Result;
            Debug.Log("✅ Environment load thành công!");

            // Kiểm tra và chuyển sang SampleScene nếu cần
            if (environmentInstance.scene.name != "SampleScene")
            {
                SceneManager.MoveGameObjectToScene(environmentInstance, sampleScene);
                Debug.Log("♻ Environment đã được chuyển sang SampleScene!");
            }
        }
        else
        {
            Debug.LogError("❌ Environment load thất bại!");
        }

        // Tắt main camera của loading scene
        mainCamera.gameObject.SetActive(false);
    }
    private void DestroyOldEnvironment()
    {
        if (environmentInstance != null)
        {
            Addressables.ReleaseInstance(environmentInstance);
            environmentInstance = null;
        }

        GameObject oldEnv = GameObject.Find("Environment");
        if (oldEnv != null) Destroy(oldEnv);

        GameObject oldEnv1 = GameObject.Find("Environment2");
        if (oldEnv1 != null) Destroy(oldEnv1);
    }

    public void LoadMenuScene()
    {
        Time.timeScale = 1f;
        mainCamera.gameObject.SetActive(true);
        StartCoroutine(GameOverCoroutine());

    }

    private IEnumerator GameOverCoroutine()
    {
        // Dọn environment
        if (environmentInstance != null)
        {
            Addressables.ReleaseInstance(environmentInstance);
            environmentInstance = null;
        }

        // Unload SampleScene
        AsyncOperation unloadOp = SceneManager.UnloadSceneAsync("SampleScene");
        if (unloadOp != null)
        {
            while (!unloadOp.isDone)
                yield return null;
        }
        mainCamera.GetComponent<AudioListener>().enabled = true;
       
        eventSystem.gameObject.SetActive(true);
        // Load TitleScreen lại và đợi xong
        AsyncOperation loadOp = SceneManager.LoadSceneAsync("TitleScreen", LoadSceneMode.Additive);
        while (!loadOp.isDone)
            yield return null;

        // Sau khi load xong thì kích hoạt lại UI và Camera
      
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

}
