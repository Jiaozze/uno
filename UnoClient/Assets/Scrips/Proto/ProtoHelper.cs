using Google.Protobuf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

public static class ProtoHelper
{
    public static void OnReceiveMsg(string name, byte[] contont)
    {
        switch (name)
        {
            case "init_toc":
                init_toc init_toc = init_toc.Parser.ParseFrom(contont);
                List<Card> cards = new List<Card>();
                foreach (var unoCard in init_toc.Cards)
                {
                    Card card = new Card((int)unoCard.CardId, (int)unoCard.Color, (int)unoCard.Num, null);
                    Debug.LogError(string.Format("-----init_toc, CardId:{0}, color:{1}, num:{2}", unoCard.CardId, unoCard.Color, unoCard.Num));
                    cards.Add(card);
                }
                {
                    Debug.LogError("-----init_toc, PlayerNum:" + init_toc.PlayerNum);
                }
                //GameManager.Singleton.gameWindow.Invoke(new invokeDelegate());
                //OnGameStart.Invoke((int)init_toc.PlayerNum, cards);
                GameManager.Singleton.OnReceiveGameStart((int)init_toc.PlayerNum, cards);
                break;
            case "other_add_hand_card_toc":
                other_add_hand_card_toc other_add_hand_card_toc = other_add_hand_card_toc.Parser.ParseFrom(contont);
                Debug.LogError("-----other_add_hand_card_toc, PlayerId, Num:" + other_add_hand_card_toc.PlayerId + "," + other_add_hand_card_toc.Num);
                GameManager.Singleton.OnOtherDrawCards((int)other_add_hand_card_toc.PlayerId, (int)other_add_hand_card_toc.Num);
                break;
            case "draw_card_toc":
                draw_card_toc draw_card_toc = draw_card_toc.Parser.ParseFrom(contont);

                List<uno_card> uno_Cards = new List<uno_card>();
                foreach (var card in draw_card_toc.Card)
                {
                    uno_Cards.Add(card);
                    //GameManager.Singleton.OnPlayerDrawCards((int)card.CardId, (int)card.Color, (int)card.Num);
                }
                //Debug.LogError("-----draw_card_toc, CardId Color Num:" + card.CardId + "," + card.Color + "," + card.Num);
                GameManager.Singleton.OnPlayerDrawCards(uno_Cards);
                break;
            case "notify_turn_toc":
                notify_turn_toc notify_turn_toc = notify_turn_toc.Parser.ParseFrom(contont);
                Debug.LogError("-----notify_turn_toc, PlayerId Dir:" + notify_turn_toc.PlayerId + "," + notify_turn_toc.Dir);

                GameManager.Singleton.OnTurnTo((int)notify_turn_toc.PlayerId, notify_turn_toc.Dir);
                break;
            case "set_deck_num_toc":
                set_deck_num_toc set_deck_num_toc = set_deck_num_toc.Parser.ParseFrom(contont);
                Debug.LogError("-----set_deck_num_toc, Num:" + set_deck_num_toc.Num);

                GameManager.Singleton.OnDeckNumTo((int)set_deck_num_toc.Num);
                break;
            case "discard_card_toc":
                discard_card_toc discard_card_toc = discard_card_toc.Parser.ParseFrom(contont);
                Debug.LogError("-----discard_card_toc, PlayerId CardId Color Num WantColor:" + discard_card_toc.PlayerId + "," + discard_card_toc.Card.CardId + "," + discard_card_toc.Card.Color + "," + discard_card_toc.Card.Num + "," + discard_card_toc.WantColor);
                GameManager.Singleton.OnDisCard((int)discard_card_toc.PlayerId, (int)discard_card_toc.Card.CardId, (int)discard_card_toc.Card.Color, (int)discard_card_toc.Card.Num, (int)discard_card_toc.WantColor);
                break;
            case "discard_card_tos":
                discard_card_tos discard_card_tos = discard_card_tos.Parser.ParseFrom(contont);
                //Debug.LogError("-----discard_card_toc, PlayerId CardId Color Num WantColor:" + discard_card_tos.PlayerId + discard_card_tos.Card.CardId + discard_card_tos.Card.Color + discard_card_tos.Card.Num + discard_card_tos.WantColor);
                Debug.Log("---------------------discard_card_tos" + discard_card_tos.CardId);
                //GameManager.Singleton.OnDisCard((int)discard_card_tos.PlayerId, (int)discard_card_tos.Card.CardId, (int)discard_card_tos.Card.Color, (int)discard_card_tos.Card.Num, (int)discard_card_tos.WantColor);

                break;
            default:
                Debug.LogError("undefine proto:" + name);
                break;
        }
    }

    public static void SendDiscardMessage(int cardId, int wantColor)
    {
        discard_card_tos discard_Card_Tos = new discard_card_tos() {CardId = (uint)cardId, WantColor = (uint)wantColor };

        byte[] proto = discard_Card_Tos.ToByteArray();
        byte[] protoName = Encoding.UTF8.GetBytes("discard_card_tos");
        short nameLen = (short)protoName.Length;
        short len = (short)(protoName.Length + proto.Length + 2);

        List<byte> vs = new List<byte>() {(byte)(len>>8), (byte)len, (byte)(nameLen >> 8), (byte)nameLen, };
        foreach(var bt in protoName)
        {
            vs.Add(bt);
        }
        foreach(var bt in proto)
        {
            vs.Add(bt);
        }
        byte[] msg = vs.ToArray();
        //Debug.Log(BitConverter.ToString(msg, 0, msg.Length));
        NetWork.Send(msg);
    }
}
