using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class DummyController : MonoBehaviour
{
    [SerializeField] private Transform _position;

    // script to reset dummy position every 5 seconds
    private Vector3 _originalPosition;

    private bool _cooldown = false;

    private void Awake()
    {
        _originalPosition = _position.position;
    }

    private void Update()
    {
        if (_cooldown == false)
        {
            _cooldown = true;
            ResetPosition();
            StartCoroutine(Cooldown());
        }
       
    }

    private void ResetPosition()
    {
        _position.position = _originalPosition;
    }

    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(5);

        _cooldown = false;
    }
}
