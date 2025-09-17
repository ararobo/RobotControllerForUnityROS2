using UnityEngine;
using UnityEngine.UI;
using ROS2;
using geometry_msgs.msg;
using std_msgs.msg;

public class SpeedManager : MonoBehaviour
{
    private ROS2UnityComponent ros2Unity;
    private ROS2Node ros2Node;
    private IPublisher<Twist> cmd_vel_pub;

    // UIトグルを割り当てるためのパブリック変数
    public Toggle speedToggle;
    public Toggle accelerationToggle;

    // 基本速度と回転速度の設定
    public float baseLinearSpeed = 1.0f;
    public float baseAngularSpeed = 1.0f;
    public float speedMultiplier = 0.25f;

    private Twist cmd_vel_msg;

    void Start()
    {
        // ROS2Unityコンポーネントの取得
        if (TryGetComponent(out ros2Unity))
        {
            // ROS2の初期化
            if (ros2Unity.Ok())
            {
                ros2Node = ros2Unity.CreateNode("UnitySpeedNode");
                cmd_vel_pub = ros2Node.CreatePublisher<Twist>("cmd_vel");
                cmd_vel_msg = new Twist();
            }
        }

        // トグルの初期値を設定
        if (speedToggle != null)
        {
            // トグルの状態変更イベントにリスナーを追加
            speedToggle.onValueChanged.AddListener(OnSpeedToggleChanged);
        }
        if (accelerationToggle != null)
        {
            // トグルの状態変更イベントにリスナーを追加
            accelerationToggle.onValueChanged.AddListener(OnAccelerationToggleChanged);
        }
    }

    void Update()
    {
        // ROS2が準備できていない場合は何もしない
        if (ros2Unity == null || !ros2Unity.Ok() || ros2Node == null || cmd_vel_pub == null)
        {
            return;
        }

        // キーボード入力による速度設定
        float linearSpeed = Input.GetAxis("Vertical") * baseLinearSpeed;
        float angularSpeed = -Input.GetAxis("Horizontal") * baseAngularSpeed;

        // 低速/高速トグルの状態を反映
        if (speedToggle != null && speedToggle.isOn)
        {
            linearSpeed *= speedMultiplier; // 速度を0.25倍にする
            angularSpeed *= speedMultiplier; // 回転速度も0.25倍にする
        }

        // メッセージのデータを更新
        cmd_vel_msg.Linear.X = linearSpeed;
        cmd_vel_msg.Angular.Z = angularSpeed;

        // メッセージをパブリッシュ
        cmd_vel_pub.Publish(cmd_vel_msg);
    }

    /// <summary>
    /// 速度トグルの状態が変更されたときに呼ばれるメソッド
    /// </summary>
    public void OnSpeedToggleChanged(bool isOn)
    {
        Debug.Log("低速モード: " + isOn);
        // このイベントはUpdate()で処理されるので、ここではログのみ
    }

    /// <summary>
    /// 加速トグルの状態が変更されたときに呼ばれるメソッド
    /// </summary>
    public void OnAccelerationToggleChanged(bool isOn)
    {
        Debug.Log("低加速モード: " + isOn);
        // このイベントはUpdate()で処理されるので、ここではログのみ
    }
}