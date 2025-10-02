using System;
using UnityEngine;

public class WelcomePanelController : MonoBehaviour
{
    private void Update()
    {
        if (Input.anyKeyDown)
        {
            if (!gameObject.activeInHierarchy)
                return;

            StateMachine.Instance.GoToState(StateMachine.UiState.TeamSetup);
        }
    }
}
