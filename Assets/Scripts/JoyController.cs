using UnityEngine;
using UnityEngine.InputSystem;
using ROS2;
using UnityEngine.UI;
using TMPro; // TextMeshProを使用するために必要なライブラリを追加
using geometry_msgs.msg;
using std_msgs.msg;

public class JoyController : MonoBehaviour
{
    private ROS2UnityComponent ros2Unity;
    private ROS2Node ros2Node;
    private IPublisher<geometry_msgs.msg.Twist> twist_pub;
    private IPublisher<std_msgs.msg.Bool> speed_pub;
    private IPublisher<std_msgs.msg.Bool> accel_pub;

    // 速度調整用のパブリック変数
    public float linearSpeed = 1.0f;
    public float angularSpeed = 1.0f;
    public float speedMultiplier = 0.25f; // 低速モードの乗数

    // スライダーの値表示用のテキスト
    public TMPro.TextMeshProUGUI linearSpeedText;
    public TMPro.TextMeshProUGUI angularSpeedText;

    // UIスライダーを割り当てるためのパブリック変数
    public Slider linearSpeedSlider;
    public Slider angularSpeedSlider;

    // トグルボタンを割り当てるためのパブリック変数
    public Toggle lowSpeedToggle;
    public Toggle lowAccelToggle;

    // 元々のスライダーの最大値を保存するプライベート変数
    private float initialMaxLinearSpeed;
    private float initialMaxAngularSpeed;

    // シーン読み込み時に一度だけ呼び出される関数
    void Start()
    {
        // TryGetComponentでコンポーネントの取得を試みる
        if (TryGetComponent(out ros2Unity))
        {
            // 取得に成功した場合のみ、ROS2の初期化処理を行う
            if (ros2Unity.Ok())
            {
                ros2Node = ros2Unity.CreateNode("UnityJoyNode");
                twist_pub = ros2Node.CreatePublisher<geometry_msgs.msg.Twist>("/phone/cmd_vel");
                speed_pub = ros2Node.CreatePublisher<std_msgs.msg.Bool>("/phone/low_speed");
                accel_pub = ros2Node.CreatePublisher<std_msgs.msg.Bool>("/phone/low_accel");
            }
        }

        // スライダーの初期値を設定
        if (linearSpeedSlider != null)
        {
            linearSpeedSlider.value = linearSpeed;
            // 元々の最大値を保存
            initialMaxLinearSpeed = linearSpeedSlider.maxValue;
            // スライダーの値変更時にテキストを更新するリスナーを追加
            linearSpeedSlider.onValueChanged.AddListener(UpdateLinearSpeedText);
        }
        if (angularSpeedSlider != null)
        {
            angularSpeedSlider.value = angularSpeed;
            // 元々の最大値を保存
            initialMaxAngularSpeed = angularSpeedSlider.maxValue;
            // スライダーの値変更時にテキストを更新するリスナーを追加
            angularSpeedSlider.onValueChanged.AddListener(UpdateAngularSpeedText);
        }

        // トグルボタンのリスナー登録
        if (lowSpeedToggle != null)
        {
            lowSpeedToggle.onValueChanged.AddListener(OnLowSpeedToggleChanged);
        }
        if (lowAccelToggle != null)
        {
            lowAccelToggle.onValueChanged.AddListener(OnAccelToggleChanged);
        }

        // 初期速度テキストの表示
        UpdateLinearSpeedText(linearSpeed);
        UpdateAngularSpeedText(angularSpeed);
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

        // メッセージをパブリッシュ
        twist_pub.Publish(msg);
    }

    /// <summary>
    /// 低加速トグルが変更されたときに呼び出されるメソッド
    /// </summary>
    /// <param name="isOn">トグルの新しい状態</param>
    public void OnAccelToggleChanged(bool isOn)
    {
        if (ros2Unity == null || !ros2Unity.Ok() || ros2Node == null || accel_pub == null || speed_pub == null)
        {
            Debug.LogWarning("ROS2 is not initialized. Cannot publish acceleration message.");
            return;
        }

        std_msgs.msg.Bool msg = new std_msgs.msg.Bool();
        msg.Data = isOn;

        speed_pub.Publish(msg);
        accel_pub.Publish(msg);
        Debug.Log($"Published acceleration toggle state: {msg.Data}");
    }

    /// <summary>
    /// 低速トグルが変更されたときに呼び出されるメソッド
    /// </summary>
    /// <param name="isOn">トグルの新しい状態</param>
    public void OnLowSpeedToggleChanged(bool isOn)
    {
        if (linearSpeedSlider != null)
        {
            // トグルがオンの場合、スライダーの最大値を速度乗数で調整
            linearSpeedSlider.maxValue = isOn ? initialMaxLinearSpeed * speedMultiplier : initialMaxLinearSpeed;
            // 現在の値を新しい最大値に合わせて調整
            linearSpeedSlider.value = Mathf.Min(linearSpeed, linearSpeedSlider.maxValue);
        }
        if (angularSpeedSlider != null)
        {
            // トグルがオンの場合、スライダーの最大値を速度乗数で調整
            angularSpeedSlider.maxValue = isOn ? initialMaxAngularSpeed * speedMultiplier : initialMaxAngularSpeed;
            // 現在の値を新しい最大値に合わせて調整
            angularSpeedSlider.value = Mathf.Min(angularSpeed, angularSpeedSlider.maxValue);
        }
    }

    /// <summary>
    /// 線形速度スライダーの値が変更されたときにテキストを更新する
    /// </summary>
    /// <param name="value">スライダーの新しい値</param>
    private void UpdateLinearSpeedText(float value)
    {
        linearSpeed = value;
        if (linearSpeedText != null) linearSpeedText.text = value.ToString("F2");
    }

    /// <summary>
    /// 角速度スライダーの値が変更されたときにテキストを更新する
    /// </summary>
    /// <param name="value">スライダーの新しい値</param>
    private void UpdateAngularSpeedText(float value)
    {
        angularSpeed = value;
        if (angularSpeedText != null) angularSpeedText.text = value.ToString("F2");
    }
}