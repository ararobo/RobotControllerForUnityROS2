using UnityEngine;
using UnityEngine.UI;
using ROS2;
using std_msgs.msg;
using TMPro;

public class HoldManager : MonoBehaviour
{
    private ROS2UnityComponent ros2Unity;
    private ROS2Node ros2Node;
    private IPublisher<std_msgs.msg.Float32> hold_depth_pub; // トピック名を/upper_hand/depthに変更

    // ホールド速度の設定
    public float holdSpeed = 1.0f;
    private bool isHolding = false; // ホールド状態を追跡するフラグ

    // UI要素の参照
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
                // /upper_hand/depthトピックにパブリッシュするように修正
                hold_depth_pub = ros2Node.CreatePublisher<std_msgs.msg.Float32>("/upper_hand/depth");
            }
        }
        else
        {
            Debug.LogError("ROS2UnityComponent not found on this GameObject.");
        }

        // ホールド速度スライダーの設定
        if (holdSpeedSlider != null)
        {
            holdSpeedSlider.value = holdSpeed;
            holdSpeedSlider.onValueChanged.AddListener(OnHoldSpeedSliderChanged);
            OnHoldSpeedSliderChanged(holdSpeedSlider.value); // 初期値をテキストに反映
        }

        // ホールドトグルの設定
        if (holdToggleButton != null)
        {
            holdToggleButton.onValueChanged.AddListener(OnHoldToggleChanged);
        }
    }

    /// <summary>
    /// ホールド速度を毎フレームパブリッシュする
    /// </summary>
    void Update()
    {
        if (isHolding)
        {
            // ROS2が初期化されているかチェック
            if (ros2Unity != null && ros2Unity.Ok() && ros2Node != null && hold_depth_pub != null)
            {
                std_msgs.msg.Float32 msg = new std_msgs.msg.Float32();
                msg.Data = -holdSpeed; // ホールドは常に負の値
                hold_depth_pub.Publish(msg);
            }
        }
    }

    /// <summary>
    /// ホールド速度スライダーの値が変更されたときに呼び出されるメソッド
    /// </summary>
    private void OnHoldSpeedSliderChanged(float value)
    {
        holdSpeed = value;
        if (holdSpeedText != null)
        {
            holdSpeedText.text = holdSpeed.ToString("F2");
        }
    }

    /// <summary>
    /// ホールドトグルの状態が変更されたときに呼び出されるメソッド
    /// </summary>
    public void OnHoldToggleChanged(bool isOn)
    {
        isHolding = isOn; // ホールド状態フラグを更新

        if (!isOn)
        {
            // トグルがオフになった場合、速度0を1回だけパブリッシュして停止を指示
            if (ros2Unity != null && ros2Unity.Ok() && ros2Node != null && hold_depth_pub != null)
            {
                std_msgs.msg.Float32 msg = new std_msgs.msg.Float32();
                msg.Data = 0.0f;
                hold_depth_pub.Publish(msg);
                Debug.Log("Hold disabled. Publishing speed: 0.0");
            }
        }
        else
        {
            Debug.Log($"Hold enabled. The speed will be published in Update().");
        }
    }
}