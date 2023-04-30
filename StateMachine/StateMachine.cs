using UnityEngine;

namespace Assets.Scripts.StateMachine
{
    public class StateMachine : MonoBehaviour
    {
        public State currentState { get; private set; }

        private State _nextState;

        // Update is called once per frame
        private void Update()
        {
            // If we have a next state, set it and clear the next state.
            if (_nextState != null)
            {
                SetState(_nextState);
                _nextState = null;
            }

            // Update the current state if we have one.
            if (currentState != null)
                currentState.OnUpdate();
        }

        private void FixedUpdate()
        {
            // Update the current state if we have one.
            if (currentState != null)
                currentState.OnFixedUpdate();
        }

        private void LateUpdate()
        {
            // Update the current state if we have one.
            if (currentState != null)
                currentState.OnLateUpdate();
        }

        /// <summary>
        /// Sets the next state to be the given state.
        /// </summary>
        /// <param name="nextState"></param>
        private void SetState(State nextState)
        {
            if (currentState != null)
            {
                currentState.OnExit();
            }

            currentState = nextState;
            currentState.OnEnter(this);
        }

        /// <summary>
        /// Sets the next state to be the given state.
        /// </summary>
        /// <param name="newState"></param>
        public void SetNextState(State newState)
        {
            if (newState != null)
            {
                _nextState = newState;
            }
        }
    }
}