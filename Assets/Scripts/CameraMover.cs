using UnityEngine;

public class CameraMover : MonoBehaviour
{

    public float _rotationSpeed = 10;
    public float _moveSpeed = 25;
    public float _shiftMultiplier = 2;
    public float _ctrlMultiplier = 4;

    GraphInstantiator gi;
    Camera cam;
    float perspectiveZPosition;
    float currentMoveSpeed;
    Rigidbody orbit;
    float holdTime;
    float orbitDistance3D;
    void Start()
    {
        gi = FindObjectOfType<GraphInstantiator>();
        perspectiveZPosition = transform.position.z;
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse1))
        {
            holdTime += Time.deltaTime;
        }
        else if (Input.GetKeyUp(KeyCode.Mouse1) && holdTime < .1f)
        {
            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out var hitInfo))
            {
                orbit = hitInfo.rigidbody;
                orbitDistance3D = Vector3.Distance(orbit.position, transform.position);
            }
            else
            {
                orbit = null;
            }
        }
        else
        {
            holdTime = 0;
        }

        if (cam.orthographic)
        {
            if (Input.GetKeyDown(KeyCode.Mouse2) || Input.GetKeyDown(KeyCode.Space))
            {
                gi._3D = true;

                cam.orthographic = false;
                transform.position = new Vector3(transform.position.x, transform.position.y, perspectiveZPosition);
                return;
            }

            if (orbit)
            {
                transform.position = new Vector3(orbit.position.x, orbit.position.y, -1);
            }

            currentMoveSpeed = _moveSpeed;

            currentMoveSpeed *= Input.GetKey(KeyCode.LeftShift) ? _shiftMultiplier : 1;

            currentMoveSpeed *= Input.GetKey(KeyCode.LeftControl) ? _ctrlMultiplier : 1;

            if (Input.GetKey(KeyCode.W))
            {
                cam.orthographicSize -= Time.deltaTime * currentMoveSpeed;
            }

            if (Input.GetKey(KeyCode.S))
            {
                cam.orthographicSize += Time.deltaTime * currentMoveSpeed;
            }

            if (Input.GetKey(KeyCode.D))
            {
                orbit = null;
                transform.Translate(Vector3.right * Time.deltaTime * currentMoveSpeed);
            }

            if (Input.GetKey(KeyCode.A))
            {
                orbit = null;
                transform.Translate(Vector3.left * Time.deltaTime * currentMoveSpeed);
            }

            if (Input.GetKey(KeyCode.Q))
            {
                orbit = null;
                transform.Translate(Vector3.up * Time.deltaTime * currentMoveSpeed);
            }

            if (Input.GetKey(KeyCode.E))
            {
                orbit = null;
                transform.Translate(Vector3.down * Time.deltaTime * currentMoveSpeed);
            }

            if (Input.GetKey(KeyCode.Mouse1))
            {
                orbit = null;
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

            if (orbit)
            {
                transform.LookAt(orbit.position, transform.up);

                transform.position = orbit.position + transform.forward * orbitDistance3D * -1;
            }

            currentMoveSpeed = _moveSpeed;

            currentMoveSpeed *= Input.GetKey(KeyCode.LeftShift) ? _shiftMultiplier : 1;

            currentMoveSpeed *= Input.GetKey(KeyCode.LeftControl) ? _ctrlMultiplier : 1;

            if (Input.GetKey(KeyCode.W))
            {
                if (orbit)
                {
                    orbitDistance3D -= Time.deltaTime * currentMoveSpeed;
                }
                else
                {
                    transform.Translate(Vector3.forward * Time.deltaTime * currentMoveSpeed);
                }
            }

            if (Input.GetKey(KeyCode.S))
            {
                if (orbit)
                {
                    orbitDistance3D += Time.deltaTime * currentMoveSpeed;
                }
                else
                {
                    transform.Translate(Vector3.back * Time.deltaTime * currentMoveSpeed);
                }
            }

            if (Input.GetKey(KeyCode.D))
            {
                orbit = null;
                transform.Translate(Vector3.right * Time.deltaTime * currentMoveSpeed);
            }

            if (Input.GetKey(KeyCode.A))
            {
                orbit = null;
                transform.Translate(Vector3.left * Time.deltaTime * currentMoveSpeed);
            }

            if (Input.GetKey(KeyCode.Q))
            {
                orbit = null;
                transform.Translate(Vector3.up * Time.deltaTime * currentMoveSpeed);
            }

            if (Input.GetKey(KeyCode.E))
            {
                orbit = null;
                transform.Translate(Vector3.down * Time.deltaTime * currentMoveSpeed);
            }

            if (Input.GetKey(KeyCode.Mouse1))
            {
                if (orbit)
                    transform.Translate(Vector3.right * Input.GetAxis("Mouse X") + Vector3.up * Input.GetAxis("Mouse Y"));
                else
                    transform.eulerAngles = new Vector3(
                        transform.eulerAngles.x + Input.GetAxis("Mouse Y") * _rotationSpeed * -1,
                        transform.eulerAngles.y + Input.GetAxis("Mouse X") * _rotationSpeed);

            }
        }
    }
}