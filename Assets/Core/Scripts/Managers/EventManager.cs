using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour {
    public static EventManager instance;

    // Define game events below
    public static UnityEvent OnPurchaseInsuranceComplete;
    public static UnityEvent OnDeath;
    public static UnityEvent OnLevelComplete;
    public static UnityEvent OnLevelQuit;

    private void OnEnable() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (this != instance) {
            Destroy(this.gameObject);
        }

        OnPurchaseInsuranceComplete = new UnityEvent();
        OnDeath = new UnityEvent();
        OnLevelComplete = new UnityEvent();
        OnLevelQuit = new UnityEvent();
    }
}
