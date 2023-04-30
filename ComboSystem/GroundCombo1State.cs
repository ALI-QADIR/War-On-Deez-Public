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
            duration = 0.5f;
            // TODO: set the duration to the length of the animation and play the animation
            Debug.Log("Player attacked Ground Combo 1");
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