using UnityEngine;
using UnityEngine.InputSystem;
using ROS2;
using UnityEngine.UI;
using TMPro;
using std_msgs.msg;
using UnityEngine.EventSystems; // Event Triggerを使用するために必要

public class UpperHandController : MonoBehaviour
{
    private ROS2UnityComponent ros2Unity;
    private ROS2Node ros2Node;
    private IPublisher<std_msgs.msg.Float32> upper_hand_width_pub;
    private IPublisher<std_msgs.msg.Float32> upper_hand_depth_pub;

    public float maxArmSpeed = 1.0f;
    public float holdSpeed = 1.0f;

    public Slider armSpeedSlider;
    public TMPro.TextMeshProUGUI armSpeedText;

    private float currentHandWidth = 0.0f;
    private float currentHandDepth = 0.0f;

    public Button handForwardButton;
    public Button handBackwardButton;
    public Button handOpenButton;
    public Button handCloseButton;

    public Toggle holdToggleButton;

    void Start()
    {
        Application.targetFrameRate = 30;

        if (TryGetComponent(out ros2Unity))
        {
            if (ros2Unity.Ok())
            {
                ros2Node = ros2Unity.CreateNode("UnityUpperHandNode");
                upper_hand_width_pub = ros2Node.CreatePublisher<std_msgs.msg.Float32>("/upper_hand/width");
                upper_hand_depth_pub = ros2Node.CreatePublisher<std_msgs.msg.Float32>("/upper_hand/depth");
            }
        }
        else
        {
            Debug.LogError("ROS2UnityComponent not found on this GameObject.");
        }

        if (armSpeedSlider != null)
        {
            armSpeedSlider.value = maxArmSpeed;
            armSpeedSlider.onValueChanged.AddListener(OnArmSpeedSliderChanged);
        }

        SetupButtonEvents(handForwardButton, 1.0f, ButtonType.Depth);
        SetupButtonEvents(handBackwardButton, -1.0f, ButtonType.Depth);
        SetupButtonEvents(handOpenButton, 1.0f, ButtonType.Width);
        SetupButtonEvents(handCloseButton, -1.0f, ButtonType.Width);

        if (holdToggleButton != null)
        {
            holdToggleButton.onValueChanged.AddListener(OnHoldToggleChanged);
        }
    }

    private void OnArmSpeedSliderChanged(float value)
    {
        maxArmSpeed = value;
        if (armSpeedText != null)
        {
            armSpeedText.text = maxArmSpeed.ToString("F2");
        }
    }

    private void SetupButtonEvents(Button button, float value, ButtonType type)
    {
        if (button == null) return;

        EventTrigger trigger = button.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = button.gameObject.AddComponent<EventTrigger>();
        }

        EventTrigger.Entry entryDown = new EventTrigger.Entry();
        entryDown.eventID = EventTriggerType.PointerDown;
        entryDown.callback.AddListener((eventData) => { OnButtonDown(value, type); });
        trigger.triggers.Add(entryDown);

        EventTrigger.Entry entryUp = new EventTrigger.Entry();
        entryUp.eventID = EventTriggerType.PointerUp;
        entryUp.callback.AddListener((eventData) => { OnButtonUp(type); });
        trigger.triggers.Add(entryUp);
    }

    public void OnButtonDown(float value, ButtonType type)
    {
        if (type == ButtonType.Width)
        {
            currentHandWidth = value;
        }
        else if (type == ButtonType.Depth)
        {
            currentHandDepth = value;
        }

        if (holdToggleButton != null)
        {
            holdToggleButton.isOn = false;
        }
    }

    public void OnButtonUp(ButtonType type)
    {
        if (type == ButtonType.Width)
        {
            currentHandWidth = 0.0f;
        }
        else if (type == ButtonType.Depth)
        {
            currentHandDepth = 0.0f;
        }
    }

    public void OnHoldToggleChanged(bool isOn)
    {
        if (!isOn)
        {
            currentHandDepth = 0.0f;
        }
        else
        {
            currentHandDepth = -holdSpeed;
        }
    }

    void Update()
    {
        if (ros2Unity == null || !ros2Unity.Ok() || ros2Node == null || upper_hand_width_pub == null || upper_hand_depth_pub == null)
        {
            return;
        }

        if (holdToggleButton != null && holdToggleButton.isOn)
        {
            currentHandDepth = -holdSpeed;
        }

        std_msgs.msg.Float32 width_msg = new std_msgs.msg.Float32();
        std_msgs.msg.Float32 depth_msg = new std_msgs.msg.Float32();

        width_msg.Data = currentHandWidth * maxArmSpeed;
        depth_msg.Data = currentHandDepth * maxArmSpeed;

        if (upper_hand_width_pub != null)
        {
            upper_hand_width_pub.Publish(width_msg);
        }
        if (upper_hand_depth_pub != null)
        {
            upper_hand_depth_pub.Publish(depth_msg);
        }
    }
}