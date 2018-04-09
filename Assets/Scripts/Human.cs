﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/* Classe basique d'un humain
 * 
 * (déplacements, animations, attaque, mort)
 * 
 */
public class Human : MonoBehaviour, ITarget
{

    /* Variable Definition */
    public float health = 100;

    public float walking_speed = 4.0f;
    public float running_speed = 8.0f;

    public float JumpForce = 650.0f;

    public bool hasGun = false; // Possède une arme   

    /* Default value for gun and punch */

    public float gunDamage = 25f;
    public float gunRange = 20f;
    public float gunFireBuff = 0.28f;

    public float punchDamage = 50f;
    public float punchRange = 1f;
    public float punchingBuff = 0.95f;


    /* Component */
    protected Rigidbody _body;
    protected Animator _animator;

    /* Movement variable */
    
    protected Vector3 _moveDirection = Vector3.zero;

    protected float jumpMult = 1.0f;
    protected float Speed = 0.0f;

    protected bool walking = false;
    protected bool jumping = false;

    protected bool dead = false;

    protected float jumpDelay = 0.0f; // sync jump animation

    protected bool isScoping = false; // Est entrain de viser
    protected float nextTimeToAttack = 0f;
    protected bool attacking = false;


    protected bool hasHitTarget = false;
    protected bool isHit = false;



    public Vector3 Position
    {
        get { return _body.transform.position; }
    }

    protected void Start () {

        _animator = GetComponent<Animator>();
        _body = GetComponent<Rigidbody>();
    }


    protected void Animate(Animator _animator)
    {
        if (_animator)
        {
            _animator.SetFloat("Speed", Speed); // La variable speed va modifier la vitesse des animations de mouvements

            if (hasGun)
                _animator.SetBool("Scope", isScoping);

            if (attacking)
            {
                if (isScoping)
                    _animator.SetTrigger("Shoot");
                else
                    _animator.SetTrigger("Punch");

                attacking = false;
            }

            _animator.SetBool("Walk", walking && !jumping);

            _animator.SetBool("Jump", jumping);

            _animator.SetBool("Dead", dead);

        }
    }

    protected void Update() // Animating, playing sounds
    {

        /* Animation */
        Animate(_animator);

        /* Sound */
    }

    protected void FixedUpdate() // Moving, Physic Stuff
    {
        /*if (IsGrounded())
        {
            _body.isKinematic = _body.velocity == Vector3.zero;
        }
        else
        {
            _body.isKinematic = !jumping;
        }*/

        //Debug.DrawRay(_body.position + new Vector3(0, 0.05f, 0), -Vector3.up, Color.red, 0.1f, true);

        if (dead)
            return;


        // On modifie directement la velocity du personnage pour les axes X et Z afin de le rendre plus controlable
        // Au lieu d'utiliser AddForce
        _body.velocity = new Vector3(Vector3.Dot(transform.forward, _moveDirection * Speed), _body.velocity.y, Vector3.Dot(transform.right, _moveDirection * Speed));

        if (jumping && Time.time > jumpDelay)  // Le delay permet de synchroniser le saut avec l'animation
        {
            // Pour rendre le saut plus réaliste, on utilise AddForce en mode Impulse
            _body.AddForce(new Vector3(0, JumpForce* jumpMult, 0), ForceMode.Impulse);
            jumpMult = 1.0f;
            jumping = false; // a présent, le joueur n'est plus entrain de sauter mais entrain de retomber
        }
    }

    public void Jump()
    {
        if(!jumping && IsGrounded())
        {
            walking = false;
            jumping = true;

            jumpDelay = Time.time + 0.15f;
        }
        
    }

    public void Forward(bool run, Vector3 _direction)
    {
        if (!isScoping || IsGrounded())
            Speed = run ? running_speed : walking_speed;
        else
            Speed = walking_speed;  // si le joueur est en l'air ou s'il vise, il ne peut pas courir


        walking = (_moveDirection.z * _moveDirection.z + _moveDirection.x * _moveDirection.x) > 0.2;

        _moveDirection = _direction;
    }

    public void Scope()
    {
        if (hasGun)
            isScoping = !isScoping;
    }

    public void Attack(Vector3 _position, Vector3 _direction)
    {
        if (Time.time > nextTimeToAttack)
        {
            nextTimeToAttack = Time.time + (isScoping ? gunFireBuff : punchingBuff);
            attacking = true;
            RaycastHit hit;
            // On tir un rayon depuis le centre de la camera du joueur jusqu'à une certaine distance
            if (Physics.Raycast(_position, _direction, out hit, (isScoping ? gunRange : punchRange)))
            {
                ITarget target = hit.transform.GetComponent<ITarget>();
                if (target != null) // Si un joueur est touché
                {
                    hasHitTarget = true;
                    target.TakeDamage((isScoping ? gunDamage : punchDamage)); // La target va perdre de la vie
                }
            }
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0f)
            Die();
        else
            _animator.SetTrigger("isHit"); // animation lorsqu'on est touché
    }

    public virtual void Die()
    {
        Debug.Log("Virtual");
        dead = true;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "jumpg")
        {
            Jump();
            jumpMult = 2.0f;
        }
    }

    protected bool IsGrounded()
    {
        return Physics.Raycast(transform.position + new Vector3(0, 0.9f, 0), -Vector3.up, 1f);
    }
}