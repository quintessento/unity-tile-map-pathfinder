using UnityEngine;
using UnityEngine.UI;

public class MessagePanel : MonoBehaviour
{
    private static MessagePanel _instance;

    [SerializeField]
    private Text _messageLabel = null;

    private CanvasGroup _canvasGroup;

    private float _timer;
    private float _messageDuration;
    private bool _isShowing;

    public static void ShowMessage(string message, float durationSec = 3f)
    {
        _instance._messageLabel.text = message;
        _instance._messageDuration = durationSec;
        _instance._timer = 0f;
        _instance._canvasGroup.alpha = 1f;
        _instance._isShowing = true;
    }

    private void Awake()
    {
        _instance = this;
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Update()
    {
        if (_isShowing)
        {
            if(_timer < _messageDuration)
            {
                _timer += Time.deltaTime;
            }
            else
            {
                _isShowing = false;
                _canvasGroup.alpha = 0f;
            }
        }
    }
}
