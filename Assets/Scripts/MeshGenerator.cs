using UnityEngine;

public static class MeshGenerator
{
    public static Mesh GetPrimitive2DMesh(int corners, float radius)
    {
        if (corners < 3) {
            return new Mesh();
        } 
        else if (corners == 3)
        {
            return new Mesh()
            {
                vertices = new Vector3[3] {
                        new Vector3(0, .5f),
                        new Vector3(.433f, -.25f),
                        new Vector3(-.433f, -.25f)
                    },
                triangles = new int[3] {
                        0,1,2
                    },
                normals = new Vector3[3] {
                        Vector3.back,
                        Vector3.back,
                        Vector3.back
                    },
                uv = new Vector2[3] {
                        new Vector2(.5f, 1),
                        new Vector2(1, 0),
                        new Vector2(0, 0)
                    }
            };
        }
        else if (corners == 4)
        {
            return new Mesh()
            {
                vertices = new Vector3[4] {
                        new Vector3(.5f, .5f),
                        new Vector3(.5f, -.5f),
                        new Vector3(-.5f, -.5f),
                        new Vector3(-.5f, .5f)
                    },
                triangles = new int[6] {
                        0,1,3,
                        1,2,3
                    },
                normals = new Vector3[4] {
                        Vector3.back,
                        Vector3.back,
                        Vector3.back,
                        Vector3.back
                    },
                uv = new Vector2[4] {
                        new Vector2(1, 1),
                        new Vector2(1, 0),
                        new Vector2(0, 0),
                        new Vector2(0, 1)
                    }
            };
        }
        else
        {
            var mesh = new Mesh();

            var uv = new Vector2[corners + 1];
            var vertices = new Vector3[corners + 1];
            var normals = new Vector3[corners + 1];
            var triangles = new int[corners * 3];

            for (int i = 0; i < corners; i++)
            {
                var angle = i * (360f / (float)corners);

                uv[i] = new Vector2(Mathf.Sin(angle * Mathf.Deg2Rad), Mathf.Cos(angle * Mathf.Deg2Rad));
                vertices[i] = uv[i] * radius * .5f;

                triangles[i * 3] = i;
                triangles[i * 3 + 1] = i + 1 == corners ? 0 : i + 1;
                triangles[i * 3 + 2] = corners;

                normals[i] = Vector3.back;
            }

            normals[corners] = Vector3.back;

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.normals = normals;
            mesh.uv = uv;

            return mesh;
        }
    }
}