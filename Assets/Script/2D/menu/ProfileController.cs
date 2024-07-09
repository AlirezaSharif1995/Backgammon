using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ProfileController : MonoBehaviour
{
    [Header("General")]
    public bool profile;
    public int avatarId;
    
    [Header("Canvas")]
    public CanvasGroup profileUI;

    [Header("UI")]
    public Text usernameTextUI;
    public Text ageTextUI;
    public Text emailTextUI;
    public Text phoneTextUI;
    public Image avatarUI;
    public Image regionIconUI;

    private PlayerInfo _playerInfo;
    private Controller _controller;
    private PopupController _popupController;
    private Authentication _authentication;

    private void Start()
    {
        _playerInfo = FindObjectOfType<PlayerInfo>();
        _controller = FindObjectOfType<Controller>();
        _popupController = FindObjectOfType<PopupController>();
        _authentication = FindObjectOfType<Authentication>();

        profileUI.alpha = 0;
    }

    void Update()
    {
        SetCanvas(profileUI, profile);
    }

    public void ButtonClick(string type)
    {
        switch (type)
        {
            case "back":
            {
                profile = false;
            }
                break;

            case "avatar":
            {
                _controller.avatar = true;
                _controller.UpdateUI();
            }
                break;
            
            case "nickname":
            {
                _controller.nickname = true;
                _controller.UpdateUI();
            }
                break;
            
            case "region":
            {
                _controller.region = true;
                _controller.UpdateUI();
            }
                break;
            
            case "logout":
            {
                _popupController.OpenPopUp(true, true, false, "Are you sure you want to logout?", PopUpNo, LogOutYes, null);
            }
                break;
            
            case "delete account":
            {
                _popupController.OpenPopUp(true, true, false, "Are you sure you want to delete your account? <color=red> All your progress will be lost </color>", PopUpNo, DeleteAccountYes, null);
            }
                break;
        }
    }

    public void PopUpNo()
    {
        _popupController.show = false;
    }
    
    public void LogOutYes()
    {
        _playerInfo.PlayerData.token = "";
        _playerInfo.SaveGame();
        _authentication.SignOut();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void DeleteAccountYes()
    {
        _playerInfo.PlayerData.token = "";
        _playerInfo.SaveGame();
        _authentication.DeleteAccount();
        _authentication.SignOut();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void SetCanvas(CanvasGroup canvas, bool active)
    {
        float speed = 10 * Time.deltaTime;
        
        if (active)
        {
            canvas.gameObject.SetActive(true);

            if (canvas.alpha < 1)
                canvas.alpha += speed;
        }else
        {
            if (canvas.alpha > 0)
                canvas.alpha -= speed;
            
            if (canvas.alpha <= 0)
                canvas.gameObject.SetActive(false);
        }
    }
}
