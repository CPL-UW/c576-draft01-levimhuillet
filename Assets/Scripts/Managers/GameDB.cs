using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDB : MonoBehaviour {
    public static GameDB instance;

    private Dictionary<string, AudioData> m_audioMap;
    private Dictionary<Nexus.Type, NexusData> m_nexusMap;
    private Dictionary<Oncomer.Type, OncomerData> m_oncomerMap;
    private Dictionary<Tower.Type, TowerData> m_towerMap;

    #region Editor

    [SerializeField]
    private AudioData[] m_audioData;
    [SerializeField]
    private NexusData[] m_nexusData;
    [SerializeField]
    private OncomerData[] m_oncomerData;
    [SerializeField]
    private TowerData[] m_towerData;
    [SerializeField]
    private Color m_deluvianNexusColor;
    [SerializeField]
    private Color m_fireSwatheNexusColor;
    [SerializeField]
    private Color m_stormNexusColor;


    #endregion

    public AudioData GetAudioData(string id) {
        // initialize the map if it does not exist
        if (instance.m_audioMap == null) {
            instance.m_audioMap = new Dictionary<string, AudioData>();
            foreach (AudioData data in instance.m_audioData) {
                instance.m_audioMap.Add(data.ID, data);
            }
        }
        if (instance.m_audioMap.ContainsKey(id)) {
            return instance.m_audioMap[id];
        }
        else {
            throw new KeyNotFoundException(string.Format("No Audio " +
                "with id `{0}' is in the database", id
            ));
        }
    }

    public NexusData GetNexusData(Nexus.Type type) {
        // initialize the map if it does not exist
        if (instance.m_nexusMap == null) {
            instance.m_nexusMap = new Dictionary<Nexus.Type, NexusData>();
            foreach (NexusData data in instance.m_nexusData) {
                instance.m_nexusMap.Add(data.Type, data);
            }
        }
        if (instance.m_nexusMap.ContainsKey(type)) {
            return instance.m_nexusMap[type];
        }
        else {
            throw new KeyNotFoundException(string.Format("No Nexus " +
                "with type `{0}' is in the database", type
            ));
        }
    }

    public OncomerData GetOncomerData(Oncomer.Type type) {
        // initialize the map if it does not exist
        if (instance.m_oncomerMap == null) {
            instance.m_oncomerMap = new Dictionary<Oncomer.Type, OncomerData>();
            foreach (OncomerData data in instance.m_oncomerData) {
                instance.m_oncomerMap.Add(data.Type, data);
            }
        }
        if (instance.m_oncomerMap.ContainsKey(type)) {
            return instance.m_oncomerMap[type];
        }
        else {
            throw new KeyNotFoundException(string.Format("No Oncomer " +
                "with type `{0}' is in the database", type
            ));
        }
    }

    public TowerData GetTowerData(Tower.Type type) {
        // initialize the map if it does not exist
        if (instance.m_towerMap == null) {
            instance.m_towerMap = new Dictionary<Tower.Type, TowerData>();
            foreach (TowerData data in instance.m_towerData) {
                instance.m_towerMap.Add(data.Type, data);
            }
        }
        if (instance.m_towerMap.ContainsKey(type)) {
            return instance.m_towerMap[type];
        }
        else {
            throw new KeyNotFoundException(string.Format("No Tower " +
                "with type `{0}' is in the database", type
            ));
        }
    }

    public Color GetNexusColor(Nexus.Type type) {
        switch (type) {
            case Nexus.Type.Deluvian:
                return m_deluvianNexusColor;
            case Nexus.Type.FireSwathe:
                return m_fireSwatheNexusColor;
            case Nexus.Type.Storm:
                return m_stormNexusColor;
            default:
                return m_stormNexusColor;
        }
    }

    #region Unity Callbacks

    private void OnEnable() {
        if (instance == null) {
            instance = this;
        }
        else if (instance != this) {
            Destroy(this.gameObject);
        }
    }

    #endregion
}
