using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SafeScript : MonoBehaviour
{
    void Update()
    {
		if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.E)) && Time.timeScale != 0f && quarterAmount != maxQuarters)
		{
            if (gc.quarterCount <= 0)
            {
                return;
            }
			Ray ray = Camera.main.ScreenPointToRay(new Vector3((float)(Screen.width / 2), (float)(Screen.height / 2), 0f));
			RaycastHit raycastHit;
			if (Physics.Raycast(ray, out raycastHit) && raycastHit.transform.name == "Safe" && (Vector3.Distance(this.player.position, base.transform.position) < 17.5f))
			{
                quarterAmount++;
                gc.AddQuarter(-1);
                if (quarterAmount == maxQuarters)
                {
                    StartCoroutine(OpenTheDoor());
                    quarterCounter.gameObject.SetActive(false);
                }
                quarterCounter.text = $"{quarterAmount}/{maxQuarters}";
            }
		}
	}

    IEnumerator OpenTheDoor()
    {
        safeDoor.SetTrigger("OPEN NOW");
        yield return new WaitForSeconds(0.35f);
        apple.SetActive(true);
        GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(4.65f);
        door.SetActive(false);
    }

    byte maxQuarters = 3;
    byte quarterAmount;
    public TMP_Text quarterCounter;

    public Transform player;
    public GameObject door, apple;
    public GameControllerScript gc;

    public Animator safeDoor;
}
