using Assets.Scripts.StateMachine;
using UnityEngine;

namespace Assets.Scripts.ComboSystem
{
    public class GroundFinisherState : MeleeBaseState
    {
        public override void OnEnter(StateMachine.StateMachine stateMachine)
        {
            base.OnEnter(stateMachine);

            attackIndex = 3;
            duration = 1.25f;
            animator.SetTrigger("GroundFinisher");
            Debug.Log("Player attacked Ground Finisher");
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (fixedTime >= duration)
                stateMachine.SetNextStateToMain();
        }
    }
}