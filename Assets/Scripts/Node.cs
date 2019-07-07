using UnityEngine;

public class Node
{
    public Vector3 position => rigidbody.position;
    public bool isKinematic => rigidbody.isKinematic;

    Vector3 unappliedForce;
    Rigidbody rigidbody;
    MeshRenderer[] meshRenderers;

    public string type;

    public Node(Rigidbody rigidbody, MeshRenderer[] meshRenderers, string type)
    {
        this.rigidbody = rigidbody;
        this.meshRenderers = meshRenderers;
        this.type = type;
    }

    public void SetComponents(Rigidbody rigidbody, MeshRenderer[] meshRenderers)
    {
        this.rigidbody = rigidbody;
        this.meshRenderers = meshRenderers;
    }

    public void SetYLock(bool lockY)
    {
        if (lockY)
            rigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
        else
            rigidbody.constraints = RigidbodyConstraints.FreezeRotation;

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

    public void DestroyGameObject()
    {
        GameObject.Destroy(rigidbody.gameObject);
    }
}