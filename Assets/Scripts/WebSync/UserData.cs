﻿using System;
using UnityEngine;

[Serializable]
public class UserData : IJsonClass
{
    public string type;
    public string username;
    public string coin;

    public void ProcessData(UnityEngine.Object caller)
    {
        if (type != "success")
            Debug.Log("couldn't retrieve user information");
        else
        {
            ((Player)caller).username = username;
        }
    }
}