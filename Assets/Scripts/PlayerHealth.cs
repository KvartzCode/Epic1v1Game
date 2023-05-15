using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alteruna;

public class PlayerHealth : AttributesSync
{
    [SynchronizableField] public int health = 100;

    private Alteruna.Avatar avatar;


    void Awake()
    {
        avatar = GetComponent<Alteruna.Avatar>();
    }

    void Start()
    {
        if (!avatar.IsMe)
            enabled = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            health -= 10;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            InvokeRemoteMethod(nameof(Test), UserId.AllInclusive, "HELLO WORLDS!");
        }

        transform.Rotate(0, Input.GetAxis("Mouse X"), 0);
        Camera.main.transform.RotateAround(transform.position, new Vector3(1, 0, 0), Input.GetAxis("Mouse Y"));
    }


    [SynchronizableMethod]
    void Test(string myString)
    {
        Debug.Log(myString);
        User test = avatar.Possessor;
        //test.Index;
    }
}
