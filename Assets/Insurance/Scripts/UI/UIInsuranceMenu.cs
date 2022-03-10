using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInsuranceMenu : MenuBase
{
    [SerializeField] private Button m_purchaseButton;
    [SerializeField] private Button m_refuseButton;

    void OnEnable() {
        m_purchaseButton.onClick.AddListener(HandlePurchase);
        m_refuseButton.onClick.AddListener(HandleRefuse);
    }

    void OnDisable() {
        m_purchaseButton.onClick.RemoveAllListeners();
        m_refuseButton.onClick.RemoveAllListeners();
    }

    public void Open() {
        base.OpenMenu();
    }

    void HandlePurchase() {
        base.CloseMenu();
        EventManager.OnPurchaseInsuranceComplete.Invoke(true);
    }

    void HandleRefuse() {
        base.CloseMenu();
        EventManager.OnPurchaseInsuranceComplete.Invoke(false);
    }
}
