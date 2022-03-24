using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {
    public static LevelManager instance;

    enum GamePhase {
        Insurance, // purchase insurance
        Main, // active gameplay
        Limbo // no more incoming waves
    }

    #region Level Data

    private LevelData m_levelData;
    private float p_fire, p_storm, p_flood, p_severe;
    private int n_butterflies;
    private TextAsset m_gridArrayTA;
    private int m_startFunds;
    private int m_numPeriods;
    private int m_periodFunds;
    private float m_periodTime;
    private float m_growthPerPeriod;
    private List<UIInsuranceMenu.Coverage> m_availableCoverages;
    private Nexus.SevereEffects m_severeEffects;

    #endregion // Level Data


    [SerializeField]
    private GameObject m_butterflyPrefab;
    [SerializeField]
    private TextMeshProUGUI[] m_forecastTexts;
    [SerializeField]
    private TextMeshProUGUI m_periodText;
    [SerializeField]
    private TextMeshProUGUI m_periodTimerText;
    [SerializeField]
    private TextMeshProUGUI m_fundsPerPeriodText;
    [SerializeField]
    private TextMeshProUGUI m_fundsText;
    [SerializeField]
    private Transform m_deluvianNexusHub, m_fireSwatheNexusHub, m_stormNexusHub;
    [SerializeField]
    private UIDeathMenu m_deathMenu;
    [SerializeField]
    private UILevelCompleteMenu m_levelCompleteMenu;
    [SerializeField]
    private UIQuitMenu m_quitMenu;

    #region Editor Coverage

    [SerializeField]
    private UIInsuranceMenu m_insuranceMenu;

    // the coverage available in the level
    private Dictionary<UIInsuranceMenu.InsuranceType, UIInsuranceMenu.Coverage> m_availableCoverageMap;

    // the coverage the player chooses to buy
    private List<UIInsuranceMenu.InsuranceType> m_insuranceSelections;
    private Dictionary<UIInsuranceMenu.InsuranceType, UIInsuranceMenu.Coverage> m_currCoverageDict;

    #endregion // Editor Coverage

    [SerializeField]
    private GameObject m_oncomerPrefab;
    [SerializeField]
    private GameObject m_stationPrefab;
    [SerializeField]
    private Transform m_stationHolder;
    [SerializeField]
    private Destination m_destination;
    [SerializeField]
    private HealthManager m_healthManager;

    private Station m_station;

    private GamePhase m_phase;

    private float p_fireTransform, p_stormTransform, p_floodTransform;
    private float m_periodTimer;
    private float m_butterflyTime;
    private float m_butterflyTimer;

    private int m_period;
    private float m_adjustedGrowth;
    private int m_funds;

    private int m_numOncomers;

    private void Awake() {
        if (instance == null) {
            instance = this;
        }
        else if (this != instance) {
            Debug.Log("Warning: multiple LevelManagers in the same scene. Undefined behavior may result.");
        }

        // Event Handlers
        EventManager.OnPurchaseInsuranceComplete.AddListener(HandlePurchaseInsuranceComplete);
        EventManager.OnDeath.AddListener(HandleDeath);
        EventManager.OnLevelComplete.AddListener(HandleLevelComplete);

        AudioManager.instance.PlayAudio("lark", true);

        m_insuranceSelections = new List<UIInsuranceMenu.InsuranceType>();
        m_numOncomers = 0;

        // Load LevelData
        LoadLevelData(GameDB.instance.GetLevelData(GameManager.instance.CurrLevelID));
    }

    private void Start() {
        // generate grid
        if (m_gridArrayTA != null) {
            TilemapManager.instance.LoadGridFromArray(m_gridArrayTA);
        }

        // set up text
        SetupText();

        // assign initial funds
        ModifyFunds(m_startFunds);

        // begin level at insurance phase
        m_phase = GamePhase.Insurance;
        m_insuranceMenu.Open();
    }

    private void LoadLevelData(LevelData data) {
        m_levelData = data;
        p_fire = data.PFire;
        p_storm = data.PStorm;
        p_flood = data.PFlood;
        p_severe = data.PSevere;
        n_butterflies = data.NumButterflies;
        m_startFunds = data.StartFunds;
        m_periodFunds = data.PeriodFunds;
        m_gridArrayTA = data.GridArrayTA;
        m_numPeriods = data.NumPeriods;
        m_periodTime = data.PeriodTime;
        m_growthPerPeriod = data.PeriodGrowth;
        m_availableCoverages = data.AvailableCoverages;
        m_severeEffects = data.SevereEffects;

        // Instantiate Station
        m_stationHolder.transform.position = data.StationPos;
        m_destination.transform.position = data.StationPos;
        m_station = Instantiate(m_stationPrefab, m_stationHolder).GetComponent<Station>();

        // Move nexuses
        m_fireSwatheNexusHub.transform.position = data.NexusHubsPos[0];
        m_stormNexusHub.transform.position = data.NexusHubsPos[1];
        m_deluvianNexusHub.transform.position = data.NexusHubsPos[2];
    }

    private void SetupText() {
        if (p_fire > 0) {
            p_fireTransform = 1 - Mathf.Pow((1 - p_fire), 1.0f / n_butterflies);
        }
        else {
            p_fireTransform = 0;
        }
        if (p_storm > 0) {
            p_stormTransform = 1 - Mathf.Pow((1 - p_storm), 1.0f / n_butterflies);
        }
        else {
            p_stormTransform = 0;
        }
        if (p_flood > 0) {
            p_floodTransform = 1 - Mathf.Pow((1 - p_flood), 1.0f / n_butterflies);
        }
        else {
            p_floodTransform = 0;
        }

        m_periodTimer = m_periodTime;
        m_butterflyTime = m_periodTime / n_butterflies;
        m_butterflyTimer = m_butterflyTime;

        m_forecastTexts[0].text = "Hurricane: " + (p_storm * 100) + "%";
        m_forecastTexts[1].text = "Wildfire: " + (p_fire * 100) + "%";
        m_forecastTexts[2].text = "Flood: " + (p_flood * 100) + "%";

        m_period = 0;
        m_periodText.text = "Period: 1";
        m_periodTimerText.text = m_periodTime.ToString("F1") + " s";
        m_fundsPerPeriodText.text = "+$" + m_periodFunds + " per period";
        m_adjustedGrowth = 1;
    }

    private void Update() {
        GetDebugInputs();

        if (GameManager.instance.IsPaused) {
            return;
        }

        GetLevelInputs();

        switch (m_phase) {
            case GamePhase.Insurance:
                UpdateInsurancePhase();
                break;
            case GamePhase.Main:
                UpdateMainPhase();
                break;
            case GamePhase.Limbo:
                UpdateLimboPhase();
                break;
            default:
                break;
        }
    }

    private void UpdateMainPhase() {
        m_periodTimer -= Time.deltaTime;

        if (m_periodTimer <= 0) {
            // End Period
            m_period++;

            // Add funds
            ModifyFunds(m_periodFunds);

            if (m_period >= m_numPeriods) {
                m_phase = GamePhase.Limbo;
                UpdateLimboPhase();
                m_periodText.text = "In Limbo";
                m_periodTimerText.text = "Indefinite";
                m_fundsPerPeriodText.text = "";
                return;
            }

            m_periodText.text = "Period: " + (m_period + 1);
            m_adjustedGrowth = 1 + m_period * m_growthPerPeriod;
            m_periodTimer = m_periodTime;

            foreach (UIInsuranceMenu.InsuranceType key in m_currCoverageDict.Keys) {
                ModifyFunds(-(int)m_currCoverageDict[key].Premium);
            }
        }
        m_periodTimerText.text = m_periodTimer.ToString("F1") + " s";

        GenerateButterflies();
    }

    private void UpdateLimboPhase() {
        // after last period, wait for all oncomers to be destroyed
        if (m_numOncomers <= 0) {
            // End level
            EventManager.OnLevelComplete.Invoke();
            return;
        }
    }

    private void CheckEndLevel() {
        if (m_period >= m_numPeriods) {
            // End level
            if (m_numOncomers <= 0) {
                EventManager.OnLevelComplete.Invoke();
            }
            return;
        }
    }

    private void GenerateButterflies() {
        m_butterflyTimer -= Time.deltaTime;
        if (m_butterflyTimer <= 0) {
            m_butterflyTimer = m_butterflyTime;

            GameObject butterfly;
            NexusButterfly nexusB;

            // fire
            if (p_fire > 0) {
                butterfly = Instantiate(m_butterflyPrefab);
                nexusB = butterfly.GetComponent<NexusButterfly>();
                nexusB.GetComponent<SpriteRenderer>().color = GameDB.instance.GetNexusColor(Nexus.Type.FireSwathe);
                nexusB.SetFields(p_fireTransform, p_severe, Nexus.Type.FireSwathe, m_adjustedGrowth);
                nexusB.ManualAwake();
            }

            // flood
            if (p_flood > 0) {
                butterfly = Instantiate(m_butterflyPrefab);
                nexusB = butterfly.GetComponent<NexusButterfly>();
                nexusB.GetComponent<SpriteRenderer>().color = GameDB.instance.GetNexusColor(Nexus.Type.Deluvian);
                nexusB.SetFields(p_floodTransform, p_severe, Nexus.Type.Deluvian, m_adjustedGrowth);
                nexusB.ManualAwake();
            }

            // tempest
            if (p_storm > 0) {
                butterfly = Instantiate(m_butterflyPrefab);
                nexusB = butterfly.GetComponent<NexusButterfly>();
                nexusB.GetComponent<SpriteRenderer>().color = GameDB.instance.GetNexusColor(Nexus.Type.Storm);
                nexusB.SetFields(p_stormTransform, p_severe, Nexus.Type.Storm, m_adjustedGrowth);
                nexusB.ManualAwake();
            }
        }
    }

    private void OnDestroy() {
        // Event Handlers
        EventManager.OnPurchaseInsuranceComplete.RemoveListener(HandlePurchaseInsuranceComplete);
    }

    private void UpdateInsurancePhase() {

    }

    public bool CheckFunds(int cost) {
        return cost <= m_funds;
    }

    public bool AttemptPurchase(int cost) {
        if (cost > m_funds) {
            return false;
        }

        ModifyFunds(-cost);
        return true;
    }

    private void ModifyFunds(int change) {
        m_funds += change;
        m_fundsText.text = "$" + m_funds;
    }

    public void DamageStation(float dmg, Oncomer.Type type) {
        m_station.ApplyDamage(dmg, type);
    }

    public void SetInsuranceSelections(List<UIInsuranceMenu.Coverage> insuranceSelections) {
        if (m_currCoverageDict == null) {
            m_currCoverageDict = new Dictionary<UIInsuranceMenu.InsuranceType, UIInsuranceMenu.Coverage>();
        }
        else {
            m_currCoverageDict.Clear();
        }

        foreach (UIInsuranceMenu.Coverage coverage in insuranceSelections) {
            m_currCoverageDict.Add(coverage.Type, coverage);
        }
    }

    public Dictionary<UIInsuranceMenu.InsuranceType, UIInsuranceMenu.Coverage> GetInsuranceSelections() {
        return m_currCoverageDict;
    }

    #region Event Handlers

    void HandlePurchaseInsuranceComplete() {
        m_phase = GamePhase.Main;

        // Pay for insurance 
        foreach (UIInsuranceMenu.InsuranceType key in m_currCoverageDict.Keys) {
            ModifyFunds(-(int)m_currCoverageDict[key].Premium);
        }

        float floodInsuranceAmt = m_currCoverageDict.ContainsKey(UIInsuranceMenu.InsuranceType.Flood) ?
            m_currCoverageDict[UIInsuranceMenu.InsuranceType.Flood].MaxCoverage : 0;
        float fireInsuranceAmt = m_currCoverageDict.ContainsKey(UIInsuranceMenu.InsuranceType.Fire) ?
            m_currCoverageDict[UIInsuranceMenu.InsuranceType.Fire].MaxCoverage : 0;
        float stormInsuranceAmt = m_currCoverageDict.ContainsKey(UIInsuranceMenu.InsuranceType.Storm) ?
            m_currCoverageDict[UIInsuranceMenu.InsuranceType.Storm].MaxCoverage : 0;
        float umbrellaInsuranceAmt = m_currCoverageDict.ContainsKey(UIInsuranceMenu.InsuranceType.Umbrella) ?
    m_currCoverageDict[UIInsuranceMenu.InsuranceType.Umbrella].MaxCoverage : 0;

        m_station.InitHealth(m_healthManager, m_levelData.StartHealth, floodInsuranceAmt, fireInsuranceAmt, stormInsuranceAmt, umbrellaInsuranceAmt);
    }

    void HandleDeath() {
        m_deathMenu.Open();

        // pause game
        GameManager.instance.IsPaused = true;
    }

    void HandleLevelComplete() {
        // display level complete UI
        m_levelCompleteMenu.Open();

        // pause game
        GameManager.instance.IsPaused = true;
    }

    #endregion

    private void GetLevelInputs() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            // open up quit menu
            m_quitMenu.Open();
        }
    }

    #region Debug

    private void GetDebugInputs() {
        if ((Input.GetKeyDown(KeyCode.E) && Input.GetKey(KeyCode.LeftShift))
            || (Input.GetKey(KeyCode.E) && Input.GetKeyDown(KeyCode.LeftShift))) {
            // Spawn one of each enemy

            if (p_flood > 0) { InstantiateDebugOncomer(Nexus.Type.Deluvian); }
            if (p_fire > 0) { InstantiateDebugOncomer(Nexus.Type.FireSwathe); }
            if (p_storm > 0) { InstantiateDebugOncomer(Nexus.Type.Storm); }
        }

        if ((Input.GetKeyDown(KeyCode.P) && Input.GetKey(KeyCode.LeftShift))
        || (Input.GetKey(KeyCode.P) && Input.GetKeyDown(KeyCode.LeftShift))) {
            // Toggle Pause

            GameManager.instance.IsPaused = !(GameManager.instance.IsPaused);
        }
    }

    private void InstantiateDebugOncomer(Nexus.Type nexusType) {
        GameObject oncomerObj = Instantiate(m_oncomerPrefab);
        oncomerObj.transform.position = TilemapManager.instance.GetNexusHubTransform(nexusType).position;
        Oncomer oncomer = oncomerObj.GetComponent<Oncomer>();

        switch (nexusType) {
            case Nexus.Type.Storm:
                oncomer.OncomerData = GameDB.instance.GetOncomerData(Oncomer.Type.Tempest);
                break;
            case Nexus.Type.Deluvian:
                oncomer.OncomerData = GameDB.instance.GetOncomerData(Oncomer.Type.Flood);
                break;
            case Nexus.Type.FireSwathe:
                oncomer.OncomerData = GameDB.instance.GetOncomerData(Oncomer.Type.Wildfire);
                break;
            default:
                Debug.Log("Unknown type of nexus. Unable to spawn oncomer.");
                Destroy(oncomerObj);
                break;
        }

        oncomer.ManualAwake();
    }

    #endregion

    #region Coverage

    public UIInsuranceMenu.Coverage GetCoverage(UIInsuranceMenu.InsuranceType type) {
        // initialize the map if it does not exist
        if (instance.m_availableCoverageMap == null) {
            instance.m_availableCoverageMap = new Dictionary<UIInsuranceMenu.InsuranceType, UIInsuranceMenu.Coverage>();
            foreach (UIInsuranceMenu.Coverage c in instance.m_availableCoverages) {
                instance.m_availableCoverageMap.Add(c.Type, c);
            }
        }
        if (instance.m_availableCoverageMap.ContainsKey(type)) {
            return instance.m_availableCoverageMap[type];
        }
        else {
            throw new KeyNotFoundException(string.Format("No Coverage " +
                "with type `{0}' is in the level database of available coverages", type
            ));
        }
    }

    public List<UIInsuranceMenu.Coverage> GetAvailableCoverages() {
        return m_availableCoverages;
    }

    #endregion

    public Nexus.SevereEffects GetSevereEffects() {
        return m_severeEffects;
    }

    public void RegisterOncomer() {
        m_numOncomers++;
    }

    public void RemoveOncomer() {
        m_numOncomers--;
    }

    #region SevereWeatherTrigger

    public string GetCurrLevelID() {
        return m_levelData.ID;
    }

    public bool TriggerConditionsMet(TutorialManager.SevereWeatherTrigger trigger) {
        if (m_period == trigger.Period - 1 && m_periodTimer <= trigger.Time) {
            return true;
        }
        return false;
    }

    #endregion // SevereWeatherTrigger
}
