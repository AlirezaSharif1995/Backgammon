using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [Header("General")]
    public bool menu;
    public bool updated;
    
    [Header("Canvas")]
    public CanvasGroup menuUI;

    [Header("UI")]
    public Text usernameTextUI;
    public Text coinTextUI;
    public Image avatarUI;
    public Image regionIconUI;

    private PlayerInfo _playerInfo;
    private SignController _signController;
    private ProfileController _profileController;
    private Controller _controller;
    private OptionsController _optionsController;

    void Start()
    {
        _playerInfo = FindObjectOfType<PlayerInfo>();
        _signController = GetComponent<SignController>();
        _profileController = GetComponent<ProfileController>();
        _controller = GetComponent<Controller>();
        _optionsController = GetComponent<OptionsController>();

        menuUI.alpha = 0;
    }
    
    void Update()
    {
        if (!updated && _playerInfo.Updated && !_signController.signing)
        {
            _controller.UpdateUI();
            updated = true;
        }
        
        menu = !_signController.signUp && !_controller.avatar && !_signController.refrral && !_controller.loading && !_profileController.profile && !_optionsController.options;

        _controller.SetCanvas(menuUI, menu);
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
        }
    }
}
