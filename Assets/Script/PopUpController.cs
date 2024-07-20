using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static PopUpController.ButtonClass;

public class PopUpController : MonoBehaviour
{
    public bool Show;
    bool hiding;
    public string Message;
    public bool FullScreen;

    [System.Serializable]
    public class ButtonClass
    {
        public bool Available;
        public string Text;
        public Color Color;
        public delegate void Function();
        public Function ButtonFunction;
    }

    public ButtonClass centerButton;
    public ButtonClass rightButton;
    public ButtonClass leftButton;

    CanvasGroup canvasGroup;

    GameObject centerButtonObj;
    GameObject rightButtonObj;
    GameObject leftButtonObj;

    Image centerButtonImage;
    Image rightButtonImage;
    Image leftButtonImage;

    Text centerButtonText;
    Text rightButtonText;
    Text leftButtonText;

    Text messageText;

    private void Start()
    { 
        canvasGroup = GetComponent<CanvasGroup>();

        canvasGroup.alpha = 0;

        centerButtonObj = transform.GetChild(1).gameObject;
        rightButtonObj = transform.GetChild(2).gameObject;
        leftButtonObj = transform.GetChild(3).gameObject;

        centerButtonImage = centerButtonObj.GetComponent<Image>();
        rightButtonImage = rightButtonObj.GetComponent<Image>();
        leftButtonImage = leftButtonObj.GetComponent<Image>();

        centerButtonText = centerButtonObj.transform.GetChild(0).GetComponent<Text>();
        rightButtonText = rightButtonObj.transform.GetChild(0).GetComponent<Text>();
        leftButtonText = leftButtonObj.transform.GetChild(0).GetComponent<Text>();

        messageText = transform.GetChild(0).GetChild(2).GetComponent<Text>();
    }

    private void Update()
    {
        float speed = 25 * Time.deltaTime;
        System.Collections.IEnumerator HideCo = HideDelay(1, speed);

        if (Show)
        {
            hiding = false;
            StopCoroutine(HideCo);

            transform.GetChild(0).GetChild(1).GetChild(0).gameObject.SetActive(FullScreen);

            // Set Active
            centerButtonObj.SetActive(centerButton.Available);
            rightButtonObj.SetActive(rightButton.Available);
            leftButtonObj.SetActive(leftButton.Available);

            // Set Color
            centerButtonImage.color = centerButton.Color;
            rightButtonImage.color = rightButton.Color;
            leftButtonImage.color = leftButton.Color;

            // Set Text
            centerButtonText.text = centerButton.Text;
            rightButtonText.text = rightButton.Text;
            leftButtonText.text = leftButton.Text;
            
            messageText.text = Message;

            if (canvasGroup.alpha < 1)
                canvasGroup.alpha += speed;

            transform.GetChild(0).gameObject.SetActive(true);
            transform.GetChild(1).gameObject.SetActive(true);
            transform.GetChild(2).gameObject.SetActive(true);
            transform.GetChild(3).gameObject.SetActive(true);
        }
        else
        {
            if (!hiding)
            {
                StartCoroutine(HideCo);
                hiding = true;
            }

            if (canvasGroup.alpha <= 0)
            {
                canvasGroup.alpha = 0;
                transform.GetChild(0).gameObject.SetActive(false);
                transform.GetChild(1).gameObject.SetActive(false);
                transform.GetChild(2).gameObject.SetActive(false);
                transform.GetChild(3).gameObject.SetActive(false);
            }
        }
    }

    IEnumerator HideDelay(float dealy, float speed)
    {
        yield return new WaitForSeconds(dealy * Time.deltaTime);
        while (!Show) 
        {
            yield return new WaitForSeconds(Time.deltaTime);

            if (canvasGroup.alpha > 0)
                canvasGroup.alpha -= speed;
        }
    }
    public void ButtonClickCenter()
    {
        if (Show && centerButton.Available)
        {
            centerButton.ButtonFunction();
        }
    }

    public void ButtonClickRight()
    {
        if (Show && rightButton.Available)
        {
            rightButton.ButtonFunction();
        }
    }

    public void ButtonClickLeft()
    {
        if (Show && leftButton.Available)
        {
            leftButton.ButtonFunction();
        }
    }

    public void ShowPopUpOne(bool fullscreen, string message, bool haveCenterButton, ButtonClass CenterButton, ButtonClass.Function CenterButtonFunction)
    {
        Message = message;
        FullScreen = fullscreen;

        if (haveCenterButton)
        {
            centerButton = CenterButton;
            centerButton.ButtonFunction = CenterButtonFunction;
        }
        else
            centerButton.Available = haveCenterButton;

        Show = true;
    }

    public void ShowPopUpTwo(bool fullscreen, string message, bool haveRightButton, ButtonClass RightButton, ButtonClass.Function RightButtonFunction, bool haveLeftButton, ButtonClass LeftButton, ButtonClass.Function LeftButtonFunction)
    {
        Message = message;
        FullScreen = fullscreen;

        if (haveRightButton)
        {
            rightButton = RightButton;
            rightButton.ButtonFunction = RightButtonFunction;
        }
        else
            rightButton.Available = haveRightButton;

        if (haveLeftButton)
        {
            leftButton = LeftButton;
            leftButton.ButtonFunction = LeftButtonFunction;
        }
        else
            leftButton.Available = haveLeftButton;

        Show = true;
    }

    public void ShowPopUp(string message, bool haveCenterButton, ButtonClass CenterButton, ButtonClass.Function CenterButtonFunction , bool haveRightButton,ButtonClass RightButton, ButtonClass.Function RightButtonFunction, bool haveLeftButton, ButtonClass LeftButton, ButtonClass.Function LeftButtonFunction)
    {
        if (!Show)
        {
            Message = message;

            if (haveCenterButton)
            {
                centerButton = CenterButton;
                centerButton.ButtonFunction = CenterButtonFunction;
            }
            else
                centerButton.Available = haveCenterButton;

            if (haveRightButton)
            {
                rightButton = RightButton;
                rightButton.ButtonFunction = RightButtonFunction;
            }
            else
                rightButton.Available = haveRightButton;

            if (haveLeftButton)
            {
                leftButton = LeftButton;
                leftButton.ButtonFunction = LeftButtonFunction;
            }  
            else
                leftButton.Available = haveLeftButton;

            Show = true;
        }
    }

}
