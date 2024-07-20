using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [Header("General")]
    public bool menu;
    public bool updated;
    public bool adsUpdated;
    
    [Header("Canvas")]
    public CanvasGroup menuUI;

    [Header("UI")]
    public Text usernameTextUI;
    public Text coinTextUI;
    public Image avatarUI;
    public Image regionIconUI;
    public Text classTextUI;
    public CanvasGroup videoAdsCanvasGroup;
    public Button videoAdsButton;

    private PlayerInfo _playerInfo;
    private SignController _signController;
    private ProfileController _profileController;
    private Controller _controller;
    private OptionsController _optionsController;
    private LoadingUpdate _loadingUpdate;
    private UnityRewardedAds _unityRewardedAds;
    private UnityBannerAds _unityBannerAds;

    void Start()
    {
        _playerInfo = FindObjectOfType<PlayerInfo>();
        _signController = GetComponent<SignController>();
        _profileController = GetComponent<ProfileController>();
        _controller = GetComponent<Controller>();
        _optionsController = GetComponent<OptionsController>();
        _loadingUpdate = FindObjectOfType<LoadingUpdate>();
        _unityRewardedAds = FindObjectOfType<UnityRewardedAds>();
        _unityBannerAds = FindObjectOfType<UnityBannerAds>();

        menuUI.alpha = 20;
    }
    
    void Update()
    {
        if (!updated && _playerInfo.Updated && !_signController.signing)
        {
            _controller.UpdateUI();
            updated = true;
        }
        
        menu = !_signController.signUp && !_controller.avatar && !_signController.refrral && !_controller.loading && !_profileController.profile && !_optionsController.options && !_loadingUpdate.loading;
        _controller.SetCanvas(menuUI, menu, 0, 1);
        _controller.SetCanvas(videoAdsCanvasGroup, videoAdsButton.interactable, 0.5f, 1);

        if (menu && !adsUpdated)
        {
            _unityRewardedAds.LoadAd();
            _unityBannerAds.LoadBanner();
            adsUpdated = true;
        }
        else if (!menu && adsUpdated)
        {
            _unityBannerAds.HideBannerAd();
            adsUpdated = false;
        }
    }

    public void ButtonClick(string type)
    {
        switch (type)
        {
            case "profile":
            {
                _profileController.profile = true;
            }
                break;
            
            case "options":
            {
                _optionsController.options = true;
            }
                break;
            
            case "play":
            {
                SceneManager.LoadScene("Game");
            }
                break;
        }
    }
}
