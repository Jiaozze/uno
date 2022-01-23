using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIOtherPlayer : MonoBehaviour
{
    public Text textNum;
    public Text textName;
    public Image bg;
    public Role model;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init(string name)
    {
        SetNum(0);
        SetTurn(false);
        textName.text = name;
        model.RandomAll();
    }

    public void ShowDrawCards(int num)
    {
        
    }

    public void SetNum(int num)
    {
        textNum.text = "" + num;
    }

    internal void SetTurn(bool onTurn)
    {
        bg.color = onTurn ? Color.white: Color.gray;
    }
}
