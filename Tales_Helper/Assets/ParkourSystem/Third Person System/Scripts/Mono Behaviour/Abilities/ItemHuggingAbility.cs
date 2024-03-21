using DiasGames.Abilities;
using DiasGames.Components;
using DiasGames.Debugging;
using UnityEngine;

namespace DiasGames.Abilities
{
    [DisallowMultipleComponent]
    public class ItemHuggingAbility : AbstractAbility
    {
        private IMover _mover = null;

        private void Awake()
        {
            _mover = GetComponent<IMover>();
        }

        public override bool ReadyToRun()
        {
            return _action.pickUp;
        }

        public override void OnStartAbility()
        {
            SetAnimationState("ItemHugging1", 0.25f);
        }


        public override void UpdateAbility()
        {

        }
    }
}