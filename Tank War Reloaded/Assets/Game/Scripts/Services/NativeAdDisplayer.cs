using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using HAVIGAME.Services.Advertisings;
using HAVIGAME;

public class NativeAdDisplayer : MonoBehaviour {
    [SerializeField] private string viewId;
    [SerializeField] private NativeAdElementView[] elementViews;
    [SerializeField] private Transform root;
    [SerializeField] private bool autoRefresh = true;
    [SerializeField] private float refreshTime = 30;
    [SerializeField] private bool delayRefreshUntilAdReady = true;
    [SerializeField] private Vector2 size = new Vector2(5, 3);

    private NativeAdView view;
    private Coroutine requestCoroutine;
    private Coroutine refreshCoroutine;

    public bool IsRequesting => requestCoroutine != null;
    public bool IsRefreshing => refreshCoroutine != null;
    public bool IsShowing => view != null && view.IsShowing;
    public Vector2 Size => size;
    public float MinX => transform.position.x;
    public float MaxX => transform.position.x + Size.x;
    public float MinY => transform.position.y;
    public float MaxY => transform.position.y + Size.y;

    public bool NativeAdReady {
        get {
#if UNITY_EDITOR || CHEAT
            return true;
#elif AD_DISABLE
            return false;
#else
            return GameAdvertising.IsNativeAdReady;
#endif
        }
    }

    private void Awake() {
        viewId += GetHashCode();
        view = new NativeAdView(viewId, elementViews);
    }

    private void Start() {
        bool isPremium = GameData.PlayerShop.IsPremium;

        if (isPremium) {
            Hide();
        }
        else {
            Show();
        }
    }

    private void OnDestroy() {
        Hide();
    }

    public void Show() {
        if (view != null && !view.IsShowing) {
            RequestAd();
        }
    }

    public void Hide() {
        StopRequestAd();
        StopRefreshAd();

        if (root) root.gameObject.SetActive(false);
        if (view != null && view.IsShowing) view.Hide();
    }

    public void Refresh() {
        Hide();

        bool isPremium = GameData.PlayerShop.IsPremium;

        if (isPremium) {
            return;
        }

        RequestAd();
    }

    private void RequestAd() {
        if (root) root.gameObject.SetActive(false);

#if UNITY_EDITOR
        if (root) root.gameObject.SetActive(true);

        if (autoRefresh) {
            refreshCoroutine = StartCoroutine(IERefreshAd());
        }
#else
        if (NativeAdReady && GameAdvertising.TryShowNativeAd(view)) {
            if (root) root.gameObject.SetActive(true);

            if (autoRefresh) {
                refreshCoroutine = StartCoroutine(IERefreshAd());
            }
        }
        else {
            if (root) root.gameObject.SetActive(false);
            requestCoroutine = StartCoroutine(IERequestAd());
        }
#endif
    }

    private void StopRequestAd() {
        if (IsRequesting) StopCoroutine(requestCoroutine);
    }

    private void StopRefreshAd() {
        if (IsRefreshing) StopCoroutine(refreshCoroutine);
    }

    private IEnumerator IERequestAd() {
        WaitForSeconds wait = Executor.Instance.WaitForSeconds(1);

        while (!NativeAdReady) {
            yield return wait;
        }

        RequestAd();

        requestCoroutine = null;
    }

    private IEnumerator IERefreshAd() {
        yield return Executor.Instance.WaitForSeconds(refreshTime);

        WaitForSeconds wait = Executor.Instance.WaitForSeconds(1);

        if (delayRefreshUntilAdReady) {
            while (!NativeAdReady) {
                yield return wait;
            }
        }

        refreshCoroutine = null;

        RequestAd();
    }
}
