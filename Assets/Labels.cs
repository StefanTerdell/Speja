using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Labels : MonoBehaviour
{
    public bool _respawn;
    public float _cullingDistance = 30;
    public float _scaleMultiplier = 10;
    public GameObject _labelPrefab;
    Camera cam;
    Dictionary<Node, (GameObject obj, RectTransform rect)> nodeLabels = new Dictionary<Node, (GameObject obj, RectTransform rect)>();

    private void OnEnable()
    {
        cam = Camera.main;
        GraphInstantiator.OnNodeAdded += AddNodeLabelHandler;
        GraphInstantiator.OnNodeRemoved += RemoveNodeLabelHandler;
        GraphInstantiator.OnGraphReset += ResetAllHandler;
    }

    private void OnDisable()
    {
        GraphInstantiator.OnNodeAdded -= AddNodeLabelHandler;
        GraphInstantiator.OnNodeRemoved -= RemoveNodeLabelHandler;
        GraphInstantiator.OnGraphReset -= ResetAllHandler;

    }

    void ResetAllHandler(object o, System.EventArgs e) => ResetAll();
    void ResetAll()
    {
        foreach (var nodeLabel in nodeLabels)
        {
            Destroy(nodeLabel.Value.obj);
        }

        nodeLabels.Clear();

        foreach (var node in GraphInstantiator.Nodes)
        {
            AddNodeLabel(node);
        }
    }

    void AddNodeLabelHandler(object o, System.EventArgs e) => AddNodeLabel(o as Node);
    void AddNodeLabel(Node node)
    {
        var obj = Instantiate(_labelPrefab, node.position, Quaternion.identity, transform);
        obj.GetComponent<TMPro.TextMeshProUGUI>().text = $"{node.nodeData.type}\n{node.nodeData.name}";
        nodeLabels.Add(node, (obj, obj.GetComponent<RectTransform>()));
    }

    void RemoveNodeLabelHandler(object o, System.EventArgs e) => RemoveNodeLabel(o as Node);
    void RemoveNodeLabel(Node node)
    {
        Destroy(nodeLabels[node].obj);
        nodeLabels.Remove(node);
    }

    void LateUpdate()
    {
        if (_respawn)
        {
            _respawn = false;

            ResetAll();
        }

        var plane = new Plane(cam.transform.forward, cam.transform.position);
        foreach (var nodeLabel in nodeLabels)
        {
            var dist = cam.orthographic ? cam.orthographicSize : plane.GetDistanceToPoint(nodeLabel.Key.position);
            if (dist < 0 || dist > _cullingDistance)
            {
                nodeLabel.Value.obj.SetActive(false); 
            }
            else
            {
                nodeLabel.Value.obj.SetActive(true);

                nodeLabel.Value.rect.position = cam.WorldToScreenPoint(nodeLabel.Key.position);
                nodeLabel.Value.obj.transform.localScale = Vector3.one * _scaleMultiplier / dist;// Vector3.one * Mathf.Max(0, (1 - Mathf.Log10((nodeLabel.Key.position - cam.transform.position).sqrMagnitude / 10))) * 3;
            }
        }
    }
}
