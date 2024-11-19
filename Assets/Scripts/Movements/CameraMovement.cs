using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class CameraMovement : MonoBehaviourPunCallbacks
{
    public Transform weapon;
    [SerializeField] private float distanceFromWeapon = 1.5f;
    [SerializeField] private float cameraMoveSpeed = 5f;

    private void Update()
    {

        if (weapon != null)
        {
        Vector3 targetPosition = weapon.position + weapon.right * distanceFromWeapon;
        targetPosition.z = -8;
        // Move the camera towards the target position
        transform.position = Vector3.Lerp(transform.position, targetPosition, cameraMoveSpeed * Time.deltaTime);
        }
    }

    public void SetWeapon(Transform newWeapon)
    {
        weapon = newWeapon;
    }
}