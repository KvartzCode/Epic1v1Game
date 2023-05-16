using System.Collections;
using System.Collections.Generic;
using Alteruna;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { 
        get
        { 
            return _instance;
        }
    }

    public User user;


    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(gameObject);
        else
            _instance = this;
    }

    public void SetUser(User user)
    {
        this.user = user;
    }
}
