using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

using EventChannel;

namespace Characters.StateMachine
{
    public class PlayerStateMachine : StateMachineBase
    {
        [Title("Listening to Events")]
        [SerializeField] private VoidPlayerDataRequestEventSO _sendPlayerDataSO;
        [SerializeField] private Vector2EventSO _lookInputSO;

        private PlayerData _player;

        public override void Init()
        {
            _player = GetComponentInParent<PlayerData>();
            foreach (StateType type in StateDict.Keys)
            {
                if (StateDict[type] != null)
                {
                    StateDict[type].Init(_player);
                    StateDict[type].SetEnabled(false);
                }
            }
            // switch to starting state
            CurrentState = StartingState;
            StateDict[CurrentState].SetEnabled(true);
        }

        #region Save Data
        private void Init(SaveData.PlayerSaveData data)
        {
            // initialize with save data if possible
            if (data != null)
            {
                // set starting state to most recent state
                StartingState = data.CurrentState;
            }
            // initialize states
            Init();
        }
        #endregion

        private void OnEnable()
        {
            _sendPlayerDataSO.OnResultEvent += Init;
            _lookInputSO.OnInvokeEvent += LookAtMouse;
        }
        private void OnDisable()
        {
            _sendPlayerDataSO.OnResultEvent -= Init;
            _lookInputSO.OnInvokeEvent -= LookAtMouse;
        }

        /// <summary>
        /// Turn player character to face the direction of the mouse
        /// </summary>
        /// <param name="mousePos"></param>
        private void LookAtMouse(Vector2 mousePos)
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(mousePos);
            _player.Position.localScale = new Vector2(Mathf.Sign(pos.x - _player.Position.position.x), 1f);
        }
    }
}