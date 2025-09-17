using UnityEngine;
using UnityEngine.UI;
using ROS2;
using UnityEngine.EventSystems; // Event Triggerを使用するために必要
using TMPro; // TextMeshProを使用するために必要なライブラリを追加

public class ModeController : MonoBehaviour
{
    private ROS2UnityComponent ros2Unity;
    private ROS2Node ros2Node;
    private IPublisher<std_msgs.msg.UInt8> mode_pub;

    // ボタンのUI要素を割り当てるためのパブリック変数
    public Button pylonButton;
    public Button sharedBButton;
    public Button sharedCUpButton;
    public Button sharedCDownLeftButton;
    public Button transportButton;

    void Start()
    {
        // ROS2Unityコンポーネントの取得
        if (TryGetComponent(out ros2Unity))
        {
            // ROS2の初期化
            if (ros2Unity.Ok())
            {
                ros2Node = ros2Unity.CreateNode("UnityModeNode");
                mode_pub = ros2Node.CreatePublisher<std_msgs.msg.UInt8>("mode_topic");
            }
        }

        // 各ボタンにイベントリスナーを登録
        RegisterButtonEvent(pylonButton, 1);
        RegisterButtonEvent(sharedBButton, 2);
        RegisterButtonEvent(sharedCUpButton, 3);
        RegisterButtonEvent(sharedCDownLeftButton, 4);
        RegisterButtonEvent(transportButton, 5);
    }

    /// <summary>
    /// ボタンにイベントリスナーを登録するヘルパーメソッド
    /// </summary>
    /// <param name="button">登録するボタン</param>
    /// <param name="modeValue">ボタンに対応するモード値</param>
    private void RegisterButtonEvent(Button button, byte modeValue)
    {
        if (button != null)
        {
            // ボタンのクリックイベントにパブリッシュ処理を追加
            button.onClick.AddListener(() => OnModeButtonClicked(modeValue));
        }
    }

    /// <summary>
    /// ボタンがクリックされたときに呼び出されるメソッド
    /// </summary>
    /// <param name="modeValue">送信するモード値</param>
    public void OnModeButtonClicked(byte modeValue)
    {
        // ROS2が準備できていない場合は何もしない
        if (ros2Unity == null || !ros2Unity.Ok() || ros2Node == null || mode_pub == null)
        {
            Debug.LogWarning("ROS2 is not initialized. Cannot publish message.");
            return;
        }

        std_msgs.msg.UInt8 msg = new std_msgs.msg.UInt8();
        msg.Data = modeValue;

        // メッセージをパブリッシュ
        mode_pub.Publish(msg);
        Debug.Log($"Published mode: {msg.Data}");
    }
}