using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card
{
    public int id = -1;
    public int color;
    public int num;
    public UICard cardUI;

    public Card()
    { 
    }

    public Card(int id, int color, int num, UICard uICard)
    {
        this.id = id;
        this.color = color;
        this.num = num;
        this.cardUI = uICard;
        if (this.cardUI != null)
        {
            this.cardUI.SetImg(id, color, num);
        }
    }

    public void SetUI(UICard uICard)
    {
        if(this.cardUI != null)
        {
            GameObject.Destroy(uICard.gameObject);
            return;
        }

        this.cardUI = uICard;
        if (this.cardUI != null)
        {
            this.cardUI.SetImg(id, color, num);
        }
    }

    public void SetInfo(int id, int color, int num)
    {
        this.id = id;
        this.color = color;
        this.num = num;
        if (this.cardUI != null)
        {
            this.cardUI.SetImg(id, color, num);
        }
    }

    public void OnDisCard()
    {
        if(cardUI != null)
        {
            GameObject.Destroy(cardUI.gameObject);
        }
    }
}
