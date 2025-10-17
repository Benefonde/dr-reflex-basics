using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TutorBaldiScript : MonoBehaviour
{
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (!anger)
        {
            float angleDifference = Mathf.DeltaAngle(base.transform.eulerAngles.y, Mathf.Atan2(player.position.x - base.transform.position.x, player.position.z - base.transform.position.z) * 57.29578f);
            if (pause > 0)
            {
                pause -= Time.deltaTime;
                return;
            }
            transform.Rotate(new Vector3(0f, this.turnSpeed * Mathf.Sign(angleDifference) * Time.deltaTime, 0f));
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.y * Vector3.up);
        }
        else
        {
            if (Vector3.Distance(transform.position, baldi.position) <= 3 && agent.destination != null)
            {
                baldi.gameObject.SetActive(true);
                gameObject.SetActive(false);
            }
        }
	}

    private void FixedUpdate()
    {
        if (anger)
        {
            if (delay <= 2)
            {
                delay++;
                return;
            }
            fpss.animFrame++;
            delay = 0;
        }
    }

    public void GetAngry()
    {
        agent.enabled = true;
        agent.SetDestination(baldi.position);
        disableOnAngerFpss.gameObject.SetActive(false);
        fpss.gameObject.SetActive(true);
        anger = true;
        fpss.animationMode = true;
    }

    public Transform player;

    public float turnSpeed;

    float pause = 0.25f;

    public bool anger;

    NavMeshAgent agent;

    public Transform baldi;
    public FirstPrizeSpriteScript disableOnAngerFpss, fpss;

    int delay;
}
