using Assets.Scripts.StateMachine;
using UnityEngine;

namespace Assets.Scripts.ComboSystem
{
    public class GroundCombo2State : MeleeBaseState
    {
        public override void OnEnter(StateMachine.StateMachine stateMachine)
        {
            base.OnEnter(stateMachine);

            attackIndex = 2;
            duration = 1.25f;
            animator.SetTrigger("GroundCombo2");
            Debug.Log("Player attacked Ground Combo 2");
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (fixedTime >= duration)
            {
                if (shouldCombo)
                    stateMachine.SetNextState(new GroundFinisherState());
                else
                    stateMachine.SetNextStateToMain();
            }
        }
    }
}