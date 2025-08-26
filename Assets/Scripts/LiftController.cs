using UnityEngine;
using UnityEngine.InputSystem;
using ROS2;
using UnityEngine.UI;
using TMPro; // TextMeshProを使用するために必要なライブラリを追加

public class LiftController : MonoBehaviour
{
    private ROS2UnityComponent ros2Unity;
    private ROS2Node ros2Node;
    private IPublisher<std_msgs.msg.Float32> lift_pub;

    // リフトの最大速度を設定
    public float maxLiftSpeed = 1.0f;

    // UIスライダーを割り当てるためのパブリック変数
    public Slider liftSpeedSlider;

    // スライダーの値表示用のテキスト
    public TMPro.TextMeshProUGUI liftSpeedText;

    void Start()
    {
        // FPS設定
        Application.targetFrameRate = 30;

        // ROS2Unityコンポーネントの取得を試みる
        if (TryGetComponent(out ros2Unity))
        {
            // 取得に成功した場合のみ、ROS2の初期化処理を行う
            if (ros2Unity.Ok())
            {
                ros2Node = ros2Unity.CreateNode("UnityLiftNode");
                lift_pub = ros2Node.CreatePublisher<std_msgs.msg.Float32>("lift_vel");
            }
        }

        // スライダーの初期値を設定
        if (liftSpeedSlider != null)
        {
            liftSpeedSlider.value = maxLiftSpeed;
        }
    }

    // フレーム更新時に呼び出される関数
    void Update()
    {
        // スライダーの値で速度を更新
        if (liftSpeedSlider != null)
        {
            maxLiftSpeed = liftSpeedSlider.value;
            if (liftSpeedText != null)
            {
                liftSpeedText.text = maxLiftSpeed.ToString("F2"); // 小数点以下2桁まで表示
            }
        }

        // ROS2が準備できていない場合は何もしない
        if (ros2Unity == null || !ros2Unity.Ok() || ros2Node == null)
        {
            return;
        }

        // ジョイスティックの入力を取得
        var current = Gamepad.current;
        if (current == null)
        {
            // ジョイスティックが接続されていない場合は、0のメッセージをパブリッシュ
            std_msgs.msg.Float32 stopMsg = new std_msgs.msg.Float32();
            stopMsg.Data = 0.0f;
            lift_pub.Publish(stopMsg);
            return;
        }

        // Float32メッセージを初期化
        std_msgs.msg.Float32 msg = new std_msgs.msg.Float32();
        msg.Data = 0.0f; // デフォルトでは速度を0に設定

        // D-padの上下ボタンの入力を取得
        var dpad = current.dpad.ReadValue();

        // 上ボタンが押されているか確認
        if (dpad.y > 0)
        {
            msg.Data = maxLiftSpeed; // 上昇
        }
        // 下ボタンが押されているか確認
        else if (dpad.y < 0)
        {
            msg.Data = -maxLiftSpeed; // 下降
        }

        // メッセージをパブリッシュ
        lift_pub.Publish(msg);
    }
}