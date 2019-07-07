using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GraphData : MonoBehaviour
{
    public int NodeCount;
    public int EdgeCount;
    public bool _addNeo4jNodes;
    public bool _sendReset;
    public int _nodesToAdd;
    public bool _addNodes;
    public int _nodesToRemove;
    public bool _removeNodes;

    public float _maxEdges = 2;
    public float _leastEdges = 1;
    public float _edgeChance = .1f;

    public List<NodeData> nodes = new List<NodeData>();
    public List<EdgeData> edges = new List<EdgeData>();

    static GraphData instance;
    public static List<NodeData> Nodes => instance.nodes;
    public static List<EdgeData> Edges => instance.edges;

    public static event EventHandler OnGraphReset;
    public static event EventHandler OnNodeAdded;
    public static event EventHandler OnNodeRemoved;
    public static event EventHandler OnEdgeAdded;
    public static event EventHandler OnEdgeRemoved;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (OnGraphReset != null)
            OnGraphReset(this, EventArgs.Empty);
    }

    void AddNeo4jNodesAndEdges()
    {
        var queryObject = Neo4jServer.QueryObject("match (n) optional match (n)-[r]-() return n, r");

        var nodeDict = new Dictionary<string, NodeData>();
        var edgeDict = new Dictionary<string, EdgeData>();

        foreach (var result in queryObject.results)
        {
            foreach (var data in result.data)
            {
                foreach (var node in data.graph.nodes)
                {
                    if (!nodeDict.ContainsKey(node.id))
                    {
                        nodeDict.Add(node.id, new NodeData()
                        {
                            type = node.labels[0],
                            name = node.properties?.name,
                            uuid = node.properties?.uuid,
                        });
                    }
                }

                foreach (var edge in data.graph.relationships)
                {
                    if (!edgeDict.ContainsKey(edge.id))
                    {
                        edgeDict.Add(edge.id, new EdgeData()
                        {
                            type = edge.type,
                            uuid = edge.properties?.uuid,
                            from = nodeDict[edge.startNode],
                            to = nodeDict[edge.endNode]
                        });
                    }
                }
            }
        }

        nodes.AddRange(nodeDict.Values.ToList());
        edges.AddRange(edgeDict.Values.ToList());

        if (OnGraphReset != null)
            OnGraphReset(null, EventArgs.Empty);
    }

    void Update()
    {
        NodeCount = nodes.Count;
        EdgeCount = edges.Count;

        if (_addNeo4jNodes)
        {
            _addNeo4jNodes = false;

            AddNeo4jNodesAndEdges();
        }

        if (_addNodes)
        {
            _addNodes = false;

            AddRandomNodes(_nodesToAdd);
        }

        if (_removeNodes)
        {
            _removeNodes = false;

            RemoveRandomNodes(_nodesToRemove);
        }

        if (_sendReset)
        {
            _sendReset = false;

            if (OnGraphReset != null)
                OnGraphReset(null, EventArgs.Empty);
        }
    }

    NodeData AddNode(string type)
    {
        var node = new NodeData();

        node.type = type;

        nodes.Add(node);

        if (OnNodeAdded != null)
            OnNodeAdded(node, EventArgs.Empty);

        return node;
    }

    EdgeData AddEdge(NodeData from, NodeData to)
    {
        var edge = new EdgeData();

        edges.Add(edge);

        edge.from = from;
        edge.to = to;

        if (OnEdgeAdded != null)
            OnEdgeAdded(edge, EventArgs.Empty);

        return edge;
    }

    void AddRandomNodes(float nodesToAdd)
    {
        for (int i = 0; i < nodesToAdd; i++)
        {
            var node = AddNode(NodeTypes.GetRandomTypeName());

            AddRandomEdges(node);
        }
    }

    void AddRandomEdges(NodeData node)
    {
        var otherNodes = nodes.Where(n => n != node).ToList();

        var edges = 0;
        for (var i = 0; i < _maxEdges; i++)
        {
            if (otherNodes.Count == 0)
                break;

            if (UnityEngine.Random.value < _edgeChance && edges >= _leastEdges)
                continue;

            var otherNode = otherNodes[UnityEngine.Random.Range(0, otherNodes.Count)];

            otherNodes.Remove(otherNode);

            AddEdge(node, otherNode);
            edges++;
        }
    }

    void RemoveRandomNodes(float nodesToRemove)
    {
        if (nodes.Count < nodesToRemove)
            nodesToRemove = nodes.Count;

        for (int i = 0; i < nodesToRemove; i++)
        {
            var node = nodes[UnityEngine.Random.Range(0, nodes.Count)];

            RemoveNode(node);
        }
    }

    void RemoveNode(NodeData node)
    {
        nodes.Remove(node);

        var edgeCount = edges.Count;
        for (var i = 0; i < edgeCount; i++)
        {
            if (edges[i].from == node || edges[i].to == node)
            {
                RemoveEdge(edges[i]);
                edgeCount--;
                i--;
            }
        }

        if (OnNodeRemoved != null)
            OnNodeRemoved(node, EventArgs.Empty);
    }

    void RemoveEdge(EdgeData edge)
    {
        edges.Remove(edge);

        if (OnEdgeRemoved != null)
            OnEdgeRemoved(edge, EventArgs.Empty);
    }
}
