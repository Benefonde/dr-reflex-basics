using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorBaldiScript : MonoBehaviour
{
    void Update()
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

    public Transform player;

    public float turnSpeed;

    float pause = 0.25f;
}
