﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Range(0f, 1f)] public float moveAmount = 0.068f;

    public EventSystemCustom eventSystem;

    public int playerScore = 0;
    public int playerHealth = 3;
    public float freeze = 1;

    private float elapsed = 0;
    public bool ateFreeze = false;
    public bool ateFreezeStarting = false;

    private void Start()
    {
        playerScore = 0;
    }

    void Update()
    {
        CheckGameOver();
        UpdatePlayerVel();

        if (this.ateFreeze)
        {
            if (this.ateFreezeStarting && this.freeze < 15)
            {
                this.elapsed += Time.deltaTime;
                if (this.elapsed >= 0.5f)
                {
                    this.elapsed = this.elapsed % 0.5f;
                    this.freeze += 2f;
                    Debug.Log("FFFFFreeeze " + this.freeze);
                }
            }
            else
            {
                this.ateFreezeStarting = false;
                this.elapsed += Time.deltaTime;
                if (this.elapsed >= 0.5f)
                {
                    this.elapsed = this.elapsed % 0.5f;
                    this.freeze -= 2f;
                    Debug.Log("FFFFFreeeze " + this.freeze);
                }

                if (this.freeze < 1.2f)
                {
                    this.ateFreeze = false;
                    this.freeze = 1f;
                }
            }
        }

        if (Input.GetKey(KeyCode.D))
        {
            transform.position += new Vector3(moveAmount, 0, 0);
        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.position += new Vector3(-moveAmount, 0, 0);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collide");
        if (collision.gameObject.CompareTag("Food"))
        {
            // access the food object config
            FoodItemConfig conf = collision.gameObject.GetComponent<FoodInstanceController>().config;

            // increase the player's score
            playerScore += conf.score;

            Debug.Log("SCORE: " + playerScore);

            // destroy the food object
            Destroy(collision.gameObject);

            eventSystem.onBoardScoresChanged.Invoke();
        }

        if (collision.gameObject.CompareTag("Combo"))
        {
            // polymorphism!
            // for example, the object of type "TimeFreezerComboController" which is the child of "ComboInstanceController", is put inside the "comboController" object below.
            ComboInstanceController comboController = collision.gameObject.GetComponent<ComboInstanceController>();

            // the CONTENT of OnConsume method inside "TimeFreezerComboController" is available inside the "comboController"
            comboController.OnConsume(this);

            Debug.Log("COMBO!!! " + comboController.config.comboName);
            Debug.Log("Player: " + this.ateFreeze);

            // destroy the combo object
            Destroy(collision.gameObject);

            eventSystem.onBoardScoresChanged.Invoke();
        }
    }

    private void CheckGameOver()
    {
        if (this.playerHealth == 0)
        {
            eventSystem.onGameOver.Invoke();
        }
    }

    private void UpdatePlayerVel()
    {
        if (this.playerScore > 4200)
        {
            this.moveAmount = 0.1f;
        }
        else if (this.playerScore > 6000)
        {
            this.moveAmount = 0.15f;
        }
    }
}