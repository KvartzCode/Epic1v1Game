using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropRespawn : MonoBehaviour
{
    private Rigidbody rg;
    private Vector3 originPos;
    private Vector3 originRot;
    // Start is called before the first frame update
    void Start()
    {
        rg = GetComponent<Rigidbody>();
        originPos = transform.position;
        originRot = transform.rotation.eulerAngles;
    }

    public void ResetProp()
    {
        rg.velocity = Vector3.zero;
        transform.position = originPos;
        transform.eulerAngles = originRot;
    }


}
