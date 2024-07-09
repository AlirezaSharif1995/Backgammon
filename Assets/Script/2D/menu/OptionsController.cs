using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsController : MonoBehaviour
{
    [Header("General")]
    public bool options;
    
    [Header("Canvas")]
    public CanvasGroup optionsUI;

    [Header("UI")]
    public Image musicButtonImage;
    public Image soundButtonImage;
    public Sprite[] toggleSprite;

    private Controller _controller;
    private PlayerInfo _playerInfo;
    
    void Start()
    {
        _controller = FindObjectOfType<Controller>();
        _playerInfo = FindObjectOfType<PlayerInfo>();
    }

    void Update()
    {
        _controller.SetCanvas(optionsUI, options);

        if (_playerInfo.PlayerData.music)
            musicButtonImage.sprite = toggleSprite[0];
        else
            musicButtonImage.sprite = toggleSprite[1];
        
        if (_playerInfo.PlayerData.sound)
            soundButtonImage.sprite = toggleSprite[0];
        else
            soundButtonImage.sprite = toggleSprite[1];
    }

    public void ButtonClick(string type)
    {
        switch (type)
        {
            case "music":
            {
                _playerInfo.PlayerData.music = !_playerInfo.PlayerData.music;
                _playerInfo.SaveGame();
            }
                break;
            
            case "sound":
            {
                _playerInfo.PlayerData.sound = !_playerInfo.PlayerData.sound;
                _playerInfo.SaveGame();
            }
                break;
            
            case "back":
            {
                options = false;
            }
                break;
        }
    }
}
