 using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInfo : MonoBehaviour
{
    public string BuildCode;

    [System.Serializable]
    public class Data
    {
        public string token;
        public string username;
        public string nickname;
        public string password;
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
    }

    public Data PlayerData;
    public bool Updated;
    public List<Sprite> avatars = new List<Sprite>();
    

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
                PlayerData.username = "";
                PlayerData.nickname = "";
                PlayerData.password = "";
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
            }
            #endregion

            // Set Audios
            SetAudio();

            #region Debug
            //PlayerData.Nickname = "Guest";
            //PlayerData.Level = 3;
            //PlayerData.Language = LanguageController.LanguageEnum.persian;
            //PlayerData.Sound = false;
            #endregion

            SaveGame();

            Updated = true;

        }
    }

    /*#if UNITY_EDITOR
        [UnityEngine.RuntimeInitializeOnLoadMethod]
        private static void ResetPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
        }
    #endif*/

    #if UNITY_EDITOR
    class ResetPlayerPrefs
    {
        [UnityEditor.MenuItem("Tools/Reset PlayerPrefs")]
        private static void Reset()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
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
}
