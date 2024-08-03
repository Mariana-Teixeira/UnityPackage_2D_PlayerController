# 2D Player Controller
1. Attach a 'PlayerController' component to your player game object.
2. Connect 'PlayerInput' component events to 'PlayerController' component:
    Running(CallbackContext) => PlayerController.OnRunning
    Jumping(CallbackContext) => PlayerController.OnJumping