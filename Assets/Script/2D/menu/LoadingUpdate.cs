using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System.Text;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingUpdate : MonoBehaviour
{
    // Options
    private string apiUrl = "https://api.github.com/repos/l3ardia/backgammon-assets/contents/Localization/book.csv";
    private string personalAccessToken;
    private string localFilePath;
    private string fileName = "book.csv";

    [Header("General")]
    public bool loaded;
    public bool loading;
    public CanvasGroup loadingUI;
    public Text versionText;
    public Image loadingBar;
    public float loadPercent;

    private Controller _controller;
    private LoadExcel _loadExcel;
    private PopupController _popupController;

    void Start()
    {
        _controller = FindObjectOfType<Controller>();
        _loadExcel = FindObjectOfType<LoadExcel>();
        _popupController = FindObjectOfType<PopupController>();

        loaded = false;
        loading = true;
        loadingBar.fillAmount = 0;
        versionText.text = "Version: " + GetProjectVersion();
        localFilePath = Path.Combine(Application.dataPath, "Resources", fileName);
    }

    private void Update()
    {
        _controller.SetCanvas(loadingUI, loading);
        //loadingBar.fillAmount = Mathf.Lerp(loadingBar.fillAmount, loadPercent/100, Time.deltaTime);

        if (loadingBar.fillAmount < loadPercent/100)
            loadingBar.fillAmount += Time.deltaTime * (loadPercent/100) * 0.3f;

        if (loadingBar.fillAmount >= 0.995f && !loaded)
        {
            _controller.SetLoadDataUI();
            loading = false;
            loaded = true;
        }
    }

    public void StartDownload(string token)
    {
        personalAccessToken = token;
        StartCoroutine(DownloadCSV());
    }

    private IEnumerator DownloadCSV()
    {
        loadPercent = 0;
        
        using (UnityWebRequest webRequest = UnityWebRequest.Get(apiUrl))
        {
            webRequest.SetRequestHeader("Authorization", "token " + personalAccessToken);
            webRequest.SetRequestHeader("User-Agent", "Unity");

            loadPercent = 60;
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + webRequest.error);
                _popupController.OpenPopUp(false, false, true, "There was a problem receiving the information, please check your internet connection", null, null, ConfirmButtonClick);
            }
            else
            {
                string json = webRequest.downloadHandler.text;
                var jsonData = JsonUtility.FromJson<GitHubFile>(json);
                byte[] data = System.Convert.FromBase64String(jsonData.content);
                File.WriteAllBytes(localFilePath, data);
                Debug.Log("<color=yellow> CSV file downloaded and saved at: </color>" + localFilePath);
                loadPercent = 80;
                _loadExcel.itemDatabase.Clear();
                _loadExcel.LoadItemData();
            }
        }
    }

    public void ConfirmButtonClick()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    [System.Serializable]
    public class GitHubFile
    {
        public string content;
    }
    
    public static string GetProjectVersion()
    {
        string gameVersion = Application.version;
        if (!string.IsNullOrEmpty(gameVersion))
        {
            return gameVersion.Trim();
        }
        return "Unknown Version";
    }
}
