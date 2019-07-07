using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ForceNodeSpawner : MonoBehaviour
{
    public class Node
    {
        public Rigidbody rb;
        public List<Edge> edges = new List<Edge>();
        public List<Node> disconnected = new List<Node>();

        public class Edge
        {
            public LineRenderer lr;
            public Node node;

            public Edge(Node node, LineRenderer lr)
            {
                this.node = node;
                this.lr = lr;
            }
        }

        public Node(Rigidbody rb)
        {
            this.rb = rb;
        }
    }
    public int _nodes = 10;
    public float _edgeChance = .3f;
    public GameObject _nodePrefab, _edgePrefab;
    public float _force = 1;
    public bool _repel, _attract, _connect;

    [Header("buttons")]
    public bool b_update;

    List<Node> nodes;

    void Start()
    {
        nodes = new List<Node>();
        Spawn();
    }

    public void Spawn()
    {
        foreach (var node in nodes)
        {
            Destroy(node.rb.gameObject);
            foreach (var edge in node.edges)
            {
                Destroy(edge.lr.gameObject);
            }
        }

        nodes.Clear();

        for (int i = 0; i < _nodes; i++)
        {
            var nodeObject = Instantiate(_nodePrefab, Random.insideUnitCircle * 10, Quaternion.identity, transform);

            var node = new Node(nodeObject.GetComponent<Rigidbody>());

            foreach (var otherNode in nodes)
                if (_connect && Random.value < _edgeChance)
                    node.edges.Add(new Node.Edge(otherNode, Instantiate(_edgePrefab, Vector3.zero, Quaternion.identity, transform).GetComponent<LineRenderer>()));
                else
                    node.disconnected.Add(otherNode);

            nodes.Add(node);
        }
    }

    void Update()
    {
        if (b_update)
        {
            b_update = false;
            Spawn();
        }

        foreach (var node in nodes)
        {
            foreach (var edge in node.edges)
            {
                if (_attract)
                {

                    var diff = node.rb.position - edge.node.rb.position;
                    var dist = diff.magnitude;
                    var f = Mathf.Log(dist / 3) * _force;

                    edge.node.rb.AddForce(diff.normalized * f * Time.deltaTime);
                }

                edge.lr.SetPositions(new[] { node.rb.position, edge.node.rb.position });
            }

            if (_repel)
                foreach (var otherNode in node.disconnected)
                {
                    var diff = otherNode.rb.position - node.rb.position;

                    otherNode.rb.AddForce(diff.normalized * Mathf.Max(10 - diff.sqrMagnitude, 0) * _force * Time.deltaTime);
                }
        }
    }
}
