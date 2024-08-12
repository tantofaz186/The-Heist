using UnityEngine.InputSystem;

namespace Utils
{
    public class ActionManager : Singleton<ActionManager>
    {
        public PlayerInputActions playerInputActions;

        private void Awake()
        {
            if (Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
            playerInputActions = new PlayerInputActions();
            playerInputActions.Enable();
            playerInputActions.Player.Enable();
        }

        private void OnDisable()
        {
            if (Instance != this) return;
            playerInputActions.Player.Disable();
            playerInputActions.Disable();
        }

        public void RebindKey(InputAction action, string newKey)
        {
            action.ChangeBinding(new InputBinding { path = newKey });
        }
    }
}