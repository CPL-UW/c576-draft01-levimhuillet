using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WinsuranceAnalytics;

public class AnalyticsManager : MonoBehaviour
{
    public static AnalyticsManager Instance;

    private void OnEnable() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (Instance != this) {
            Destroy(this.gameObject);
        }
    }

    private void Start() {
        EventManager.OnPurchaseInsuranceComplete.AddListener(HandlePurchaseInsuranceComplete);
        EventManager.OnLevelStart.AddListener(HandleLevelStart);
        EventManager.OnDeath.AddListener(HandleDeath);
        EventManager.OnLevelComplete.AddListener(HandleLevelComplete);
        EventManager.OnLevelQuit.AddListener(HandleLevelQuit);
    }

    private void OnDisable() {
        WinsuranceAnalytics.Close();
    }

    #region Event Handlers

    private void HandlePurchaseInsuranceComplete() {
        WinsuranceAnalytics.ReportEvent("dummyId", "purchase-insurance-complete", LevelManager.instance.GetCurrLevelID());
    }
    private void HandleLevelStart() {
        WinsuranceAnalytics.ReportEvent("dummyId", "level-start", LevelManager.instance.GetCurrLevelID());
    }

    private void HandleDeath() {
        WinsuranceAnalytics.ReportEvent("dummyId", "level-death", LevelManager.instance.GetCurrLevelID());
    }

    private void HandleLevelComplete() {
        WinsuranceAnalytics.ReportEvent("dummyId", "level-complete", LevelManager.instance.GetCurrLevelID());
    }

    private void HandleLevelQuit() {
        WinsuranceAnalytics.ReportEvent("dummyId", "level-quit", LevelManager.instance.GetCurrLevelID());
    }

    #endregion // Event Handlers
}
