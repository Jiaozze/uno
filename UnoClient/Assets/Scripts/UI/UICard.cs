using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class UICard : MonoBehaviour
{
    public Transform transContainer;
    public Image image;
    public int id = -1;
    public bool handCard = false;
    public bool isSelect = false; 
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    UICard(int id, int color, int num)
    {

    }

    public void SetId(int id)
    {

    }

    public void SetImg(int id, int color, int num)
    {
        this.id = id;
        string path = "Assets/Image/icon/";

        Sprite sprite = Resources.Load("icon/" + color + "_" + num, typeof(Sprite)) as Sprite;
        image.overrideSprite = sprite;
        //Addressables.LoadAssetAsync<Sprite>(path + color + "_" + num + ".png").Completed += (r) =>
        //{
        //    if (r.Result != null)
        //    {
        //        image.overrideSprite = r.Result;
        //    }
        //};

    }

    public void SetHand(bool isHand)
    {
        handCard = isHand;
        //transform.parent = GameManager.Singleton.gameWindow.transCards;
        GameManager.Singleton.gameWindow.InsertCard(this);
    }

    public void SetSelect(bool select)
    {
        if(select)
        {
            transContainer.localPosition = new Vector3(0, 20);
        }
        else
        {
            transContainer.localPosition = Vector3.zero;
        }
        isSelect = select;
    }

    public void OnClick()
    {
        if(handCard)
        {
            GameManager.Singleton.SetCardSelect(id, !isSelect);
            SetSelect(!isSelect);
        }
    }
}
