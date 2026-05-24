using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace SojaExiles
{
    public class OpenDoorKey : MonoBehaviour
    {
        public Animator openandclose;
        public bool open;
        public Transform Player;
        public bool isCode = true;

        void Start()
        {
            open = false;
            GameObject playerObj = GameObject.Find("Player");
            if (playerObj != null)
            {
                Player = playerObj.transform;
            }
            else
            {
                Debug.LogWarning("Không tìm thấy GameObject tên là 'Player' trong Hierarchy.");
            }
        }

        void OnMouseOver()
        {
            if (!isCode) return;
            if (Player)
            {
                float dist = Vector3.Distance(Player.position, transform.position);
                if (dist < 15f)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (!open)
                            StartCoroutine(opening());
                        else
                            StartCoroutine(closing());
                    }
                }
            }
        }

        public void Open()
        {
            if (Player)
            {
                float dist = Vector3.Distance(Player.position, transform.position);
                if (dist < 15f)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (!open)
                            StartCoroutine(opening());
                        else
                            StartCoroutine(closing());
                    }
                }
            }
            isCode = true;
        }

        IEnumerator opening()
        {
            Debug.Log("you are opening the door");
            openandclose.SetBool("open", true);
            open = true;
            yield return new WaitForSeconds(3f);
        }

        IEnumerator closing()
        {
            Debug.Log("you are closing the door");
            openandclose.SetBool("open", false);
            open = false;
            yield return new WaitForSeconds(2f);
        }
    }
}
