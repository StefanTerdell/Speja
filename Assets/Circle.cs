using UnityEngine;

[ExecuteAlways]
public class Circle : MonoBehaviour
{
    [Range(3, 100)]
    public int segments = 6;
    public float radius = 1;
    public float thickness = .1f;
    public bool fill;
    public bool update;

    void Update() {
        if (update) {
            var lr = GetComponent<LineRenderer>();

            if (!lr)
                return;

            update = false;

            Vector3[] positions = new Vector3[segments];

            lr.positionCount = segments;
            lr.widthCurve = AnimationCurve.Constant(0, 1, fill ? radius : thickness);

            for (int i = 0; i < segments; i++)
            {
                var angle = i * (360f / (float)segments);

                positions[i] = new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), Mathf.Cos(angle* Mathf.Deg2Rad));
                positions[i] *= radius - (fill ? radius : thickness) / 2f;
            }

            lr.SetPositions(positions);
        }
    }
}