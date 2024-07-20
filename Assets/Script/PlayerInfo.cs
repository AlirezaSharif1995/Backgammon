 using System;
 using System.Collections.Generic;
 using System.Net.Http;
 using System.Threading.Tasks;
 using Newtonsoft.Json.Linq;
 using Unity.Services.Authentication;
 using Unity.Services.Core;
 using Unity.Services.RemoteConfig;
 using UnityEngine;
 using Unity.RemoteConfig;
 using Unity.Services.RemoteConfig;
 using Unity.Services.Core;
 using UnityEngine.SceneManagement;

public class PlayerInfo : MonoBehaviour
{
    SocketManager socketManager = SocketManager.Instance;
    private LoadExcel _loadExcel;
    private LoadingUpdate _loadingUpdate;
    
    public string BuildCode;
    public string ipRegion;

    [System.Serializable]
    public class Data
    {
        public string token;
        public string nickname;
        public int avatarId;
        public string region;
        public string state;
        public string email;
        public string phone; 
        public string refrralCode;
        public int coin;
        public bool sound;
        public bool music;
        public int[] birthdate = new[] {-1, -1, -1};
        public string accessToken;
        public int boardId;
        public int checkerId;
        public string playerClass;
    }

    public Data PlayerData;
    public bool Updated;
    public List<Sprite> avatars = new List<Sprite>();

    private void Start()
    {
        _loadExcel = FindObjectOfType<LoadExcel>();
        _loadingUpdate = FindObjectOfType<LoadingUpdate>();
        
        socketManager = SocketManager.Instance;
    }

    AudioSource[] Audios;

    private void Update()
    {
        if (!Updated)
        {
            string json = PlayerPrefs.GetString("PlayerData");

            if (PlayerPrefs.HasKey("PlayerData"))
                PlayerData = JsonUtility.FromJson<Data>(json);

            #region Default
            if (!PlayerPrefs.HasKey("PlayerData"))
            {
                Debug.Log("PlayerData: Default");
                PlayerData.token = "";
                PlayerData.nickname = "";
                PlayerData.avatarId = 0;
                PlayerData.region = "";
                PlayerData.state = "";
                PlayerData.refrralCode = "";
                PlayerData.coin = 0;
                PlayerData.sound = true;
                PlayerData.music = true;
                PlayerData.email = "";
                PlayerData.phone = "";
                PlayerData.birthdate = new[] {-1, -1, -1};
                PlayerData.boardId = 0;
                PlayerData.checkerId = 0;
                PlayerData.playerClass = "C";
            }
            #endregion
            
            #region Debug
            //PlayerData.token = "";
            #endregion
            
            SetAudio();
            
            if (_loadingUpdate != null) GetRemoteKeys();
            
            SaveGame();

            Updated = true;
        }

        if (Updated)
        {
            switch (PlayerData.coin)
            {
                case <= 1000: PlayerData.playerClass = "C";
                    break;
                
                case <= 5000: PlayerData.playerClass = "B";
                    break;
                
                case > 5000: PlayerData.playerClass = "A";
                    break;
            }
        }
    }

    #if UNITY_EDITOR
    class ResetPlayerPrefs
    {
        [UnityEditor.MenuItem("Tools/Reset PlayerPrefs")]
        private static void Reset()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            Debug.Log("<color=red> PlayerPrefs Deleted </color>");
        }
    }
    #endif

    public void SaveGame()
    {
        Debug.Log("<color=green> PlayerData: Save </color>");
        string json = JsonUtility.ToJson(PlayerData);
        PlayerPrefs.SetString("PlayerData", json);
        PlayerPrefs.Save();
    }

    void OnApplicationQuit()
    {
        Debug.Log("<color=red> Application: Quit </color>");
    }

    public void SetAudio()
    {
        Audios = Resources.FindObjectsOfTypeAll<AudioSource>();

        foreach (AudioSource audio in Audios)
        {
            if (audio.gameObject.layer == LayerMask.NameToLayer("Sound"))
                audio.mute = !PlayerData.sound;
            else
            if (audio.gameObject.layer == LayerMask.NameToLayer("Music"))
                audio.mute = !PlayerData.music;
        }
    }
    
    public void SetDefault()
    {
        PlayerData.nickname = "Guest";
        PlayerData.avatarId = 0;
        PlayerData.state = "";
        PlayerData.birthdate = new []{-1, -1, -1};
        PlayerData.coin = int.Parse(_loadExcel.itemDatabase[0].defaultCoin);
        PlayerData.region = ipRegion;
    }

    #region Cloud Config
    struct userAttributes { }
    struct appAttributes { }
    
    async void GetRemoteKeys()
    {
        _loadingUpdate.loadPercent = 40;
        _loadingUpdate.infoTextUI.text = "Receiving keys";
        
        try
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            var res = await RemoteConfigService.Instance.FetchConfigsAsync(new userAttributes(), new appAttributes());
            GetCountry(RemoteConfigService.Instance.appConfig.GetString("IPAPI_ACCESS_KEY"));
            _loadingUpdate.StartDownload(RemoteConfigService.Instance.appConfig.GetString("GITHUB_ACCESS_KEY"));
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            Debug.LogError("Failed to fetch configs, is your project linked and configuration deployed?");
        }
    }
    #endregion

    #region Get Player Region
    private static readonly HttpClient client = new HttpClient();
    private string apiUrl;

    public async void GetCountry(string apiKey)
    {
        apiUrl = "https://ipapi.co/json/?key=" + apiKey;
        string countryName = await GetCountryNameAsync();
        if (!string.IsNullOrEmpty(countryName))
        {
            Debug.Log($"<color=yellow> Player is located in: </color> {countryName}");
            ipRegion = countryName;
        }
    }

    public async Task<string> GetCountryCodeAsync()
    {
        try
        {
            string responseString = await client.GetStringAsync(apiUrl);
            JObject responseJson = JObject.Parse(responseString);
            string countryCode = responseJson["country"].ToString();
            return countryCode;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to get country code: {e.Message}");
            return null;
        }
    }

    public async Task<string> GetCountryNameAsync()
    {
        try
        {
            string responseString = await client.GetStringAsync(apiUrl);
            JObject responseJson = JObject.Parse(responseString);
            string countryName = responseJson["country_name"].ToString();
            return countryName;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to get country name: {e.Message}");
            return null;
        }
    }
    #endregion
}
