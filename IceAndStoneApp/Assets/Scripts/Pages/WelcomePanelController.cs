using System;
using UnityEngine;

public class WelcomePanelController : MonoBehaviour
{
    public event Action ProceedRequested;

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            if (!gameObject.activeInHierarchy)
                return;

            ProceedRequested?.Invoke();
        }
    }
}
