﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinBox : Box {

    // Donne un gain de coin

    public int reward = 10;

    new void Update()
    {
        transform.Rotate(new Vector3(0, 0, 45) * Time.deltaTime);

    }

    public override void Action(Player p)
    {
        FindObjectsOfType<FadeAnim>()[0].GetBoxFade("coin");
        p.getReward(reward);
        Destroy(gameObject);
    }

}
