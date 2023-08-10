using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HAVIGAME.Audios;

[RequireComponent(typeof(Toggle))]
public class VibrateToggle : MonoBehaviour {
    private Toggle toggle;

    private void Awake() {
        toggle = GetComponent<Toggle>();
    }

    private void Start() {
        toggle.isOn = AudioManager.Instance.VibrateEnabled;
        toggle.onValueChanged.AddListener(OnValueChanged);
    }

    private void OnDestroy() {
        toggle.onValueChanged.RemoveListener(OnValueChanged);
    }

    private void OnValueChanged(bool value) {
        AudioManager.Instance.VibrateEnabled = value;
    }
}
