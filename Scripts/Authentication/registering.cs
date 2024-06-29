using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.UI;
using AuthenticationException = System.Security.Authentication.AuthenticationException;

public class registering : MonoBehaviour
{
    public InputField usernameUI;
    public InputField passwordUI;

    async void Start()
    {
        await UnityServices.InitializeAsync();
        SetupEvents();
    }

    void SetupEvents()
    {
        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in as: " + AuthenticationService.Instance.PlayerId);
        };
        AuthenticationService.Instance.SignInFailed += error =>
        {
            Debug.LogError("Sign-in failed: " + error);
        };
        AuthenticationService.Instance.SignedOut += () =>
        {
            Debug.Log("Signed out.");
        };
    }

    public async void ClickSignUp()
    {
        string username = usernameUI.text;
        string password = passwordUI.text;

        await SignUpWithUsernamePasswordAsync(username, password);
    }
    
    public async void ClickSignIn()
    {
        string username = usernameUI.text;
        string password = passwordUI.text;

        await SignInWithUsernamePasswordAsync(username, password);
    }
    
    async Task SignUpWithUsernamePasswordAsync(string username, string password)
    {
        try
        {
            await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(username, password);
            Debug.Log("Sign-up is successful.");
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
    
    async Task SignInWithUsernamePasswordAsync(string username, string password)
    {
        try
        {
            await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username, password);
            Debug.Log("SignIn is successful.");
        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
    }
    
}