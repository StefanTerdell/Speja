using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;
using System;

public class FruchtermanReingold : MonoBehaviour
{
    [Range(0.01f, 10)] public float _dispersion = 1;
    [Range(0.1f, 100)] public float _speed = 20;
    public float _maximumForceMagnitude = 50000;


    List<Edge> edges => GraphInstantiator.Edges;
    List<Node> nodes => GraphInstantiator.Nodes;
    NativeArray<Vector3> nodePositions;
    NativeArray<Vector3> nodeForces;

    void OnEnable()
    {
        GraphInstantiator.OnGraphReset += AllocateNativeArraysEventHandler;
        GraphInstantiator.OnNodeAdded += AllocateNativeArraysEventHandler;
        GraphInstantiator.OnNodeRemoved += AllocateNativeArraysEventHandler;

        if (!nodePositions.IsCreated || !nodeForces.IsCreated)
            AllocateNativeArrays();
    }

    void AllocateNativeArraysEventHandler(System.Object o, EventArgs e) => AllocateNativeArrays();

    void AllocateNativeArrays()
    {
        if (nodePositions.IsCreated)
            nodePositions.Dispose();

        if (nodeForces.IsCreated)
            nodeForces.Dispose();

        nodePositions = new NativeArray<Vector3>(nodes.Count, Allocator.Persistent);
        nodeForces = new NativeArray<Vector3>(nodes.Count, Allocator.Persistent);
    }

    private void OnDisable()
    {
        GraphInstantiator.OnGraphReset -= AllocateNativeArraysEventHandler;
        GraphInstantiator.OnNodeAdded -= AllocateNativeArraysEventHandler;
        GraphInstantiator.OnNodeRemoved -= AllocateNativeArraysEventHandler;

        nodePositions.Dispose();
        nodeForces.Dispose();
    }

    void Update()
    {
        foreach (var edge in edges)
        {
            var diff = edge.from.position - edge.to.position;
            var sqrDist = diff.sqrMagnitude;

            edge.from.AddForce(diff * sqrDist / _dispersion * _speed * -1);
            edge.to.AddForce(diff * sqrDist / _dispersion * _speed);
        }

        for (int i = 0; i < nodes.Count; i++)
        {
            nodePositions[i] = nodes[i].position;
        }

        for (int i = 0; i < nodes.Count; i++)
        {
            (new RepelNodesJob(_dispersion, _speed, i, nodePositions[i], nodePositions, nodeForces).Schedule(nodes.Count, 100)).Complete();
        }

        for (int i = 0; i < nodes.Count; i++)
        {
            nodes[i].AddForce(nodeForces[i]);
            nodes[i].ApplyForce(_maximumForceMagnitude * Time.deltaTime);

            nodeForces[i] = Vector3.zero;
        }
    }

    [BurstCompile]
    public struct RepelNodesJob : IJobParallelFor
    {
        float dispersion;
        float speed;
        int repellingNodeIndex;
        Vector3 repellingNodePosition;
        NativeArray<Vector3> nodePositions;
        NativeArray<Vector3> nodeForces;

        public RepelNodesJob(float dispersion, float speed, int repellingNodeIndex, Vector3 repellingNodePosition, NativeArray<Vector3> nodePositions, NativeArray<Vector3> nodeForces)
        {
            this.dispersion = dispersion;
            this.speed = speed;
            this.repellingNodePosition = repellingNodePosition;
            this.repellingNodeIndex = repellingNodeIndex;
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
}
