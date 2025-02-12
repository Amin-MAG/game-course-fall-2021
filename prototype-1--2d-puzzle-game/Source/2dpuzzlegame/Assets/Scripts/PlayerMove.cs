﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMove : MonoBehaviour
{
    public float factor = 0.01f;
    public float jumpAmount = 0.5f;

    public SpriteRenderer spriteRenderer;
    public Rigidbody2D rb;

    public GameObject clones;
    public CloneMove[] cloneMoves;

    private bool canJump;

    private Vector3 moveVector;

    // new variables
    public int collectedKeys = 0;
    private GameObject pickableItem = null;
    public EventSystemCustom eventSystem;
    private bool isNearExitDoor = false;
    public GameObject isNearTeleportSource = null;
    public bool hasTeleportKey = false;

    void Start()
    {
        cloneMoves = clones.GetComponentsInChildren<CloneMove>();

        canJump = true;
        moveVector = new Vector3(4 * factor, 0, 0);
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += moveVector;

            MoveClones(moveVector, true);

            spriteRenderer.flipX = false;
        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.position -= moveVector;

            MoveClones(moveVector, false);

            spriteRenderer.flipX = true;
        }

        // E To pick a pickable item for clones
        if (Input.GetKey(KeyCode.E))
        {
            foreach (var cloneMove in cloneMoves)
            {
                if (cloneMove.pickableItem != null)
                {
                    if (cloneMove.pickableItem.gameObject.tag.Equals(TagNames.TeleportKey.ToString()))
                    {
                        hasTeleportKey = true;
                    }
                    else if (cloneMove.pickableItem.gameObject.tag.Equals(TagNames.PickableItem.ToString()))
                    {
                        collectedKeys++;
                        eventSystem.OnKeyPickUp.Invoke();
                    }

                    cloneMove.pickableItem.gameObject.SetActive(false);
                }
            }
        }

        // E To pick a pickable item for main character 
        if (Input.GetKey(KeyCode.E) && pickableItem != null)
        {
            if (pickableItem.gameObject.tag.Equals(TagNames.TeleportKey.ToString()))
            {
                hasTeleportKey = true;
            }
            else if (pickableItem.gameObject.tag.Equals(TagNames.PickableItem.ToString()))
            {
                collectedKeys++;
                eventSystem.OnKeyPickUp.Invoke();
            }

            pickableItem.SetActive(false);
        }

        // E To exit the room and win the game
        if (Input.GetKey(KeyCode.E) && isNearExitDoor && collectedKeys == 5)
        {
            // Debug.Log(GameObject.FindGameObjectsWithTag(TagNames.PickableItem.ToString()).Length);
            this.gameObject.SetActive(false);
            eventSystem.WinningGame.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.Space) && canJump)
        {
            rb.AddForce(transform.up * jumpAmount, ForceMode2D.Impulse);
            JumpClones(jumpAmount);
        }



        // This was added to answer a question.
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Destroy(this.gameObject);
        }


        // This is too dirty. We must decalare/calculate the bounds in another way. 
        /*if (transform.position.x < -0.55f) 
        {
            transform.position = new Vector3(0.51f, transform.position.y, transform.position.z);
        }
        else if (transform.position.x > 0.53f)
        {
            transform.position = new Vector3(-0.53f, transform.position.y, transform.position.z);
        }*/
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(TagNames.DeathZone.ToString()))
        {
            Debug.Log("DEATH ZONE");
            eventSystem.LoosingGame.Invoke();
        }

        if (collision.gameObject.CompareTag(TagNames.CollectableItem.ToString()))
        {
            collision.gameObject.SetActive(false);
            Debug.Log("POTION! enter trigger magic glass");
        }

        if (collision.gameObject.CompareTag(TagNames.PickableItem.ToString()) ||
            collision.gameObject.CompareTag(TagNames.TeleportKey.ToString()))
        {
            Debug.Log("POTION! enter trigger pickable");
            pickableItem = collision.gameObject;
        }

        if (collision.gameObject.CompareTag(TagNames.ExitDoor.ToString()))
        {
            Debug.Log("POTION! enter trigger exit door");
            isNearExitDoor = true;
        }


        if (collision.gameObject.CompareTag(TagNames.TeleportDoor.ToString()))
        {
            isNearTeleportSource = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(TagNames.PickableItem.ToString()))
        {
            Debug.Log("POTION! exit trigger pickable key");
            pickableItem = null;
        }


        if (collision.gameObject.CompareTag(TagNames.ExitDoor.ToString()))
        {
            Debug.Log("POTION! exit trigger exit door");
            isNearExitDoor = false;
        }


        if (collision.gameObject.CompareTag(TagNames.TeleportDoor.ToString()))
        {
            isNearTeleportSource = null;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(TagNames.StickyPlatform.ToString()))
        {
            Debug.LogWarning("sticky");
            canJump = false;
        }

        if (collision.gameObject.CompareTag(TagNames.ExitDoor.ToString()))
        {
            Debug.Log("exit door");
        }

        if (collision.gameObject.CompareTag(TagNames.PickableItem.ToString()))
        {
            Debug.Log("POTION! pickable key collision enter");
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(TagNames.StickyPlatform.ToString()))
        {
            Debug.LogWarning("sticky no more bruh");
            canJump = true;
        }
    }

    private void MoveClones(Vector3 vec, bool isDirRight)
    {
        foreach (var c in cloneMoves)
            c.Move(vec, isDirRight);
    }

    private void JumpClones(float amount)
    {
        foreach (var c in cloneMoves)
            c.Jump(amount);
    }
}