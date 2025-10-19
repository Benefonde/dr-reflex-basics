using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Token: 0x020000C0 RID: 192
public class GameControllerScript : MonoBehaviour
{
	// Token: 0x06000963 RID: 2403 RVA: 0x00021A00 File Offset: 0x0001FE00
	public GameControllerScript()
	{
		int[] array = new int[3] { -23, 17, 57};
		itemSelectOffset = array;
		//base..ctor();
	}

	// Token: 0x06000964 RID: 2404 RVA: 0x00021AC4 File Offset: 0x0001FEC4
	private void Start()
	{
		cullingMask = camera.cullingMask; // Changes cullingMask in the Camera
		audioDevice = base.GetComponent<AudioSource>(); //Get the Audio Source
		mode = PlayerPrefs.GetString("CurrentMode"); //Get the current mode
		if (mode == "endless") //If it is endless mode
		{
			baldiScrpt.endless = true; //Set Baldi use his slightly changed endless anger system
		}
		schoolMusic.Play(); //Play the school music
		LockMouse(); //Prevent the mouse from moving
		UpdateNotebookCount(); //Update the notebook count
		UpdateItemName();
		itemSelected = 0; //Set selection to item slot 0(the first item slot)
		gameOverDelay = 0.5f;
	}

	// Token: 0x06000965 RID: 2405 RVA: 0x00021B5C File Offset: 0x0001FF5C
	private void Update()
	{
		if (!learningActive)
		{
			if (Input.GetButtonDown("Pause"))
			{
				if (!gamePaused)
				{
					PauseGame();
				}
				else
				{
					UnpauseGame();
				}
			}
			if (Input.GetKeyDown(KeyCode.Y) & gamePaused)
			{
				ExitGame();
			}
			else if (Input.GetKeyDown(KeyCode.N) & gamePaused)
			{
				UnpauseGame();
			}
			if (!gamePaused & Time.timeScale != 1f)
			{
				Time.timeScale = 1f;
			}
			if ((Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0)) && quarterCount > 0)
            {
				QuarterCheck();
			}
			if ((Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Q)) && Time.timeScale != 0f)
			{
				UseItem();
			}
			if ((Input.GetAxis("Mouse ScrollWheel") > 0f && Time.timeScale != 0f))
			{
				DecreaseItemSelection();
			}
			else if ((Input.GetAxis("Mouse ScrollWheel") < 0f && Time.timeScale != 0f))
			{
				IncreaseItemSelection();
			}
			if (Time.timeScale != 0f)
			{
				if (Input.GetKeyDown(KeyCode.Alpha1))
				{
					itemSelected = 0;
					UpdateItemSelection();
				}
				else if (Input.GetKeyDown(KeyCode.Alpha2))
				{
					itemSelected = 1;
					UpdateItemSelection();
				}
				else if (Input.GetKeyDown(KeyCode.Alpha3))
				{
					itemSelected = 2;
					UpdateItemSelection();
				}
			}
		}
		else
		{
			if (Time.timeScale != 0f)
			{
				Time.timeScale = 0f;
			}
		}
		if (player.stamina < 0f & !warning.activeSelf)
		{
			warning.SetActive(true); //Set the warning text to be visible
		}
		else if (player.stamina > 0f & warning.activeSelf)
		{
			warning.SetActive(false); //Set the warning text to be invisible
		}
		if (player.gameOver)
		{
			if (mode == "endless" && notebooks > PlayerPrefs.GetInt("HighBooks") && !highScoreText.activeSelf)
			{
				highScoreText.SetActive(true);
			}
			Time.timeScale = 0f;
			gameOverDelay -= Time.unscaledDeltaTime * 0.5f;
			camera.farClipPlane = gameOverDelay * 400f; //Set camera farClip 
			audioDevice.PlayOneShot(aud_buzz);
			if (gameOverDelay <= 0f)
			{
				if (mode == "endless")
				{
					if (notebooks > PlayerPrefs.GetInt("HighBooks"))
					{
						PlayerPrefs.SetInt("HighBooks", notebooks);
					}
					PlayerPrefs.SetInt("CurrentBooks", notebooks);
				}
				Time.timeScale = 1f;
				SceneManager.LoadScene("GameOver");
			}
		}
		if (finaleMode && !audioDevice.isPlaying && exitsReached == 3)
		{
			audioDevice.clip = aud_MachineLoop;
			audioDevice.loop = true;
			audioDevice.Play();
		}
	}

	// Token: 0x06000966 RID: 2406 RVA: 0x00021F8C File Offset: 0x0002038C
	private void UpdateNotebookCount()
	{
		if (mode == "story" && notebooks != 6) // fuck you
		{
			notebookCount.text = notebooks.ToString() + "/7 Notebooks";
		}
		else
		{
			notebookCount.text = notebooks.ToString() + " Notebooks";
		}
		if (notebooks == 7 & mode == "story")
		{
			ActivateFinaleMode();
		}
	}

	// Token: 0x06000967 RID: 2407 RVA: 0x00022024 File Offset: 0x00020424
	public void CollectNotebook()
	{
		notebooks++;
		UpdateNotebookCount();
	}

	// Token: 0x06000968 RID: 2408 RVA: 0x0002203A File Offset: 0x0002043A
	public void LockMouse()
	{
		if (!learningActive)
		{
			cursorController.LockCursor(); //Prevent the cursor from moving
			mouseLocked = true;
			reticle.SetActive(true);
		}
	}

	// Token: 0x06000969 RID: 2409 RVA: 0x00022065 File Offset: 0x00020465
	public void UnlockMouse()
	{
		cursorController.UnlockCursor(); //Allow the cursor to move
		mouseLocked = false;
		reticle.SetActive(false);
	}

	// Token: 0x0600096A RID: 2410 RVA: 0x00022085 File Offset: 0x00020485
	public void PauseGame()
	{
		if (!learningActive)
		{
			{
				UnlockMouse();
			}
			Time.timeScale = 0f;
			gamePaused = true;
			pauseMenu.SetActive(true);
		}
	}

	// Token: 0x0600096B RID: 2411 RVA: 0x000220C5 File Offset: 0x000204C5
	public void ExitGame()
	{
		SceneManager.LoadScene("MainMenu");
	}

	// Token: 0x0600096C RID: 2412 RVA: 0x000220D1 File Offset: 0x000204D1
	public void UnpauseGame()
	{
		Time.timeScale = 1f;
		gamePaused = false;
		pauseMenu.SetActive(false);
		LockMouse();
	}

	// Token: 0x0600096D RID: 2413 RVA: 0x000220F8 File Offset: 0x000204F8
	public void ActivateSpoopMode()
	{
		spoopMode = true; //Tells the game its time for spooky
		entrance_0.Lower(); //Lowers all the exits
		entrance_1.Lower();
		entrance_2.Lower();
		entrance_3.Lower();
		baldiTutor.GetComponent<TutorBaldiScript>().GetAngry(); //Anger friendly Baldi
        principal.SetActive(true); //Turns on Principal
        crafters.SetActive(true); //Turns on Crafters
        playtime.SetActive(true); //Turns on Playtime
        gottaSweep.SetActive(true); //Turns on Gotta Sweep
        bully.SetActive(true); //Turns on Bully
        firstPrize.SetActive(true); //Turns on First-Prize
		//TestEnemy.SetActive(true); //Turns on Test-Enemy
		audioDevice.PlayOneShot(aud_Hang); //Plays the hang sound
		learnMusic.Stop(); //Stop all the music
		schoolMusic.Stop();
	}

	// Token: 0x0600096E RID: 2414 RVA: 0x000221BF File Offset: 0x000205BF
	private void ActivateFinaleMode()
	{
		finaleMode = true;
		entrance_0.Raise(); //Raise all the enterances(make them appear)
		entrance_1.Raise();
		entrance_2.Raise();
		entrance_3.Raise();
	}

	// Token: 0x0600096F RID: 2415 RVA: 0x000221F4 File Offset: 0x000205F4
	public void GetAngry(float value) //Make Baldi get angry
	{
		if (!spoopMode)
		{
			ActivateSpoopMode();
		}
		baldiScrpt.GetAngry(value);
	}

	// Token: 0x06000970 RID: 2416 RVA: 0x00022214 File Offset: 0x00020614
	public void ActivateLearningGame()
	{
		//camera.cullingMask = 0; //Sets the cullingMask to nothing
		learningActive = true;
		UnlockMouse(); //Unlock the mouse
		tutorBaldi.Stop(); //Make tutor Baldi stop talking
		if (!spoopMode) //If the player hasn't gotten a question wrong
		{
			schoolMusic.Stop(); //Start playing the learn music
			learnMusic.Play();
		}
	}

	// Token: 0x06000971 RID: 2417 RVA: 0x00022278 File Offset: 0x00020678
	public void DeactivateLearningGame(GameObject subject)
	{
		camera.cullingMask = cullingMask; //Sets the cullingMask to Everything
		learningActive = false;
		UnityEngine.Object.Destroy(subject);
		LockMouse(); //Prevent the mouse from moving
		if (player.stamina < 100f) //Reset Stamina
		{
			player.stamina = 100f;
		}
		if (!spoopMode) //If it isn't spoop mode, play the school music
		{
			schoolMusic.Play();
			learnMusic.Stop();
		}
		if (notebooks == 1 & !spoopMode) // If this is the players first notebook and they didn't get any questions wrong, reward them with a quarter
		{
			quarter.SetActive(true);
			tutorBaldi.PlayOneShot(aud_Prize);
		}
		else if (notebooks == 7 & mode == "story") // Plays the all 7 notebook sound
		{
			audioDevice.PlayOneShot(aud_AllNotebooks, 0.8f);
		}
	}

	// Token: 0x06000972 RID: 2418 RVA: 0x00022360 File Offset: 0x00020760
	private void IncreaseItemSelection()
	{
		itemSelected++;
		if (itemSelected > 2)
		{
			itemSelected = 0;
		}
		itemSelect.anchoredPosition = new Vector3((float)itemSelectOffset[itemSelected], 18f, 0f); //Moves the item selector background(the red rectangle)
		UpdateItemName();
	}

	// Token: 0x06000973 RID: 2419 RVA: 0x000223C4 File Offset: 0x000207C4
	private void DecreaseItemSelection()
	{
		itemSelected--;
		if (itemSelected < 0)
		{
			itemSelected = 2;
		}
		itemSelect.anchoredPosition = new Vector3((float)itemSelectOffset[itemSelected], 18f, 0f); //Moves the item selector background(the red rectangle)
		UpdateItemName();
	}

	// Token: 0x06000974 RID: 2420 RVA: 0x00022425 File Offset: 0x00020825
	private void UpdateItemSelection()
	{
		itemSelect.anchoredPosition = new Vector3((float)itemSelectOffset[itemSelected], 18f, 0f); //Moves the item selector background(the red rectangle)
		UpdateItemName();
	}

	// Token: 0x06000975 RID: 2421 RVA: 0x0002245C File Offset: 0x0002085C
	public void CollectItem(int item_ID)
	{
		if (item_ID == 5)
        {
			AddQuarter();
			return;
        }

		if (item[0] == 0)
		{
			item[0] = item_ID; //Set the item slot to the Item_ID provided
			itemSlot[0].texture = hudItemTextures[item_ID]; //Set the item slot's texture to a texture in a list of textures based on the Item_ID
		}
		else if (item[1] == 0)
		{
			item[1] = item_ID; //Set the item slot to the Item_ID provided
            itemSlot[1].texture = hudItemTextures[item_ID]; //Set the item slot's texture to a texture in a list of textures based on the Item_ID
        }
		else if (item[2] == 0)
		{
			item[2] = item_ID; //Set the item slot to the Item_ID provided
            itemSlot[2].texture = hudItemTextures[item_ID]; //Set the item slot's texture to a texture in a list of textures based on the Item_ID
        }
		else //This one overwrites the currently selected slot when your inventory is full
		{
			item[itemSelected] = item_ID;
			itemSlot[itemSelected].texture = hudItemTextures[item_ID];
		}
		UpdateItemName();
	}

	public void AddQuarter(sbyte amount = 1)
    {
		quarterCount += amount;
		if (quarterCount < 0)
        {
			quarterCount = 0;
        }
		if (quarterCount > 4)
        {
			quarterCount = 4;
        }
		UpdateItemName();
    }

	void QuarterCheck()
    {
		Ray ray3 = Camera.main.ScreenPointToRay(new Vector3((float)(Screen.width / 2), (float)(Screen.height / 2), 0f));
		RaycastHit raycastHit3;
		if (Physics.Raycast(ray3, out raycastHit3))
		{
			if (raycastHit3.collider.name == "BSODAMachine" & Vector3.Distance(playerTransform.position, raycastHit3.transform.position) <= 10f)
			{
				AddQuarter(-1);
				CollectItem(4);
			}
			else if (raycastHit3.collider.name == "ZestyMachine" & Vector3.Distance(playerTransform.position, raycastHit3.transform.position) <= 10f)
			{
				AddQuarter(-1);
				CollectItem(1);
			}
			else if (raycastHit3.collider.name == "PayPhone" & Vector3.Distance(playerTransform.position, raycastHit3.transform.position) <= 10f)
			{
				raycastHit3.collider.gameObject.GetComponent<TapePlayerScript>().Play();
				AddQuarter(-1);
			}
		}
	}

	// Token: 0x06000976 RID: 2422 RVA: 0x00022528 File Offset: 0x00020928
	private void UseItem()
	{
		if (item[itemSelected] != 0)
		{
			if (item[itemSelected] == 1)
			{
				player.stamina = player.maxStamina * 2f;
				ResetItem();
			}
			else if (item[itemSelected] == 2)
			{
				Ray ray = Camera.main.ScreenPointToRay(new Vector3((float)(Screen.width / 2), (float)(Screen.height / 2), 0f));
				RaycastHit raycastHit;
				if (Physics.Raycast(ray, out raycastHit) && (raycastHit.collider.tag == "SwingingDoor" & Vector3.Distance(playerTransform.position, raycastHit.transform.position) <= 10f))
				{
					raycastHit.collider.gameObject.GetComponent<SwingingDoorScript>().LockDoor(15f);
					ResetItem();
				}
			}
			else if (item[itemSelected] == 3)
			{
				Ray ray2 = Camera.main.ScreenPointToRay(new Vector3((float)(Screen.width / 2), (float)(Screen.height / 2), 0f));
				RaycastHit raycastHit2;
				if (Physics.Raycast(ray2, out raycastHit2) && (raycastHit2.collider.tag == "Door" & Vector3.Distance(playerTransform.position, raycastHit2.transform.position) <= 10f))
				{
					DoorScript component = raycastHit2.collider.gameObject.GetComponent<DoorScript>();
					if (component.DoorLocked)
					{
						component.UnlockDoor();
						component.OpenDoor();
						ResetItem();
					}
				}
			}
			else if (item[itemSelected] == 4)
			{
				UnityEngine.Object.Instantiate<GameObject>(bsodaSpray, playerTransform.position, cameraTransform.rotation);
				ResetItem();
				player.ResetGuilt("drink", 1f);
				audioDevice.PlayOneShot(aud_Soda);
			}
			else if (item[itemSelected] == 5)
			{
				// replaced with QuarterCheck()
			}
			else if (item[itemSelected] == 6)
			{
				Ray ray4 = Camera.main.ScreenPointToRay(new Vector3((float)(Screen.width / 2), (float)(Screen.height / 2), 0f));
				RaycastHit raycastHit4;
				if (Physics.Raycast(ray4, out raycastHit4) && (raycastHit4.collider.name == "TapePlayer" & Vector3.Distance(playerTransform.position, raycastHit4.transform.position) <= 10f))
				{
					raycastHit4.collider.gameObject.GetComponent<TapePlayerScript>().Play();
					ResetItem();
				}
			}
			else if (item[itemSelected] == 7)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(alarmClock, playerTransform.position, cameraTransform.rotation);
				gameObject.GetComponent<AlarmClockScript>().baldi = baldiScrpt;
				ResetItem();
			}
			else if (item[itemSelected] == 8)
			{
				Ray ray5 = Camera.main.ScreenPointToRay(new Vector3((float)(Screen.width / 2), (float)(Screen.height / 2), 0f));
				RaycastHit raycastHit5;
				if (Physics.Raycast(ray5, out raycastHit5) && (raycastHit5.collider.tag == "Door" & Vector3.Distance(playerTransform.position, raycastHit5.transform.position) <= 10f))
				{
					raycastHit5.collider.gameObject.GetComponent<DoorScript>().SilenceDoor();
					ResetItem();
					audioDevice.PlayOneShot(aud_Spray);
				}
			}
			else if (item[itemSelected] == 9)
			{
				Ray ray6 = Camera.main.ScreenPointToRay(new Vector3((float)(Screen.width / 2), (float)(Screen.height / 2), 0f));
				RaycastHit raycastHit6;
				if (player.jumpRope)
				{
					player.DeactivateJumpRope();
					playtimeScript.Disappoint();
					ResetItem();
				}
				else if (Physics.Raycast(ray6, out raycastHit6) && raycastHit6.collider.name == "1st Prize")
				{
					firstPrizeScript.GoCrazy();
					ResetItem();
				}
			}
			else if (item[itemSelected] == 10)
			{
				player.ActivateBoots();
				base.StartCoroutine(BootAnimation());
				ResetItem();
			}
		}
	}

	// Token: 0x06000977 RID: 2423 RVA: 0x00022B40 File Offset: 0x00020F40
	private IEnumerator BootAnimation()
	{
		float time = 15f;
		float height = 375f;
		Vector3 position = default(Vector3);
		boots.gameObject.SetActive(true);
		while (height > -375f)
		{
			height -= 375f * Time.deltaTime;
			time -= Time.deltaTime;
			position = boots.localPosition;
			position.y = height;
			boots.localPosition = position;
			yield return null;
		}
		position = boots.localPosition;
		position.y = -375f;
		boots.localPosition = position;
		boots.gameObject.SetActive(false);
		while (time > 0f)
		{
			time -= Time.deltaTime;
			yield return null;
		}
		boots.gameObject.SetActive(true);
		while (height < 375f)
		{
			height += 375f * Time.deltaTime;
			position = boots.localPosition;
			position.y = height;
			boots.localPosition = position;
			yield return null;
		}
		position = boots.localPosition;
		position.y = 375f;
		boots.localPosition = position;
		boots.gameObject.SetActive(false);
		yield break;
	}

	// Token: 0x06000978 RID: 2424 RVA: 0x00022B5B File Offset: 0x00020F5B
	private void ResetItem()
	{
		item[itemSelected] = 0;
		itemSlot[itemSelected].texture = hudItemTextures[0];
		UpdateItemName();
	}

	// Token: 0x06000979 RID: 2425 RVA: 0x00022B8B File Offset: 0x00020F8B
	public void LoseItem(int id)
	{
		item[id] = 0;
		itemSlot[id].texture = hudItemTextures[0];
		UpdateItemName();
	}

	// Token: 0x0600097A RID: 2426 RVA: 0x00022BB1 File Offset: 0x00020FB1
	private void UpdateItemName()
	{
		itemText.text = itemNames[item[itemSelected]];
		for (int i = 0; i < 4; i++)
		{
			quarterSlot[i].gameObject.SetActive(false);
		}
		for (int i = 0; i < quarterCount; i++)
        {
			quarterSlot[i].gameObject.SetActive(true);
        }
		if (quarterCount > 0)
        {
			quarterCounter.text = quarterCount.ToString();
			if (quarterCount >= 4)
            {
				quarterCounter.color = Color.red;
            }
            else
			{
				quarterCounter.color = Color.black;
			}
        }
        else
        {
			quarterCounter.text = string.Empty;
        }
	}

	// Token: 0x0600097B RID: 2427 RVA: 0x00022BD4 File Offset: 0x00020FD4
	public void ExitReached()
	{
		exitsReached++;
		if (exitsReached == 1)
		{
			RenderSettings.ambientLight = Color.red; //Make everything red and start player the weird sound
			//RenderSettings.fog = true;
			audioDevice.PlayOneShot(aud_Switch, 0.8f);
			audioDevice.clip = aud_MachineQuiet;
			audioDevice.loop = true;
			audioDevice.Play();
		}
		if (exitsReached == 2) //Play a sound
		{
			audioDevice.clip = aud_MachineStart;
			audioDevice.loop = true;
			audioDevice.Play();
		}
		if (exitsReached == 3) //Play a even louder sound
		{
			audioDevice.clip = aud_MachineRev;
			audioDevice.loop = false;
			audioDevice.Play();
		}
	}

	// Token: 0x0600097C RID: 2428 RVA: 0x00022CC1 File Offset: 0x000210C1
	public void DespawnCrafters()
	{
		crafters.SetActive(false); //Make Arts And Crafters Inactive
	}

	// Token: 0x0600097D RID: 2429 RVA: 0x00022CD0 File Offset: 0x000210D0
	public void Fliparoo()
	{
		player.height = 6f;
		player.fliparoo = 180f;
		player.flipaturn = -1f;
		Camera.main.GetComponent<CameraScript>().offset = new Vector3(0f, -1f, 0f);
	}

	// Token: 0x040005F7 RID: 1527
	public CursorControllerScript cursorController;

	// Token: 0x040005F8 RID: 1528
	public PlayerScript player;

	// Token: 0x040005F9 RID: 1529
	public Transform playerTransform;

	// Token: 0x040005FA RID: 1530
	public Transform cameraTransform;

	// Token: 0x040005FB RID: 1531
	public Camera camera;

	// Token: 0x040005FC RID: 1532
	private int cullingMask;

	// Token: 0x040005FD RID: 1533
	public EntranceScript entrance_0;

	// Token: 0x040005FE RID: 1534
	public EntranceScript entrance_1;

	// Token: 0x040005FF RID: 1535
	public EntranceScript entrance_2;

	// Token: 0x04000600 RID: 1536
	public EntranceScript entrance_3;

	// Token: 0x04000601 RID: 1537
	public GameObject baldiTutor;

	// Token: 0x04000602 RID: 1538
	public GameObject baldi;

	// Token: 0x04000603 RID: 1539
	public BaldiScript baldiScrpt;

	// Token: 0x04000604 RID: 1540
	public AudioClip aud_Prize;

	// Token: 0x04000605 RID: 1541
	public AudioClip aud_PrizeMobile;

	// Token: 0x04000606 RID: 1542
	public AudioClip aud_AllNotebooks;

	// Token: 0x04000607 RID: 1543
	public GameObject principal;

	// Token: 0x04000608 RID: 1544
	public GameObject crafters;

	// Token: 0x04000609 RID: 1545
	public GameObject playtime;

	// Token: 0x0400060A RID: 1546
	public PlaytimeScript playtimeScript;

	// Token: 0x0400060B RID: 1547
	public GameObject gottaSweep;

	// Token: 0x0400060C RID: 1548
	public GameObject bully;

	// Token: 0x0400060D RID: 1549
	public GameObject firstPrize;

	// Token: 0x0400060D RID: 1549
	public GameObject TestEnemy;

	// Token: 0x0400060E RID: 1550
	public FirstPrizeScript firstPrizeScript;

	// Token: 0x0400060F RID: 1551
	public GameObject quarter;

	// Token: 0x04000610 RID: 1552
	public AudioSource tutorBaldi;

	// Token: 0x04000611 RID: 1553
	public RectTransform boots;

	// Token: 0x04000612 RID: 1554
	public string mode;

	// Token: 0x04000613 RID: 1555
	public int notebooks;

	// Token: 0x04000614 RID: 1556
	public GameObject[] notebookPickups;

	// Token: 0x04000615 RID: 1557
	public int failedNotebooks;

	// Token: 0x04000616 RID: 1558
	public bool spoopMode;

	// Token: 0x04000617 RID: 1559
	public bool finaleMode;

	// Token: 0x04000618 RID: 1560
	public bool debugMode;

	// Token: 0x04000619 RID: 1561
	public bool mouseLocked;

	// Token: 0x0400061A RID: 1562
	public int exitsReached;

	// Token: 0x0400061B RID: 1563
	public int itemSelected;

	// Token: 0x0400061C RID: 1564
	public int[] item = new int[3];

	// Token: 0x0400061D RID: 1565
	public RawImage[] itemSlot = new RawImage[3];

	public RawImage[] quarterSlot;
	public TMP_Text quarterCounter;

	// Token: 0x0400061E RID: 1566
	private string[] itemNames = new string[]
	{
		"Nothing",
		"Energy flavored Zesty Bar",
		"Yellow Door Lock",
		"Principal's Keys",
		"BSODA",
		"Quarter",
		"Reflex Anti Hearing and Disorienting Tape",
		"Alarm Clock",
		"WD-NoSquee (Door Type)",
		"Safety Scissors",
		"Big Ol' Boots",
		"An Apple"
	};

	// Token: 0x0400061F RID: 1567
	public TMP_Text itemText;

	// Token: 0x04000620 RID: 1568
	public sbyte quarterCount;

	// Token: 0x04000621 RID: 1569
	public Texture[] itemTextures = new Texture[11];
	public Texture[] hudItemTextures = new Texture[10];

	// Token: 0x04000622 RID: 1570
	public GameObject bsodaSpray;

	// Token: 0x04000623 RID: 1571
	public GameObject alarmClock;

	// Token: 0x04000624 RID: 1572
	public TMP_Text notebookCount;

	// Token: 0x04000625 RID: 1573
	public GameObject pauseMenu;

	// Token: 0x04000626 RID: 1574
	public GameObject highScoreText;

	// Token: 0x04000627 RID: 1575
	public GameObject warning;

	// Token: 0x04000628 RID: 1576
	public GameObject reticle;

	// Token: 0x04000629 RID: 1577
	public RectTransform itemSelect;

	// Token: 0x0400062A RID: 1578
	private int[] itemSelectOffset;

	// Token: 0x0400062B RID: 1579
	private bool gamePaused;

	// Token: 0x0400062C RID: 1580
	private bool learningActive;

	// Token: 0x0400062D RID: 1581
	private float gameOverDelay;

	// Token: 0x0400062E RID: 1582
	private AudioSource audioDevice;

	// Token: 0x0400062F RID: 1583
	public AudioClip aud_Soda;

	// Token: 0x04000630 RID: 1584
	public AudioClip aud_Spray;

	// Token: 0x04000631 RID: 1585
	public AudioClip aud_buzz;

	// Token: 0x04000632 RID: 1586
	public AudioClip aud_Hang;

	// Token: 0x04000633 RID: 1587
	public AudioClip aud_MachineQuiet;

	// Token: 0x04000634 RID: 1588
	public AudioClip aud_MachineStart;

	// Token: 0x04000635 RID: 1589
	public AudioClip aud_MachineRev;

	// Token: 0x04000636 RID: 1590
	public AudioClip aud_MachineLoop;

	// Token: 0x04000637 RID: 1591
	public AudioClip aud_Switch;

	// Token: 0x04000638 RID: 1592
	public AudioSource schoolMusic;

	// Token: 0x04000639 RID: 1593
	public AudioSource learnMusic;

	// Token: 0x0400063A RID: 1594
	//private Player playerInput;
}
