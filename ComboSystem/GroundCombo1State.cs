using Assets.Scripts.StateMachine;
using UnityEngine;

namespace Assets.Scripts.ComboSystem
{
    public class GroundCombo1State : MeleeBaseState
    {
        public override void OnEnter(StateMachine.StateMachine stateMachine)
        {
            base.OnEnter(stateMachine);

            attackIndex = 2;
            duration = 0.6f;
            animator.SetTrigger("GroundCombo1");
            // Debug.Log("Player attacked Ground Combo 1");
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (fixedTime >= duration)
            {
                if (shouldCombo)
                    stateMachine.SetNextState(new GroundCombo2State());
                else
                    stateMachine.SetNextStateToMain();
            }
        }
    }
}