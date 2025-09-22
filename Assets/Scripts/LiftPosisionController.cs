using UnityEngine;
using UnityEngine.InputSystem;
using ROS2;
using UnityEngine.UI;
using TMPro; // TextMeshProを使用するために必要なライブラリを追加
using UnityEngine.EventSystems; // Event Triggerを使用するために必要

public class LiftPosisionController : MonoBehaviour
{
    private ROS2UnityComponent ros2Unity;
    private ROS2Node ros2Node;
    private IPublisher<std_msgs.msg.Float32> lift_pub;

    // リフトの最大速度を設定 (この変数はポジション制御では使用しません)
    public float maxLiftSpeed = 1.0f;

    // UIスライダーを割り当てるためのパブリック変数
    public Slider liftPositionSlider;

    // スライダーの値表示用のテキスト
    public TMPro.TextMeshProUGUI liftPositionText;

    // ボタンのUI要素を割り当てるためのパブリック変数
    public Button upButton;
    public Button downButton;

    // 目標ポジションを保持する変数
    private float targetPosition = 0.0f;

    void Start()
    {
        // ROS2Unityコンポーネントの取得を試みる
        if (TryGetComponent(out ros2Unity))
        {
            // 取得に成功した場合のみ、ROS2の初期化処理を行う
            if (ros2Unity.Ok())
            {
                ros2Node = ros2Unity.CreateNode("UnityLiftNode");
                // トピック名と型は元のまま
                lift_pub = ros2Node.CreatePublisher<std_msgs.msg.Float32>("lift_vel");
            }
        }

        // スライダーの初期値を設定
        if (liftPositionSlider != null)
        {
            // スライダーの初期値を0.0fに設定
            liftPositionSlider.value = 0.0f;
            targetPosition = 0.0f;
            // スライダーの値が変更されたときのイベントリスナーを追加
            liftPositionSlider.onValueChanged.AddListener(OnSliderValueChanged);
            // ゲーム開始時のスライダーの初期値をテキストに反映
            OnSliderValueChanged(liftPositionSlider.value);
        }
    }

    // スライダーの値が変更されたときに呼び出されるメソッド
    private void OnSliderValueChanged(float value)
    {
        targetPosition = value;
        if (liftPositionText != null)
        {
            // スライダーの値をテキストに反映
            liftPositionText.text = targetPosition.ToString();
        }
    }

    // フレーム更新時に呼び出される関数
    void Update()
    {
        // ROS2が準備できていない場合は何もしない
        if (ros2Unity == null || !ros2Unity.Ok() || ros2Node == null || lift_pub == null)
        {
            return;
        }

        // 目標ポジションのメッセージを作成
        std_msgs.msg.Float32 msg = new std_msgs.msg.Float32();
        // スライダーの値をそのままメッセージのデータとして設定
        msg.Data = targetPosition;

        // メッセージをパブリッシュ
        lift_pub.Publish(msg);
    }
}