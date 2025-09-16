using UnityEngine;
using UnityEngine.UI;

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

    private void Start()
    {
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
    }
}