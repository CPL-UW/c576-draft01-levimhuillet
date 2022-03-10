using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager instance;

    private bool m_isPaused;
    private bool m_hasReadInfo;

    public bool IsPaused { get { return m_isPaused; } }
    public bool HasReadInfo {
        get { return m_hasReadInfo; }
    }
    public void SetHasReadInfo(bool hasRead) {
        m_hasReadInfo = hasRead;
    }

    #region Unity Callbacks

    private void OnEnable() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (this != instance) {
            Destroy(this.gameObject);
        }

        m_isPaused = false;
        m_hasReadInfo = false;
    }

    #endregion

}
