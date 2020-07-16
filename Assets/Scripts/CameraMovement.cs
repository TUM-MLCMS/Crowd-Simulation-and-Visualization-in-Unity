using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float Speed = 100.0f;
    public float MouseSensitivity = 0.6f;
    public float VerticalAngle, HorizontalAngle;
    public float VerticalAngleLimit = 75.0f;

    private void Start() {
        VerticalAngle = transform.eulerAngles.x % 360.0f;
        HorizontalAngle = transform.eulerAngles.y % 360.0f;
    }

    private void Update() {
        if(Input.GetMouseButton(1)) {
            transform.rotation = Quaternion.Euler(GetEulerRotation());
        }
        transform.Translate(GetLocalDirection() * Speed * Time.deltaTime);
        transform.Translate(GetGlobalDirection() * Speed * Time.deltaTime, Space.World);
    }

    private Vector2 GetMouseDelta() {
        return new Vector2(
            Input.GetAxis("Mouse X") * MouseSensitivity,
            -Input.GetAxis("Mouse Y") * MouseSensitivity
        );
    }

    private Vector3 GetEulerRotation() {
        var mouseDelta = GetMouseDelta();
        HorizontalAngle += mouseDelta.x;
        VerticalAngle = Mathf.Clamp(VerticalAngle + mouseDelta.y, -VerticalAngleLimit, VerticalAngleLimit);

        return new Vector3(VerticalAngle, HorizontalAngle, 0.0f);
    }

    private Vector3 GetLocalDirection() {
        var direction = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) {
            direction += Vector3.forward;
        }

        if (Input.GetKey(KeyCode.S)) {
            direction += Vector3.back;
        }

        if (Input.GetKey(KeyCode.A)) {
            direction += Vector3.left;
        }

        if (Input.GetKey(KeyCode.D)) {
            direction += Vector3.right;
        }

        return direction.normalized;
    }

    private Vector3 GetGlobalDirection() {
        var direction = Vector3.zero;

        if (Input.GetKey(KeyCode.Q)) {
            direction += Vector3.up;
        }

        if (Input.GetKey(KeyCode.E)) {
            direction += Vector3.down;
        }

        return direction.normalized;
    }
}
