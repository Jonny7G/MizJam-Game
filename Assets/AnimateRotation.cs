using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateRotation : MonoBehaviour
{
    public float rotSpeed;
    public float rot1Val, rot2Val;

    private Quaternion rot1, rot2;
    private Quaternion currentRot;
    private void Start()
    {
        rot1 = Quaternion.Euler(0, 0, rot1Val);
        rot2 = Quaternion.Euler(0, 0, rot2Val);
        currentRot = Quaternion.Euler(0,0,Random.Range(rot1Val,rot2Val));
    }
    private void Update()
    {
        transform.rotation = Quaternion.RotateTowards(transform.rotation, currentRot, Time.deltaTime * rotSpeed);
        if (transform.rotation == currentRot)
        {
            if (currentRot == rot1)
            {
                currentRot = rot2;
            }
            else
            {
                currentRot = rot1;
            }
        }
    }
}
