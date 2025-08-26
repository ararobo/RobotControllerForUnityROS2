using UnityEngine;
using UnityEngine.InputSystem;
using ROS2;

public class JoyController : MonoBehaviour
{
    private ROS2UnityComponent ros2Unity;
    private ROS2Node ros2Node;
    private IPublisher<geometry_msgs.msg.Twist> twist_pub;

    // シーン読み込み時に一度だけ呼び出される関数
    void Start()
    {
        //FPS設定
        Application.targetFrameRate = 30;

        // TryGetComponentでコンポーネントの取得を試みる
        if (TryGetComponent(out ros2Unity))
        {
            // 取得に成功した場合のみ、ROS2の初期化処理を行う
            if (ros2Unity.Ok())
            {
                ros2Node = ros2Unity.CreateNode("UnityJoyNode");
                twist_pub = ros2Node.CreatePublisher<geometry_msgs.msg.Twist>("cmd_vel");
            }
        }
    }

    // フレーム更新時に呼び出される関数
    void Update()
    {
        // ROS2が準備できていない、またはノードが作成されていない場合は何もしない
        if (ros2Unity == null || !ros2Unity.Ok() || ros2Node == null)
        {
            return;
        }

        // ジョイスティックの入力を取得
        var current = Gamepad.current;
        if (current == null)
        {
            return; // ジョイスティックが接続されていない場合は何もしない
        }

        // Twistメッセージを初期化
        geometry_msgs.msg.Twist msg = new geometry_msgs.msg.Twist();

        // 左スティックの入力を線形速度にマッピング
        var leftStickInput = current.leftStick.ReadValue();
        msg.Linear.X = leftStickInput.x; // 前後移動
        msg.Linear.Y = leftStickInput.y; // 左右移動

        // 右スティックのX軸を角速度にマッピング
        var rightStickInput = current.rightStick.ReadValue();
        msg.Angular.Z = rightStickInput.x; // 回転

        // メッセージをパブリッシュ
        twist_pub.Publish(msg);
    }
}