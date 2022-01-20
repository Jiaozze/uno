using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIOtherPlayer : MonoBehaviour
{
    public Text text_num;
    public Image bg;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init()
    {
        SetNum(0);
        SetTurn(false);
    }

    public void ShowDrawCards(int num)
    {
        
    }

    public void SetNum(int num)
    {
        text_num.text = " ÷≈∆£∫" + num;
    }

    internal void SetTurn(bool onTurn)
    {
        bg.color = onTurn ? Color.white: Color.gray;
    }
}
