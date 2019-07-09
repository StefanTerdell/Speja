using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GraphData : MonoBehaviour
{
    [Header("Current")]
    public int NodeCount;
    public int EdgeCount;

    public bool _reset;

    [Header("Neo4j")]
    public string _neo4jQuery = "MATCH (n) OPTIONAL MATCH (n)-[r]-() RETURN n, r";
    public bool _addNeo4jNodes;
    [Header("Random")]
    public int _randomNodesToAdd;
    public bool _addRandomNodes;
    public int _randomNodesToRemove;
    public bool _removeRandomNodes;
    [Space(6)]
    public float _maxRandomEdges = 2;
    public float _leastRandomEdges = 1;
    public float _randomEdgeChance = .1f;

    public static List<NodeData> NodeDataSet => instance.nodeDataSet;
    public static List<EdgeData> EdgeDataSet => instance.edgeDataSet;
    public static event EventHandler OnGraphReset;
    public static event EventHandler OnNodeAdded;
    public static event EventHandler OnNodeRemoved;
    public static event EventHandler OnEdgeAdded;
    public static event EventHandler OnEdgeRemoved;
    static GraphData instance;

    void Awake()
    {
        instance = this;
    }

    List<NodeData> nodeDataSet = new List<NodeData>();
    List<EdgeData> edgeDataSet = new List<EdgeData>();

    void Start()
    {
        if (OnGraphReset != null)
            OnGraphReset(this, EventArgs.Empty);
    }

    void AddNeo4jNodesAndEdges()
    {
        var queryObject = Neo4jServer.QueryObject(_neo4jQuery);

        var neo4jNodes = new Dictionary<string, NodeData>();
        var neo4jEdges = new Dictionary<string, EdgeData>();

        foreach (var result in queryObject.results)
        {
            foreach (var data in result.data)
            {
                foreach (var neo4jNode in data.graph.nodes)
                {
                    if (!neo4jNodes.ContainsKey(neo4jNode.id))
                    {
                        neo4jNodes.Add(neo4jNode.id, new NodeData()
                        {
                            type = neo4jNode.labels[0],
                            name = neo4jNode.properties.name,
                            uuid = neo4jNode.properties.uuid,
                        });
                    }
                }

                foreach (var neo4jEdge in data.graph.relationships)
                {
                    if (!neo4jEdges.ContainsKey(neo4jEdge.id))
                    {
                        neo4jEdges.Add(neo4jEdge.id, new EdgeData()
                        {
                            type = neo4jEdge.type,
                            // uuid = edge.properties?.uuid,
                            from = neo4jNodes[neo4jEdge.startNode],
                            to = neo4jNodes[neo4jEdge.endNode]
                        });
                    }
                }
            }
        }

        nodeDataSet.AddRange(neo4jNodes.Values.ToList());
        edgeDataSet.AddRange(neo4jEdges.Values.ToList());

        if (OnGraphReset != null)
            OnGraphReset(null, EventArgs.Empty);
    }

    void Update()
    {
        NodeCount = nodeDataSet.Count;
        EdgeCount = edgeDataSet.Count;

        if (_addNeo4jNodes)
        {
            _addNeo4jNodes = false;

            AddNeo4jNodesAndEdges();
        }

        if (_addRandomNodes)
        {
            _addRandomNodes = false;

            AddRandomNodes(_randomNodesToAdd);
        }

        if (_removeRandomNodes)
        {
            _removeRandomNodes = false;

            RemoveRandomNodes(_randomNodesToRemove);
        }

        if (_reset)
        {
            _reset = false;

            if (OnGraphReset != null)
                OnGraphReset(null, EventArgs.Empty);
        }
    }

    NodeData AddNode(string uuid, string type, string name)
    {
        var node = new NodeData()
        {
            uuid = uuid,
            type = type,
            name = name
        };

        nodeDataSet.Add(node);

        if (OnNodeAdded != null)
            OnNodeAdded(node, EventArgs.Empty);

        return node;
    }

    EdgeData AddEdge(string type, NodeData from, NodeData to)
    {
        var edge = new EdgeData()
        {
            // uuid = uuid,
            type = type,
            from = from,
            to = to
        };

        edgeDataSet.Add(edge);

        if (OnEdgeAdded != null)
            OnEdgeAdded(edge, EventArgs.Empty);

        return edge;
    }

    void AddRandomNodes(float nodesToAdd)
    {
        for (int i = 0; i < nodesToAdd; i++)
        {
            var node = AddNode(Guid.NewGuid().ToString(), NodeTypes.GetRandomTypeName(), RandomString(10));

            AddRandomEdges(node);
        }
    }

    public static string RandomString(int length)
    {
        const string chars = "abcdefghijklmnopqrstuvwxyzåäö";
        return new string(Enumerable.Repeat(chars, length)
          .Select(s => s[UnityEngine.Random.Range(0, s.Length)]).ToArray());
    }

    void AddRandomEdges(NodeData node)
    {
        var otherNodes = nodeDataSet.Where(n => n != node).ToList();

        var edges = 0;
        for (var i = 0; i < _maxRandomEdges; i++)
        {
            if (otherNodes.Count == 0)
                break;

            if (UnityEngine.Random.value < _randomEdgeChance && edges >= _leastRandomEdges)
                continue;

            var otherNode = otherNodes[UnityEngine.Random.Range(0, otherNodes.Count)];

            otherNodes.Remove(otherNode);

            AddEdge(RandomString(5), node, otherNode);
            edges++;
        }
    }

    void RemoveRandomNodes(float nodesToRemove)
    {
        if (nodeDataSet.Count < nodesToRemove)
            nodesToRemove = nodeDataSet.Count;

        for (int i = 0; i < nodesToRemove; i++)
        {
            var node = nodeDataSet[UnityEngine.Random.Range(0, nodeDataSet.Count)];

            RemoveNode(node);
        }
    }

    void RemoveNode(NodeData node)
    {
        nodeDataSet.Remove(node);

        var edgeCount = edgeDataSet.Count;
        for (var i = 0; i < edgeCount; i++)
        {
            if (edgeDataSet[i].from == node || edgeDataSet[i].to == node)
            {
                RemoveEdge(edgeDataSet[i]);
                edgeCount--;
                i--;
            }
        }

        if (OnNodeRemoved != null)
            OnNodeRemoved(node, EventArgs.Empty);
    }

    void RemoveEdge(EdgeData edge)
    {
        edgeDataSet.Remove(edge);

        if (OnEdgeRemoved != null)
            OnEdgeRemoved(edge, EventArgs.Empty);
    }
}
