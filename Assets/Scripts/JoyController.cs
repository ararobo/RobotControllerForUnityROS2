using UnityEngine;
using UnityEngine.InputSystem;
using ROS2;
using UnityEngine.UI;
using TMPro; // TextMeshProを使用するために必要なライブラリを追加

public class JoyController : MonoBehaviour
{
    private ROS2UnityComponent ros2Unity;
    private ROS2Node ros2Node;
    private IPublisher<geometry_msgs.msg.Twist> twist_pub;
    private IPublisher<std_msgs.msg.Bool> accel_pub;

    // 速度調整用のパブリック変数
    public float linearSpeed = 1.0f;
    public float angularSpeed = 1.0f;

    // スライダーの値表示用のテキスト
    public TMPro.TextMeshProUGUI linearSpeedText;
    public TMPro.TextMeshProUGUI angularSpeedText;

    // UIスライダーを割り当てるためのパブリック変数
    public Slider linearSpeedSlider;
    public Slider angularSpeedSlider;

    // ----- 新規追加部分 -----
    // 低速モードの乗数
    public float speedMultiplier = 0.25f;

    // トグルボタンを割り当てるためのパブリック変数
    public Toggle lowSpeedToggle;
    public Toggle lowAccelToggle;
    // ----------------------

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
                // ----- 新規追加: 加速トピックのパブリッシャー -----
                accel_pub = ros2Node.CreatePublisher<std_msgs.msg.Bool>("low_accel_topic");
                // ------------------------------------------------
            }
        }

        // スライダーの初期値を設定
        if (linearSpeedSlider != null)
        {
            linearSpeedSlider.value = linearSpeed;
        }
        if (angularSpeedSlider != null)
        {
            angularSpeedSlider.value = angularSpeed;
        }

        // ----- 新規追加: トグルボタンのリスナー登録 -----
        if (lowSpeedToggle != null)
        {
            // Update()で状態を確認するため、ここではリスナーは不要
        }
        if (lowAccelToggle != null)
        {
            lowAccelToggle.onValueChanged.AddListener(OnAccelToggleChanged);
        }
        // ------------------------------------------------
    }

    // フレーム更新時に呼び出される関数
    void Update()
    {
        // スライダーの値で速度を更新
        if (linearSpeedSlider != null)
        {
            linearSpeed = linearSpeedSlider.value;
            if (linearSpeedText != null)
            {
                linearSpeedText.text = linearSpeed.ToString("F2"); // 小数点以下2桁まで表示
            }
        }
        if (angularSpeedSlider != null)
        {
            angularSpeed = angularSpeedSlider.value;
            if (angularSpeedText != null)
            {
                angularSpeedText.text = angularSpeed.ToString("F2"); // 小数点以下2桁まで表示
            }
        }

        // ROS2が準備できていない、またはノードが作成されていない場合は何もしない
        if (ros2Unity == null || !ros2Unity.Ok() || ros2Node == null)
        {
            return;
        }

        // ジョイスティックの入力を取得
        var current = Gamepad.current;
        if (current == null)
        {
            // ジョイスティックが接続されていない場合は、0のメッセージをパブリッシュ
            geometry_msgs.msg.Twist stopMsg = new geometry_msgs.msg.Twist();
            twist_pub.Publish(stopMsg);
            return;
        }

        // Twistメッセージを初期化
        geometry_msgs.msg.Twist msg = new geometry_msgs.msg.Twist();

        // 左スティックのY軸入力を線形速度にマッピング（前後の移動）
        var leftStickInputY = current.leftStick.y.ReadValue();
        msg.Linear.Y = leftStickInputY * linearSpeed;

        // 左スティックのX軸入力を線形速度のY軸にマッピング（横移動）
        var leftStickInputX = current.leftStick.x.ReadValue();
        msg.Linear.X = leftStickInputX * linearSpeed;

        // 右スティックのX軸入力を角速度にマッピング（回転）
        var rightStickInputX = current.rightStick.x.ReadValue();
        msg.Angular.Z = -rightStickInputX * angularSpeed;

        // ----- 速度調整ロジックの追加 -----
        // 低速トグルがオンの場合、速度に乗数を掛ける
        if (lowSpeedToggle != null && lowSpeedToggle.isOn)
        {
            msg.Linear.Y *= speedMultiplier;
            msg.Linear.X *= speedMultiplier;
            msg.Angular.Z *= speedMultiplier;
        }
        // ---------------------------------

        // メッセージをパブリッシュ
        twist_pub.Publish(msg);
    }

    // ----- 新規追加メソッド -----
    /// <summary>
    /// 低加速トグルが変更されたときに呼び出されるメソッド
    /// </summary>
    /// <param name="isOn">トグルの新しい状態</param>
    public void OnAccelToggleChanged(bool isOn)
    {
        if (ros2Unity == null || !ros2Unity.Ok() || ros2Node == null || accel_pub == null)
        {
            Debug.LogWarning("ROS2 is not initialized. Cannot publish acceleration message.");
            return;
        }

        std_msgs.msg.Bool msg = new std_msgs.msg.Bool();
        msg.Data = isOn;

        accel_pub.Publish(msg);
        Debug.Log($"Published acceleration toggle state: {msg.Data}");
    }
    // ----------------------------
}