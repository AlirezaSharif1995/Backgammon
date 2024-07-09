using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Input = UnityEngine.Input;
using System.Text.RegularExpressions;

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
        
        signUpError.SetActive(false);
        signUpUI.alpha = 0;

        avatar[0].sprite = _playerInfo.avatars[0];
        avatar[1].sprite = _playerInfo.avatars[1];
    }
    
    void Update()
    {
        if (_playerInfo.Updated && !updated)
        {
            signUp = _playerInfo.PlayerData.token == "";

            if (!signUp)
            {
                _authentication.LoadDataAsync(_playerInfo.PlayerData.token);
                signing = true;
            }
            else
                signing = false;

            updated = true;
        }

        RefrralCodeSetup();
        SetupAvatar();

        _controller.SetCanvas(signUpUI, signUp);
        _controller.SetCanvas(refrralUI, refrral);
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
            case "confirm":
            {
                if (refrral)
                {
                    refrral = false;
                }
                else if (signUp)
                {
                    //if (CheckInput())
                    {
                        //_authentication.username = usernameInputUI.text.ToString();
                        //_authentication.password = passwordInputUI.text.ToString();
                        //_authentication.age = int.Parse(ageInputUI.text.ToString());
                        //_authentication.refrralCode = refrralInput[0].text + refrralInput[1].text + refrralInput[2].text + refrralInput[3].text + refrralInput[4].text;
                        
                        signUpError.SetActive(false);
                        _authentication.username = GenerateRandomUsername(11);
                        _authentication.password = GenerateRandomPassword(8);
                        _authentication.nickname = "Guest";
                        _authentication.avatarId = avatarId;
                        _authentication.region = "";
                        _authentication.state = "";
                        _authentication.birthdate = new []{-1, -1, -1};
                        _authentication.birthdate2 = new []{-1, -1, -1};
                        
                        _authentication.ClickSignUp();
                        signing = true;
                    }
                }
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
    
    string GenerateRandomUsername(int length)
    {
        const string chars = "0123456789";
        System.Random random = new System.Random();
        char[] stringChars = new char[length];

        for (int i = 0; i < length; i++)
        {
            stringChars[i] = chars[random.Next(chars.Length)];
        }

        return "User" + new string(stringChars);
    }
    
    string GenerateRandomPassword(int length)
    {
        if (length < 8 || length > 30)
        {
            throw new System.ArgumentException("Password length must be between 8 and 30 characters.");
        }

        const string upperCaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string lowerCaseChars = "abcdefghijklmnopqrstuvwxyz";
        const string digitChars = "0123456789";
        const string symbolChars = "!@#$%^&*()-_=+[]{}|;:,.<>?";

        System.Random random = new System.Random();
        StringBuilder passwordBuilder = new StringBuilder();

        passwordBuilder.Append(upperCaseChars[random.Next(upperCaseChars.Length)]);
        passwordBuilder.Append(lowerCaseChars[random.Next(lowerCaseChars.Length)]);
        passwordBuilder.Append(digitChars[random.Next(digitChars.Length)]);
        passwordBuilder.Append(symbolChars[random.Next(symbolChars.Length)]);

        string allChars = upperCaseChars + lowerCaseChars + digitChars + symbolChars;
        for (int i = 4; i < length; i++)
        {
            passwordBuilder.Append(allChars[random.Next(allChars.Length)]);
        }

        char[] passwordArray = passwordBuilder.ToString().ToCharArray();
        for (int i = passwordArray.Length - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            var temp = passwordArray[i];
            passwordArray[i] = passwordArray[j];
            passwordArray[j] = temp;
        }

        return new string(passwordArray);
    }

    /*bool CheckInput()
    {
        bool ok = true;
        if (usernameInputUI.text.Length == 0)
        {
            usernameErrorTextUI.text = "Nickname cannot be empty";
            ok = false;
        }
        else
        if (usernameInputUI.text.Length > 15)
        {
            ok = false;
            usernameErrorTextUI.text = "Nickname cannot be more than 15 characters";
        }

        if (ageInputUI.text.Length == 0)
        {
            ageErrorTextUI.text = "Age cannot be empty";
            ok = false;
        }else
        if (ageInputUI.text.Length > 2)
        {
            ok = false;
            ageErrorTextUI.text = "Age is not valid";
        }
                    
        if (passwordInputUI.text.Length == 0)
        {
            passwordErrorTextUI.text = "Password cannot be empty";
            ok = false;
        }else
        if (!IsPasswordValid(passwordInputUI.text))
        {
            ok = false;
            passwordErrorLongTextUI.text = "Password does not match requirements. Insert at least 1 uppercase letter, 1 lowercase letter, 1 digit, and 1 symbol. The password must be between 8 and 30 characters long.";
        }

        return ok;
    }*/
    
    bool IsPasswordValid(string password)
    {
        string pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,30}$";
        Regex regex = new Regex(pattern);
        return regex.IsMatch(password);
    }

    public void InputChanged()
    {
        signUpError.SetActive(false);
    }
}
