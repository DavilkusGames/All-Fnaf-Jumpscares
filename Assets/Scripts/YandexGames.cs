using System;
using System.Collections;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

public class YandexGames : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern bool SDKInit();

    [DllImport("__Internal")]
    private static extern void GameReady();

    [DllImport("__Internal")]
    private static extern void ShowAdv();

    [DllImport("__Internal")]
    private static extern void ShowRewarded();

    [DllImport("__Internal")]
    private static extern string GetLang();

    public static YandexGames Instance { get; private set; }
    public static bool IsInit { get; private set; }
    public static bool IsRus { get; private set; }

    public TranslateText[] translateTxts;
    public GameObject loadingPanel;
    private string[] RusLangDomens = { "ru", "be", "kk", "uk", "uz" };

    private Action rewardedCallback = null;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void Start()
    {
        if (!Application.isEditor)
        {
            loadingPanel.SetActive(true);
            StartCoroutine(nameof(WaitForSDKInit));
        }
    }

    private IEnumerator WaitForSDKInit()
    {
        yield return new WaitForSeconds(1.0f);
        while (!SDKInit()) yield return new WaitForSeconds(0.3f);
        yield return new WaitForSeconds(1.0f);
        IsInit = true;
        IsRus = RusLangDomens.Contains(GetLang());
        Debug.Log("IsRus: " + IsRus.ToString());
        for (int i = 0; i < translateTxts.Length; i++) translateTxts[i].Translate(IsRus);

        loadingPanel.SetActive(false);
        GameReady();
        yield return new WaitForSeconds(0.5f);
        ShowAdv();
    }

    public void ShowRewardedAd(Action callback)
    {
        if (Application.isEditor)
        {
            callback();
            return;
        }
        if (!IsInit) return;
        rewardedCallback = callback;
        ShowRewarded();
    }

    public void RewardedSuccess()
    {
        rewardedCallback();
    }
}
