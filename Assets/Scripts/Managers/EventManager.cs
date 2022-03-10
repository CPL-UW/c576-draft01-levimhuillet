using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour {
    public static EventManager instance;

    // Define game events below
    public static UnityEvent<bool> OnPurchaseInsuranceComplete;
    public static UnityEvent OnDeath;

    private void OnEnable() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (this != instance) {
            Destroy(this.gameObject);
        }

        OnPurchaseInsuranceComplete = new UnityEvent<bool>();
        OnDeath = new UnityEvent();
    }
}
