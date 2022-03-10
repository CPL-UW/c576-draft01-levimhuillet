using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Station : MonoBehaviour
{
    [SerializeField]
    private HealthManager m_healthManager;

    public void ApplyDamage(float dmg) {
        m_healthManager.ModifyHealth(-dmg);
    }

    public void InitHealth(float startBase, float startInsurance) {
        m_healthManager.InitFields(startBase, startInsurance);
    }
}
