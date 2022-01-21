using Google.Protobuf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

public static class ProtoHelper
{
    public static void OnReceiveMsg(int id, byte[] contont)
    {
        if (GetIdFromProtoName("init_toc") == id)
        {
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
        }
        else if (GetIdFromProtoName("other_add_hand_card_toc") == id)
        {
            other_add_hand_card_toc other_add_hand_card_toc = other_add_hand_card_toc.Parser.ParseFrom(contont);
            Debug.LogError("-----other_add_hand_card_toc, PlayerId, Num:" + other_add_hand_card_toc.PlayerId + "," + other_add_hand_card_toc.Num);
            GameManager.Singleton.OnOtherDrawCards((int)other_add_hand_card_toc.PlayerId, (int)other_add_hand_card_toc.Num);
        }
        else if (GetIdFromProtoName("draw_card_toc") == id)
        {
            draw_card_toc draw_card_toc = draw_card_toc.Parser.ParseFrom(contont);

            List<uno_card> uno_Cards = new List<uno_card>();
            foreach (var card in draw_card_toc.Card)
            {
                uno_Cards.Add(card);
                //GameManager.Singleton.OnPlayerDrawCards((int)card.CardId, (int)card.Color, (int)card.Num);
            }
            //Debug.LogError("-----draw_card_toc, CardId Color Num:" + card.CardId + "," + card.Color + "," + card.Num);
            GameManager.Singleton.OnPlayerDrawCards(uno_Cards);
        }
        else if (GetIdFromProtoName("notify_turn_toc") == id)
        {
            notify_turn_toc notify_turn_toc = notify_turn_toc.Parser.ParseFrom(contont);
            Debug.LogError("-----notify_turn_toc, PlayerId Dir:" + notify_turn_toc.PlayerId + "," + notify_turn_toc.Dir);

            GameManager.Singleton.OnTurnTo((int)notify_turn_toc.PlayerId, notify_turn_toc.Dir);
        }
        else if (GetIdFromProtoName("set_deck_num_toc") == id)
        {
            set_deck_num_toc set_deck_num_toc = set_deck_num_toc.Parser.ParseFrom(contont);

            Debug.LogError("-----set_deck_num_toc, Num:" + set_deck_num_toc.Num);

            GameManager.Singleton.OnDeckNumTo((int)set_deck_num_toc.Num);
        }
        else if (GetIdFromProtoName("discard_card_toc") == id)
        {
            discard_card_toc discard_card_toc = discard_card_toc.Parser.ParseFrom(contont);
            Debug.LogError("-----discard_card_toc, PlayerId CardId Color Num WantColor:" + discard_card_toc.PlayerId + "," + discard_card_toc.Card.CardId + "," + discard_card_toc.Card.Color + "," + discard_card_toc.Card.Num + "," + discard_card_toc.WantColor);
            GameManager.Singleton.OnDisCard((int)discard_card_toc.PlayerId, (int)discard_card_toc.Card.CardId, (int)discard_card_toc.Card.Color, (int)discard_card_toc.Card.Num, (int)discard_card_toc.WantColor);
        }
        else if (GetIdFromProtoName("discard_card_tos") == id)
        {
            discard_card_tos discard_card_tos = discard_card_tos.Parser.ParseFrom(contont);
            //Debug.LogError("-----discard_card_toc, PlayerId CardId Color Num WantColor:" + discard_card_tos.PlayerId + discard_card_tos.Card.CardId + discard_card_tos.Card.Color + discard_card_tos.Card.Num + discard_card_tos.WantColor);
            Debug.Log("---------------------discard_card_tos" + discard_card_tos.CardId);
            //GameManager.Singleton.OnDisCard((int)discard_card_tos.PlayerId, (int)discard_card_tos.Card.CardId, (int)discard_card_tos.Card.Color, (int)discard_card_tos.Card.Num, (int)discard_card_tos.WantColor);

        }
        else
        {
            Debug.LogError("undefine proto:" + id);
        }
    }

    public static void SendDiscardMessage(int cardId, int wantColor)
    {
        discard_card_tos discard_Card_Tos = new discard_card_tos() {CardId = (uint)cardId, WantColor = (uint)wantColor };

        byte[] proto = discard_Card_Tos.ToByteArray();
        int protoId = GetIdFromProtoName("discard_card_tos");
        short len = (short)(proto.Length + 2);
        short shortId = (short)protoId;
        List<byte> vs = new List<byte>() {(byte)len, (byte)(len >> 8), (byte)shortId, (byte)(shortId >> 8), };

        foreach(var bt in proto)
        {
            vs.Add(bt);
        }
        byte[] msg = vs.ToArray();
        //Debug.Log(BitConverter.ToString(msg, 0, msg.Length));
        NetWork.Send(msg);
    }

    private static Dictionary<string, int> proto_name_id = new Dictionary<string, int>();
    public static int GetIdFromProtoName(string protoName)
    {
        if(proto_name_id.ContainsKey(protoName))
        {
            return proto_name_id[protoName];
        }
        else
        {
            ushort hash = 0;
            foreach (var c in protoName)
            {
                ushort ch = (ushort)c;
                hash = (ushort)(hash + ((hash) << 5) + ch + (ch << 7));
            }
            proto_name_id.Add(protoName, (int)hash);
            Debug.LogError("protoName:" + hash);
            return (int)hash;
        }
    }

}
