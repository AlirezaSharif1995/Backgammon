using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class Controller : MonoBehaviour
{
    private PlayerInfo _playerInfo;
    private Authentication _authentication;
    private MenuController _menuController;
    private ProfileController _profileController;
    private SignController _signController;
    private LoadingUpdate _loadingUpdate;
    private LoadExcel _loadExcel;
    
    [Header("Loading")]
    public bool loading;
    public CanvasGroup loadingUI;

    [Header("Nickname")] 
    public CanvasGroup nicknameUI;
    public bool nickname;
    public InputField nicknameInputUI;
    public Text nicknameErrorTextUI;
    public GameObject usernameErrorIcon;

    [Header("Avatar")]
    public bool avatar;
    public CanvasGroup avatarUI;
    public int avatarId;
    public Image avatarRight;
    public Image avatarLeft;
    public Image avatarCenter;
    public GameObject avatarFrameRight;
    public GameObject avatarFrameLeft;
    public GameObject avatarFrameCenter;
    public GameObject rightArrow;
    public GameObject leftArrow;

    [Header("Region")]
    public string regionData;
    public bool region;
    public bool regionScroll;
    public Image regionIcon;
    public Sprite globalIcon;
    public CanvasGroup regionUI;
    public CanvasGroup regionScrollUI;
    public Image regionScrollButton;
    public Sprite[] regionButtonSprite;
    public GameObject regionItemPrefab;
    public Transform regionContent;
    public ScrollRect regionScrollRect;
    public InputField searchInputUI;
    public GameObject clearButton;
    [System.Serializable]
    public class Region
    {
        public string name;
        public Sprite flag;
    }
    public List<Region> regions = new List<Region>();
    private List<GameObject> regionItems = new List<GameObject>();

    [Header("Birthdate")]
    public int[] birthdateData = new []{-1,-1,-1};
    public bool birthdate;
    public bool monthScroll;
    public bool dayScroll;
    public bool yearScroll;
    public CanvasGroup birthdateUI;
    public CanvasGroup monthScrollUI;
    public CanvasGroup dayScrollUI;
    public CanvasGroup yearScrollUI;
    public Text[] birthdateTextUI;
    public ScrollRect monthScrollRect;
    public ScrollRect dayScrollRect;
    public GameObject numberItemPrefab;
    public Transform dayContent;
    public ScrollRect yearScrollRect;
    public Transform yearContent;
    
    [Header("Board")] 
    public int boardId;
    public bool board;
    public CanvasGroup boardUI;
    public Image[] boardFrameImage;
    public Image[] boardVertexImage;
    public Color[] boardVertexColor;
    
    [Header("Checker")] 
    public int checkerId;
    public bool checker;
    public CanvasGroup checkerUI;
    public Image[] checkerFrameImage;
    public Image[] checkerVertexImage;
    public Color[] checkerVertexColor;

    [Header("Text UI")] // Load Data
    public Text termsTextUI;
    public Text[] avatarTitleTextUI;
    public Text signUpErrorTextUI;
    public Text optionsTitleTextUI;
    public Text profileTitleTextUI;
    public Text nicknameTitleTextUI;
    public Text nicknameHolderTextUI;
    public Text regionTitleTextUI;
    public Text birthdateTitleTextUI;
    public Text boardTitleTextUI;
    public Text checkerTitleTextUI;

    private void Start()
    {
        _playerInfo = FindObjectOfType<PlayerInfo>();
        _authentication = FindObjectOfType<Authentication>();
        _menuController = FindObjectOfType<MenuController>();
        _profileController = FindObjectOfType<ProfileController>();
        _signController = FindObjectOfType<SignController>();
        _loadingUpdate = FindObjectOfType<LoadingUpdate>();
        _loadExcel = FindObjectOfType<LoadExcel>();

        nicknameErrorTextUI.text = "";
        avatarUI.alpha = 0;
        nicknameUI.alpha = 0;
        loadingUI.alpha = 0;
        regionUI.alpha = 0;
        regionScrollUI.alpha = 0;
        birthdateUI.alpha = 0;
        monthScrollUI.alpha = 0;
        boardUI.alpha = 0;
        checkerUI.alpha = 0;
        
        SetupDayList();
        SetupYearList();
        SetupRegionList();
        searchInputUI.onValueChanged.AddListener(FilterList);
        RegionScrollToTop();
    }

    private void Update()
    {
        loading = _signController.signing && !_loadingUpdate.loading;
        
        SetCanvas(avatarUI, avatar, 0, 1);
        SetCanvas(nicknameUI, nickname, 0, 1);
        SetCanvas(loadingUI, loading, 0, 1);
        SetCanvas(regionUI, region, 0, 1);
        SetCanvas(regionScrollUI, regionScroll, 0, 1);
        SetCanvas(birthdateUI, birthdate, 0, 1);
        SetCanvas(monthScrollUI, monthScroll, 0, 1);
        SetCanvas(dayScrollUI, dayScroll, 0, 1);
        SetCanvas(yearScrollUI, yearScroll, 0, 1);
        SetCanvas(yearScrollUI, yearScroll, 0, 1);
        SetCanvas(boardUI, board, 0, 1);
        SetCanvas(checkerUI, checker, 0, 1);
        
        AvatarSetup();
        BoardSetup();
        CheckerSetup();
        
        usernameErrorIcon.SetActive(nicknameErrorTextUI.text.Length > 0);
        avatarFrameRight.SetActive(_playerInfo.avatars.Count >= avatarId + 2);
        avatarFrameLeft.SetActive(avatarId > 0);
        rightArrow.SetActive(avatarFrameRight.activeSelf);
        leftArrow.SetActive(avatarFrameLeft.activeSelf);
        
        clearButton.SetActive(searchInputUI.text.Length > 0);

        if (regionScroll)
            regionScrollButton.sprite = regionButtonSprite[1];
        else
            regionScrollButton.sprite = regionButtonSprite[0];
    }

    public void ButtonClick(string type)
    {
        switch (type)
        {
            // Avatar
            
            case "avatar back":
            {
                avatar = false;
            }
                break;
            
            case "avatar confirm":
            {
                UpdateParameter("avatarId", avatarId.ToString());
                avatar = false;
            }
                break;
            
            // Nickname
            
            case "nickname back":
            {
                nickname = false;
            }
                break;
            
            case "nickname confirm":
            {
                if (CheckInput("nickname"))
                {
                    UpdateParameter("nickname", nicknameInputUI.text);
                    nickname = false;
                }
            }
                break;
            
            // Region
            
            case "clear":
            {
                searchInputUI.text = "";
            }
                break;
            
            case "region":
            {
                UpdateUI();
                RegionScrollToTop();
                region = true;
            }
                break;
            
            case "region scroll":
            {
                regionScroll = true;
            }
                break;
            
            case "region back":
            {
                region = false;
            }
                break;
            
            case "region scroll back":
            {
                regionScroll = false;
            }
                break;
            
            case "region confirm":
            {
                UpdateParameter("region", regionData);
                region = false;
            }
                break;
            
            // Birthdate
            
            case "month scroll":
            {
                MonthScrollToTop();
                monthScroll = true;
            }
                break;
            
            case "month scroll back":
            {
                monthScroll = false;
            }
                break;
            
            case "day scroll":
            {
                DayScrollToTop();
                dayScroll = true;
            }
                break;
            
            case "day scroll back":
            {
                dayScroll = false;
            }
                break;
            
            case "year scroll":
            {
                YearScrollToTop();
                yearScroll = true;
            }
                break;
            
            case "year scroll back":
            {
                yearScroll = false;
            }
                break;
            
            case "birthdate":
            {
                UpdateUI();
                birthdate = true;
            }
                break;
            
            case "birthdate back":
            {
                birthdate = false;
            }
                break;
            
            case "birthdate confirm":
            {
                UpdateParameter("birthdate", "[" + birthdateData[0].ToString() + "," + birthdateData[1].ToString() + "," + birthdateData[2].ToString() + "]");
                birthdate = false;
            }
                break;
            
            // Board
            
            case "board":
            {
                board = true;
                UpdateUI();
            }
                break;
            
            case "board back":
            {
                board = false;
            }
                break;
            
            case "board confirm":
            {
                _playerInfo.PlayerData.boardId = boardId;
                _playerInfo.SaveGame();
                board = false;
            }
                break;
            
            // Checker
            
            case "checker":
            {
                checker = true;
                UpdateUI();
            }
                break;
            
            case "checker back":
            {
                checker = false;
            }
                break;
            
            case "checker confirm":
            {
                _playerInfo.PlayerData.checkerId = checkerId;
                _playerInfo.SaveGame();
                checker = false;
            }
                break;
        }
    }

    public void SetLoadDataUI()
    {
        termsTextUI.text = _loadExcel.itemDatabase[0].terms;
        avatarTitleTextUI[0].text = _loadExcel.itemDatabase[0].avatarTitle;
        avatarTitleTextUI[1].text = _loadExcel.itemDatabase[0].avatarTitle;
        signUpErrorTextUI.text = _loadExcel.itemDatabase[0].signUpError;
        optionsTitleTextUI.text = _loadExcel.itemDatabase[0].optionsTitle;
        profileTitleTextUI.text = _loadExcel.itemDatabase[0].profileTitle;
        nicknameTitleTextUI.text = _loadExcel.itemDatabase[0].nicknameTitle;
        nicknameHolderTextUI.text = _loadExcel.itemDatabase[0].nicknameHolder;
        regionTitleTextUI.text = _loadExcel.itemDatabase[0].regionTitle;
        birthdateTitleTextUI.text = _loadExcel.itemDatabase[0].birthdateTitle;
        boardTitleTextUI.text = _loadExcel.itemDatabase[0].boardTitle;
        checkerTitleTextUI.text = _loadExcel.itemDatabase[0].checkerTitle;
    }
    
    void BoardSetup()
    {
        float speed = 3 * Time.deltaTime;
        if (boardId == 0)
        {
            boardFrameImage[0].transform.localScale = Vector2.Lerp(boardFrameImage[0].transform.localScale, new Vector2(0.9f, 0.9f), speed);
            boardFrameImage[1].transform.localScale = Vector2.Lerp( boardFrameImage[1].transform.localScale, new Vector2(0.8f, 0.8f), speed);
            boardFrameImage[2].transform.localScale = Vector2.Lerp( boardFrameImage[2].transform.localScale, new Vector2(0.8f, 0.8f), speed);
            boardVertexImage[0].color = boardVertexColor[1];
            boardVertexImage[1].color = boardVertexColor[0];
            boardVertexImage[2].color = boardVertexColor[0];
        }
        else if (boardId == 1)
        {
            boardFrameImage[1].transform.localScale = Vector2.Lerp(boardFrameImage[1].transform.localScale, new Vector2(0.9f, 0.9f), speed);
            boardFrameImage[0].transform.localScale = Vector2.Lerp( boardFrameImage[0].transform.localScale, new Vector2(0.8f, 0.8f), speed);
            boardFrameImage[2].transform.localScale = Vector2.Lerp( boardFrameImage[2].transform.localScale, new Vector2(0.8f, 0.8f), speed);
            boardVertexImage[1].color = boardVertexColor[1];
            boardVertexImage[0].color = boardVertexColor[0];
            boardVertexImage[2].color = boardVertexColor[0];
        }
        else if (boardId == 2)
        {
            boardFrameImage[2].transform.localScale = Vector2.Lerp(boardFrameImage[2].transform.localScale, new Vector2(0.9f, 0.9f), speed);
            boardFrameImage[1].transform.localScale = Vector2.Lerp( boardFrameImage[1].transform.localScale, new Vector2(0.8f, 0.8f), speed);
            boardFrameImage[0].transform.localScale = Vector2.Lerp( boardFrameImage[0].transform.localScale, new Vector2(0.8f, 0.8f), speed);
            boardVertexImage[2].color = boardVertexColor[1];
            boardVertexImage[1].color = boardVertexColor[0];
            boardVertexImage[0].color = boardVertexColor[0];
        }

        boardFrameImage[0].enabled = boardId == 0;
        boardFrameImage[1].enabled = boardId == 1;
        boardFrameImage[2].enabled = boardId == 2;
    }
    
    void CheckerSetup()
    {
        float speed = 3 * Time.deltaTime;
        if (checkerId == 0)
        {
            checkerFrameImage[0].transform.localScale = Vector2.Lerp(checkerFrameImage[0].transform.localScale, new Vector2(0.9f, 0.9f), speed);
            checkerFrameImage[1].transform.localScale = Vector2.Lerp( checkerFrameImage[1].transform.localScale, new Vector2(0.8f, 0.8f), speed);
            checkerFrameImage[2].transform.localScale = Vector2.Lerp( checkerFrameImage[2].transform.localScale, new Vector2(0.8f, 0.8f), speed);
            checkerVertexImage[0].color = checkerVertexColor[1];
            checkerVertexImage[1].color = checkerVertexColor[0];
            checkerVertexImage[2].color = checkerVertexColor[0];
        }
        else if (checkerId == 1)
        {
            checkerFrameImage[1].transform.localScale = Vector2.Lerp(checkerFrameImage[1].transform.localScale, new Vector2(0.9f, 0.9f), speed);
            checkerFrameImage[0].transform.localScale = Vector2.Lerp( checkerFrameImage[0].transform.localScale, new Vector2(0.8f, 0.8f), speed);
            checkerFrameImage[2].transform.localScale = Vector2.Lerp( checkerFrameImage[2].transform.localScale, new Vector2(0.8f, 0.8f), speed);
            checkerVertexImage[1].color = checkerVertexColor[1];
            checkerVertexImage[0].color = checkerVertexColor[0];
            checkerVertexImage[2].color = checkerVertexColor[0];
        }
        else if (checkerId == 2)
        {
            checkerFrameImage[2].transform.localScale = Vector2.Lerp(checkerFrameImage[2].transform.localScale, new Vector2(0.9f, 0.9f), speed);
            checkerFrameImage[1].transform.localScale = Vector2.Lerp( checkerFrameImage[1].transform.localScale, new Vector2(0.8f, 0.8f), speed);
            checkerFrameImage[0].transform.localScale = Vector2.Lerp( checkerFrameImage[0].transform.localScale, new Vector2(0.8f, 0.8f), speed);
            checkerVertexImage[2].color = checkerVertexColor[1];
            checkerVertexImage[1].color = checkerVertexColor[0];
            checkerVertexImage[0].color = checkerVertexColor[0];
        }

        checkerFrameImage[0].enabled = checkerId == 0;
        checkerFrameImage[1].enabled = checkerId == 1;
        checkerFrameImage[2].enabled = checkerId == 2;
    }
    
    public void CheckerClick(int id)
    {
        checkerId = id;
    }
    
    public void BoardClick(int id)
    {
        boardId = id;
    }

    public void FlagClick(string name)
    {
        regionData = name;
        UpdateRegionIcon();
        regionScroll = false;
    }
    
    public void MonthClick(int id)
    {
        birthdateData[0] = id;
        UpdateBirthdateUI();
        monthScroll = false;
    }
    
    public void DayClick(int id)
    {
        birthdateData[1] = id;
        UpdateBirthdateUI();
        dayScroll = false;
    }
    
    public void YearClick(int id)
    {
        birthdateData[2] = id;
        UpdateBirthdateUI();
        yearScroll = false;
    }

    void UpdateBirthdateUI()
    {
        for (int i = 0; i < birthdateData.Length; i++)
        {
            if (birthdateData[i] != -1)
            {
                switch (i)
                {
                    case 0:
                    {
                        switch (birthdateData[i])
                        {
                            case 1: birthdateTextUI[i].text = "January";
                                break;
                            case 2: birthdateTextUI[i].text = "February";
                                break;
                            case 3: birthdateTextUI[i].text = "March";
                                break;
                            case 4: birthdateTextUI[i].text = "April";
                                break;
                            case 5: birthdateTextUI[i].text = "May";
                                break;
                            case 6: birthdateTextUI[i].text = "June";
                                break;
                            case 7: birthdateTextUI[i].text = "July";
                                break;
                            case 8: birthdateTextUI[i].text = "August";
                                break;
                            case 9: birthdateTextUI[i].text = "September";
                                break;
                            case 10: birthdateTextUI[i].text = "October";
                                break;
                            case 11: birthdateTextUI[i].text = "November";
                                break;
                            case 12: birthdateTextUI[i].text = "December";
                                break;
                        }
                    }
                        break;
                    
                    case 1:
                        birthdateTextUI[i].text = birthdateData[i].ToString();
                        break;
                    
                    case 2:
                        birthdateTextUI[i].text = birthdateData[i].ToString();
                        break;
                }
            }
            else
            {
                switch (i)
                {
                    case 0: birthdateTextUI[i].text = "Month";
                        break;
                    
                    case 1: birthdateTextUI[i].text = "Day";
                        break;
                    case 2: birthdateTextUI[i].text = "Year";
                        break;
                }
            }
        }
    }

    void RegionScrollToTop()
    {
        Canvas.ForceUpdateCanvases();
        regionScrollRect.verticalNormalizedPosition = 1f;
    }
    
    void MonthScrollToTop()
    {
        Canvas.ForceUpdateCanvases();
        monthScrollRect.verticalNormalizedPosition = 1f;
    }
    
    void DayScrollToTop()
    {
        Canvas.ForceUpdateCanvases();
        dayScrollRect.verticalNormalizedPosition = 1f;
    }
    
    void YearScrollToTop()
    {
        Canvas.ForceUpdateCanvases();
        yearScrollRect.verticalNormalizedPosition = 1f;
    }
    
    void SetupRegionList()
    {
        foreach (var region in regions)
        {
            GameObject newItem = Instantiate(regionItemPrefab, regionContent);
            newItem.transform.Find("flag").GetComponent<Image>().sprite = region.flag;
            newItem.transform.Find("name").GetComponent<Text>().text = region.name;
            newItem.GetComponent<Button>().onClick.AddListener(() => FlagClick(region.name));
            regionItems.Add(newItem);
        }

        RegionScrollToTop();
    }
    
    void SetupDayList()
    {
        for (int i=1; i<=31;i++)
        {
            GameObject newItem = Instantiate(numberItemPrefab, dayContent);
            newItem.transform.Find("name").GetComponent<Text>().text = i.ToString();
            int day = i;
            newItem.GetComponent<Button>().onClick.AddListener(() => DayClick(day));
        }

        DayScrollToTop();
    }
    
    void SetupYearList()
    {
        int currentYear = System.DateTime.Now.Year;
        int startYear = currentYear - 10;
        int endYear = currentYear - 70;

        for (int year = startYear; year >= endYear; year--)
        {
            GameObject newItem = Instantiate(numberItemPrefab, yearContent);
            newItem.transform.Find("name").GetComponent<Text>().text = year.ToString();
            int y = year;
            newItem.GetComponent<Button>().onClick.AddListener(() => YearClick(y));
        }

        YearScrollToTop();
    }
    
    void FilterList(string searchText)
    {
        List<GameObject> filteredItems = new List<GameObject>();

        foreach (var item in regionItems)
        {
            var countryName = item.transform.Find("name").GetComponent<Text>().text;
            bool isMatch = countryName.ToLower().Contains(searchText.ToLower());
            item.SetActive(isMatch);
            
            if (isMatch)
                filteredItems.Add(item);
        }

        for (int i = 0; i < filteredItems.Count; i++)
            filteredItems[i].transform.SetSiblingIndex(i);

        RegionScrollToTop();
    }

    public void UpdateParameter(string label, string data)
    {
        switch (label)
        {
            case "avatarId":
            {
                UpdateData(int.Parse(data), _playerInfo.PlayerData.region, _playerInfo.PlayerData.state, _playerInfo.PlayerData.refrralCode, _playerInfo.PlayerData.birthdate, _playerInfo.PlayerData.nickname, _playerInfo.PlayerData.coin);
            }
                break;
            
            case "region":
            {
                UpdateData(_playerInfo.PlayerData.avatarId, data, _playerInfo.PlayerData.state, _playerInfo.PlayerData.refrralCode, _playerInfo.PlayerData.birthdate, _playerInfo.PlayerData.nickname, _playerInfo.PlayerData.coin);
            }
                break;
            
            case "state":
            {
                UpdateData(_playerInfo.PlayerData.avatarId, _playerInfo.PlayerData.region, data, _playerInfo.PlayerData.refrralCode, _playerInfo.PlayerData.birthdate, _playerInfo.PlayerData.nickname, _playerInfo.PlayerData.coin);
            }
                break;
            
            case "refrralCode":
            {
                UpdateData(_playerInfo.PlayerData.avatarId, _playerInfo.PlayerData.region, _playerInfo.PlayerData.state, data, _playerInfo.PlayerData.birthdate, _playerInfo.PlayerData.nickname, _playerInfo.PlayerData.coin);
            }
                break;
            
            case "birthdate":
            {
                UpdateData(_playerInfo.PlayerData.avatarId, _playerInfo.PlayerData.region, _playerInfo.PlayerData.state, _playerInfo.PlayerData.refrralCode, ConvertStringToIntArray(data), _playerInfo.PlayerData.nickname, _playerInfo.PlayerData.coin);
            }
                break;
            
            case "nickname":
            {
                UpdateData(_playerInfo.PlayerData.avatarId, _playerInfo.PlayerData.region, _playerInfo.PlayerData.state, _playerInfo.PlayerData.refrralCode, _playerInfo.PlayerData.birthdate, data, _playerInfo.PlayerData.coin);
            }
                break;
            
            case "coin":
            {
                UpdateData(_playerInfo.PlayerData.avatarId, _playerInfo.PlayerData.region, _playerInfo.PlayerData.state, _playerInfo.PlayerData.refrralCode, _playerInfo.PlayerData.birthdate, _playerInfo.PlayerData.nickname, int.Parse(data));
            }
                break;
        }
        
        Debug.Log("<color=yellow> Data Updated: </color> <color=white>( "+ label +" = " + data + " ) </color>");
    }

    public async Task UpdateData(int avatarId, string region, string state, string refrralCode, int[] birthdate, string nickname, int coin)
    {
        await _authentication.SaveDataAsync(_playerInfo.PlayerData.token, coin, avatarId, region, state, refrralCode, birthdate, nickname);
        _playerInfo.PlayerData.avatarId = avatarId;
        _playerInfo.PlayerData.coin = coin;
        _playerInfo.PlayerData.nickname = nickname;
        _playerInfo.PlayerData.region = region;
        _playerInfo.PlayerData.state = state;
        _playerInfo.PlayerData.refrralCode = refrralCode;
        _playerInfo.PlayerData.birthdate = birthdate;
        _playerInfo.SaveGame();
        
        UpdateUI();
    }

    public void InputChanged()
    {
        nicknameErrorTextUI.text = "";
    }
    
    bool CheckInput(string type)
    {
        bool ok = true;
        
        switch (type)
        {
            case "nickname":
            {
                if (nicknameInputUI.text.Length == 0)
                {
                    string message = _loadExcel.itemDatabase[0].nicknameEmptyError;
                    nicknameErrorTextUI.text = message;
                    ok = false;
                }
                else
                if (nicknameInputUI.text.Length > 11)
                {
                    ok = false;
                    string message = _loadExcel.itemDatabase[0].nicknameCharError;
                    nicknameErrorTextUI.text = message;
                }
            }
                break;
        }

        return ok;
    }

    void UpdateRegionIcon()
    {
        if (regionData != "")
        {
            foreach (Region region in regions)
            {
                if (region.name == regionData)
                {
                    regionIcon.sprite = region.flag;
                    break;
                }
            }
        }
        else
            regionIcon.sprite = globalIcon;
    }
    
    public void UpdateUI()
    {
        _menuController.usernameTextUI.text = _playerInfo.PlayerData.nickname;
        _menuController.avatarUI.sprite = _playerInfo.avatars[_playerInfo.PlayerData.avatarId];
        _menuController.coinTextUI.text = _playerInfo.PlayerData.coin.ToString();
        _profileController.usernameTextUI.text = _playerInfo.PlayerData.nickname;
        _profileController.avatarUI.sprite = _playerInfo.avatars[_playerInfo.PlayerData.avatarId];
        avatarId = _playerInfo.PlayerData.avatarId;
        nicknameInputUI.text = _playerInfo.PlayerData.nickname;
        regionData = _playerInfo.PlayerData.region;
        birthdateData = new []{_playerInfo.PlayerData.birthdate[0], _playerInfo.PlayerData.birthdate[1], _playerInfo.PlayerData.birthdate[2]};
        boardId = _playerInfo.PlayerData.boardId;
        checkerId = _playerInfo.PlayerData.checkerId;
        _menuController.classTextUI.text = _playerInfo.PlayerData.playerClass;
        UpdateRegionIcon();
        UpdateBirthdateUI();

        if (_playerInfo.PlayerData.region != "")
        {
            _profileController.regionIconUI.sprite = regionIcon.sprite;
            _menuController.regionIconUI.sprite = regionIcon.sprite;
        }
        else
        {
            _profileController.regionIconUI.sprite = globalIcon;
            _menuController.regionIconUI.sprite = globalIcon;
        }

        Vector2 pos = _profileController.usernameTextUI.transform.position;
        if (_playerInfo.PlayerData.birthdate[2] != -1)
        {
            int currentYear = System.DateTime.Now.Year; _profileController.ageTextUI.text = "<color=grey> Age: </color>" + "<color=black>" + (currentYear - _playerInfo.PlayerData.birthdate[2]) + "</color>";
            
            pos.y = _profileController.usernameTextPosY_default;
        }
        else
        {
            //_profileController.ageTextUI.text = "<color=grey> Age: </color>" + "<color=red>" + "(Unregistered)" + "</color>";
            _profileController.ageTextUI.text = "";
            pos.y = _profileController.usernameTextPosY_default - 20;
        }
        _profileController.usernameTextUI.transform.position = pos;
            

        if (_playerInfo.PlayerData.email == "")
            _profileController.emailTextUI.text = "<color=red>  " + "(Unregistered)" + "</color>";
        else
            _profileController.emailTextUI.text = _playerInfo.PlayerData.email;
        
        if (_playerInfo.PlayerData.phone == "")
            _profileController.phoneTextUI.text = "<color=red>   " + "(Unregistered)" + "</color>";
        else
            _profileController.phoneTextUI.text = _playerInfo.PlayerData.phone;
    }

    void AvatarSetup()
    {
        if (_playerInfo.avatars.Count >= avatarId + 2)
            avatarRight.sprite = _playerInfo.avatars[avatarId + 1];

        if (avatarId > 0)
            avatarLeft.sprite = _playerInfo.avatars[avatarId - 1];
        
        avatarCenter.sprite = _playerInfo.avatars[avatarId];

        avatarFrameRight.transform.localScale = Vector3.Lerp(avatarFrameRight.transform.localScale, new Vector3(0.9f, 0.9f, 0.9f), 2 * Time.deltaTime);
        avatarFrameLeft.transform.localScale = Vector3.Lerp(avatarFrameLeft.transform.localScale, new Vector3(0.9f, 0.9f, 0.9f), 2 * Time.deltaTime);
        avatarFrameCenter.transform.localScale = Vector3.Lerp(avatarFrameCenter.transform.localScale, new Vector3(1, 1, 1), 2 * Time.deltaTime);
    }
    
    public void ArrowClick(bool isRight)
    {
        if (isRight && avatarId + 1 < _playerInfo.avatars.Count)
        {
            AvatarNextShape();
            avatarId += 1;
        }
        else
        if (!isRight && avatarId > 0)
        {
            AvatarNextShape();
            avatarId -= 1;
        }
    }
    
    void AvatarNextShape()
    {
        avatarFrameRight.transform.localScale = new Vector3(1, 1, 1);
        avatarFrameLeft.transform.localScale = new Vector3(1, 1, 1);
        avatarFrameCenter.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
    }

    public void SetCanvas(CanvasGroup canvas, bool active, float min, float max)
    {
        float speed = 10 * Time.deltaTime;
        
        if (active)
        {
            canvas.gameObject.SetActive(true);

            if (canvas.alpha < max)
                canvas.alpha += speed;
        }else
        {
            if (canvas.alpha > min)
                canvas.alpha -= speed;
            
            if (canvas.alpha <= 0 && min == 0)
                canvas.gameObject.SetActive(false);
        }
    }
    
    public int[] ConvertStringToIntArray(string input)
    {
        input = input.Trim('[', ']');
        string[] stringArray = input.Split(',');
        int[] intArray = new int[stringArray.Length];
        for (int i = 0; i < stringArray.Length; i++)
        {
            intArray[i] = int.Parse(stringArray[i]);
        }

        return intArray;
    }
}
