using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;
using System.Threading.Tasks;

public class FruchtermanReingoldSpawner : MonoBehaviour
{
    [Header("Node Options")]
    public int _nodes = 100;
    public float _maxEdges = 2;
    public float _edgeChance = .1f;
    public float _medianSize = 1;
    public float _sizeRange;

    [Header("Spawning Options")]
    public bool _3D;
    public GameObject _2DNodePrefab;
    public GameObject _3DNodePrefab;
    public GameObject _edgePrefab;
    public int _cameraLayer;
    public bool _useEmissiveMaterials;
    public Material[] _emissiveMaterials;
    public Material[] _unlitMaterials;
    public float _spawnRadius = 10;
    public bool _respawn;

    [Header("Frucherman-Reingold Options")]
    [Range(0.01f, 10)]
    public float _dispersion = 1;
    [Range(0.1f, 50)]
    public float _speed = 20;
    public float _proximitySqr = 1;
    public bool _applyForces;


    public class Node
    {
        public Vector3 position => rigidbody.position;

        Vector3 unappliedForce;
        Rigidbody rigidbody;
        MeshRenderer[] meshRenderers;

        public Node(Rigidbody rigidbody, MeshRenderer[] meshRenderers)
        {
            this.rigidbody = rigidbody;
            this.meshRenderers = meshRenderers;
        }

        public void SetMaterial(Material mat)
        {
            foreach (var meshRenderer in meshRenderers)
                meshRenderer.material = mat;
        }

        public void AddForce(Vector3 force)
        {
            unappliedForce += force;
        }

        public void ApplyForce()
        {
            rigidbody.AddForce(unappliedForce);
            unappliedForce = Vector3.zero;
        }

        public void Destroy()
        {
            GameObject.Destroy(rigidbody.gameObject);
        }
    }

    public class Edge
    {
        LineRenderer lineRenderer;
        public Node from;
        public Node to;
        Vector3 lineRendererOffset;
        public Edge(Node from, Node to, LineRenderer lr, bool _3D)
        {
            this.from = from;
            this.to = to;
            this.lineRenderer = lr;

            if (_3D)
            {
                lineRenderer.alignment = LineAlignment.View;
                lineRendererOffset = Vector3.zero;
            }
            else
            {
                lineRenderer.alignment = LineAlignment.TransformZ;
                lineRendererOffset = new Vector3(0, 0, 0.01f);
            }
        }

        public void SetMaterial(Material mat)
        {
            lineRenderer.material = mat;
        }

        public void UpdateLinerendererPositions()
        {
            lineRenderer.SetPositions(new[] { from.position + lineRendererOffset, to.position + lineRendererOffset });
        }

        public void Destroy()
        {
            GameObject.Destroy(lineRenderer.gameObject);
        }
    }

    bool useEmissiveMaterials;
    List<Node> nodes;
    List<Edge> edges;
    NativeArray<Vector3> nodePositions;
    NativeArray<Vector3> nodeForces;

    void OnEnable()
    {
        nodes = new List<Node>();
        edges = new List<Edge>();

        Spawn();
    }

    private void OnDisable()
    {
        nodePositions.Dispose();
        nodeForces.Dispose();
    }


    void Update()
    {
        if (_respawn)
        {
            _respawn = false;
            Spawn();
        }

        foreach (var edge in edges)
        {
            edge.UpdateLinerendererPositions();

            if (_applyForces)
            {
                var diff = edge.from.position - edge.to.position;

                if (diff.sqrMagnitude > _proximitySqr)
                {
                    edge.from.AddForce(diff * 2 / _dispersion * _speed * -1);
                    edge.to.AddForce(diff * 2 / _dispersion * _speed);
                }
            }
        }

        if (_applyForces)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                nodePositions[i] = nodes[i].position;
            }

            for (int i = 0; i < nodes.Count; i++)
            {
                (new PushJob(_dispersion, _speed, i, nodePositions[i], nodePositions, nodeForces).Schedule(nodes.Count, 100)).Complete();
            }

            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].AddForce(nodeForces[i]);
                nodeForces[i] = Vector3.zero;

                nodes[i].ApplyForce();
            }
        }
    }

    [BurstCompile]
    public struct PushJob : IJobParallelFor
    {
        Vector3 repellingNodePosition;
        int repellingNodeIndex;
        NativeArray<Vector3> nodePositions;
        NativeArray<Vector3> nodeForces;

        float dispersion, speed;

        public PushJob(float dispersion, float speed, int nodeAIndex, Vector3 nodeAPosition, NativeArray<Vector3> nodePositions, NativeArray<Vector3> nodeForces)
        {
            this.dispersion = dispersion;
            this.speed = speed;
            this.repellingNodePosition = nodeAPosition;
            this.repellingNodeIndex = nodeAIndex;
            this.nodePositions = nodePositions;
            this.nodeForces = nodeForces;
        }

        public void Execute(int targetNodeIndex)
        {
            if (repellingNodeIndex == targetNodeIndex)
                return;

            var spatialDifference = nodePositions[targetNodeIndex] - repellingNodePosition;

            nodeForces[targetNodeIndex] += spatialDifference / spatialDifference.sqrMagnitude * dispersion * dispersion * speed;
        }
    }

    void Spawn()
    {
        Camera.main.gameObject.layer = _cameraLayer;
        
        ClearSpawnedObjects();

        SpawnRandomNodesAndEdges();

        AllocateNativeArrays();
    }

    void SpawnRandomNodesAndEdges()
    {
        for (int i = 0; i < _nodes; i++)
        {
            var nodeObj = Instantiate(_3D ? _3DNodePrefab : _2DNodePrefab, (_3D ? Random.insideUnitSphere : (Vector3)Random.insideUnitCircle) * _spawnRadius, Quaternion.identity, transform);


            nodeObj.transform.localScale = Vector3.one * (_medianSize + Random.value * _sizeRange - _sizeRange * .5f);

            var node = new Node(nodeObj.GetComponent<Rigidbody>(), nodeObj.GetComponentsInChildren<MeshRenderer>());

            node.SetMaterial(_useEmissiveMaterials
                ? _emissiveMaterials[Random.Range(0, _emissiveMaterials.Length)]
                : _unlitMaterials[Random.Range(0, _unlitMaterials.Length)]);

            nodes.Add(node);
        }

        foreach (var node in nodes)
        {
            for (int i = 0; i < _maxEdges; i++)
            {
                if (Random.value > _edgeChance)
                    continue;

                var edgeObj = Instantiate(_edgePrefab, Vector3.zero, Quaternion.identity, transform);

                var edge = new Edge(node, nodes[Random.Range(0, nodes.Count)], edgeObj.GetComponent<LineRenderer>(), _3D);

                edge.SetMaterial(_useEmissiveMaterials
                    ? _emissiveMaterials[_emissiveMaterials.Length - 1]
                    : _unlitMaterials[_unlitMaterials.Length - 1]);

                edges.Add(edge);
            }
        }
    }

    void ClearSpawnedObjects()
    {
        foreach (var node in nodes)
            node.Destroy();

        nodes.Clear();

        foreach (var edge in edges)
            edge.Destroy();

        edges.Clear();
    }

    void AllocateNativeArrays()
    {
        if (nodePositions.IsCreated)
            nodePositions.Dispose();

        if (nodeForces.IsCreated)
            nodeForces.Dispose();

        nodePositions = new NativeArray<Vector3>(nodes.Count, Allocator.Persistent);
        nodeForces = new NativeArray<Vector3>(nodes.Count, Allocator.Persistent);
    }
}
