using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HAVIGAME.Audios;

[RequireComponent(typeof(Toggle))]
public class MusicToggle : MonoBehaviour {
    private Toggle toggle;

    private void Awake() {
        toggle = GetComponent<Toggle>();
    }

    private void Start() {
        toggle.isOn = AudioManager.Instance.MusicVolume > 0;
        toggle.onValueChanged.AddListener(OnValueChanged);
    }

    private void OnDestroy() {
        toggle.onValueChanged.RemoveListener(OnValueChanged);
    }

    private void OnValueChanged(bool value) {
        AudioManager.Instance.MusicVolume = value ? 1 : 0;
    }
}
