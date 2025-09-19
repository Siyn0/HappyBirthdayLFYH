using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MobileInputUI : MonoBehaviour
{
    [Header("摇杆")]
    public RectTransform joystickHandle;
    public RectTransform joystickBG;
    public float joystickRange = 50f;

    [Header("按钮")]
    public Button jumpButton;
    public Button dashButton;

    [HideInInspector]
    public float horizontalInput = 0f;
    [HideInInspector]
    public bool jumpPressed = false;
    [HideInInspector]
    public bool dashPressed = false;

    private Vector2 joystickStartPos;
    private bool dragging = false;

    void Start()
    {
        joystickStartPos = joystickHandle.anchoredPosition;
        jumpButton.onClick.AddListener(() => jumpPressed = true);
        dashButton.onClick.AddListener(() => dashPressed = true);
    }

    public void OnBeginDrag(BaseEventData data)
    {
        dragging = true;
    }

    public void OnDrag(BaseEventData data)
    {
        PointerEventData ped = (PointerEventData)data;
        Vector2 delta = ped.position - (Vector2)joystickBG.position;
        delta.x = Mathf.Clamp(delta.x, -joystickRange, joystickRange);
        joystickHandle.anchoredPosition = new Vector2(delta.x, joystickStartPos.y);
        horizontalInput = delta.x / joystickRange;
    }

    public void OnEndDrag(BaseEventData data)
    {
        dragging = false;
        joystickHandle.anchoredPosition = joystickStartPos;
        horizontalInput = 0f;
    }

    void LateUpdate()
    {
        // 按钮按下后只触发一次
        jumpPressed = false;
        dashPressed = false;
    }
}