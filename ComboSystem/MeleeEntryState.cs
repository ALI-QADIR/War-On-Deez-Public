using Assets.Scripts.StateMachine;

namespace Assets.Scripts.ComboSystem
{
    public class MeleeEntryState : State
    {
        public override void OnEnter(StateMachine.StateMachine stateMachine)
        {
            base.OnEnter(stateMachine);

            State nextState = (State)new GroundEntryState();
            stateMachine.SetNextState(nextState);
        }
    }
}