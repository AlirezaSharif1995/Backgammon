using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    public string logout;
    public string deleteAccount;
    public string defaultCoin;
    public string terms;
    public string avatarTitle;
    public string signUpError;
    public string optionsTitle;
    public string profileTitle;
    public string nicknameTitle;
    public string nicknameHolder;
    public string regionTitle;
    public string birthdateTitle;
    public string boardTitle;
    public string checkerTitle;
    public string nicknameEmptyError;
    public string nicknameCharError;

    public Item(Item d)
    {
        logout = d.logout;
        deleteAccount = d.deleteAccount;
        defaultCoin = d.defaultCoin;
        terms = d.terms;
        avatarTitle = d.avatarTitle;
        signUpError = d.signUpError;
        optionsTitle = d.optionsTitle;
        profileTitle = d.profileTitle;
        nicknameTitle = d.nicknameTitle;
        nicknameHolder = d.nicknameHolder;
        regionTitle = d.regionTitle;
        birthdateTitle = d.birthdateTitle;
        boardTitle = d.boardTitle;
        checkerTitle = d.checkerTitle;
        nicknameEmptyError = d.nicknameEmptyError;
        nicknameCharError = d.nicknameCharError;
    }
}
