using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIInsuranceMenu : MenuBase {
    public enum InsuranceType {
        Flood,
        Fire,
        Storm,
        Umbrella,
        None
    }

    [Serializable]
    public struct Coverage {
        public InsuranceType Type;
        public string Title;
        public float Premium;
        public float Deductible;
        public float MaxCoverage;

        public Coverage(InsuranceType type, string title, float premium, float deductible, float maxCoverage) {
            Type = type;
            Title = title;
            Premium = premium;
            Deductible = deductible;
            MaxCoverage = maxCoverage;
        }
    }

    #region Editor Button Generator

    [SerializeField]
    private float m_colSpacing;
    [SerializeField]
    private float m_rowSpacing;
    [SerializeField]
    private int m_numCols;

    [SerializeField] private GameObject m_insuranceButtonPrefab;
    [SerializeField] private GameObject m_buttonHolder;

    private List<GameObject> m_buttonObjs;

    #endregion // Editor Button Generator

    private Button m_selectUmbrellaButton;

    private List<Button> m_selectButtons;

    [SerializeField] private Button m_confirmButton;
    [SerializeField] private TextMeshProUGUI m_detailsText;

    private List<Coverage> m_insuranceSelections;

    void OnEnable() {
        // TODO: create a coverage button for each coverage in LevelManager.GetCoverages()
        if (LevelManager.instance == null) {
            base.CloseMenu();
            return;
        }
        if (m_buttonObjs != null) {
            Cleanup();
        }
        GenerateButtons();

        if (m_insuranceSelections == null) {
            m_insuranceSelections = new List<Coverage>();
        }
        if (m_selectButtons == null) {
            m_selectButtons = new List<Button>();

            foreach(GameObject buttonObj in m_buttonObjs) {
                m_selectButtons.Add(buttonObj.GetComponent<Button>());
            }
        }

        m_confirmButton.onClick.AddListener(HandleConfirm);

        foreach (Button b in m_selectButtons) {
            b.onClick.AddListener(delegate { UpdateSelectColor(b); });
        }
        if (m_selectButtons.Count > 0) {
            m_detailsText.text = "Win-surance Coverage";
        }
        else {
            m_detailsText.text = "No Providers Available";
        }
    }

    void OnDisable() {
        if (LevelManager.instance != null) {
            Cleanup();
        }
    }

    public void Open() {
        base.OpenMenu();
    }

    #region Button Handlers

    void HandleSelect(Coverage coverage) {
        if (m_insuranceSelections.Contains(coverage)) {
            m_insuranceSelections.Remove(coverage);

            // check if umbrella insurance is still valid,
            if (m_selectUmbrellaButton != null 
                && m_insuranceSelections.Count == 1
                && m_insuranceSelections.Contains(LevelManager.instance.GetCoverage(InsuranceType.Umbrella))) {
                m_insuranceSelections.Clear();
                UpdateSelectColor(m_selectUmbrellaButton);
            }
        }
        else {
            // if umbrella, check that there is at least one other selected
            if (coverage.Type == InsuranceType.Umbrella) {
                if (m_selectUmbrellaButton != null
                    && m_insuranceSelections.Count < 1) {
                    UpdateSelectColor(m_selectUmbrellaButton);
                }
                else {
                    m_insuranceSelections.Add(coverage);
                }
            }
            // add normally
            else {
                m_insuranceSelections.Add(coverage);
            }
        }

        // TODO: open supporting checklist
    }

    void HandleConfirm() {
        base.CloseMenu();
        // TODO: may need to set these to coverages, not insurance types
        LevelManager.instance.SetInsuranceSelections(m_insuranceSelections);
        EventManager.OnPurchaseInsuranceComplete.Invoke();
    }

    void UpdateSelectColor(Button b) {
        Image bImage = b.GetComponent<Image>();

        // TODO: specify and load these colors externally
        if (bImage.color == Color.green) {
            bImage.color = Color.white;
        }
        else {
            bImage.color = Color.green;
        }
    }

    #endregion // Button Handlers


    #region Button Generator

    private void GenerateButtons() {
        if (m_buttonObjs == null) {
            m_buttonObjs = new List<GameObject>();
        }
        Cleanup();

        List<Coverage> coverages = LevelManager.instance.GetAvailableCoverages();

        int colIndex = 0;
        foreach (Coverage coverage in coverages) {
            // instantiate button
            GameObject insuranceButtonObj = Instantiate(m_insuranceButtonPrefab, m_buttonHolder.transform);

            // set spacing
            float horizSpacing = (colIndex % m_numCols) * m_colSpacing;
            float vertSpacing = (colIndex / m_numCols) * -m_rowSpacing;
            insuranceButtonObj.transform.position += new Vector3(horizSpacing, vertSpacing, 0);

            // assign scene and text
            Button insuranceButton = insuranceButtonObj.GetComponent<Button>();
            insuranceButton.onClick.AddListener(delegate { HandleSelect(coverage); });
            insuranceButtonObj.GetComponent<InsuranceButton>().SetText(coverage.Title, ("" + coverage.Premium), ("" + coverage.Deductible));

            // save to buttons
            m_buttonObjs.Add(insuranceButtonObj);
            if (coverage.Type == InsuranceType.Umbrella) {
                m_selectUmbrellaButton = insuranceButton;
            }

            // move to next column
            ++colIndex;
        }
    }

    private void Cleanup() {
        foreach (GameObject obj in m_buttonObjs) {
            Destroy(obj);
        }
        m_buttonObjs.Clear();
    }

    #endregion
}
