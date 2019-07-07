using UnityEngine;

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

        Set3D(_3D);
    }

    public void SetMaterial(Material mat)
    {
        lineRenderer.material = mat;
    }

    public void UpdateLinerendererPositions()
    {
        lineRenderer.SetPositions(new[] { from.position + lineRendererOffset, to.position + lineRendererOffset });
    }

    public void Set3D(bool _3D)
    {
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


    public void Destroy()
    {
        GameObject.Destroy(lineRenderer.gameObject);
    }
}