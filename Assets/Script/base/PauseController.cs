using Newtonsoft.Json;
using PimDeWitte.UnityMainThreadDispatcher;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseController : MonoBehaviour
{
    SocketManager socketManager = SocketManager.Instance;

    [Header("General")]
    public bool Pause;

    [Header("Access")]
    public CanvasGroup PauseUI;
    public Transform IconTimer;
    public Transform IconAvatar;
    public Text TimerUI;

    PlayerInfo playerInfo;
    GameSetup GameSetup;
    float pauseTimer;

    private void Start()
    {
        playerInfo = FindObjectOfType<PlayerInfo>();
        GameSetup = FindObjectOfType<GameSetup>();

        socketManager = SocketManager.Instance;
    }

    private void Update()
    {
        SetAlpha();

        if (!Pause) {  return; }

        SetLoading();
        TimerUI.text = Mathf.FloorToInt(pauseTimer).ToString();
        pauseTimer -= Time.deltaTime;

        if (pauseTimer <= 0)
            pauseTimer = 0;
    }

    public void PauseOn()
    {
        socketManager.socket.On("PlayerDisconnected", (response) =>
        {
            if (GameSetup.DeveloperMode)
                Debug.Log("<color=red>(DEVELOPER MODE) </color> <color=yellow> Pause: (Disconnected): </color> <color=white>" + response.ToString() + "</color>");

            var result = JsonConvert.DeserializeObject<List<SocketManager.PauseData>>(response.ToString());

            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                if (result[0].playerToken != playerInfo.PlayerData.token)
                {
                    Pause = true;
                    pauseTimer = socketManager.GameData.notReadyTime;

                    Debug.Log("<color=yellow> Pause: (Disconnected):  </color> <color=white> Received </color>");
                }
            });
        });

        socketManager.socket.On("PlayerReconnected", (response) =>
        {
            if (GameSetup.DeveloperMode)
                Debug.Log("<color=red>(DEVELOPER MODE) </color> <color=yellow> Pause: (Reconnected):  </color> <color=white>" + response.ToString() + "</color>");

            var result = JsonConvert.DeserializeObject<List<SocketManager.PauseData>>(response.ToString());
            

            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                if (result[0].playerToken != playerInfo.PlayerData.token)
                {
                    Pause = false;

                    Debug.Log("<color=yellow> Pause: (Reconnected):  </color> <color=white> Received </color>");
                }
            });
        });
    }

    void SetAlpha()
    {
        float speed = 10 * Time.deltaTime;

        if (Pause)
        {
            PauseUI.gameObject.SetActive(true);

            if (PauseUI.alpha < 1)
                PauseUI.alpha += speed;

            if (PauseUI.alpha >= 1)
                PauseUI.alpha = 1;
        }else
        {
            if (PauseUI.alpha > 0)
                PauseUI.alpha -= speed;

            if (PauseUI.alpha <= 0)
            {
                PauseUI.alpha = 0;
                PauseUI.gameObject.SetActive(false);
            }
        }
    }

    void SetLoading()
    {
        float rotSpeed = 150 * Time.deltaTime;
        Quaternion rot = IconTimer.rotation;
        Vector3 eulerRot = rot.eulerAngles;
        eulerRot.z += rotSpeed;
        rot = Quaternion.Euler(eulerRot);
        IconTimer.rotation = rot;
        IconAvatar.rotation = rot;
    }
}
