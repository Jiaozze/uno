using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Role : MonoBehaviour
{
    private const string BODY_PATH = "Role/Body/";
    private const string HAIR_PATH = "Role/Hair/";
    private const string FACE_PATH = "Role/Face/";
    private static List<string> BODY_IDs = new List<string>() { "1", "2" };
    private static List<string> HAIR_IDs = new List<string>() { "1", "2"};
    private static List<string> FACE_IDs = new List<string>() { "1", "2", "3", "4" };

    public SpriteRenderer body;
    public SpriteRenderer hair;
    public SpriteRenderer face;
    public GameObject acc;
    public GameObject hairBack;

    //public int bodyId;
    //public int hairId;
    // 随机一个模型
    public void RandomAll()
    {
        RandomPart(ModelPart.Body);
        RandomPart(ModelPart.Hair);
        RandomPart(ModelPart.Face);
        RandomPart(ModelPart.Acc);
        RandomPart(ModelPart.HairBack);
    }

    public void RandomPart(ModelPart modelPart)
    {
        int rand = 0;
        string path = "";
        SpriteRenderer part = null;

        switch(modelPart)
        {
            case ModelPart.Body:
                rand = Random.Range(0, BODY_IDs.Count);
                path = BODY_PATH + BODY_IDs[rand];
                part = body;
                break;
            case ModelPart.Hair:
                rand = Random.Range(0, HAIR_IDs.Count);
                path = HAIR_PATH + HAIR_IDs[rand];
                part = hair;
                break;
            case ModelPart.Face:
                rand = Random.Range(0, FACE_IDs.Count);
                path = FACE_PATH + FACE_IDs[rand];
                part = face;
                break;
            case ModelPart.Acc:
                rand = Random.Range(0, 2);
                acc?.SetActive(rand == 0);
                return;
            case ModelPart.HairBack:
                rand = Random.Range(0, 2);
                hairBack?.SetActive(rand == 0);
                return;
        }
        if (part != null)
        {
            Sprite Sprite = Resources.Load(path, typeof(Sprite)) as Sprite;
            part.sprite = Sprite;
        }
    }
}

public enum ModelPart
{
    Body = 1,
    Hair = 2,
    Face = 3,
    Acc = 4, //头饰
    HairBack = 5, //
}
