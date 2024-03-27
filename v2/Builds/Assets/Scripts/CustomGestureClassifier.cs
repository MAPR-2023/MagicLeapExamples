using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.MagicLeap;
using HandGestures = UnityEngine.XR.MagicLeap.InputSubsystem.Extensions.DeviceFeatureUsages.HandGesture;
using GestureClassification = UnityEngine.XR.MagicLeap.InputSubsystem.Extensions.MLGestureClassification;

public class CustomGestureClassifier : MonoBehaviour
{
    private InputDevice leftHandDevice;
    private InputDevice rightHandDevice;
    public GameObject gestureIndicator;

    public bool isGrabbingLeft = false;
    private int particleGrabbedLeft = -1;
    public bool isGrabbingRight = false;
    private int particleGrabbedRight = -1;

    GrabBehaviourController gbc;

    public GameObject handRigidbodyLeft;
    public GameObject handRigidbodyRight;

    private void Awake()
    {
        gbc = this.GetComponent<GrabBehaviourController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!MLPermissions.CheckPermission(MLPermission.HandTracking).IsOk)
        {
            Debug.LogError($"You must include the {MLPermission.HandTracking} permission in the AndroidManifest.xml to run this example.");
            enabled = false;
            return;
        }
        GestureClassification.StartTracking();
    }

    // Update is called once per frame
    void Update()
    {
        if (!leftHandDevice.isValid || !rightHandDevice.isValid)
        {
            List<InputDevice> foundDevices = new List<InputDevice>();
            InputDevices.GetDevices(foundDevices);

            foreach (InputDevice device in foundDevices)
            {
                if (device.name == GestureClassification.LeftGestureInputDeviceName)
                {
                    leftHandDevice = device;
                    continue;
                }

                if (device.name == GestureClassification.RightGestureInputDeviceName)
                {
                    rightHandDevice = device;
                    continue;
                }

                if (leftHandDevice.isValid && rightHandDevice.isValid)
                {
                    break;
                }
            }
            return;
        }

        leftHandDevice.TryGetFeatureValue(HandGestures.GesturesEnabled, out bool EnableCheck);

        GestureClassification.TryGetHandPosture(leftHandDevice, out GestureClassification.PostureType leftPosture);
        GestureClassification.TryGetHandPosture(rightHandDevice, out GestureClassification.PostureType rightPosture);
        //if (EnableCheck && !hasGrabbedLeft)
        if (EnableCheck)
        {
            if (leftPosture == GestureClassification.PostureType.Grasp)
            {
                leftHandDevice.TryGetFeatureValue(HandGestures.GestureTransformPosition, out Vector3 leftPos);
                leftHandDevice.TryGetFeatureValue(HandGestures.GestureTransformRotation, out Quaternion leftRot);

                if (!isGrabbingLeft)
                {
                    particleGrabbedLeft = gbc.GrabParticle(leftPos);
                    isGrabbingLeft = true;
                    handRigidbodyLeft.SetActive(false);
                } else
                {
                    gbc.MoveParticle(particleGrabbedLeft, leftPos);
                }
            }
            

            if (rightPosture == GestureClassification.PostureType.Grasp)
            {
                rightHandDevice.TryGetFeatureValue(HandGestures.GestureTransformPosition, out Vector3 rightPos);
                rightHandDevice.TryGetFeatureValue(HandGestures.GestureTransformRotation, out Quaternion rightRot);

                if (!isGrabbingRight)
                {
                    particleGrabbedRight = gbc.GrabParticle(rightPos);
                    isGrabbingRight = true;
                    handRigidbodyRight.SetActive(false);
                } else
                {
                    gbc.MoveParticle(particleGrabbedRight, rightPos);
                }
            }
        }

        if (leftPosture != GestureClassification.PostureType.Grasp)
        {
            isGrabbingLeft = false;
            gbc.UnfixParticle(particleGrabbedLeft);
            particleGrabbedLeft = -1;
            //handRigidbodyLeft.SetActive(true);
        }
        if (rightPosture != GestureClassification.PostureType.Grasp)
        {
            isGrabbingRight = false;
            gbc.UnfixParticle(particleGrabbedRight);
            particleGrabbedRight = -1;
            //handRigidbodyRight.SetActive(true);
        }
    }
}
