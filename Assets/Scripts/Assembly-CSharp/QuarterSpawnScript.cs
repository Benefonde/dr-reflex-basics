using System;
using UnityEngine;

// Token: 0x0200001F RID: 31
public class QuarterSpawnScript : MonoBehaviour
{
	// Token: 0x0600006F RID: 111 RVA: 0x00003D0A File Offset: 0x0000210A
	private void Start()
	{
		this.wanderer.QuarterExclusive();
		if (!Manager)
        {
			transform.name = "Pickup_Quarter";
			GetComponent<AudioSource>().Play();
			QuarterCloseCheck();
        }
		base.transform.position = this.location.position + Vector3.up * 4f;
	}

	void QuarterCloseCheck()
    {
		Vector3 pos = location.position + Vector3.up * 4f;
		Collider[] colliders = Physics.OverlapSphere(pos, 0.5f);
		while (colliders.Length > 0)
		{
			foreach (Collider collider in colliders)
            {
				if (collider.gameObject.GetComponent<QuarterSpawnScript>() == null)
                {
					break;
                }
            }
			print("ok fun we have others TIME TO MIGRATEe");
			this.wanderer.QuarterExclusive();
			pos = location.position + Vector3.up * 4f;
			colliders = Physics.OverlapSphere(pos, 0.5f);
		}
		transform.position = pos;
	}

	// Token: 0x06000070 RID: 112 RVA: 0x00003D41 File Offset: 0x00002141
	private void Update()
	{
		if (gc.mode == "endless" && gc.spoopMode && Manager)
        {
			waitTime -= Time.deltaTime;
			if (waitTime <= 0 && FindObjectsOfType<QuarterSpawnScript>().Length < 12) //10 bonus quarters only
            {
				waitTime = 75 + (FindObjectsOfType<QuarterSpawnScript>().Length * 5);
				QuarterSpawnScript quarter = Instantiate(gameObject, transform.parent).GetComponent<QuarterSpawnScript>();
				quarter.Manager = false;
            }
        }
	}

	// Token: 0x0400008C RID: 140
	public AILocationSelectorScript wanderer;

	// Token: 0x0400008D RID: 141
	public Transform location;

	public GameControllerScript gc;

	public bool Manager; //there should only be 1 quarter with this bool

	[SerializeField]
	float waitTime = 90;
}
