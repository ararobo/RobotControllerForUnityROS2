using UnityEngine;
using UnityEngine.InputSystem;
using ROS2;
using UnityEngine.UI;
using TMPro;
using std_msgs.msg;
using UnityEngine.EventSystems; // Event Triggerを使用するために必要

public class UnderHandController : MonoBehaviour
{
    private ROS2UnityComponent ros2Unity;
    private ROS2Node ros2Node;
    private IPublisher<std_msgs.msg.Float32> under_hand_slide_pub;
    private IPublisher<std_msgs.msg.Float32> under_hand_raise_pub;

    // 個別の速度設定
    public float maxHandSlideSpeed = 1.0f;
    public float maxHandRaiseSpeed = 1.0f;

    // UIスライダーとテキスト
    public Slider handSlideSpeedSlider;
    public TMPro.TextMeshProUGUI handSlideSpeedText;
    public Slider handRaiseSpeedSlider;
    public TMPro.TextMeshProUGUI handRaiseSpeedText;

    // GUI入力用の変数
    private float currentHandSlide = 0.0f;
    private float currentHandRaise = 0.0f;

    // UIボタンを割り当てるためのパブリック変数
    public Button handSlideForwardButton;
    public Button handSlideBackwardButton;
    public Button handRaiseUpButton;
    public Button handRaiseDownButton;

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
                ros2Node = ros2Unity.CreateNode("UnityUnderHandNode");
                under_hand_slide_pub = ros2Node.CreatePublisher<std_msgs.msg.Float32>("/under_hand/slide");
                under_hand_raise_pub = ros2Node.CreatePublisher<std_msgs.msg.Float32>("/under_hand/raise");
            }
        }
        else
        {
            Debug.LogError("ROS2UnityComponent not found on this GameObject.");
        }

        // スライダーの初期値を設定
        if (handSlideSpeedSlider != null)
        {
            handSlideSpeedSlider.value = maxHandSlideSpeed;
            handSlideSpeedSlider.onValueChanged.AddListener(OnHandSlideSpeedSliderChanged);
        }
        if (handRaiseSpeedSlider != null)
        {
            handRaiseSpeedSlider.value = maxHandRaiseSpeed;
            handRaiseSpeedSlider.onValueChanged.AddListener(OnHandRaiseSpeedSliderChanged);
        }

        // ボタンにイベントを登録
        // Event Triggerをコードで設定
        SetupButtonEvents(handSlideForwardButton, 1.0f, ButtonType.Slide);
        SetupButtonEvents(handSlideBackwardButton, -1.0f, ButtonType.Slide);
        SetupButtonEvents(handRaiseUpButton, 1.0f, ButtonType.Raise);
        SetupButtonEvents(handRaiseDownButton, -1.0f, ButtonType.Raise);
    }

    // スライダーの値が変更されたときに呼び出されるメソッド
    private void OnHandSlideSpeedSliderChanged(float value)
    {
        maxHandSlideSpeed = value;
        if (handSlideSpeedText != null)
        {
            handSlideSpeedText.text = maxHandSlideSpeed.ToString("F2");
        }
    }

    private void OnHandRaiseSpeedSliderChanged(float value)
    {
        maxHandRaiseSpeed = value;
        if (handRaiseSpeedText != null)
        {
            handRaiseSpeedText.text = maxHandRaiseSpeed.ToString("F2");
        }
    }

    // ボタンのイベントをセットアップするヘルパーメソッド
    private void SetupButtonEvents(Button button, float value, ButtonType type)
    {
        if (button == null) return;

        EventTrigger trigger = button.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = button.gameObject.AddComponent<EventTrigger>();
        }

        // PointerDownイベントを追加
        EventTrigger.Entry entryDown = new EventTrigger.Entry();
        entryDown.eventID = EventTriggerType.PointerDown;
        entryDown.callback.AddListener((eventData) => { OnButtonDown(value, type); });
        trigger.triggers.Add(entryDown);

        // PointerUpイベントを追加
        EventTrigger.Entry entryUp = new EventTrigger.Entry();
        entryUp.eventID = EventTriggerType.PointerUp;
        entryUp.callback.AddListener((eventData) => { OnButtonUp(type); });
        trigger.triggers.Add(entryUp);
    }

    // ボタンが押されたときに呼び出されるメソッド
    public void OnButtonDown(float value, ButtonType type)
    {
        if (type == ButtonType.Slide)
        {
            currentHandSlide = value;
        }
        else if (type == ButtonType.Raise)
        {
            currentHandRaise = value;
        }
    }

    // ボタンが離されたときに呼び出されるメソッド
    public void OnButtonUp(ButtonType type)
    {
        if (type == ButtonType.Slide)
        {
            currentHandSlide = 0.0f;
        }
        else if (type == ButtonType.Raise)
        {
            currentHandRaise = 0.0f;
        }
    }

    // フレーム更新時に呼び出される関数
    void Update()
    {
        // ROS2が準備できていない場合は何もしない
        if (ros2Unity == null || !ros2Unity.Ok() || ros2Node == null || under_hand_slide_pub == null || under_hand_raise_pub == null)
        {
            return;
        }

        // Float32メッセージを初期化
        std_msgs.msg.Float32 slide_msg = new std_msgs.msg.Float32();
        std_msgs.msg.Float32 raise_msg = new std_msgs.msg.Float32();

        // GUI入力用の変数に最大速度を適用
        slide_msg.Data = currentHandSlide * maxHandSlideSpeed;
        raise_msg.Data = currentHandRaise * maxHandRaiseSpeed;

        // メッセージをパブリッシュ
        if (under_hand_slide_pub != null)
        {
            under_hand_slide_pub.Publish(slide_msg);
        }
        if (under_hand_raise_pub != null)
        {
            under_hand_raise_pub.Publish(raise_msg);
        }
    }
}