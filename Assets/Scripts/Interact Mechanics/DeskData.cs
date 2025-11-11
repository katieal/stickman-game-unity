using System;
using System.Collections;
using UnityEngine;


public class DeskData : MonoBehaviour, IInteractable
{
    [SerializeField] private Shop.ShopMenuController _shop;

    private bool _isTriggered = false;

    public void Interact()
    {
        if (!_isTriggered)
        {
            _isTriggered = true;
            // open ui
            _shop.OpenStoreMenu();
            
            // resets trigger after 1 second
            // future: when ui closes, set triggered to false
            StartCoroutine(MenuDelay());
        }
    }

    IEnumerator MenuDelay()
    {
        yield return new WaitForSeconds(1);
        _isTriggered = false;
    }
}
