using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using System;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Collections;

using EventChannel;

[RequireComponent(typeof(IInteractable))]
public class InteractLogic : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private GameObject _tooltipCanvas;
    [Header("Listening to Events")]
    [SerializeField] private VoidEventSO _interactInputSO;

    private bool _isEnabled = false;

    // small cooldown to prevent spamming
    //private float _cooldown = 0.2f;

    IInteractable _interactItem;

    private void Awake()
    {
        _interactItem = gameObject.GetComponent<IInteractable>();
    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        // need to unsubscribe when changing scenes as well
        _interactInputSO.OnInvokeEvent -= OnInteract;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.LogAssertion(gameObject.name + " enabled");
        _isEnabled = true;
        if (_tooltipCanvas != null) { _tooltipCanvas.SetActive(true); }
        _interactInputSO.OnInvokeEvent += OnInteract;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //Debug.LogAssertion(gameObject.name + " disabled");
        _isEnabled = false;
        if (_tooltipCanvas != null) { _tooltipCanvas.SetActive(false); }
        _interactInputSO.OnInvokeEvent -= OnInteract;
    }

    private void OnInteract()
    {
        if (_isEnabled) 
        { 
            _interactItem.Interact();
        }
    }
}
