﻿using System;
using UnityEngine;

[Serializable]
public class LoginData : IJsonClass
{
    public string type;
    public string result;

    public bool ProcessData(UnityEngine.Object caller)
    {
        if (type != "success")
            ((LoginScript)caller).message.text = result;
        else
        {
            StaticInfo.Token = result;
            ((LoginScript)caller).message.text = "Welcome back";
            //((LoginScript)caller).getUserInfo();
            return true;

        }

        return false;

    }
}
