using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIQuitMenu : MenuBase
{
    [SerializeField] Button m_returnButton;
    [SerializeField] Button m_continueButton;

    void OnEnable() {
        m_returnButton.onClick.AddListener(HandleReturnLevelSelect);
        m_continueButton.onClick.AddListener(HandleContinue);
    }

    void OnDisable() {
        m_returnButton.onClick.RemoveAllListeners();
        m_continueButton.onClick.RemoveAllListeners();
    }

    public void Open() {
        base.OpenMenu();
    }

    void HandleReturnLevelSelect() {
        base.CloseMenu();
        EventManager.OnLevelQuit.Invoke();
        AudioManager.instance.StopAudio();
        SceneManager.LoadScene("LevelSelect");
    }

    void HandleContinue() {
        base.CloseMenu();
    }
}
