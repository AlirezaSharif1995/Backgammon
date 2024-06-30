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
    public InputField usernameUI;
    public InputField passwordUI;

    async void Start()
    {
        await UnityServices.InitializeAsync();
        SetupEvents();
        if (AuthenticationService.Instance.IsSignedIn)
        {
            // Load saved data on successful initialization if already signed in
            await LoadDataAsync(AuthenticationService.Instance.PlayerId);
        }
    }

    void SetupEvents()
    {
        var authService = AuthenticationService.Instance;
        authService.SignedIn += async () =>
        {
            Debug.Log("Signed in as: " + authService.PlayerId);
            await LoadDataAsync(authService.PlayerId);
        };
        authService.SignInFailed += error => Debug.LogError("Sign-in failed: " + error);
        authService.SignedOut += () => Debug.Log("Signed out.");
    }

    public async void ClickSignUp()
    {
        string username = usernameUI.text;
        string password = passwordUI.text;
        await HandleAuthentication(async () =>
        {
            await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(username, password);
            string playerId = AuthenticationService.Instance.PlayerId;
            // Save initial data after sign-up
            await SaveDataAsync(playerId, 0, 0, string.Empty, 0); // Example default values
        }, "Sign-up is successful.");
    }
    
    public async void ClickSignIn()
    {
        string username = usernameUI.text;
        string password = passwordUI.text;
        await HandleAuthentication(() => AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username, password), "Sign-in is successful.");
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
    public async Task SaveDataAsync(string userId, int avatarCode, int location, string enteredReferral, int age)
    {
        try
        {
            var data = new Dictionary<string, object>
            {
                { "userId", userId },
                { "avatarCode", avatarCode },
                { "location", location },
                { "enteredReferral", enteredReferral },
                { "age", age }
            };

            await CloudSaveService.Instance.Data.ForceSaveAsync(data);
            Debug.Log("Data saved successfully.");
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
            var data = await CloudSaveService.Instance.Data.LoadAllAsync();

            if (data.TryGetValue("userId", out var savedUserId) && savedUserId.ToString() == userId)
            {
                if (data.TryGetValue("avatarCode", out var avatarCode))
                {
                    Debug.Log("Loaded avatarCode: " + avatarCode);
                }

                if (data.TryGetValue("location", out var location))
                {
                    Debug.Log("Loaded location: " + location);
                }

                if (data.TryGetValue("enteredReferral", out var enteredReferral))
                {
                    Debug.Log("Loaded enteredReferral: " + enteredReferral);
                }

                if (data.TryGetValue("age", out var age))
                {
                    Debug.Log("Loaded age: " + age);
                }
            }
            else
            {
                Debug.Log("No data found for this user.");
            }
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
        }
    }
}
