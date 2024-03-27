using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerMover : MonoBehaviour
{
    private MagicLeapInputs mlInputs;
    private MagicLeapInputs.ControllerActions controllerActions;
    private Vector3 lastPosition;
    private bool bumperDown = false;
    private bool hasLastPosition = false;

    void Start()
    {
        mlInputs = new MagicLeapInputs();
        mlInputs.Enable();
        controllerActions = new MagicLeapInputs.ControllerActions(mlInputs);
        controllerActions.Bumper.performed += HandleOnBumper;
        controllerActions.Bumper.canceled += HandleOnBumperCancel;
    }

    private void HandleOnBumper(InputAction.CallbackContext obj)
    {
        Debug.Log("The Bumper is pressed down (" + bumperDown + ") - Moving cube");
        lastPosition = controllerActions.Position.ReadValue<Vector3>();
        Debug.Log("Starting Position: " + lastPosition);
        hasLastPosition = true;  
    }

    private void HandleOnBumperCancel(InputAction.CallbackContext obj)
    {
        Vector3 currentPosition = controllerActions.Position.ReadValue<Vector3>();
        Vector3 movementMade = currentPosition - lastPosition;
        const float enlargeFactor = 1;
        movementMade.Scale(new Vector3(enlargeFactor, enlargeFactor, enlargeFactor));
        this.transform.position = this.transform.position - movementMade;
        Debug.Log("Starting Position: " + lastPosition + " - Current Position: " + currentPosition);
        lastPosition = currentPosition;
    }

    void Update()
    {
        if (controllerActions.IsTracked.IsPressed())
        {
            MoveCubeLikeController();
        }
    }

    void OnDestroy()
    {
        mlInputs.Dispose();
    }

    private void MoveCubeLikeController()
    {
        //if (bumperDown && !hasLastPosition)
        //{
            
        //}
        //else if (bumperDown && hasLastPosition)
        //{
            
        //}
        //else
        //{
        //    hasLastPosition = false;
        //}
    }
}
