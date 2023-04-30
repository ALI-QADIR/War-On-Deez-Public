using UnityEngine;

namespace Assets.Scripts.ComboSystem
{
    public class ComboCharacter : MonoBehaviour
    {
        private StateMachine.StateMachine _meleeStateMachine;

        // Start is called before the first frame update
        private void Start()
        {
            _meleeStateMachine = GetComponent<StateMachine.StateMachine>();
        }

        // Update is called once per frame
        private void Update()
        {
            if (Input.GetMouseButtonDown(0) && _meleeStateMachine.currentState.GetType() == typeof(IdleCombatState))
                _meleeStateMachine.SetNextState(new GroundEntryState());
        }
    }
}