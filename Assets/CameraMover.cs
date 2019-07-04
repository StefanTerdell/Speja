using UnityEngine;

public class CameraMover : MonoBehaviour
{
    float walkSpeed = 10, runSpeed = 25, rotationSpeed = 10, moveSpeed;

    float perspectiveZPosition;
    Camera cam;
    void Start()
    {
        perspectiveZPosition = transform.position.z;
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        if (cam.orthographic)
        {
            if (Input.GetKeyDown(KeyCode.Mouse2))
            {
                cam.orthographic = false;
                transform.position = new Vector3(transform.position.x, transform.position.y, perspectiveZPosition);
                return;
            }

            if (Input.GetKey(KeyCode.Mouse1))
            {
                moveSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

                if (Input.GetKey(KeyCode.W))
                    cam.orthographicSize += Time.deltaTime * moveSpeed;

                if (Input.GetKey(KeyCode.S))
                    cam.orthographicSize -= Time.deltaTime * moveSpeed;

                if (Input.GetKey(KeyCode.D))
                    transform.Translate(Vector3.right * Time.deltaTime * moveSpeed);

                if (Input.GetKey(KeyCode.A))
                    transform.Translate(Vector3.left * Time.deltaTime * moveSpeed);

                if (Input.GetKey(KeyCode.Q))
                    transform.Translate(Vector3.up * Time.deltaTime * moveSpeed);

                if (Input.GetKey(KeyCode.E))
                    transform.Translate(Vector3.down * Time.deltaTime * moveSpeed);
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Mouse2))
            {
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

                moveSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

                if (Input.GetKey(KeyCode.W))
                    transform.Translate(Vector3.forward * Time.deltaTime * moveSpeed);

                if (Input.GetKey(KeyCode.S))
                    transform.Translate(Vector3.back * Time.deltaTime * moveSpeed);

                if (Input.GetKey(KeyCode.D))
                    transform.Translate(Vector3.right * Time.deltaTime * moveSpeed);

                if (Input.GetKey(KeyCode.A))
                    transform.Translate(Vector3.left * Time.deltaTime * moveSpeed);

                if (Input.GetKey(KeyCode.Q))
                    transform.Translate(Vector3.up * Time.deltaTime * moveSpeed);

                if (Input.GetKey(KeyCode.E))
                    transform.Translate(Vector3.down * Time.deltaTime * moveSpeed);
            }
        }
    }
}