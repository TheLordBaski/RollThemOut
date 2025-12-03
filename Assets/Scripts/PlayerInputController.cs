using System;
using DragesStudio;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    private InputAction _moveAction; 
    private InputAction _lookAction;
    private InputAction _jumpAction;

    private InputAction _grabAction;
    private InputAction _dropAction;
    
    private InputAction _rotateGrabbableAction;
    private InputAction _moveGrabbableActionAction;
    
    private PlayerMovement _playerMovement;
    
    void Awake()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        GatherActions();
        InitializeCallback();
    }

    private void Update()
    {
        _playerMovement.Move(_moveAction.ReadValue<Vector2>());
        _playerMovement.Look(_lookAction.ReadValue<Vector2>());
    }

    private void GatherActions()
    {
        _moveAction = InputSystem.actions.FindAction("Move");
        _lookAction = InputSystem.actions.FindAction("Look"); 
        _jumpAction = InputSystem.actions.FindAction("Jump");
        
        _grabAction = InputSystem.actions.FindAction("Grab");
        _dropAction = InputSystem.actions.FindAction("Drop");
        
        _rotateGrabbableAction = InputSystem.actions.FindAction("RotateGrabbable");
        _moveGrabbableActionAction = InputSystem.actions.FindAction("MoveGrabbable");
    }

    private void InitializeCallback()
    {
        _moveAction.performed += OnMovePerformed;
        _lookAction.performed += OnLookPerformed;
    }

    private void RemoveCallback()
    {
        _moveAction.performed -= OnMovePerformed;
        _lookAction.performed -= OnLookPerformed;
    }
    
    
    
    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            //_playerMovement.Move(context.ReadValue<Vector2>());
        }
    }

    private void OnLookPerformed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            //_playerMovement.Look(context.ReadValue<Vector2>());
        }
    }
    
}
