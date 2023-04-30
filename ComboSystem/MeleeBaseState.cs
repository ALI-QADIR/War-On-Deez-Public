using Assets.Scripts.StateMachine;
using UnityEngine;

namespace Assets.Scripts.ComboSystem
{
    public class MeleeBaseState : State
    {
        public float duration;

        protected Animator animator;

        // bool to check whether or not the next attack in the sequence should be played or not
        protected bool shouldCombo;

        // the attack index in the sequence of attacks
        protected int attackIndex;

        public override void OnEnter(StateMachine.StateMachine stateMachine)
        {
            base.OnEnter(stateMachine);
            animator = GetComponent<Animator>();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (Input.GetMouseButtonDown(0))
                shouldCombo = true;
        }

        public override void OnExit()
        {
            base.OnExit();
        }
    }
}