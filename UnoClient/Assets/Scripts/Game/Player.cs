using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : PlayerBase
{
    public Dictionary<int,Card> cards = new Dictionary<int, Card>();
    private static Player player;
    private bool onTurn;
    public static Player Singleton
    {
        get { 
            if (player == null) 
            {
                player = new Player();
            }
            else
            {
            }
            return player;
        }
    }
    private Player()
    {
    }

    public void InitCards()
    {
        List<Card> cards = GameManager.Singleton.cards;
        this.cards.Clear();
        if(cards != null && cards.Count > 0)
        {
            List<UICard> uICards = CardFactory.GetUICards(cards.Count);
            for(int i = 0; i < cards.Count; i++)
            {
                cards[i].SetUI(uICards[i]);
                uICards[i].SetHand(true);
                this.cards.Add(cards[i].id, cards[i]);
            }
        }
        else
        {
            this.cards = new Dictionary<int, Card>();
            Debug.Log("≥ı º ÷≈∆Œ™ø’");
        }
    }

    public void DrawCards(int id, int color, int num)
    {
        UICard ui = CardFactory.GetCardUI();
        Card card = new Card(id, color, num, ui);
        ui.SetHand(true);
        cards.Add(id, card);
        GameManager.Singleton.gameWindow.ShowDrawCard(0, ui);
    }

    public void DisCard(int cardId)
    {
        cards[cardId].OnDisCard();
        cards.Remove(cardId);
    }

    public void SetTurn(bool isTurn)
    {
        if(onTurn == isTurn)
        {
            return;
        }

        if(isTurn)
        {
            GameManager.Singleton.gameWindow.ShowButtons(true);
        }
        else
        {
            GameManager.Singleton.gameWindow.ShowButtons(false);
        }
        onTurn = isTurn;
    }
}
