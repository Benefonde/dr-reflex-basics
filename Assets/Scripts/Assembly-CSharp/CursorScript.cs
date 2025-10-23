using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CursorScript : MonoBehaviour
{
    void Start()
    {
        sprite = GetComponent<Image>();
        sprite.enabled = false;
    }
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3((float)(Screen.width / 2), (float)(Screen.height / 2), 0f));
        Physics.Raycast(ray, out RaycastHit raycastHit);
        if (raycastHit.transform == null)
        {
            return;
        }
        string a = raycastHit.transform.tag;
        if (Vector3.Distance(player.position, raycastHit.transform.position) > 10 && (a == "Notebook" || a == "Item"))
        {
            sprite.enabled = false;
            return;
        }
        if (a == "Door" || a == "Notebook" || a == "Interactable" || a == "Item")
        {
            if (Vector3.Distance(player.position, raycastHit.transform.position) > 15)
            {
                sprite.enabled = false;
                return;
            }
            sprite.enabled = true;
        }
        else
        {
            sprite.enabled = false;
        }
    }
    Image sprite;
    public Transform player;
}