using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPreserver : MonoBehaviour
{
    public static NetworkPreserver instance;
    [SerializeField] private List<GameObject> m_toPreserve;

    private void OnEnable() {
        if (instance == null) {
            instance = this;
        }
        else if (this != instance) {
            Destroy(this.gameObject);
            return;
        }

        DontDestroyOnLoad(this.gameObject);
        foreach(GameObject obj in m_toPreserve) {
            DontDestroyOnLoad(obj);
        }

        EventManager.OnLevelQuit.AddListener(HandleLevelComplete);
        EventManager.OnDeath.AddListener(HandleLevelComplete);
        EventManager.OnLevelComplete.AddListener(HandleLevelComplete);
    }

    private void HandleLevelComplete() {
        // Release the preserved
        foreach (GameObject obj in m_toPreserve) {
            Destroy(obj);
        }
    }
}
