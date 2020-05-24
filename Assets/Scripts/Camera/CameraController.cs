using UnityEngine;

/// <summary>
/// Simple free look camera with an alternative orthographic mode.
/// </summary>
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
    [SerializeField]
    private float _shiftMovementSpeed = 30f;
    [SerializeField]
    private float _lookSpeed = 5f;

    private Camera _camera;

    private Vector3 _rotation;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        Main = _camera;

        _camera.orthographic = Settings.IsCameraOrthographic;
        Settings.SettingsChanged += OnSettingsChanged;
    }

    private void Start()
    {
        _rotation = _camera.transform.eulerAngles;
    }

    private void OnSettingsChanged(object sender, System.EventArgs e)
    {
        if (_camera.orthographic != Settings.IsCameraOrthographic)
        {
            _camera.orthographic = Settings.IsCameraOrthographic;

            if (_camera.orthographic)
            {
                _camera.transform.eulerAngles = _rotation = Vector3.right * 90f;
            }
        }
    }

    private void Update()
    {
        if (Input.GetMouseButton(1) && !_camera.orthographic)
        {
            _rotation.y += Input.GetAxis("Mouse X");
            _rotation.x += -Input.GetAxis("Mouse Y");
            _camera.transform.eulerAngles = _rotation * _lookSpeed;
        }

        float cameraSpeed = _movementSpeed;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            cameraSpeed = _shiftMovementSpeed;
        }

        if(Input.GetKey(_forwardKey) || Input.GetKey(_forwardKeyAlt))
        {
            _camera.transform.position += (_camera.orthographic ? _camera.transform.up : _camera.transform.forward) * Time.deltaTime * cameraSpeed;
        }
        else if(Input.GetKey(_backKey) || Input.GetKey(_backKeyAlt))
        {
            _camera.transform.position -= (_camera.orthographic ? _camera.transform.up : _camera.transform.forward) * Time.deltaTime * cameraSpeed;
        }

        if (Input.GetKey(_leftKey) || Input.GetKey(_leftKeyAlt))
        {
            _camera.transform.position -= _camera.transform.right * Time.deltaTime * cameraSpeed;
        }
        else if (Input.GetKey(_rightKey) || Input.GetKey(_rightKeyAlt))
        {
            _camera.transform.position += _camera.transform.right * Time.deltaTime * cameraSpeed;
        }

        if (_camera.orthographic)
        {
            _camera.orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * 10f;
            _camera.orthographicSize = Mathf.Max(1f, _camera.orthographicSize);
        }
    }
}
