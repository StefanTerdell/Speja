using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class NodeType
{
    public string name;
    public Color color;
    public float size;
    public int layer;
}
public class NodeTypes : MonoBehaviour
{

    public List<NodeType> types = new List<NodeType>();

    static NodeTypes instance;
    private void Awake()
    {
        instance = this;
    }

    public static NodeType GetType(string name)
    {
        var type = instance.types.SingleOrDefault(t => t.name == name);

        if (type != null)
            return type;

        int highestLayer = 0, lowestLayer = 0;
        float averageSize = 0;

        foreach (var t in instance.types)
        {
            if (t.layer > highestLayer)
                highestLayer = t.layer;
            
            if (t.layer < lowestLayer)
                lowestLayer = t.layer;

            averageSize += t.size;
        }

        averageSize /= instance.types.Count();

        type = new NodeType() {
            name = name,
            layer = highestLayer > Mathf.Abs(lowestLayer) ? highestLayer : lowestLayer,
            color = Random.ColorHSV(),
            size = averageSize
        };

        instance.types.Add(type);

        return type;
    }

    public static List<NodeType> GetTypes()
    {
        return instance.types;
    }

    public static string GetRandomTypeName()
    {
        return instance.types[UnityEngine.Random.Range(0, instance.types.Count)].name;
    }
}