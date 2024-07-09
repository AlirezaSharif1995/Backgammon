using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.CloudSave;
using UnityEngine;
using UnityEngine.UI;
using AuthenticationException = System.Security.Authentication.AuthenticationException;

public class Authentication : MonoBehaviour
{
    public string username;
    public string nickname;
    public string password;
    public int avatarId;
    public string region;
    public string state;
    public string refrralCode;
    public int[] birthdate = new []{-1,-1,-1};
    public int[] birthdate2 = new []{-1,-1,-1};

    private PlayerInfo _playerInfo;
    private SignController _signController;
    private Controller _controller;

    async void Start()
    {
        _playerInfo = FindObjectOfType<PlayerInfo>();
        _signController = FindObjectOfType<SignController>();
        _controller = FindObjectOfType<Controller>();
        
        try
        {
            await UnityServices.InitializeAsync();
            SetupEvents();
            Debug.Log("<color=green> Unity Services Initialized </color>");

            if (AuthenticationService.Instance.IsSignedIn)
            {
                await LoadDataAsync(AuthenticationService.Instance.PlayerId);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error during Unity Services initialization: " + ex);
        }
    }

    void SetupEvents()
    {
        var authService = AuthenticationService.Instance;
        authService.SignedIn += async () =>
        {
            Debug.Log("<color=orange> Signed in as: " + authService.PlayerId + "</color>");
            _playerInfo.PlayerData.token = authService.PlayerId;
            _playerInfo.SaveGame();
            await LoadDataAsync(authService.PlayerId);
        };
        authService.SignInFailed += error =>
        {
            Debug.LogError("Sign-in failed: " + error);
            _signController.signUpError.SetActive(true);
            _signController.signing = false;
        };
        authService.SignedOut += () => Debug.Log("<color=red> Signed out </color>");
    }

    public async void ClickSignUp()
    {
        await HandleAuthentication(async () =>
        {
            try
            {
                await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(username, password);
                Debug.Log("<color=green> SignUp is successful </color>");
                
                _playerInfo.PlayerData.username = username;
                _playerInfo.PlayerData.nickname = nickname;
                _playerInfo.PlayerData.password = password;
                _playerInfo.PlayerData.avatarId = avatarId;
                _playerInfo.PlayerData.region = region;
                _playerInfo.PlayerData.state = state;
                _playerInfo.PlayerData.refrralCode = refrralCode;
                _playerInfo.PlayerData.birthdate = new []{birthdate[0],birthdate[1],birthdate[2]};
                _playerInfo.SaveGame();
                
                _controller.UpdateUI();
                _signController.signing = false;
                _signController.signUp = false;
            }
            catch (AuthenticationException ex)
            {
                Debug.LogException(ex);
            }
            catch (RequestFailedException ex)
            {
                Debug.LogException(ex);
            }

            string playerId = AuthenticationService.Instance.PlayerId;
            // Save initial data after sign-up
            await SaveDataAsync(playerId, avatarId, region, state, refrralCode, birthdate, nickname);
        }, "Sign-up is successful.");
    }
    
    public async void ClickSignIn()
    {
        await HandleAuthentication(async () =>
        {
            await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username, password);

            // After successful sign-in, load the data
            if (AuthenticationService.Instance.IsSignedIn)
            {
                await LoadDataAsync(AuthenticationService.Instance.PlayerId);
            }
        }, "Sign-in is successful.");
    }
    
    public void SignOut()
    {
        try
        {
            AuthenticationService.Instance.SignOut();
            Debug.Log("<color=red> Signed out successfully </color>");
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
        }
    }
    
    public async void DeleteAccount()
    {
        try
        {
            await AuthenticationService.Instance.DeleteAccountAsync();
            Debug.Log("<color=red> Account deleted successfully </color>");
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
        }
    }
    
    private async Task HandleAuthentication(Func<Task> authTask, string successMessage)
    {
        try
        {
            await authTask();
            Debug.Log(successMessage);
        }
        catch (AuthenticationException ex)
        {
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
        }
    }

    // Method to save data
    public async Task SaveDataAsync(string userId, int avatarCode, string region, string state, string enteredReferral, int[] birthdate, string nickname)
    {
        try
        {
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                Debug.LogWarning("User is not signed in.");
                return;
            }

            var data = new Dictionary<string, object>
            {
                { "userId", userId },
                { "avatarCode", avatarCode },
                { "region", region },
                { "state", state },
                { "enteredReferral", enteredReferral },
                { "birthdate", birthdate },
                { "nickname", nickname }
            };

            await CloudSaveService.Instance.Data.ForceSaveAsync(data);
            Debug.Log("<color=green> Data saved successfully </color>");
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
        }
    }

    // Method to load data
    public async Task LoadDataAsync(string userId)
    {
        try
        {
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                Debug.Log("<color=yellow> User is not signed in, signing in... </color>");
                // Try to sign in the user
                await SignInAsync();
                if (!AuthenticationService.Instance.IsSignedIn)
                    Debug.LogError("Sign-in failed. Unable to load data.");
                return;
            }
            
            var data = await CloudSaveService.Instance.Data.LoadAllAsync();

            if (data.TryGetValue("userId", out var savedUserId) && savedUserId.ToString() == userId)
            {
                if (data.TryGetValue("avatarCode", out var avatarCode))
                {
                    _playerInfo.PlayerData.avatarId = int.Parse(avatarCode);
                    Debug.Log("<color=orange> Loaded (avatarCode): </color> <color=white>" + avatarCode + "</color>");
                }else
                    Debug.LogError("No data found for (avatarCode)");

                if (data.TryGetValue("region", out var region))
                {
                    _playerInfo.PlayerData.region = region;
                    Debug.Log("<color=orange> Loaded (region): </color> <color=white>" + region + "</color>");
                }else
                    Debug.LogError("No data found for (region)");
                
                if (data.TryGetValue("state", out var state))
                {
                    _playerInfo.PlayerData.state = state;
                    Debug.Log("<color=orange> Loaded (state): </color> <color=white>" + state + "</color>");
                }else
                    Debug.LogError("No data found for (state)");

                if (data.TryGetValue("enteredReferral", out var enteredReferral))
                {
                    _playerInfo.PlayerData.refrralCode = enteredReferral;
                    Debug.Log("<color=orange> Loaded (enteredReferral): </color> <color=white>" + enteredReferral + "</color>");
                }else
                    Debug.LogError("No data found for (enteredReferral)");

                if (data.TryGetValue("nickname", out var nickname))
                {
                    _playerInfo.PlayerData.nickname = nickname;
                    Debug.Log("<color=orange> Loaded (nickname): </color> <color=white>" + nickname + "</color>");
                }else
                    Debug.LogError("No data found for (nickname)");
                
                if (data.TryGetValue("birthdate", out var birthdate))
                {
                    _playerInfo.PlayerData.birthdate = _controller.ConvertStringToIntArray(birthdate);
                    Debug.Log("<color=orange> Loaded (birthdate): </color> <color=white>" + birthdate + "</color>");
                }else
                    Debug.LogError("No data found for (birthdate)");
            }
            else
            {
                Debug.LogError("No data found for this user");
            }
            
            _playerInfo.SaveGame();
            _signController.signing = false;
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
        }
    }
    
    private async Task SignInAsync()
    {
        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("<color=green> Sign-in successful </color>");
        }
        catch (AuthenticationException ex)
        {
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
        }
    }
}
