using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Characters
{
    // PASSIVE NPCS ONLY
    public class NpcData : MonoBehaviour, ICharacter, INpc
    {
        [field: Title("Base Settings")]
        [field: SerializeField] public NpcName Name {  get; private set; }
        [field: SerializeField] public NpcSettingsSO Settings { get; private set; }

        #region Character Data

        [field: Title("Character Fields")]
        [field: SerializeField] public float MoveSpeed { get; private set; }
        [field: SerializeField] public Transform Position { get; private set; }
        [field: SerializeField] public Rigidbody2D RigidBody { get; private set; }
        [field: SerializeField] public Animator Animator { get; private set; }

        [field: Title("Scripts")]
        [field: SerializeField] public MoveLogic MoveLogic { get; private set; }
        [field: SerializeField] public AudioController Audio { get; private set; }
        [field: SerializeField] public AnimationController Animation { get; private set; }
        #endregion

        [field: Title("NPC Fields")]
        public string Schedule;

    }
}