using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class CheckerController : MonoBehaviour
{
    public bool Updated;
    float alpha;
    bool change;
    float Scale;
    
    [Header("Place")]
    public int ID;
    public List<GameSetup.ColumnClass> columnTarget = new List<GameSetup.ColumnClass>();
    public GameSetup.ColumnClass columnStart;
    public bool finished;
    
    [Header("Fix")]
    public bool Fixing;
    public Vector3 FixTarget;
    public float FixSpeed;
    public bool canFix;
    public float distanceToFix;
    public float multipy;
    
    [Header("Drag")]
    public bool canDrag;
    public bool isDragging;
    public float touchDistance;

    [Header("Audio")]
    public List<AudioSource> PlaceAudio;
    public bool played;
    public bool FirstAudio;
    public bool kickplayed;

    public enum side
    {
        White,
        Black
    }
    [Header("Side")]
    public side Side;


    [Header("HighLight")]
    public bool HighLight;

    [Header("Rotation")]
    public bool Stand;

    [Header("Sprite")]
    public GameObject whiteSprite;
    public GameObject blackSprite;
    
    PauseController pauseController;
    GameSetup GameSetup;
    private SortingGroup _sortingGroup;

    void Start()
    {
        GameSetup = FindObjectOfType<GameSetup>();
        pauseController = FindObjectOfType<PauseController>();
        _sortingGroup = GetComponent<SortingGroup>();

        FirstAudio = true;
        kickplayed = true;
        HighLight = false;
        //SetLayer("Default"); // itsnew
        Scale = 1;
    }

    void Update()
    {
        //if (GameSetup.Starting) SetLayer("OnTop"); // itsnew

        if (isDragging)
            Scale = Mathf.Lerp(Scale,1.3f, 10 * Time.deltaTime);
        else
            Scale = Mathf.Lerp(Scale, 1, 10 * Time.deltaTime);
        
        transform.localScale = new Vector3(Scale, Scale, Scale);

        HighLight = canDrag && !isDragging && !GameSetup.disableCheckersHighLight && GameSetup.Roll.playerTurn && ((GameSetup.GeneralOptions.Online && !pauseController.Pause) || GameSetup.GeneralOptions.AI);

        if (!Updated && GameSetup.Updated)
        {
            SetSpriteSide();
            Updated = true;
        }

        distanceToFix = Vector3.Distance(transform.position, FixTarget);

        if (!Updated) { return; }

        //Coll.enabled = canDrag; // itsnew

        Drag();
        Fix();
        setRotation();
        PlayPlaceAudio();
        SetHighLightMaterial();

        if (canDrag) Touch();
    }

    public void SetLayerOrder(string type)
    {
        switch (type)
        {
            case "place":
                _sortingGroup.sortingOrder = GetPlaceID();
                break;
            
            case "top":
                _sortingGroup.sortingOrder = 40;
                break;
        }
        
    }

    int GetPlaceID()
    {
        for (int i = columnStart.Place.Count-1; i >= 0; i--)
        {
            if (columnStart.Place[i].Checker == this)
            {
                return i;
            }
        }

        return -1;
    }

    void SetSpriteSide()
    {
        switch (Side)
        {
            case side.White:
            {
                whiteSprite.SetActive(true);
                blackSprite.SetActive(false);
            }
                break;
            
            case side.Black:
            {
                whiteSprite.SetActive(false);
                blackSprite.SetActive(true);
            }
                break;
        }
    }

    void Touch()
    {
        if (Input.GetMouseButtonDown(0) && GameSetup.GetTouchNearChecker() == this && !GameSetup.moveVerifying)
            TouchDrag();
    }

    public void TouchDrag()
    {
        if (canDrag && ((GameSetup.GeneralOptions.Online && !pauseController.Pause) || GameSetup.GeneralOptions.AI))
        {
            isDragging = true;
            Fixing = false;
            //SetLayer("OnTop"); // itsnew
            GameSetup.touchNearChecker = this;
        }
    }

    void PlayPlaceAudio()
    {
        if (distanceToFix <= 3)
        {
            if (!played)
            {
                int play = 0;
                int canplay = 1;

                if (FirstAudio)
                {
                    play = Random.Range(0, PlaceAudio.Count - 4);
                    canplay = Random.Range(1, 3);
                }
                else
                    play = Random.Range(8, PlaceAudio.Count);

                if (canplay == 1)
                    PlaceAudio[play].Play();

                played = true;
            }

            if (!kickplayed)
            {
                PlaceAudio[0].Play();
                kickplayed = true;
            }
        }
    }

    void SetHighLightMaterial()
    {
        float speed = 2;
        Color color = GameSetup.GeneralOptions.HighLightColor;
        if (HighLight)
        {
            if (alpha <= 0)
                change = true;

            if (alpha >= 0.8f)
                change = false;

            if (change)
                alpha += speed * Time.deltaTime;
            else
                alpha -= speed * Time.deltaTime;
        }
        else
        {
            alpha = -1;
            color.a = 0;
        }

        /*material.SetFloat("_alpha", alpha);
        material.SetColor("_Color", color);*/ // itsnew
    }

    void setRotation()
    {
        Quaternion rot = transform.rotation;
        float speed = 10;

        if (Stand)
            rot = Quaternion.Euler(0, rot.eulerAngles.y, rot.eulerAngles.z);
        else
            rot = Quaternion.Euler(-90, rot.eulerAngles.y, rot.eulerAngles.z);

        transform.rotation = Quaternion.Lerp(transform.rotation,rot, speed * Time.deltaTime);
    }

    GameSetup.ColumnClass CheckColumnRange()
    {
        bool inRange = false;

        for (int i = 0; i < GameSetup.Column.Count + 2; i++)
        {
            Vector3 cubeMin = Vector3.zero;
            Vector3 cubeMax = Vector3.zero;
            Bounds cubeBounds = new Bounds();

            if (i < GameSetup.Column.Count)
            {
                cubeMin = GameSetup.Column[i].Pos - GameSetup.GeneralOptions.rangeSizeColumn / 2f;
                cubeMax = GameSetup.Column[i].Pos + GameSetup.GeneralOptions.rangeSizeColumn / 2f;
            }
            else
            {
                if (i == GameSetup.Column.Count)
                {
                    cubeMin = GameSetup.finishColumn[0].Pos - GameSetup.GeneralOptions.rangeSizeFinishColumn / 2f;
                    cubeMax = GameSetup.finishColumn[0].Pos + GameSetup.GeneralOptions.rangeSizeFinishColumn / 2f;
                }
                else
                if (i == GameSetup.Column.Count + 1)
                {
                    cubeMin = (GameSetup.finishColumn[1].Pos + GameSetup.GeneralOptions.rangeDistance) - GameSetup.GeneralOptions.rangeSizeFinishColumn / 2f;
                    cubeMax = (GameSetup.finishColumn[1].Pos + GameSetup.GeneralOptions.rangeDistance) + GameSetup.GeneralOptions.rangeSizeFinishColumn / 2f;
                }
            }

            cubeBounds = new Bounds((cubeMin + cubeMax) / 2f, cubeMax - cubeMin);

            inRange = cubeBounds.Contains(transform.position);

            if (inRange)
            {
                if (i < GameSetup.Column.Count)
                    return GameSetup.Column[i];
                else
                {
                    if (i == GameSetup.Column.Count)
                        return GameSetup.finishColumn[0];
                    else
                    if (i == GameSetup.Column.Count + 1)
                        return GameSetup.finishColumn[1];
                }
            }
        }

        return null;
    }

    void Fix()
    {
        if (Fixing && canFix)
        {
            float speed = GameSetup.GeneralOptions.fixSpeed * multipy;
            Vector3 pos = transform.position;

            Vector3 direction = (FixTarget - pos).normalized;
            pos += direction * Mathf.Min(speed * Time.deltaTime, Vector3.Distance(pos, FixTarget));
            
            //pos.y = Mathf.Lerp(pos.y, FixTarget.y + (distanceToFix/3), speed * Time.deltaTime); //itsnew
            transform.position = pos;
        }
    }

    public void CheckFinish(GameSetup.ColumnClass column)
    {
        if (column.ID == -1 || column.ID == 24)
        {
            finished = true;
            Stand = true;
        }
    }

    void Drag()
    {
        if (Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended) || GameSetup.GetTouchPosition() == Vector3.zero)
        {
            if (isDragging)
            {
                if (GameSetup.touchNearChecker == this) GameSetup.touchNearChecker = null;

                GameSetup.ColumnClass inRangeColumn = CheckColumnRange();

                if (inRangeColumn != null && columnTarget.Contains(inRangeColumn))
                {
                    multipy = 2;
                    UpdateRolls(inRangeColumn, true, true);
                    CheckFinish(inRangeColumn);
                    GameSetup.ResetCheckers();
                    GameSetup.PlaceChecker(this, inRangeColumn, true, columnStart,false);

                    if (GameSetup.Roll.Rolls.Count > 0) StartCoroutine(UpdateNaxtMove(GameSetup.GeneralOptions.delayUpdateNaxtMove, "player"));
                    else
                        GameSetup.playerDone(true);
                }
                else
                {
                    multipy = 3.2f;
                    //SetLayer("Default"); // itsnew
                }
                
                Fixing = true;
                SetColumnHighLight(false);
                SetLayerOrder("place");

                isDragging = false;
            }
        }

        if (isDragging)
        {
            if (!canDrag || (GameSetup.GeneralOptions.Online && pauseController.Pause)) { isDragging = false; return; }

            Vector3 Pos = transform.position;
            Pos.x = GameSetup.GetTouchPosition().x;
            Pos.y = GameSetup.GetTouchPosition().y; // itsnew
            Pos.z = 4; // itsnew
            transform.position = Pos; 

            SetColumnHighLight(true);
            SetLayerOrder("top");

            /*Vector3 pos = transform.position;
            pos.y = 0.1f;
            transform.position = pos;*/ // itsnew
        }
    }

    public IEnumerator UpdateNaxtMove(float delay,string side)
    {
        GameSetup.ResetColumnHighLight();
        yield return new WaitForSeconds(delay * Time.deltaTime);

        if (side == "player")
            GameSetup.UsableChecker(GameSetup.GeneralOptions.PlayerLocation,GameSetup.GeneralOptions.playerSide,GameSetup.Roll.Rolls);
        else
        if (side == "opponent")
        {
            if (GameSetup.GeneralOptions.AI)
                StartCoroutine(GameSetup.AIAct(GameSetup.GeneralOptions.delayAIMove,"intelligent"));
        }
    }

    void SetColumnHighLight(bool active)
    {
        if (columnTarget.Count > 0)
        {
            foreach(GameSetup.ColumnClass column in columnTarget)
            {
                column.HighLight = active;
            }
        }

        GameSetup.disableCheckersHighLight = active;
    }

    /*void OnMouseDown()
    {
        TouchDrag();
    }*/

    /*public void SetLayer(string layer)
    {
        if (Updated)
        {
            render1.layer = LayerMask.NameToLayer(layer);
            render2.layer = LayerMask.NameToLayer(layer);
        }
    }*/ // itsnew

    /*public IEnumerator SetLayerTime(string layerStart,string layerTarget,float holdTime)
    {
        SetLayer(layerStart);
        yield return new WaitForSeconds(holdTime * Time.deltaTime);
        SetLayer(layerTarget);
    }*/ // itsnew

    public void UpdateRolls(GameSetup.ColumnClass column, bool createHistory, bool verify)
    {
        int changeDistance = Mathf.Abs(columnStart.ID - column.ID);

        List<int> indexesToRemove = new List<int>();

        bool oneByone = false;
        bool haveKickRoll = false;

        for (int i = 0; i < GameSetup.Roll.Rolls.Count; i++)
        {
            if (GameSetup.Roll.Rolls[i] == changeDistance)
            {
                indexesToRemove.Add(i);
                oneByone = true;
                break;
            }
        }

        if (!oneByone)
        {
            for (int i = 0; i < GameSetup.Roll.Rolls.Count; i++)
            {
                if (GameSetup.Roll.Rolls.Count == 2)
                {
                    if (GameSetup.Roll.Rolls[0] + GameSetup.Roll.Rolls[1] == changeDistance)
                    {
                        if (columnStart.BlockedRoll != 0 && columnStart.BlockedRoll == GameSetup.Roll.Rolls[0])
                        {
                            indexesToRemove.Add(1);
                            indexesToRemove.Add(0);
                            Debug.Log("have Block Roll");
                            haveKickRoll = true;
                        }
                        else
                        {
                            indexesToRemove.Add(0);
                            indexesToRemove.Add(1);
                        }
                    }

                    break;
                }

                if (GameSetup.Roll.Rolls.Count == 3)
                {
                    if (GameSetup.Roll.Rolls[0] + GameSetup.Roll.Rolls[1] == changeDistance)
                    {
                        indexesToRemove.Add(0);
                        indexesToRemove.Add(1);
                    }
                    else
                    if (GameSetup.Roll.Rolls[0] + GameSetup.Roll.Rolls[2] == changeDistance)
                    {
                        indexesToRemove.Add(0);
                        indexesToRemove.Add(2);
                    }
                    else
                    if (GameSetup.Roll.Rolls[1] + GameSetup.Roll.Rolls[2] == changeDistance)
                    {
                        indexesToRemove.Add(1);
                        indexesToRemove.Add(2);
                    }
                    else
                    if (GameSetup.Roll.Rolls[0] + GameSetup.Roll.Rolls[1] + GameSetup.Roll.Rolls[2] == changeDistance)
                    {
                        indexesToRemove.Add(0);
                        indexesToRemove.Add(1);
                        indexesToRemove.Add(2);
                    }

                    break;
                }

                if (GameSetup.Roll.Rolls.Count == 4)
                {
                    if (GameSetup.Roll.Rolls[0] + GameSetup.Roll.Rolls[1] == changeDistance)
                    {
                        indexesToRemove.Add(0);
                        indexesToRemove.Add(1);
                    }
                    else
                    if (GameSetup.Roll.Rolls[0] + GameSetup.Roll.Rolls[2] == changeDistance)
                    {
                        indexesToRemove.Add(0);
                        indexesToRemove.Add(2);
                    }
                    else
                    if (GameSetup.Roll.Rolls[0] + GameSetup.Roll.Rolls[3] == changeDistance)
                    {
                        indexesToRemove.Add(0);
                        indexesToRemove.Add(3);
                    }
                    else
                    if (GameSetup.Roll.Rolls[1] + GameSetup.Roll.Rolls[2] == changeDistance)
                    {
                        indexesToRemove.Add(1);
                        indexesToRemove.Add(2);
                    }
                    else
                    if (GameSetup.Roll.Rolls[1] + GameSetup.Roll.Rolls[3] == changeDistance)
                    {
                        indexesToRemove.Add(1);
                        indexesToRemove.Add(3);
                    }
                    else
                    if (GameSetup.Roll.Rolls[2] + GameSetup.Roll.Rolls[3] == changeDistance)
                    {
                        indexesToRemove.Add(2);
                        indexesToRemove.Add(3);
                    }
                    else
                    if (GameSetup.Roll.Rolls[0] + GameSetup.Roll.Rolls[1] + GameSetup.Roll.Rolls[2] == changeDistance)
                    {
                        indexesToRemove.Add(0);
                        indexesToRemove.Add(1);
                        indexesToRemove.Add(2);
                    }
                    else
                    if (GameSetup.Roll.Rolls[0] + GameSetup.Roll.Rolls[1] + GameSetup.Roll.Rolls[3] == changeDistance)
                    {
                        indexesToRemove.Add(0);
                        indexesToRemove.Add(1);
                        indexesToRemove.Add(3);
                    }
                    else
                    if (GameSetup.Roll.Rolls[0] + GameSetup.Roll.Rolls[2] + GameSetup.Roll.Rolls[3] == changeDistance)
                    {
                        indexesToRemove.Add(0);
                        indexesToRemove.Add(2);
                        indexesToRemove.Add(3);
                    }
                    else
                    if (GameSetup.Roll.Rolls[1] + GameSetup.Roll.Rolls[2] + GameSetup.Roll.Rolls[3] == changeDistance)
                    {
                        indexesToRemove.Add(1);
                        indexesToRemove.Add(2);
                        indexesToRemove.Add(3);
                    }
                    else
                    if (GameSetup.Roll.Rolls[0] + GameSetup.Roll.Rolls[1] + GameSetup.Roll.Rolls[2] + GameSetup.Roll.Rolls[3] == changeDistance)
                    {
                        indexesToRemove.Add(0);
                        indexesToRemove.Add(1);
                        indexesToRemove.Add(2);
                        indexesToRemove.Add(3);
                    }

                    break;
                }
            }
        }

        GameSetup.HistoryClass history = null;

        if (createHistory) // Create History
        {
            history = new GameSetup.HistoryClass();
            GameSetup.History.Add(history);
            history.checker = this;
            history.startColumn = columnStart;
            history.targetColumn = column;
        }

        if (indexesToRemove.Count > 0)
        {
            for (int j=0;j< indexesToRemove.Count;j++)
            {
                if (createHistory)
                    history.roll.Add(GameSetup.Roll.Rolls[indexesToRemove[j]]);
            }

            if (haveKickRoll)
            {
                for (int i = 0; i < indexesToRemove.Count; i++)
                {
                    if (GameSetup.Roll.Rolls.Contains(GameSetup.Roll.Rolls[indexesToRemove[i]]))
                    {
                        GameSetup.Roll.Rolls.RemoveAt(indexesToRemove[i]);
                    }
                    else
                    {
                        if (createHistory)
                            history.roll.Add(GameSetup.FindMaxRoll(GameSetup.Roll.Rolls));

                        GameSetup.Roll.Rolls.Remove(GameSetup.FindMaxRoll(GameSetup.Roll.Rolls));
                    }
                }
            }
            else
            {
                for (int i = indexesToRemove.Count - 1; i >= 0; i--)
                {
                    if (GameSetup.Roll.Rolls.Contains(GameSetup.Roll.Rolls[indexesToRemove[i]]))
                    {

                        GameSetup.Roll.Rolls.RemoveAt(indexesToRemove[i]);
                    }
                    else
                    {
                        if (createHistory)
                            history.roll.Add(GameSetup.FindMaxRoll(GameSetup.Roll.Rolls));

                        GameSetup.Roll.Rolls.Remove(GameSetup.FindMaxRoll(GameSetup.Roll.Rolls));
                    }
                }
            }
            
        }else
        {
            if (createHistory)
                history.roll.Add(GameSetup.FindMaxRoll(GameSetup.Roll.Rolls));

            GameSetup.Roll.Rolls.Remove(GameSetup.FindMaxRoll(GameSetup.Roll.Rolls));
        }
        
        if (createHistory && verify) GameSetup.VerifyMove(history); // Verify Move


        if (GameSetup.Roll.Rolls.Count > 4)
        {
            for (int i = GameSetup.Roll.Rolls.Count - 1; i > 3; i--)
            {
                GameSetup.Roll.Rolls.Remove(GameSetup.Roll.Rolls[i]);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        /*if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            int play = Random.Range(1, 4);
            if (play == 2) checkerPlaceAudio.Play();

            print("SS");
        }*/
    }
}