using HAVIGAME;
using HAVIGAME.Scenes;
using HAVIGAME.Services.IAU;

[System.Serializable]
public class GameLauncher : Launcher {
    public override void Launch() {
        if(!IAUManager.CheckForUpdate(OnCheckForUpdateCompleted, OnCheckForUpdateFailed)) {
            LaunchGame();
        }
    }

    private void OnCheckForUpdateCompleted(IAUManager.UpdateHandle handle) {
        if (handle.UpdateAvailability == IAUManager.UpdateAvailability.UpdateAvailable) {
            handle.StartUpdate(OnStartUpdateCallback);
        }
    }

    private void OnCheckForUpdateFailed(string error) {
        LaunchGame();
    }

    private void OnStartUpdateCallback(bool isCompleted) {
        LaunchGame();
    }

    private void LaunchGame() {
        GameData.Initialize();
        ScenesManager.Instance.LoadSceneAsyn(GameScene.ByIndex.Home);
    }
}
