using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinner : MonoBehaviour
{
    [SerializeField] float speed = 5;

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.Rotate(new Vector3(0, speed * Time.deltaTime, 0));
    }
}
