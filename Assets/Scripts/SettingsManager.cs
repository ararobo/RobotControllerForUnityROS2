using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshProを使用するため追加

public class SettingsManager : MonoBehaviour
{
    public GameObject settingsPanel;
    public Slider volumeSlider;
    public Toggle bgmToggle;
    // FPS設定用のドロップダウンを追加
    public TMP_Dropdown fpsDropdown;

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

        // ドロップダウンの初期設定
        if (fpsDropdown != null)
        {
            // オプションをクリア
            fpsDropdown.ClearOptions();
            // オプションを追加
            fpsDropdown.AddOptions(new System.Collections.Generic.List<string> { "30", "50", "60" });
            // ドロップダウンの初期値を現在のFPSに設定
            fpsDropdown.value = fpsDropdown.options.FindIndex(option => option.text == Application.targetFrameRate.ToString());
            // ドロップダウンの値が変更されたときのリスナーを追加
            fpsDropdown.onValueChanged.AddListener(OnFPSDropdownChanged);
        }

        // 初期FPSを設定
        SetFPS(30);
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

    // ドロップダウンの値が変更されたときに呼ばれるメソッド
    public void OnFPSDropdownChanged(int index)
    {
        int fps = int.Parse(fpsDropdown.options[index].text);
        SetFPS(fps);
    }

    // FPSを設定するメソッド
    private void SetFPS(int fps)
    {
        Application.targetFrameRate = fps;
        Debug.Log("FPS: " + fps);
    }
}