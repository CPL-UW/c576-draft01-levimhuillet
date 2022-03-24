using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InsuranceButton : MonoBehaviour {

    [SerializeField] private TextMeshProUGUI m_titleText;
    [SerializeField] private TextMeshProUGUI m_premiumText;
    [SerializeField] private TextMeshProUGUI m_deductibleText;

    public void SetText(string title, string premium, string deductible) {
        m_titleText.text = title;
        m_premiumText.text = "-$" + premium + " premium";
        m_deductibleText.text = "-$" + deductible + " deductible";
    }
}
