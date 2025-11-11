using Characters;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Testing
{
    public class CharacterHitter : MonoBehaviour
    {
        public float Damage = 10f;
        public float KBMultiplier = 1f;
        public float StunDuration = 0f;

        private InputActionMap _testingActions;
        private InputAction _clickAction;
        private InputAction _posAction;

        public void OnEnable()
        {
            _testingActions = InputSystem.actions.FindActionMap("Testing");
            _clickAction = _testingActions.FindAction("Left Click");
            _posAction = _testingActions.FindAction("MousePos");

            _testingActions.Disable();
        }

        public void OnDisable()
        {
            _clickAction.performed -= OnLeftClick;
        }

        [Button]
        public void Toggle() 
        { 
            // disable if enabled and enable if disabled
            if (_testingActions.enabled) 
            {
                _testingActions.Disable();
                _clickAction.performed -= OnLeftClick;
            }
            else
            {
                _testingActions.Enable();
                _clickAction.performed += OnLeftClick;
            }
        }

        private void OnLeftClick(InputAction.CallbackContext context)
        {
            Combat.DamageInstance damage = new Combat.DamageInstance(Damage, KBMultiplier, StunDuration);
            Vector3 pos = Camera.main.ScreenToWorldPoint(_posAction.ReadValue<Vector2>());

            RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero, float.PositiveInfinity, LayerMask.GetMask("HitBox"));

            if (hit.collider != null)
            {
                var kbdir = Mathf.Sign(hit.collider.transform.position.x - pos.x);
                damage.KnockbackMultiplier *= kbdir;
                
                ICharacter character = hit.rigidbody.gameObject.GetComponent<ICharacter>();
                if (character is IDamagable damagable)
                {
                    damagable?.GetHit(damage);
                }
            }
        }
    }
}