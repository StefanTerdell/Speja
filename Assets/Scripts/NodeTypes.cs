using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NodeTypes : MonoBehaviour
{
    [System.Serializable]
    public class Type {
        public string name = "Unassigned";
        public Color color = Color.grey;
        [ColorUsage(true, true)]
        public Color emissionColor = Color.red;
        public float size = 1;
    }

    public List<Type> types = new List<Type>();

    static NodeTypes instance;
    private void Awake() {
        instance = this;
    }

    public static Type GetType(string name)
    {
        return instance.types.SingleOrDefault(t => t.name == name) ?? new Type();
    }

    public static List<Type> GetTypes()
    {
        return instance.types;
    }

    public static string GetRandomTypeName()
    {
        return instance.types[UnityEngine.Random.Range(0, instance.types.Count)].name;
    }
}