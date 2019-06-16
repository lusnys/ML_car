using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sensor : MonoBehaviour
{
    public Vector3 primaryDirection;
    public float primaryDistance;
    public Vector3 secondaryDirection;
    public float secondaryDistance;
    public Vector3 longRageDirection;
    public float longRangeDistance;
    public Vector3 sideDirection;
    public float sideDistance;

    private Ray ray;
    private RaycastHit hit;
    private bool didHit;

    public float SensorFlashPrimary()
    {
        ray = new Ray(this.transform.position, primaryDirection);

        Debug.DrawRay(this.transform.position, primaryDirection, Color.green);
        didHit = Physics.Raycast(ray, out hit, primaryDistance);
        if (didHit && hit.collider.gameObject.tag == "Block")
        {
            return hit.distance;
        }

        return 100.0f;
    }

    public float SensorFlashSide()
    {
        ray = new Ray(this.transform.position, sideDirection);

        Debug.DrawRay(this.transform.position, sideDirection, Color.green);
        didHit = Physics.Raycast(ray, out hit, sideDistance);
        if (didHit && hit.collider.gameObject.tag == "Block")
        {
            return hit.distance;
        }

        return 100.0f;
    }

    public float SensorFlashLongRange()
    {
        ray = new Ray(this.transform.position, longRageDirection);

        Debug.DrawRay(this.transform.position, longRageDirection, Color.green);
        didHit = Physics.Raycast(ray, out hit, longRangeDistance);
        if (didHit && hit.collider.gameObject.tag == "Block")
        {
            return hit.distance;
        }

        return 100.0f;
    }

    public float SensorFlashSecondary()
    {
        ray = new Ray(this.transform.position, secondaryDirection);

        Debug.DrawRay(this.transform.position, secondaryDirection, Color.green);
        didHit = Physics.Raycast(ray, out hit, secondaryDistance);
        if (didHit && hit.collider.gameObject.tag == "Block")
        {
            return hit.distance;
        }

        return 100.0f;
    }
}
