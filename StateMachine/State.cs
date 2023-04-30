using UnityEngine;

namespace Assets.Scripts.StateMachine
{
    public abstract class State
    {
        protected float time { get; set; }
        protected float fixedTime { get; set; }
        protected float lateTime { get; set; }

        public global::Assets.Scripts.StateMachine.StateMachine stateMachine;

        public virtual void OnEnter(global::Assets.Scripts.StateMachine.StateMachine _stateMachine)
        {
            stateMachine = _stateMachine;
        }

        public virtual void OnUpdate()
        {
            time += Time.deltaTime;
        }

        public virtual void OnFixedUpdate()
        {
            fixedTime += Time.fixedDeltaTime;
        }

        public virtual void OnLateUpdate()
        {
            lateTime += Time.deltaTime;
        }

        public virtual void OnExit()
        { }

        #region Passthrough Methods

        /// <summary>
        /// Removes a game-object, component, or asset from the game.
        /// </summary>
        /// <param name="obj">The component to retrieve.</param>
        protected static void Destroy(Object obj)
        {
            Object.Destroy(obj);
        }

        /// <summary>
        /// Returns a GameObject of type T if the game object has one attached, null if it doesn't.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected T GetComponent<T>() where T : Component
        {
            return stateMachine.GetComponent<T>();
        }

        /// <summary>
        /// Returns the Component of type <paramref name="type"/> if the game object has one attached, null if it doesn't.
        /// </summary>
        /// <param name="type">Type of Component to retrieve./param>
        /// <returns></returns>
        protected Component GetComponent(System.Type type)
        {
            return stateMachine.GetComponent(type);
        }

        /// <summary>
        /// Returns the Component of type <paramref name="type"/> if the game object has one attached, null if it doesn't.
        /// </summary>
        /// <param name="type">The type of component to retrieve.</param>
        /// <returns></returns>
        protected Component GetComponent(string type)
        {
            return stateMachine.GetComponent(type);
        }

        #endregion Passthrough Methods
    }
}