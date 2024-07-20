using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LoadExcel : MonoBehaviour
{
    public Item blankItem;
    public List<Item> itemDatabase = new List<Item>();

    private LoadingUpdate _loadingUpdate;
    private Controller _controller;

    private void Start()
    {
        _loadingUpdate = FindObjectOfType<LoadingUpdate>();
        _controller = FindObjectOfType<Controller>();
    }

    public void LoadItemData()
    {
        //Clear Database
        
        //READ CSV files
        string filePath = Path.Combine(Application.persistentDataPath, "Resources", "book.csv");
        string fileContent = File.ReadAllText(filePath);
        List<Dictionary<string, object>> data = CSVReader.ReadFromString(fileContent);
        for (var i = 0; i < data.Count; i++)
        {
            string logout = data[i]["logout"].ToString();
            string deleteAccount = data[i]["deleteAccount"].ToString();
            string defaultCoin = data[i]["defaultCoin"].ToString();
            string terms = data[i]["terms"].ToString();
            string avatarTitle = data[i]["avatarTitle"].ToString();
            string signUpError = data[i]["signUpError"].ToString();
            string optionsTitle = data[i]["optionsTitle"].ToString();
            string profileTitle = data[i]["profileTitle"].ToString();
            string nicknameTitle = data[i]["nicknameTitle"].ToString();
            string nicknameHolder = data[i]["nicknameHolder"].ToString();
            string regionTitle = data[i]["regionTitle"].ToString();
            string birthdateTitle = data[i]["birthdateTitle"].ToString();
            string boardTitle = data[i]["boardTitle"].ToString();
            string checkerTitle = data[i]["checkerTitle"].ToString();
            string nicknameEmptyError = data[i]["nicknameEmptyError"].ToString();
            string nicknameCharError = data[i]["nicknameCharError"].ToString();

            AddItem(logout, deleteAccount, defaultCoin, terms, avatarTitle, signUpError, optionsTitle, profileTitle, nicknameTitle, nicknameHolder, regionTitle, birthdateTitle, boardTitle, checkerTitle, nicknameEmptyError, nicknameCharError);
        }
        
        _loadingUpdate.infoTextUI.text = "All set";
        _loadingUpdate.loadPercent = 100;
    }

    void AddItem(string logout, string deleteAccount, string defaultCoin, string terms, string avatarTitle, string signUpError, string optionsTitle, string profileTitle, string nicknameTitle, string nicknameHolder, string regionTitle, string birthdateTitle, string boardTitle, string checkerTitle, string nicknameEmptyError, string nicknameCharError)
    {
        Item tempItem = new Item(blankItem);

        tempItem.logout = logout;
        tempItem.deleteAccount = deleteAccount;
        tempItem.defaultCoin = defaultCoin;
        tempItem.terms = terms;
        tempItem.avatarTitle = avatarTitle;
        tempItem.signUpError = signUpError;
        tempItem.optionsTitle = optionsTitle;
        tempItem.profileTitle = profileTitle;
        tempItem.nicknameTitle = nicknameTitle;
        tempItem.nicknameHolder = nicknameHolder;
        tempItem.regionTitle = regionTitle;
        tempItem.birthdateTitle = birthdateTitle;
        tempItem.boardTitle = boardTitle;
        tempItem.checkerTitle = checkerTitle;
        tempItem.nicknameEmptyError = nicknameEmptyError;
        tempItem.nicknameCharError = nicknameCharError;

        itemDatabase.Add(tempItem);
    }
}
