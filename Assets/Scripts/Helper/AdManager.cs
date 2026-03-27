using UnityEngine;
using System;
using YG;

public class AdManager : MonoBehaviour
{
    public static AdManager Instance { get; private set; }

    [SerializeField] private float cooldownDuration = 120f;

    private const string LAST_AD_TIME_KEY = "LAST_AD_SHOW_TIME";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void TryShowAd()
    {
        if (!CheckCooldown())
        {
            return;
        }

        ShowAdInternal();
        SaveLastAdTime();
    }

    private bool CheckCooldown()
    {
        var lastShowTime = PlayerPrefs.GetFloat(LAST_AD_TIME_KEY);
        var currentTime = DateTime.Now.ToOADate();

        var timePassed = (currentTime - lastShowTime) * 24 * 60 * 60;

        if (timePassed < cooldownDuration)
        {
            return false;
        }

        return true;
    }

    private void ShowAdInternal()
    {
        YG2.InterstitialAdvShow();
        YG2.InterstitialAdvShow();
    }

    private void SaveLastAdTime()
    {
        PlayerPrefs.SetFloat(LAST_AD_TIME_KEY, (float)DateTime.Now.ToOADate());
        PlayerPrefs.Save();
    }
}