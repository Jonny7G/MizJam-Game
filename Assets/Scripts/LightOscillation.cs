using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightOscillation : MonoBehaviour
{
    public float min, max;
    public float speed;
    public Light SpotLight;
    private bool isMin = true;

    private void Update()
    {
        if (isMin)
        {
            SpotLight.spotAngle = Mathf.MoveTowards(SpotLight.spotAngle, max, Time.deltaTime * speed);
            if(SpotLight.spotAngle == max)
            {
                isMin = false;
            }
        }
        else if (!isMin)
        {
            SpotLight.spotAngle = Mathf.MoveTowards(SpotLight.spotAngle, min, Time.deltaTime * speed);
            if (SpotLight.spotAngle == min)
            {
                isMin = true;
            }
        }
    }
}
