using System.Collections.Generic;
using UnityEngine;

public class GraphInstantiator : MonoBehaviour
{
    public bool _3D;
    public GameObject _2DNodePrefab;
    public GameObject _3DNodePrefab;
    public GameObject _edgePrefab;
    public int _cameraLayer;
    public bool _useEmissiveMaterials;
    public Material[] _emissiveMaterials;
    public Material[] _unlitMaterials;
    public float _spawnRadius = 10;

    Dictionary<NodeData, Node> nodeTable = new Dictionary<NodeData, Node>();
    Dictionary<EdgeData, Edge> edgeTable = new Dictionary<EdgeData, Edge>();

    public static List<Node> Nodes = new List<Node>();
    public static List<Edge> Edges = new List<Edge>();

    public static event System.EventHandler OnGraphReset;
    public static event System.EventHandler OnNodeAdded;
    public static event System.EventHandler OnNodeRemoved;

    bool __3D;

    private void Update()
    {
        if (_3D != __3D)
        {
            __3D = _3D;
            SwitchProjection();
        }

        foreach (var edge in Edges)
            edge.UpdateLinerendererPositions();
    }

    private void OnEnable()
    {
        GraphData.OnGraphReset += RespawnAllHandler;
        GraphData.OnNodeAdded += InstantiateNodeHandler;
        GraphData.OnNodeRemoved += DestroyNodeHandler;
        GraphData.OnEdgeAdded += InstantiateEdgeHandler;
        GraphData.OnEdgeRemoved += DestroyEdgeHandler;
    }

    private void OnDisable()
    {
        GraphData.OnGraphReset -= RespawnAllHandler;
        GraphData.OnNodeAdded -= InstantiateNodeHandler;
        GraphData.OnNodeRemoved -= DestroyNodeHandler;
        GraphData.OnEdgeAdded -= InstantiateEdgeHandler;
        GraphData.OnEdgeRemoved -= DestroyEdgeHandler;
    }

    void SwitchProjection()
    {
        foreach (var node in Nodes)
        {
            ReinstantiateNode(node);
        }
        
        foreach (var edge in Edges)
        {
            edge.Set3D(_3D);
        }
    }

    void ClearSpawnedObjects()
    {
        foreach (var node in nodeTable.Values)
            node.DestroyGameObject();

        nodeTable.Clear();

        foreach (var edge in edgeTable.Values)
            edge.Destroy();

        edgeTable.Clear();
    }

    void RespawnAllHandler(object o, System.EventArgs e) => RespawnAll();
    void RespawnAll()
    {
        Camera.main.gameObject.layer = _cameraLayer;

        foreach (var nodeData in GraphData.Nodes)
        {
            if (nodeTable.ContainsKey(nodeData))
                DestroyNode(nodeData);

            InstantiateNode(nodeData);
        }

        foreach (var edgeData in GraphData.Edges)
        {
            if (edgeTable.ContainsKey(edgeData))
                DestroyEdge(edgeData);

            InstantiateEdge(edgeData);
        }

        if (OnGraphReset != null)
            OnGraphReset(null, System.EventArgs.Empty);
    }

    void InstantiateNodeHandler(object o, System.EventArgs e) => InstantiateNode(o as NodeData);
    void InstantiateNode(NodeData nodeData)
    {
        var type = NodeTypes.GetType(nodeData.type);
        
        var nodeObj = Instantiate(_3D ? _3DNodePrefab : _2DNodePrefab, (_3D ? Random.insideUnitSphere : (Vector3)Random.insideUnitCircle) * _spawnRadius, Quaternion.identity, transform);

        nodeObj.transform.localScale = Vector3.one * type.size;

        var rb = nodeObj.GetComponent<Rigidbody>();

        var renderers = nodeObj.GetComponentsInChildren<MeshRenderer>();

        foreach (var render in renderers)
        {
            if (_useEmissiveMaterials)
            {
                render.material = _emissiveMaterials[0];
                render.material.SetColor("_EmissionColor", type.color);
            }
            else
            {
                render.material = _unlitMaterials[0];
            }

            render.material.color = type.color;
        }

        var node = new Node(rb, renderers, nodeData.type);

        nodeTable.Add(nodeData, node);

        Nodes.Add(node);

        if (OnNodeAdded != null)
            OnNodeAdded(null, System.EventArgs.Empty);
    }

    void ReinstantiateNode(Node node)
    {
        var type = NodeTypes.GetType(node.type);

        var nodeObj = Instantiate(_3D ? _3DNodePrefab : _2DNodePrefab, new Vector3(node.position.x, node.position.y, _3D ? Random.value - .5f : 0), Quaternion.identity, transform);
        
        nodeObj.transform.localScale = Vector3.one * type.size;
        
        var rb = nodeObj.GetComponent<Rigidbody>();

        rb.isKinematic = node.isKinematic;

        var renderers = nodeObj.GetComponentsInChildren<MeshRenderer>();

        foreach (var render in renderers)
        {
            if (_useEmissiveMaterials)
            {
                render.material = _emissiveMaterials[0];
                render.material.SetColor("_EmissionColor", type.color);
            }
            else
            {
                render.material = _unlitMaterials[0];
            }

            render.material.color = type.color;
        }

        node.DestroyGameObject();

        node.SetComponents(rb, renderers);
    }

    void InstantiateEdgeHandler(object o, System.EventArgs e) => InstantiateEdge(o as EdgeData);
    void InstantiateEdge(EdgeData edgeData)
    {
        var edgeObj = Instantiate(_edgePrefab, Vector3.zero, Quaternion.identity, transform);

        var edge = new Edge(nodeTable[edgeData.from], nodeTable[edgeData.to], edgeObj.GetComponent<LineRenderer>(), _3D);

        edge.SetMaterial(_useEmissiveMaterials
            ? _emissiveMaterials[_emissiveMaterials.Length - 1]
            : _unlitMaterials[_unlitMaterials.Length - 1]);

        edgeTable.Add(edgeData, edge);
        Edges.Add(edge);
    }

    void DestroyNodeHandler(object o, System.EventArgs e) => DestroyNode(o as NodeData);
    void DestroyNode(NodeData nodeData)
    {
        var node = nodeTable[nodeData];
        Nodes.Remove(node);
        node.DestroyGameObject();
        nodeTable.Remove(nodeData);

        if (OnNodeRemoved != null)
            OnNodeRemoved(null, System.EventArgs.Empty);
    }

    void DestroyEdgeHandler(object o, System.EventArgs e) => DestroyEdge(o as EdgeData);
    void DestroyEdge(EdgeData edgeData)
    {
        var edge = edgeTable[edgeData];
        Edges.Remove(edge);
        edge.Destroy();
        edgeTable.Remove(edgeData);
    }
}