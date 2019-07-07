using UnityEngine;

public class CameraMover : MonoBehaviour
{
    
    GraphInstantiator gi;
    public float rotationSpeed;
    public float moveSpeed;
    public float shiftMultiplier;
    public float ctrlMultiplier;
    float currentMoveSpeed;

    float perspectiveZPosition;
    Camera cam;
    void Start()
    {
        gi = FindObjectOfType<GraphInstantiator>();
        perspectiveZPosition = transform.position.z;
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        if (cam.orthographic)
        {
            if (Input.GetKeyDown(KeyCode.Mouse2) || Input.GetKeyDown(KeyCode.Space))
            {
                gi._3D = true;

                cam.orthographic = false;
                transform.position = new Vector3(transform.position.x, transform.position.y, perspectiveZPosition);
                return;
            }

            if (Input.GetKey(KeyCode.Mouse1))
            {
                currentMoveSpeed = moveSpeed;

                currentMoveSpeed *= Input.GetKey(KeyCode.LeftShift) ? shiftMultiplier : 1;

                currentMoveSpeed *= Input.GetKey(KeyCode.LeftControl) ? ctrlMultiplier : 1;

                if (Input.GetKey(KeyCode.W))
                    cam.orthographicSize -= Time.deltaTime * currentMoveSpeed;

                if (Input.GetKey(KeyCode.S))
                    cam.orthographicSize += Time.deltaTime * currentMoveSpeed;

                if (Input.GetKey(KeyCode.D))
                    transform.Translate(Vector3.right * Time.deltaTime * currentMoveSpeed);

                if (Input.GetKey(KeyCode.A))
                    transform.Translate(Vector3.left * Time.deltaTime * currentMoveSpeed);

                if (Input.GetKey(KeyCode.Q))
                    transform.Translate(Vector3.up * Time.deltaTime * currentMoveSpeed);

                if (Input.GetKey(KeyCode.E))
                    transform.Translate(Vector3.down * Time.deltaTime * currentMoveSpeed);

                transform.Translate(new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * currentMoveSpeed * Time.deltaTime * -2);
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Mouse2) || Input.GetKeyDown(KeyCode.Space))
            {
                gi._3D = false;

                cam.orthographic = true;
                transform.eulerAngles = Vector3.zero;
                perspectiveZPosition = transform.position.z;
                transform.position = new Vector3(transform.position.x, transform.position.y, -1);
                return;
            }

            if (Input.GetKey(KeyCode.Mouse1))
            {
                transform.eulerAngles = new Vector3(
                    transform.eulerAngles.x + Input.GetAxis("Mouse Y") * rotationSpeed * -1,
                    transform.eulerAngles.y + Input.GetAxis("Mouse X") * rotationSpeed);

                currentMoveSpeed = moveSpeed;

                currentMoveSpeed *= Input.GetKey(KeyCode.LeftShift) ? shiftMultiplier : 1;

                currentMoveSpeed *= Input.GetKey(KeyCode.LeftControl) ? ctrlMultiplier : 1;

                if (Input.GetKey(KeyCode.W))
                    transform.Translate(Vector3.forward * Time.deltaTime * currentMoveSpeed);

                if (Input.GetKey(KeyCode.S))
                    transform.Translate(Vector3.back * Time.deltaTime * currentMoveSpeed);

                if (Input.GetKey(KeyCode.D))
                    transform.Translate(Vector3.right * Time.deltaTime * currentMoveSpeed);

                if (Input.GetKey(KeyCode.A))
                    transform.Translate(Vector3.left * Time.deltaTime * currentMoveSpeed);

                if (Input.GetKey(KeyCode.Q))
                    transform.Translate(Vector3.up * Time.deltaTime * currentMoveSpeed);

                if (Input.GetKey(KeyCode.E))
                    transform.Translate(Vector3.down * Time.deltaTime * currentMoveSpeed);
            }
        }
    }
}