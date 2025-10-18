using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SafeScript : MonoBehaviour
{
    void Update()
    {
		if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.E)) && Time.timeScale != 0f)
		{
            if (gc.quarterCount <= 0)
            {
                return;
            }
			Ray ray = Camera.main.ScreenPointToRay(new Vector3((float)(Screen.width / 2), (float)(Screen.height / 2), 0f));
			RaycastHit raycastHit;
			if (Physics.Raycast(ray, out raycastHit) && (Vector3.Distance(this.player.position, base.transform.position) < 17.5f))
			{
                quarterAmount++;
                gc.AddQuarter(-1);
                if (quarterAmount == maxQuarters)
                {
                    OpenTheDoor();
                    quarterCounter.gameObject.SetActive(false);
                }
                quarterCounter.text = $"{quarterAmount}/{maxQuarters}";
            }
		}
	}

    void OpenTheDoor()
    {
        door.SetActive(false);
        apple.SetActive(true);
    }

    byte maxQuarters = 3;
    byte quarterAmount;
    public TMP_Text quarterCounter;

    public Transform player;
    public GameObject door, apple;
    public GameControllerScript gc;
}
