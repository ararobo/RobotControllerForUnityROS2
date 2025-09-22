using UnityEngine;
using UnityEngine.UI;
using ROS2;
using std_msgs.msg;

public class CanvasToggleManager : MonoBehaviour
{
    // 自動モード用のキャンバスをインスペクターからアタッチします。
    [SerializeField]
    private GameObject automaticCanvas;

    // マニュアルモード用のキャンバスをインスペクターからアタッチします。
    [SerializeField]
    private GameObject manualCanvas;

    // UIのトグルをインスペクターからアタッチします。
    [SerializeField]
    private Toggle canvasToggle;

    // ROS2関連の変数
    private ROS2UnityComponent ros2Unity;
    private ROS2Node ros2Node;
    private IPublisher<std_msgs.msg.Bool> canvas_toggle_pub;

    private void Start()
    {
        // ROS2の初期化
        ros2Unity = FindAnyObjectByType<ROS2UnityComponent>();
        if (ros2Unity != null)
        {
            ros2Node = ros2Unity.CreateNode("UnityCanvasToggleNode");
            canvas_toggle_pub = ros2Node.CreatePublisher<std_msgs.msg.Bool>("/phone/auto");
        }
        else
        {
            Debug.LogError("ROS2UnityComponent not found in the scene.");
        }

        // アプリケーション起動時に、トグルの現在の状態に基づいてキャンバスを初期設定します。
        OnToggleValueChanged(canvasToggle.isOn);

        // トグルの値が変更されたときに呼び出されるリスナーを設定します。
        canvasToggle.onValueChanged.AddListener(OnToggleValueChanged);
    }

    /// <summary>
    /// トグルの値が変更されたときに呼び出されるメソッド。
    /// </summary>
    /// <param name="isOn">トグルの現在の状態 (true: オン, false: オフ)</param>
    private void OnToggleValueChanged(bool isOn)
    {
        // トグルの状態に応じてキャンバスを切り替えます。
        // isOnがtrueの場合、automaticCanvasをアクティブにし、manualCanvasを非アクティブにします。
        // isOnがfalseの場合、manualCanvasをアクティブにし、automaticCanvasを非アクティブにします。
        automaticCanvas.SetActive(isOn);
        manualCanvas.SetActive(!isOn);

        // ROS2トピックにbool値をパブリッシュ
        if (canvas_toggle_pub != null)
        {
            std_msgs.msg.Bool msg = new std_msgs.msg.Bool();
            msg.Data = isOn;
            canvas_toggle_pub.Publish(msg);
            Debug.Log($"Published automatic mode state: {isOn}");
        }
    }
}