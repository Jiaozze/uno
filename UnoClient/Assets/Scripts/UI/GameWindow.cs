using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameWindow : MonoBehaviour
{
    public GridLayoutGroup cardsGrid;
    public GameObject buttons;
    public Text TextDeckNum;
    public Transform otherPlayerTrans;
    public GameObject otherPlayerFab;
    public RectTransform transCards;
    public UICard topCard;
    public Image imgBg;
    public Text TextLog;
    public Text TextTip;
    public GameObject EnterUI;
    public GameObject WantColorUI;
    public Button color1;
    public Button color2;
    public Button color3;
    public Button color4;
    public Transform arrow;
    // Start is called before the first frame update
    void Start()
    {
        EnterUI.SetActive(true);
        WantColorUI.SetActive(false);
        TextTip.gameObject.SetActive(false);
        color1.onClick.AddListener(() => OnClickWantColor(1));
        color2.onClick.AddListener(() => OnClickWantColor(2));
        color3.onClick.AddListener(() => OnClickWantColor(3));
        color4.onClick.AddListener(() => OnClickWantColor(4));
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void InitCards()
    {
        if (cardsGrid.transform.childCount != 0)
        {
            for (int i = cardsGrid.transform.childCount - 1; i >= 0; i--)
            {
                GameObject.DestroyImmediate(cardsGrid.transform.GetChild(i).gameObject);
            }
        }
    }

    public void OnClickDrawCard()
    {
        GameManager.Singleton.DrawCard();
    }

    public void OnClickDiscard()
    {
        GameManager.Singleton.Discard();
    }

    public void ShowButtons(bool show)
    {
        buttons.SetActive(show);
    }

    public void ShowDrawCard(int v, UICard ui = null)
    {
        if (ui != null)
        {
            ui.transform.parent = cardsGrid.transform;
        }

    }

    public List<UIOtherPlayer> CreatPlayers(int v)
    {
        List<UIOtherPlayer> ret = new List<UIOtherPlayer>();
        for (int i = 0; i < v; i++)
        {
            GameObject go = GameObject.Instantiate(otherPlayerFab, otherPlayerTrans);
            go.SetActive(true);
            var other = go.GetComponent<UIOtherPlayer>();
            ret.Add(other);
        }

        return ret;
    }

    internal void SetEnterUI(bool open)
    {
        EnterUI.SetActive(open);
    }

    public void SetDeckNum(int num)
    {
        TextDeckNum.text = "" + num;
    }

    public void Init()
    {
        InitCards();
        GameManager.Singleton.InitOtherPlayer();
        Player.Singleton.InitCards();
    }

    public void ShowColorSelect()
    {
        WantColorUI.SetActive(true);
    }

    public void OnClickWantColor(int id)
    {
        WantColorUI.SetActive(false);
        GameManager.Singleton.wantColor = id;
        GameManager.Singleton.Discard();
    }

    public void RefreshOtherPlayers()
    {

    }

    public void RefreshPlayerCards()
    {

    }

    internal void RefreshColor()
    {
        switch (GameManager.Singleton.topColor)
        {
            case 1:
                imgBg.color = Color.red;
                break;
            case 2:
                imgBg.color = Color.green;
                break;
            case 3:
                imgBg.color = Color.yellow;
                break;
            case 4:
                imgBg.color = Color.blue;
                break;

            default:
                imgBg.color = Color.gray;

                break;
        }
    }

    internal void AddMsg(string newLog)
    {
        string log = TextLog.text;
        log = log + "\n" + newLog;
        TextLog.text = log;
    }

    public void InsertCard(UICard uICard)
    {
        int index = 0;
        uICard.transform.parent = transCards;
        for (int i = 0; i < transCards.childCount; i++)
        {
            int cardId = transCards.GetChild(i).GetComponent<UICard>().id;
            if (cardId > uICard.id)
            {
                uICard.transform.SetSiblingIndex(index);
                return;
            }
            index++;
        }
        float space = transCards.sizeDelta.x / transCards.childCount - cardsGrid.cellSize.x;
        space = space > 0 ? 0 : space;
        cardsGrid.spacing = new Vector2(space, 0);
    }

    public void ShowTip(string tip)
    {
        TextTip.text = tip;
        TextTip.gameObject.SetActive(true);
        StartCoroutine(CloseTip());
    }

    private IEnumerator CloseTip()
    {
        yield return new WaitForSeconds(2);
        TextTip.gameObject.SetActive(false);
        yield break;
    }

    internal void SetTurn()
    {
        arrow.localRotation = Quaternion.Euler(GameManager.Singleton.dir ? 180:0, 0f, 90);
    }

    //private Ie
}
