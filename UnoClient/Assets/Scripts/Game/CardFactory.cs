using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.AddressableAssets;

public class CardFactory
{
    private static GameObject CardGo;

    public static void Init()
    {
        CardGo = Resources.Load("Prefabs/Card") as GameObject;
        //Addressables.LoadAssetAsync<GameObject>("UI/Card.prefab").Completed += (r) =>
        //{
        //    if (r.Result != null)
        //    {
        //        Debug.Log("CardFactory Init");
        //        CardGo = r.Result;
        //        CardGo.SetActive(false);
        //    }
        //};
    }
    public static UICard GetCardUI()
    {
        GameObject go = GameObject.Instantiate(CardGo);
        go.SetActive(true);
        UICard uICard = go.GetComponent<UICard>();
        return uICard;
    }

    public static List<UICard> GetUICards(int n)
    {
        List<UICard> ret = new List<UICard>();
        for (int i = 0; i < n; i++)
        {
            GameObject go = GameObject.Instantiate(CardGo);
            go.SetActive(true);
            var card = go.GetComponent<UICard>();
            ret.Add(card);
        }

        return ret;
    }
}
