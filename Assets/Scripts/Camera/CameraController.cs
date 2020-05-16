using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static Camera Main { get; private set; }

    [Header("Controls")]
    [SerializeField]
    private KeyCode _forwardKey = KeyCode.W;
    [SerializeField]
    private KeyCode _forwardKeyAlt = KeyCode.UpArrow;
    [SerializeField]
    private KeyCode _backKey = KeyCode.S;
    [SerializeField]
    private KeyCode _backKeyAlt = KeyCode.DownArrow;
    [SerializeField]
    private KeyCode _leftKey = KeyCode.A;
    [SerializeField]
    private KeyCode _leftKeyAlt = KeyCode.LeftArrow;
    [SerializeField]
    private KeyCode _rightKey = KeyCode.D;
    [SerializeField]
    private KeyCode _rightKeyAlt = KeyCode.RightArrow;

    [Header("Settings")]
    [SerializeField]
    private float _movementSpeed = 10f;

    private Camera _camera;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        Main = _camera;
    }

    private void Update()
    {
        if (Input.GetMouseButton(1))
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            _camera.transform.forward = Vector3.MoveTowards(_camera.transform.forward, ray.direction, Time.deltaTime);
        }

        if(Input.GetKey(_forwardKey) || Input.GetKey(_forwardKeyAlt))
        {
            _camera.transform.position += _camera.transform.forward * Time.deltaTime * _movementSpeed;
        }
        else if(Input.GetKey(_backKey) || Input.GetKey(_backKeyAlt))
        {
            _camera.transform.position -= _camera.transform.forward * Time.deltaTime * _movementSpeed;
        }

        if (Input.GetKey(_leftKey) || Input.GetKey(_leftKeyAlt))
        {
            _camera.transform.position -= _camera.transform.right * Time.deltaTime * _movementSpeed;
        }
        else if (Input.GetKey(_rightKey) || Input.GetKey(_rightKeyAlt))
        {
            _camera.transform.position += _camera.transform.right * Time.deltaTime * _movementSpeed;
        }
    }
}
