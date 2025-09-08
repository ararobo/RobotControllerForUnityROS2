using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VibrationManager : MonoBehaviour
{
    public static bool isVibrationEnabled = true;

    [Tooltip("バイブレーションを有効にするボタンをここにドラッグ＆ドロップしてください。")]
    public List<Button> vibrationButtons = new List<Button>();

    private void OnEnable()
    {
        // リスト内のすべてのボタンにイベントリスナーを追加
        foreach (Button button in vibrationButtons)
        {
            if (button != null)
            {
                // ポインターダウンとポインターアップのイベントをリスニング
                button.gameObject.AddComponent<VibrationButtonHandler>().SetVibrationManager(this);
            }
        }
    }

    private void OnDisable()
    {
        // シーンが終了する際にイベントリスナーを削除（不要な場合は省略可）
        foreach (Button button in vibrationButtons)
        {
            if (button != null)
            {
                var handler = button.gameObject.GetComponent<VibrationButtonHandler>();
                if (handler != null)
                {
                    Destroy(handler);
                }
            }
        }
    }

    // ボタンが押されたときに呼び出される
    public void OnButtonPointerDown()
    {
        if (isVibrationEnabled)
        {
#if UNITY_ANDROID || UNITY_IOS
            Handheld.Vibrate();
#endif
            Debug.Log("Vibrating...");
        }
    }

    // ボタンが離されたときに呼び出される
    public void OnButtonPointerUp()
    {
#if UNITY_ANDROID || UNITY_IOS
        Handheld.Vibrate(); // 1回の呼び出しでバイブレーションが停止するプラットフォームもあるため、再度呼び出し
#endif
        Debug.Log("Vibration stopped.");
    }
}

public class VibrationButtonHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private VibrationManager _vibrationManager;

    public void SetVibrationManager(VibrationManager manager)
    {
        _vibrationManager = manager;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_vibrationManager != null)
        {
            _vibrationManager.OnButtonPointerDown();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_vibrationManager != null)
        {
            _vibrationManager.OnButtonPointerUp();
        }
    }
}