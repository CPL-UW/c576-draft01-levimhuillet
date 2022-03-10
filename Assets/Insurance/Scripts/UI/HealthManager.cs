using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    [SerializeField]
    private Slider m_baseSlider;
    [SerializeField]
    private Slider m_insuranceSlider;

    private float m_currBaseHealth, m_totalBaseHealth;
    private float m_currInsuranceHealth, m_totalInsuranceHealth;
    private float m_totalWidth;
    private float m_baseLeftAnchorOffset;

    public void InitFields(float startBaseHealth, float startInsuranceHealth) {
        m_currBaseHealth = m_totalBaseHealth = startBaseHealth;
        m_currInsuranceHealth = m_totalInsuranceHealth = startInsuranceHealth;

        m_baseSlider.value = m_currBaseHealth > 0 ? 1 : 0;
        m_insuranceSlider.value = m_currInsuranceHealth > 0 ? 1 : 0;

        //float totalHealth = startBaseHealth + startInsuranceHealth;
        //m_insuranceSlider.value = m_currInsuranceHealth / totalHealth;
    }

    public void ModifyHealth(float change) {
        if (m_currBaseHealth > 0) {
            m_currBaseHealth += change;
            if (m_currBaseHealth > m_totalBaseHealth) {
                // cannot go above max health
                m_currBaseHealth = m_totalBaseHealth;
            }
            else if (m_currBaseHealth < 0) {
                // eat into insurance
                m_currInsuranceHealth += m_currBaseHealth;

                // set base health to 0
                m_currBaseHealth = 0;
            }
        }
        else {
            m_currInsuranceHealth += change;
            if (m_currInsuranceHealth > m_totalInsuranceHealth) {
                // cannot go above max health
                m_currInsuranceHealth = m_totalInsuranceHealth;
            }
            else if (m_currInsuranceHealth < 0) {
                // set insurance health to 0
                m_currInsuranceHealth = 0;

                // trigger death
                EventManager.OnDeath.Invoke();
            }
        }

        // Update sliders
        m_baseSlider.value = m_currBaseHealth / m_totalBaseHealth;
        m_insuranceSlider.value = m_currInsuranceHealth / m_totalInsuranceHealth;
    }
}
