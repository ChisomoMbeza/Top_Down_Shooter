using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    //triggers

    public Renderer leftTriggerSphere;
    public Renderer rightTriggerSphere;

    //Bumpers
    public Renderer LeftBumberBox;
    public Renderer rightBumberBox;

    //Buttons
    public Renderer aButtonSphere;
    public Renderer bButtonSphere;
    public Renderer xButtonSphere;
    public Renderer yButtonSphere;

    //D - Pad
    public Renderer dpadLeft;
    public Renderer dpadUp;
    public Renderer dpadRight;
    public Renderer dpadDown;

    //Start and Back
    public Renderer startBox;
    public Renderer backBox;

    //Start and Back
    public Transform cubeLS;
    public Transform cubeRS;
    float movementSpeed = 2.0f;

    private void Update()
    {
        float rTriggerFloat = Input.GetAxis("Right Trigger");
        float lTriggerFloat = Input.GetAxis("Left Trigger");
        bool leftBumper = Input.GetButton("Left Bumper");
        bool rightBumper = Input.GetButton("Right Bumper");
        bool backButton = Input.GetButton("Back");
        bool startButton = Input.GetButton("Start");
        bool aButton = Input.GetButton("A Button");
        bool bButton = Input.GetButton("B Button");
        bool xButton = Input.GetButton("X Button");
        bool yButton = Input.GetButton("Y Button");
        float dPadHorizontal = Input.GetAxis("Horizontal") * movementSpeed;
        float dPadVertical = Input.GetAxis("Vertical") * movementSpeed;
        float moveHL = Input.GetAxis("Horizontal") * movementSpeed;
        float moveVL = Input.GetAxis("Vertical") * movementSpeed;
        float moveHR = Input.GetAxis("Mouse X") * movementSpeed;
        float moveVR = Input.GetAxis("Mouse Y") * movementSpeed;

        //Colors for Trigger Sphere
        rightTriggerSphere.material.color = new Color(rTriggerFloat, rTriggerFloat, rTriggerFloat);
        leftTriggerSphere.material.color = new Color(lTriggerFloat, lTriggerFloat, lTriggerFloat);

        // Dpad
        if (dPadHorizontal < 0)
        {
            dpadLeft.material.color = new Color(1, 1, 1);
            dpadRight.material.color = new Color(0, 0, 0);
        }
        else if (dPadHorizontal > 0)
        {
            dpadRight.material.color = new Color(1, 1, 1);
            dpadLeft.material.color = new Color(0, 0, 0);
        }
        else if (dPadHorizontal == 0)
        {
            dpadRight.material.color = new Color(0, 0, 0);
            dpadLeft.material.color = new Color(0, 0, 0);
        }

        // Dpad
        if (dPadVertical < 0)
        {
            dpadDown.material.color = new Color(1, 1, 1);
            dpadUp.material.color = new Color(0, 0, 0);
        }
        else if (dPadVertical > 0)
        {
            dpadUp.material.color = new Color(1, 1, 1);
            dpadDown.material.color = new Color(0, 0, 0);
        }
        else if (dPadVertical == 0)
        {
            dpadUp.material.color = new Color(0, 0, 0);
            dpadDown.material.color = new Color(0, 0, 0);
        }

        //Bumpers
        if (leftBumper)
        {
            LeftBumberBox.material.color = new Color(1, 1, 1);
        }
        else if (!leftBumper)
        {
            LeftBumberBox.material.color = new Color(0, 0, 0);
        }


        if (rightBumper)
        {
            rightBumberBox.material.color = new Color(1, 1, 1);
        }
        else if (!rightBumper)
        {
            rightBumberBox.material.color = new Color(0, 0, 0);
        }

        //A. B, X, Y

        if (aButton)
        {
            aButtonSphere.material.color = new Color(1, 1, 1);
        }
        else if (!aButton)
        {
            aButtonSphere.material.color = new Color(0, 0, 0);
        }

        if (bButton)
        {
            bButtonSphere.material.color = new Color(1, 1, 1);
        }
        else if (!bButton)
        {
            bButtonSphere.material.color = new Color(0, 0, 0);
        }

        if (xButton)
        {
            xButtonSphere.material.color = new Color(1, 1, 1);
        }
        else if (!xButton)
        {
            xButtonSphere.material.color = new Color(0, 0, 0);
        }

        if (yButton)
        {
            yButtonSphere.material.color = new Color(1, 1, 1);
        }
        else if (!yButton)
        {
            yButtonSphere.material.color = new Color(0, 0, 0);
        }


        //Back and Start buttons
        if (backButton)
        {
            backBox.material.color = new Color(1, 1, 1);
        }
        else if (!backButton)
        {
            backBox.material.color = new Color(0, 0, 0);
        }

        if (startButton)
        {
            startBox.material.color = new Color(1, 1, 1);
        }
        else if (!startButton)
        {
            startBox.material.color = new Color(0, 0, 0);
        }


        //Movement in per second measuerments
        moveHL *= Time.deltaTime;
        moveVL *= Time.deltaTime;
        moveHR *= Time.deltaTime;
        moveVR *= Time.deltaTime;


        //Move cube with left Joystic Joystic
        cubeLS.Translate(moveVL, 0, moveHL);
        cubeRS.Translate(moveVR, 0, moveHR);
    }

}
