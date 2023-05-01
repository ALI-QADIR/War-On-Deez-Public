using UnityEngine;

namespace Assets.Scripts.Character
{
    public class AnimationManager : MonoBehaviour
    {
        private Animator _animator;

        [HideInInspector] public float horizontalInput;
        [HideInInspector] public float verticalInput;
        [HideInInspector] public bool jumping;

        private int _horizontalInputHash;
        private int _verticalInputHash;
        private int _isJumpingHash;

        // Start is called before the first frame update
        private void Awake()
        {
            _animator = GetComponent<Animator>();

            _horizontalInputHash = Animator.StringToHash("Horizontal");
            _verticalInputHash = Animator.StringToHash("Vertical");
            _isJumpingHash = Animator.StringToHash("isJumping");
        }

        // Update is called once per frame
        private void Update()
        {
            _animator.SetFloat(_horizontalInputHash, horizontalInput);
            _animator.SetFloat(_verticalInputHash, verticalInput);
            _animator.SetBool(_isJumpingHash, jumping);
        }
    }
}