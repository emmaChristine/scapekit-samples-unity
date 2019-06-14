using System;
using UnityEngine;
using System.Collections;
using GoogleARCore;

public class robotAnimScript : MonoBehaviour
{
    Vector3 rot = Vector3.zero;
    float rotSpeed = 40f;
    Animator anim;
    private Vector3 position;
    private Vector2 _fp; // first finger position
    private Vector2 _lp; // last finger position
    private float _angle;
    private float _swipeDistanceX;
    private float _swipeDistanceY;

    // Use this for initialization
    void Awake()
    {
        anim = gameObject.GetComponent<Animator>();
        //gameObject.transform.eulerAngles = rot;
    }

    // Update is called once per frame
    void Update()
    {
        CheckKey();
        UpdateMobile();
       // gameObject.transform.eulerAngles = rot;
    }

    void CheckKey()
    {
        // Walk
        if (Input.GetKey(KeyCode.W))
        {
            anim.SetBool("Walk_Anim", true);
        }
        else if (Input.GetKeyUp(KeyCode.W))
        {
            anim.SetBool("Walk_Anim", false);
        }

        // Rotate Left
        if (Input.GetKey(KeyCode.A))
        {
            rot[1] -= rotSpeed * Time.fixedDeltaTime;
        }

        // Rotate Right
        if (Input.GetKey(KeyCode.D))
        {
            rot[1] += rotSpeed * Time.fixedDeltaTime;
        }

        // Roll
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (anim.GetBool("Roll_Anim"))
            {
                anim.SetBool("Roll_Anim", false);
            }
            else
            {
                anim.SetBool("Roll_Anim", true);
            }
        }


        // Close
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (!anim.GetBool("Entry_Anim"))
            {
                anim.SetBool("Entry_Anim", true);
                anim.SetBool("Open_Anim", true);
            }
            else
            {
                anim.SetBool("Entry_Anim", false);
                anim.SetBool("Open_Anim", false);
            }
        }
        
    }

    private void UpdateMobile()
    {

        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                _fp = touch.position;
                _lp = touch.position;
            }

            if (touch.phase == TouchPhase.Moved)
            {
                _lp = touch.position;
                _swipeDistanceX = Mathf.Abs((_lp.x - _fp.x));
                _swipeDistanceY = Mathf.Abs((_lp.y - _fp.y));
            }

            if (touch.phase == TouchPhase.Ended)
            {
                _angle = Mathf.Atan2((_lp.x - _fp.x), (_lp.y - _fp.y)) * 57.2957795f;

                if (_angle > 60 && _angle < 120 && _swipeDistanceX > 40)
                {
                    if (anim.GetBool("Roll_Anim"))
                    {
                        anim.SetBool("Roll_Anim", false);
                    }
                    else
                    {
                        anim.SetBool("Roll_Anim", true);
                    }

                    Debug.Log("right swipe...");
                }

                if (_angle > 150 || _angle < -150 && _swipeDistanceY > 40)
                {
                    Debug.Log("down  swipe...");
                    if (anim.GetBool("Entry_Anim"))
                    {
                        anim.SetBool("Entry_Anim", false);
                        anim.SetBool("Open_Anim", false);
                    }
                }

                if (_angle < -60 && _angle > -120 && _swipeDistanceX > 40)
                {
                    if (anim.GetBool("Roll_Anim"))
                    {
                        anim.SetBool("Roll_Anim", false);
                    }
                    else
                    {
                        anim.SetBool("Roll_Anim", true);
                    }

                    Debug.Log("left  swipe...");
                }

                if (_angle > -30 && _angle < 30 && _swipeDistanceY > 40)
                {
                    Debug.Log("up  swipe...");
                    if (!anim.GetBool("Entry_Anim"))
                    {
                        anim.SetBool("Entry_Anim", true);
                        anim.SetBool("Open_Anim", true);
                    }
                }
            }
        }
    }
}