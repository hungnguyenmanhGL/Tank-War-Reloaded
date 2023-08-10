using Com.LuisPedroFonseca.ProCamera2D;
using HAVIGAME;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : Singleton<CameraController> {
    [SerializeField] private ProCamera2D proCamera;
    [SerializeField] private ProCamera2DNumericBoundaries proCameraBoundaries;
    [SerializeField] private ProCamera2DShake proCameraShake;
    [SerializeField] private CameraMode cameraMode = CameraMode.Follow;
    [SerializeField] private List<CameraTarget> followTargets = new List<CameraTarget>();
    [SerializeField] private List<CameraTarget> freeTargets = new List<CameraTarget>();

    public float CameraSize => MainCamera.orthographicSize;
    public Camera MainCamera => proCamera.GameCamera;
    public CameraMode CameraMode => cameraMode;

    public void ChangeToFollowMode() {
        cameraMode = CameraMode.Follow;
        SyncCameraTargets();
    }

    public void ChangeToFreeMode() {
        cameraMode = CameraMode.Free;
        SyncCameraTargets();
    }

    public void SyncCameraTargets() {
        proCamera.CameraTargets.Clear();

        switch (CameraMode) {
            case CameraMode.Free:
                proCamera.CameraTargets.AddRange(freeTargets);
                break;
            case CameraMode.Follow:
                proCamera.CameraTargets.AddRange(followTargets);
                break;
        }
    }

    public void AddTarget(Transform target, CameraMode mode, float influence = 1, float offsetX = 0, float offsetY = 0) {
        CameraTarget cameraTarget = new CameraTarget() { TargetTransform = target, TargetInfluence = influence, TargetOffset = new Vector2(offsetX, offsetY) };

        switch (mode) {
            case CameraMode.Free:
                freeTargets.Add(cameraTarget);
                break;
            case CameraMode.Follow:
                followTargets.Add(cameraTarget);
                break;
        }

        SyncCameraTargets();
    }

    public void RemoveTarget(Transform target, CameraMode mode) {
        switch (mode) {
            case CameraMode.Free:
                for (int i = 0; i < freeTargets.Count; i++) {
                    if (freeTargets[i].TargetTransform == target) {
                        freeTargets.RemoveAt(i);
                        break;
                    }
                }
                break;
            case CameraMode.Follow:
                for (int i = 0; i < followTargets.Count; i++) {
                    if (followTargets[i].TargetTransform == target) {
                        followTargets.RemoveAt(i);
                        break;
                    }
                }
                break;
        }

        SyncCameraTargets();
    }

    public void SetTarget(CameraMode mode, params Transform[] targets) {
        switch (mode) {
            case CameraMode.Free:
                freeTargets.Clear();

                foreach (var target in targets) {
                    CameraTarget cameraTarget = new CameraTarget() { TargetTransform = target };
                    freeTargets.Add(cameraTarget);
                }
                break;
            case CameraMode.Follow:
                followTargets.Clear();

                foreach (var target in targets) {
                    CameraTarget cameraTarget = new CameraTarget() { TargetTransform = target };
                    followTargets.Add(cameraTarget);
                }
                break;
        }

        SyncCameraTargets();
    }

    public void ClearTargets(CameraMode mode) {
        switch (mode) {
            case CameraMode.Free:
                freeTargets.Clear();
                break;
            case CameraMode.Follow:
                followTargets.Clear();
                break;
        }

        SyncCameraTargets();
    }

    public void CenterOnTargets() {
        proCamera.CenterOnTargets();
    }

    public void SetBound(float leftBoundary, float rightBoundary, float topBoundary, float bottomBoundary) {
        SetBoundState(true);
        proCameraBoundaries.LeftBoundary = leftBoundary;
        proCameraBoundaries.RightBoundary = rightBoundary;
        proCameraBoundaries.TopBoundary = topBoundary;
        proCameraBoundaries.BottomBoundary = bottomBoundary;
    }

    public void SetBoundState(bool enable) {
        proCameraBoundaries.UseLeftBoundary = enable;
        proCameraBoundaries.UseRightBoundary = enable;
        proCameraBoundaries.UseTopBoundary = enable;
        proCameraBoundaries.UseBottomBoundary = enable;
    }

    public void SetBoundState(bool useLeftBoundary, bool useRightBoundary, bool useTopBoundary, bool useBottomBoundary) {
        proCameraBoundaries.UseLeftBoundary = useLeftBoundary;
        proCameraBoundaries.UseRightBoundary = useRightBoundary;
        proCameraBoundaries.UseTopBoundary = useTopBoundary;
        proCameraBoundaries.UseBottomBoundary = useBottomBoundary;
    }

    public void Zoom(float zoomAmount, float duration = 1f) {
        proCamera.Zoom(zoomAmount - CameraSize, duration);
    }

    public void ResetSize() {
        proCamera.ResetSize();
    }
    
    public void ShakeCamera(ShakePreset preset) {
        proCameraShake.Shake(preset);
    }
}

public enum CameraMode {
    Free,
    Follow,
}