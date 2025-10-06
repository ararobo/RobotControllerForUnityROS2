using UnityEngine;
using UnityEngine.UI;
using ROS2;
using std_msgs.msg;
using TMPro;

public class HoldManager : MonoBehaviour
{
    private ROS2UnityComponent ros2Unity;
    private ROS2Node ros2Node;
    private IPublisher<std_msgs.msg.Float32> hold_depth_pub;
    private ISubscription<std_msgs.msg.Bool> cancel_hold_sub;

    public float holdSpeed = 1.0f;
    private bool isHolding = false;

    public Slider holdSpeedSlider;
    public TMPro.TextMeshProUGUI holdSpeedText;
    public Toggle holdToggleButton;

    void Start()
    {
        if (TryGetComponent(out ros2Unity))
        {
            if (ros2Unity.Ok())
            {
                ros2Node = ros2Unity.CreateNode("UnityHoldManagerNode");
                hold_depth_pub = ros2Node.CreatePublisher<std_msgs.msg.Float32>("/phone/upper_hand/depth");

                cancel_hold_sub = ros2Node.CreateSubscription<std_msgs.msg.Bool>(
                    "/cancel_hold",
                    msg => OnCancelHoldReceived(msg)
                );
            }
        }
        else
        {
            Debug.LogError("ROS2UnityComponent not found on this GameObject.");
        }

        if (holdSpeedSlider != null)
        {
            holdSpeedSlider.value = holdSpeed;
            holdSpeedSlider.onValueChanged.AddListener(OnHoldSpeedSliderChanged);
            OnHoldSpeedSliderChanged(holdSpeedSlider.value);
        }

        if (holdToggleButton != null)
        {
            holdToggleButton.onValueChanged.AddListener(OnHoldToggleChanged);
        }
    }

    void Update()
    {
        if (isHolding)
        {
            if (ros2Unity != null && ros2Unity.Ok() && ros2Node != null && hold_depth_pub != null)
            {
                std_msgs.msg.Float32 msg = new std_msgs.msg.Float32
                {
                    Data = -holdSpeed
                };
                hold_depth_pub.Publish(msg);
            }
        }
    }

    private void OnHoldSpeedSliderChanged(float value)
    {
        holdSpeed = value;
        if (holdSpeedText != null)
        {
            holdSpeedText.text = holdSpeed.ToString("F2");
        }
    }

    public void OnHoldToggleChanged(bool isOn)
    {
        isHolding = isOn;

        if (!isOn)
        {
            if (ros2Unity != null && ros2Unity.Ok() && ros2Node != null && hold_depth_pub != null)
            {
                std_msgs.msg.Float32 msg = new std_msgs.msg.Float32
                {
                    Data = 0.0f
                };
                hold_depth_pub.Publish(msg);
            }
        }
    }

    private void OnCancelHoldReceived(std_msgs.msg.Bool msg)
    {
        if (msg.Data && isHolding)
        {
            if (holdToggleButton != null)
            {
                holdToggleButton.isOn = false;
            }
        }
    }
}