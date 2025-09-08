using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public GameObject settingsPanel;
    public Slider volumeSlider;
    public Toggle bgmToggle;
    public Toggle vibrationToggle; // バイブレーション設定用のToggleを追加

    void Start()
    {
        // 初期状態では設定パネルを非表示にする
        settingsPanel.SetActive(false);

        // スライダーとトグルの初期値を設定
        if (volumeSlider != null)
        {
            volumeSlider.value = 0.5f; // 例: 0.5に初期設定
        }
        if (bgmToggle != null)
        {
            bgmToggle.isOn = true; // 例: ONに初期設定
        }

        // バイブレーション設定の初期値を設定
        if (vibrationToggle != null)
        {
            // VibrationManagerの静的変数から現在の状態を取得
            vibrationToggle.isOn = VibrationManager.isVibrationEnabled;
        }
    }

    // 設定パネルの表示/非表示を切り替えるメソッド
    public void ToggleSettingsPanel()
    {
        bool isActive = settingsPanel.activeSelf;
        settingsPanel.SetActive(!isActive);
    }

    // 音量スライダーの値が変更されたときに呼ばれるメソッド
    public void OnVolumeChanged(float value)
    {
        Debug.Log("音量: " + value);
        // ここで実際のオーディオソースの音量を変更する処理を記述
        // 例: AudioListener.volume = value;
    }

    // BGMトグルの状態が変更されたときに呼ばれるメソッド
    public void OnBGM_ToggleChanged(bool isOn)
    {
        Debug.Log("BGM ON/OFF: " + isOn);
        // ここでBGMの再生/停止を切り替える処理を記述
    }

    // バイブレーションのトグル状態が変更されたときに呼ばれるメソッド
    public void OnVibrationToggleChanged(bool isOn)
    {
        // VibrationManagerの静的変数を変更して、バイブレーションを有効/無効にする
        VibrationManager.isVibrationEnabled = isOn;
        Debug.Log("バイブレーション ON/OFF: " + isOn);
    }
}