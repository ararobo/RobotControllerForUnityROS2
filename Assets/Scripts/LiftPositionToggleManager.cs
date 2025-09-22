using UnityEngine;
using UnityEngine.UI;

public class LiftPositionToggleManager : MonoBehaviour
{
    // 上昇位置用のキャンバスをインスペクターからアタッチします。
    [SerializeField]
    private GameObject LiftBottonObject;

    // 下降位置用のキャンバスをインスペクターからアタッチします。
    [SerializeField]
    private GameObject LiftPosisionObject;

    // UIのトグルをインスペクターからアタッチします。
    [SerializeField]
    private Toggle liftPositionToggle;

    private void Start()
    {
        // アプリケーション起動時にトグルの状態を強制的にオフにします。
        liftPositionToggle.isOn = false;

        // トグルの現在の状態に基づいてキャンバスを初期設定します。
        OnToggleValueChanged(liftPositionToggle.isOn);

        // トグルの値が変更されたときに呼び出されるリスナーを設定します。
        liftPositionToggle.onValueChanged.AddListener(OnToggleValueChanged);
    }

    /// <summary>
    /// トグルの値が変更されたときに呼び出されるメソッド。
    /// </summary>
    /// <param name="isOn">トグルの現在の状態 (true: オン, false: オフ)</param>
    private void OnToggleValueChanged(bool isOn)
    {
        // トグルの状態に応じてキャンバスを切り替えます。
        // isOnがtrueの場合、LiftPosisionObjectをアクティブにし、LiftBottonObjectを非アクティブにします。
        // isOnがfalseの場合、LiftBottonObjectをアクティブにし、LiftPosisionObjectを非アクティブにします。
        LiftBottonObject.SetActive(!isOn);
        LiftPosisionObject.SetActive(isOn);
    }
}