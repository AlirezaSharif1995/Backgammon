using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Input = UnityEngine.Input;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Unity.Services.Authentication;
using Unity.Services.RemoteConfig;
using UnityEngine.Networking;

public class SignController : MonoBehaviour
{
    [Header("General")]
    public bool signUp;
    public bool refrral;
    public bool updated;
    
    [Header("Status")]
    public bool signing;

    [Header("Canvas")]
    public CanvasGroup signUpUI;
    public CanvasGroup refrralUI;

    private Authentication _authentication;
    private PlayerInfo _playerInfo;
    private Controller _controller;
    private LoadingUpdate _loadingUpdate;
    private LoadExcel _loadExcel;

    [Header("Refrral")]
    public InputField[] refrralInput;
    public Image[] refrralImage;
    public Sprite[] refralSprite;
    private int presentInput;

    [Header("Error")]
    public GameObject signUpError;

    [Header("Avatar")] 
    public int avatarId;
    public Image[] avatar;
    public Image[] avatarFrameImage;
    public Image[] avatarVertexImage;
    public Color[] avatarVertexColor;

    void Start()
    {
        _authentication = FindObjectOfType<Authentication>();
        _playerInfo = FindObjectOfType<PlayerInfo>();
        _controller = FindObjectOfType<Controller>();
        _loadingUpdate = FindObjectOfType<LoadingUpdate>();
        _loadExcel = FindObjectOfType<LoadExcel>();
        
        signUpError.SetActive(false);
        signUpUI.alpha = 0;

        avatar[0].sprite = _playerInfo.avatars[0];
        avatar[1].sprite = _playerInfo.avatars[1];
    }

    void Update()
    {
        if (_playerInfo.Updated && _loadingUpdate.loaded && !updated)
        {
            signUp = _playerInfo.PlayerData.token == "";

            if (signUp)
            {
                _authentication.SignOut();
                signing = false;
            }
            else
            {
                signing = true;
                _authentication.TrySignIn();
            }

            updated = true;
        }

        RefrralCodeSetup();
        SetupAvatar();

        _controller.SetCanvas(signUpUI, signUp, 0, 1);
        _controller.SetCanvas(refrralUI, refrral, 0, 1);
    }

    void SetupAvatar()
    {
        float speed = 3 * Time.deltaTime;
        if (avatarId == 0)
        {
            avatarFrameImage[0].transform.localScale = Vector2.Lerp(avatarFrameImage[0].transform.localScale, new Vector2(0.9f, 0.9f), speed);
            avatarFrameImage[1].transform.localScale = Vector2.Lerp( avatarFrameImage[1].transform.localScale, new Vector2(0.8f, 0.8f), speed);
            avatarVertexImage[0].color = avatarVertexColor[1];
            avatarVertexImage[1].color = avatarVertexColor[0];
        }
        else if (avatarId == 1)
        {
            avatarFrameImage[1].transform.localScale = Vector2.Lerp(avatarFrameImage[1].transform.localScale, new Vector2(0.9f, 0.9f), speed);
            avatarFrameImage[0].transform.localScale = Vector2.Lerp(avatarFrameImage[0].transform.localScale, new Vector2(0.8f, 0.8f), speed);
            avatarVertexImage[1].color = avatarVertexColor[1];
            avatarVertexImage[0].color = avatarVertexColor[0];
        }

        avatarFrameImage[0].enabled = avatarId == 0;
        avatarFrameImage[1].enabled = avatarId == 1;
    }

    public void AvatarClick(int id)
    {
        avatarId = id;
    }

    void RefrralCodeSetup()
    {

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            for (int i = refrralInput.Length-1;i>=0;i--)
            {
                if (refrralInput[i].text.Length > 0)
                {
                    refrralInput[i].text = "";
                    break;
                }
            }
        }

        for (int i=0; i<refrralInput.Length;i++)
        {
            if (refrralInput[i].text.Length > 1 && i != 4)
            {
                int targetIndex = -1;

                if (refrralInput.Length > i+1 && refrralInput[i + 1].text.Length == 0)
                    targetIndex = i + 1;
                else if (refrralInput.Length > i+2 && refrralInput[i + 2].text.Length == 0)
                    targetIndex = i + 2;
                else if (refrralInput.Length > i+3 && refrralInput[i + 3].text.Length == 0)
                    targetIndex = i + 3;
                else if (refrralInput.Length > i+4 && refrralInput[i + 4].text.Length == 0)
                    targetIndex = i + 4;
                
                if (i != targetIndex && targetIndex != -1)
                {
                    refrralInput[targetIndex].text = refrralInput[i].text[1].ToString();
                    string updatedText = refrralInput[i].text.Remove(1, 1);
                    refrralInput[i].text = updatedText;
                }
            }

            if (refrralInput[0].text.Length == 0)
                presentInput = 0;
            else
            {
                if (i > 0 && refrralInput[i - 1].text.Length > 0 && refrralInput[i].text.Length == 0)
                    presentInput = i;
            }
            
            if (i != presentInput)
            {
                if (refrralInput[i].text.Length == 0)
                    refrralImage[i].sprite = refralSprite[0];
                else
                if (refrralInput[i].text.Length > 0)
                    refrralImage[i].sprite = refralSprite[1];
            }else if (refrralInput[i].text.Length == 0)
                refrralImage[i].sprite = refralSprite[2];
            else
                refrralImage[i].sprite = refralSprite[1];
        }
        
        foreach (InputField input in refrralInput)
        {
            if (input.text.Length > 1)
            {
                string updatedText = input.text.Remove(1, 1);
                input.text = updatedText;
            }
        }
    }

    public void ButtonClick(string type)
    {
        switch (type)
        {
            case "sign as guest":
            {
                //_authentication.refrralCode = refrralInput[0].text + refrralInput[1].text + refrralInput[2].text + refrralInput[3].text + refrralInput[4].text;
                
                signUpError.SetActive(false);
                _authentication.nickname = "Guest";
                _authentication.avatarId = avatarId;
                _authentication.state = "";
                _authentication.birthdate = new []{-1, -1, -1};
                _authentication.coin = int.Parse(_loadExcel.itemDatabase[0].defaultCoin);
                _authentication.region = _playerInfo.ipRegion;
                
                _authentication.ClickSignInAsGuest();
                    signing = true;
            }
                break;
            
            case "sign with google":
            {
                signUpError.SetActive(false);
                _authentication.ClickSignInWithGoogle();
                signing = true;
            }
                break;

            case "refrral":
            {
                refrral = true;
            }
                break;
            
            case "avatar":
            {
                _controller.avatar = true;
                _controller.UpdateUI();
            }
                break;
            
            case "back":
            {
                if (refrral)
                {
                    refrral = false;
                }else 
                if (signUp)
                {
                    signUp = false;
                }
            }
                break;
        }
        
    }

    public void InputChanged()
    {
        signUpError.SetActive(false);
    }
}
