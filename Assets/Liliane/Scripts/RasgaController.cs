﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RasgaController : Singleton<RasgaController>
{
    public SpriteRenderer rasgaSr;
    public Animator rasgaAnim;
    public Transform[] posRasga;
    public Transform Rasga;
    public Transform posToRun;
    public Transform posOrigin;
    
    public float speed;
    public float speedToAttack;
    public bool isLookLeft = true;

    private Rigidbody2D rasgaControllerRb;
    private Vector2 direction;

    private int idTarget;
    private int signal = 1;
    private bool canAttack = true;
    private float distance;

    public Animator animator;

    private bool _isGamePaused;

    void Start()
    {
        rasgaControllerRb = GetComponent<Rigidbody2D>();

        Rasga.position = posRasga[0].position;
        idTarget = 1;
    }

    void Update()
    {
        if (_isGamePaused) return;

        if (!canAttack) return;

        if(PlayerController.Instance.GetPlayerOnTheLight())
        {
            rasgaAnim.SetBool("attack", true);

            distance = Vector2.Distance(PlayerController.Instance.transform.position, transform.position);

            if(distance > 0)
            {
                direction = (PlayerController.Instance.transform.position - transform.position).normalized;
                print(direction);

                if (direction.x < 0 && !isLookLeft)
                {
                    Flip();
                }
                else if (direction.x > 0 && isLookLeft)
                {
                    Flip();
                }

                    
            }
            
            if(direction.x < 0)
            {
                signal = -1;
            }
            else
            {
                signal = 1;
            }   

            rasgaControllerRb.velocity = new Vector2(signal, direction.y) * speedToAttack;
        }
        else
        {
            MoveToLeftToRight();
        }        
    }

    private void MoveToLeftToRight()
    {
        rasgaAnim.SetBool("attack", false);

        rasgaControllerRb.velocity = Vector2.zero;

        Rasga.position = Vector3.MoveTowards(Rasga.position, posRasga[idTarget].position, speed * Time.deltaTime);

        if (Rasga.position == posRasga[idTarget].position)
        {
            idTarget += 1;
            if (idTarget == posRasga.Length)
            {
                idTarget = 0;
            }
        }

        if (Rasga.position.x > posRasga[idTarget].position.x && !isLookLeft)
        {
            Flip();
        }
        else if (Rasga.position.x < posRasga[idTarget].position.x && isLookLeft)
        {
            Flip();
        }
    }

    private void Flip()
    {
        isLookLeft = !isLookLeft;
        rasgaSr.flipX = !rasgaSr.flipX;
    }

    public void SetPause(bool pause)
    {
        _isGamePaused = pause;
        animator.enabled = !pause;
    }

    public void UpdateCanAttack(bool value)
    {
        canAttack = value;
    }

    public void RasgaAffected()
    {
        StartCoroutine("FlyAway");
    }

    private IEnumerator FlyAway()
    {
        SoundFxController.Instance.playFx(2);
        
        direction = (posToRun.transform.position - transform.position).normalized;
        rasgaControllerRb.velocity = direction * speedToAttack;

        yield return new WaitForSeconds(0.2f);

        SoundFxController.Instance.playFx(2);

        yield return new WaitForSeconds(0.1f);

        SoundFxController.Instance.playFx(2);
        
        MoveToLeftToRight();

        yield return new WaitForSeconds(1.5f);

        UpdateCanAttack(true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            MoveToLeftToRight();
        }
    }

}
