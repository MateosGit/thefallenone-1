﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // faut utiliser l'UI

public class Player : Human { // Hérite de la classe human

    private Slider playerHealth;

    private Animator ArmAnimator; // les mains en vue fps

    private PlayerController controller;

    public bool FPSView = true;

    public GameObject crossHair;
    private GameObject gun;
    public GameObject head;

    public GameObject[] ArmExt; // last object is the gun

    public GameObject[] ArmFPS;

    private Vector3 spawnPoint;

    // For PNJ
    public bool hasWallhack = false;
    public bool hasShotCitizen = false;

    // Sync server

    public LoginData _login;
    public UserData _user;

    int currentScene;

    // Use this for initialization
    protected override void Start () {
        base.Start();

        myAudio = GetComponent<AudioSource>();

        controller = GetComponent<PlayerController>();
        ArmAnimator = GetComponentsInChildren<Animator>()[1];

        if (FPSView)
        {
            head.GetComponent<Renderer>().enabled = false; // On cache la tête du joueur (car vue FPS)
            foreach (var obj in ArmExt)
            {
                obj.GetComponent<Renderer>().enabled = false;
            }

            gun = ArmFPS[ArmFPS.Length - 1];
        }
        else
        {
            head.GetComponent<Renderer>().enabled = true; // On cache la tête du joueur (car vue FPS)
            foreach (var obj in ArmFPS)
            {
                obj.GetComponent<Renderer>().enabled = false;
            }

            gun = ArmExt[ArmExt.Length - 1];
        }

        playerHealth = FindObjectsOfType<Slider>()[0]; // On recupère le slider

        spawnPoint = transform.position;

        if (StaticInfo.Username != "")
            username = StaticInfo.Username;
        else
            username = "Offline Player";

        currentScene = SceneManager.GetActiveScene().buildIndex;

        crossHair.SetActive(isScoping);
    }

    // Update is called once per frame
    protected override void Update () {

        /* Animation FPS Arm */
        Animate(ArmAnimator);

        base.Update();

        /* UI */
        playerHealth.value = health;

        /* Hide and Show gun */
        gun.GetComponent<Renderer>().enabled = hasGun && isScoping;

    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (_body.transform.position.y < -20f)
        {
            Die();
        }
    }

    public override void Scope()
    {
        base.Scope();

        crossHair.SetActive(isScoping);
    }

    public override void Stand()
    {
        base.Stand();
        // Call camera change in PlayerController
        if(!crouching)
            controller.adjustingCamera(false);
    }

    public void Heal(float value)
    {
        if (dead)
            return;

        health += value;
        if (health > 100.0f)
            health = 100.0f;
    }
    
    public override void Die()
    {
        if (dead)
            return;
        base.Die();
        
        controller.resetCamera(true);
        StartCoroutine(Respawn());
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(5); // delay

        dead = false;
        controller.resetCamera(false);
        
        /*_animator.Play("Idle", -1, 0f);
        _body.MovePosition(spawnPoint);
        health = 100f;*/
        // On recommence le niveau
        SceneManager.LoadScene(currentScene);
    }
    
    // On récupère un FallenCoin
    public void getReward(int coin)
    {
        if (StaticInfo.Token == "")
            return;

        WWWForm form = new WWWForm();
        form.AddField("token", StaticInfo.Token);
        form.AddField("reward", coin.ToString());

        WWW www = new WWW("https://thefallen.one/sync/userInfo.php", form);

        StartCoroutine(WaitForRequest<UserData>(www));
    }
    // On fini le niveau
    public void finishLevel(int level)
    {
        if (StaticInfo.Token == "")
            return;

        WWWForm form = new WWWForm();
        form.AddField("token", StaticInfo.Token);
        form.AddField("level", level.ToString());

        WWW www = new WWW("https://thefallen.one/sync/userInfo.php", form);

        StartCoroutine(WaitForRequest<UserData>(www));
    }

    public void updateStat(StaticInfo.Stat stat)
    {
        if (StaticInfo.Token == "")
            return;

        WWWForm form = new WWWForm();
        form.AddField("token", StaticInfo.Token);
        form.AddField(stat.ToString(), 1);

        WWW www = new WWW("https://thefallen.one/sync/userInfo.php", form);

        StartCoroutine(WaitForRequest<UserData>(www));
    }


    public void getUserInfo(string token)
    {
        if (StaticInfo.Token == "")
            return;

        WWWForm form = new WWWForm();
        form.AddField("token", token);

        WWW www = new WWW("https://thefallen.one/sync/userInfo.php", form);

        StartCoroutine(WaitForRequest<UserData>(www));
    }
    IEnumerator WaitForRequest<T>(WWW data)
    {
        yield return data; // Wait for the data
        if (data.error != null)
        {
            Debug.Log("There was an error sending request: " + data.error);
        }
        else
        {
            T jsonClass = JsonUtility.FromJson<T>(data.text); // La réponse est en JSON
            ((IJsonClass)jsonClass).ProcessData(this);
        }
    }


}
