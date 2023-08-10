using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Toggle))]
public class ToggleLabel : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI txtLabel;
    [SerializeField] private string isOnText = "ON";
    [SerializeField] private Color isOnColor = Color.black;
    [SerializeField] private string isOffText = "OFF";
    [SerializeField] private Color isOffColor = Color.white;

    private Toggle toggle;

    private void Awake() {
        toggle = GetComponent<Toggle>();
    }

    private void Start() {
        OnValueChanged(toggle.isOn);
        toggle.onValueChanged.AddListener(OnValueChanged);
    }

    private void OnDestroy() {
        toggle.onValueChanged.RemoveListener(OnValueChanged);
    }

    private void OnValueChanged(bool value) {
        txtLabel.text = value ? isOnText : isOffText;
        txtLabel.color = value ? isOnColor : isOffColor;
    }
}
