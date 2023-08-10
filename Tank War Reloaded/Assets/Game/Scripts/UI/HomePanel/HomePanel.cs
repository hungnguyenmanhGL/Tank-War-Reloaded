using HAVIGAME.UI;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using HAVIGAME.Scenes;

public class HomePanel : UIFrame {
    [Header("[References]")]
    [SerializeField] private Button btnAdvertising;
    [SerializeField] private Button btnSetting;
    [SerializeField] private Button btnPlay;
    [SerializeField] private TextMeshProUGUI txtLevel;
    [SerializeField] private Button btnPreviousLevel;
    [SerializeField] private Button btnNextLevel;

    private int currentLevel;

    private void Start() {
        btnAdvertising.onClick.AddListener(OpenAdvertising);
        btnSetting.onClick.AddListener(OpenSettings);
        btnPlay.onClick.AddListener(PlayGame);
        btnPreviousLevel.onClick.AddListener(PreviousLevel);
        btnNextLevel.onClick.AddListener(NextLevel);
    }

    protected override void OnShow(bool instant = false) {
        base.OnShow(instant);

        int levelUnlocked = GameData.PlayerLevel.LevelUnlocked;
        ShowLevel(levelUnlocked);

    }

    protected override void OnBack() {
        OpenSettings();
    }

    private void ShowLevel(int level) {
        currentLevel = level;
        txtLevel.text = string.Format("LEVEL-{0}", level);

        btnPreviousLevel.gameObject.SetActive(currentLevel > 1);
        btnNextLevel.gameObject.SetActive(currentLevel < GameData.PlayerLevel.LevelUnlocked);
    }

    private void PreviousLevel() {
        ShowLevel(currentLevel - 1);
    }

    private void NextLevel() {
        ShowLevel(currentLevel + 1);
    }

    private void PlayGame() {
        int levelToPlay = currentLevel;
        GameSceneController.pendingLoadLevelOption = LoadLevelOption.Create(levelToPlay);
        ScenesManager.Instance.LoadSceneAsyn(GameScene.ByIndex.Game);
    }

    private void OpenSettings() {
        UIManager.Instance.Push<SettingPanel>();
    }

    private void OpenAdvertising() {
        UIManager.Instance.Push<AdvertisingPanel>();
    }
}
