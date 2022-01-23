using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager
{
    public int PlayerNum; // 游戏人数
    public bool dir;
    public GameWindow gameWindow;

    public Card topCard;
    public int topColor; // 黑色牌声明的颜色
    public int topCardCount;
    public int wantColor;

    public int onTurnPlayerId = -1;
    private List<int> selectCardIds = new List<int>();
    private int selectCard = -1;
    private List<PlayerOther> otherPlayers;
    private PlayerOther curPlayer;
    public List<Card> cards;
    //public 
    private static GameManager gameManager;
    private static bool inited = false;
    private int DeckNum;

    public static GameManager Singleton
    {
        get
        {
            if (gameManager == null && !inited)
            {
                gameManager = new GameManager();
                gameManager.Init();
            }
            return gameManager;
        }
    }
    private GameManager()
    {
    }

    public void Init()
    {
        if(!inited)
        {
            inited = true;
            otherPlayers = new List<PlayerOther>();
            GameObject windowGo = GameObject.Find("GameWindow");
            if (gameWindow == null && windowGo != null)
            {
                gameWindow = windowGo.GetComponent<GameWindow>();
                if (gameWindow == null)
                {
                    gameWindow = windowGo.AddComponent<GameWindow>();
                }
            }
            else
            {
                //TODO
            }
        }
    }

    public void InitOtherPlayer()
    {
        
        int num = PlayerNum - 1;
        if(otherPlayers != null && otherPlayers.Count > 0)
        {
            foreach(var other in otherPlayers)
            {
                other.Dispose();
            }
        }
        otherPlayers.Clear();
        List<UIOtherPlayer> uIOtherPlayers = gameWindow.CreatPlayers(num);
        for (int i = 0; i < num; i++)
        {
            PlayerOther player = new PlayerOther();
            player.Init("" + (i + 1) + "号玩家");
            player.SetUI(uIOtherPlayers[i]);
            otherPlayers.Add(player);
        }
    }

    public void SetDeckNum(int num)
    {
        DeckNum = num;
        gameWindow.SetDeckNum(num);
    }

    public void SetCardSelect(int id, bool select)
    {
        //if(select)
        //{
        //    selectCardIds.Add(id);
        //}
        //else
        //{
        //    selectCardIds.Remove(id);
        //}
        if (selectCard != -1 && id != selectCard)
        {
            Player.Singleton.cards[selectCard].cardUI.SetSelect(false);
            selectCard = -1;
        }

        if (select)
        {
            selectCard = id;
        }
    }

    public bool IsCardAvailable(int color, int num)
    {
        if (topCard == null)
        {
            Debug.LogError("牌堆没牌？");
            return true;
        }

        if (color == 0) //黑色牌
        {
            return true;
        }
        else //非黑色牌
        {
            if (color == topColor)
            {
                return true;
            }
            else if (topCard.num < 10 && topCard.num == num)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public Color GetColorById(int colorId)
    {
        Color color = Color.black;
        switch (colorId)
        {
            case 0:
                color = Color.black;
                break;
            case 1:
                color = Color.red;
                break;
            case 2:
                color = Color.green;
                break;
            case 3:
                color = Color.yellow;
                break;
            case 4:
                color = Color.blue;
                break;

            default:
                color = Color.gray;

                break;
        }
        return color;
    }

    public string GetCardInfo(int colorId, int num)
    {
        Color color1 = GetColorById(colorId);

        string nunStr = "";

        if (num < 10)
        {
            nunStr = "" + num;
        }
        else
        {
            switch (num)
            {
                case 10:
                    nunStr = "跳过";
                    break;
                case 11:
                    nunStr = "反向";
                    break;
                case 12:
                    nunStr = "+2";
                    break;
                case 13:
                    nunStr = "变色";
                    break;
                case 14:
                    nunStr = "+4";
                    break;
                default:
                    break;
            }
        }

            string colorStr = "<color=#" + ColorUtility.ToHtmlStringRGBA(color1) + ">" + num + "</color>";
        return colorStr;
    }

    public string GetCardsInfo(List<Card> cards)
    {
        string ret = "";
        foreach (var card in cards)
        {
            ret += GetCardInfo(card.color, card.num);
            ret += ",";
        }
        return ret;
    }

    #region 服务器消息处理


    public void OnReceiveGameStart(int player_num, List<Card> cards)
    {
        SoundManager.Singleton.PlayMusic(Music.Play);
        PlayerNum = player_num;
        if(this.cards != null && cards.Count > 0)
        {
            foreach(var card in cards)
            {
                GameObject.Destroy(card.cardUI);
            }
        }
        this.cards = cards;

        gameWindow.Init();

        gameWindow.AddMsg(string.Format("你摸了{0}张牌, {1}", cards.Count, GetCardsInfo(cards)));
        //gameWindow.Invoke("Init", 0.1f);
        
    }

    public void OnPlayerDrawCards(List<uno_card> cards)
    {
        string cardInfo = "";
        foreach(var card in cards)
        {
            Player.Singleton.DrawCards((int)card.CardId, (int)card.Color, (int)card.Num);
            cardInfo += GetCardInfo((int)card.Color, (int)card.Num) + ",";

        }
        DeckNum = DeckNum - 1;
        SetDeckNum(DeckNum);

        gameWindow.AddMsg(string.Format("你摸了{0}张牌; {1}", cards.Count, cardInfo));

    }

    public void OnOtherDrawCards(int id, int num)
    {
        otherPlayers[id - 1].DrawCard(num);
        DeckNum = DeckNum - num;
        SetDeckNum(DeckNum);
        gameWindow.AddMsg(string.Format("{0}号玩家摸了{1}张牌", id, num));
    }

    public void OnTurnTo(int id, bool dir)
    {
        if(onTurnPlayerId > 0)
        {
            otherPlayers[onTurnPlayerId -1].SetTurn(false);
        }
        onTurnPlayerId = id;
        if (onTurnPlayerId > 0)
        {
            otherPlayers[onTurnPlayerId -1].SetTurn(true);
        }

        this.dir = dir;
        if (id == 0)
        {
            Player.Singleton.SetTurn(true);
        }
        else
        {
            Player.Singleton.SetTurn(false);
        }
        gameWindow.SetTurn();
        
    }

    public void OnDeckNumTo(int num)
    {
        DeckNum = num;
        SetDeckNum(num);
    }

    //public void OnDisCard(int playerId, List<uno_card> uno_Cards)
    //{
    //    if(uno_Cards.Count < 1)
    //    {
    //        Debug.LogError("OnDisCard num error");
    //        return;
    //    }

    //    int count = uno_Cards.Count;
    //    int id = (int)uno_Cards[0].CardId;
    //    int color = (int)uno_Cards[0].Color;
    //    int num = (int)uno_Cards[0].Num;

    //    if(topCard == null)
    //    {
    //        topCard = new Card();
    //        topCard.SetUI(gameWindow.topCard);
    //    }
    //    topCard.SetInfo(id, color, num);
    //    topCardCount = count;

    //    if (playerId == 0)
    //    {
    //        //if(uno_Cards.Count > 1)
    //        //{
    //        //    List<int> ids = new List<int>();

    //        //}
    //        //else
    //        //{
    //        //    var card = uno_Cards[0];
    //        //    Player.Singleton.DisCard((int)card.CardId);
    //        //}
    //        foreach (var card in uno_Cards)
    //        {
    //            Player.Singleton.DisCard((int)card.CardId);
    //        }
    //    }
    //    else
    //    {
    //        otherPlayers[playerId - 1].DisCard(count);
    //    }
    //}

    public void OnDisCard(int playerId, int id, int color, int num, int colorEx)
    {
        if (topCard == null)
        {
            topCard = new Card();
            topCard.SetInfo(id, color, num);
            topCard.SetUI(gameWindow.topCard);
        }
        else
        {
            topCard.SetInfo(id, color, num);
        }
        topCardCount = 1;
        if (color == 0)
        {
            topColor = colorEx;
        }
        else
        {
            topColor = color;
        }

        if (playerId == 0)
        {
            Player.Singleton.DisCard(id);
        }
        else if (playerId > 999)
        {

        }
        else
        {
            otherPlayers[playerId - 1].DisCard(1);
        }

        gameWindow.RefreshColor();

        string log = "";
        if (playerId > 999)
        {
            log = "从牌堆顶翻出一张牌：{0}";
        }
        else if (playerId == 0)
        {
            log = "你打出了一张牌：{0}";
        }
        else
        {
            log = "" + playerId + "号玩家打出一张牌：{0}";
        }
        gameWindow.AddMsg(string.Format(log, GetCardInfo(color, num)));
    }
    #endregion


    #region
    public void Discard()
    {
        if (selectCard == -1)
        {
            return;
        }
        Card card = Player.Singleton.cards[selectCard];

        if (IsCardAvailable(card.color, card.num))
        {
            if (card.color == 0 && wantColor == 0)
            {
                gameWindow.ShowColorSelect();
                return;
                //ProtoHelper.SendDiscardMessage(card.id, UnityEngine.Random.Range(1, 5));
            }
            else
            {
                ProtoHelper.SendDiscardMessage(card.id, wantColor);
                wantColor = 0;
            }
        }
        Player.Singleton.cards[selectCard].cardUI.SetSelect(false);
        selectCard = -1;
        //TODO
        //OnDisCard(0, card.id, card.color, card.num);
    }

    public void DrawCard()
    {
        ProtoHelper.SendDiscardMessage(0, 0);

        //OnPlayerDrawCards(3, 3, 3);
    }

    #endregion
}
