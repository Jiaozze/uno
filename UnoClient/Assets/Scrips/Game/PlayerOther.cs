using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOther : PlayerBase
{
    SkinnedMeshRenderer skinnedMeshRenderer;
    private UIOtherPlayer ui;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetUI(UIOtherPlayer ui)
    {
        this.ui = ui;
    }

    public void DrawCard(int num)
    {
        cardNum = cardNum + num;
        ui.SetNum(cardNum);
        ui.ShowDrawCards(num);
    }

    internal void DisCard(int num)
    {
        cardNum = cardNum - num;
        ui.SetNum(cardNum);
    }

    internal void SetTurn(bool onTurn)
    {
        if(ui != null)
        {
            ui.SetTurn(onTurn);
        }
    }
}
