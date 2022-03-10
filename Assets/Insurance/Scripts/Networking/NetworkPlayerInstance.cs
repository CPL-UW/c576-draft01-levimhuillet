using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkPlayerInstance : NetworkBehaviour
{
    //public NetworkVariable<bool> IsRequesting = new NetworkVariable<bool>();

    [SerializeField] private RequestMenu m_requestMenu;
    [SerializeField] private RequestReponseMenu m_requestResponseMenu;

    public override void OnNetworkSpawn() {
        if (IsOwner) {

        }
        else {
            // set new player to offset position?
        }

        //IsRequesting.Value = false;
        ActivateRequestMenu();
    }

    private void Update() {
        if (NetworkRequestManager.instance.AnyPlayerRequesting.Value) {
            if (!m_requestResponseMenu.gameObject.activeSelf) {
                HandleIncomingRequest();
            }
        }
        else {
            // if menu is still open, close it
            if (m_requestResponseMenu.gameObject.activeSelf) {
                m_requestResponseMenu.Close();
            }
        }
    }

    #region Own Request Menu

    void ActivateRequestMenu() {
        m_requestMenu.Open(HandleOwnRequestHelp);
    }

    public void HandleOwnRequestHelp() {
        //IsRequesting.Value = true;
        NetworkRequestManager.instance.AnyPlayerRequesting.Value = true;
    }

    void HandleOwnRequestResolved() {
        //IsRequesting.Value = false;
        NetworkRequestManager.instance.AnyPlayerRequesting.Value = false;
    }

    #endregion // Own Request Menu

    #region Incoming Request Response Menu

    void HandleIncomingRequest() {
        m_requestResponseMenu.Open(HandleIncomingHelpButton, HandleIncomingRefuseButton);
    }

    void HandleIncomingHelpButton() {
        m_requestResponseMenu.Close();
        //IsRequesting.Value = false;
        NetworkRequestManager.instance.AnyPlayerRequesting.Value = false;
    }

    void HandleIncomingRefuseButton() {
        m_requestResponseMenu.Close();
        //IsRequesting.Value = false;
        NetworkRequestManager.instance.AnyPlayerRequesting.Value = false;
    }

    #endregion // Incoming Request Reponse Menu
}
