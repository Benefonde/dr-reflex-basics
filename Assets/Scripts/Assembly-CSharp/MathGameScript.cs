using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Token: 0x020000C2 RID: 194
public class MathGameScript : MonoBehaviour
{
    // Token: 0x06000982 RID: 2434 RVA: 0x000231E0 File Offset: 0x000215E0
    private void Start()
    {
        learnMusic.pitch = 0.9f;
        if (gc.spoopMode)
        {
            learnMusic.pitch -= 0.65f;
        }
        this.gc.ActivateLearningGame();
        circle.GetComponent<RectTransform>().anchoredPosition = new Vector2(9999, -120);
        cross.GetComponent<RectTransform>().anchoredPosition = new Vector2(9999, -120);
        baldiFeed.SetTrigger("idle");
        if (gc.spoopMode)
        {
            string[] meanStart = { "MAYBE YOU CAN FINALLY PROVE ME WRONG. TEST YOUR REFLEXES.", "I SHOULD STOP BOTHERING WITH THIS. YOUR REFLEXES ARE WITH NO HOPE.", "JUST TEST YOUR REFLEXES ALREADY." };
            questionText.text = meanStart[UnityEngine.Random.Range(0, meanStart.Length)];
        }
        StartCoroutine(ThiinkFast());
    }

    // Token: 0x06000983 RID: 2435 RVA: 0x00023270 File Offset: 0x00021670
    private void Update()
    {
        if (thinkFastChucklenuts > 0)
        {
            thinkFastChucklenuts -= Time.unscaledDeltaTime;
        }
        if (waitingForHit && thinkFastChucklenuts <= 0)
        {
            print("WrongyWrongWrong");
            CheckAnswer(false);
        }
        if (this.problem > 4)
        {
            this.endDelay -= 1f * Time.unscaledDeltaTime;
            if (endDelay <= 2 && !endTextDone)
            {
                EndQuestionText();
            }
            if (this.endDelay <= 0f)
            {
                GC.Collect();
                this.ExitGame();
            }
        }
    }

    IEnumerator ThiinkFast()
    {
        circle.GetComponent<RectTransform>().anchoredPosition = new Vector2(9999, -120);
        cross.GetComponent<RectTransform>().anchoredPosition = new Vector2(9999, -120);
        if (problem == 0)
        {
            yield return new WaitForSecondsRealtime(0.5f - (gc.notebooks / 14));
            if (gc.notebooks == 1)
            {
                yield return new WaitForSecondsRealtime(1);
            }
        }
        if (problem < 4)
        {
            baldiAudio.pitch = UnityEngine.Random.Range(1f, 2f);
            baldiAudio.Play();
            yield return new WaitForSecondsRealtime(baldiAudio.clip.length / baldiAudio.pitch);
        }
        NewProblem();
    }

    // Token: 0x06000984 RID: 2436 RVA: 0x00023350 File Offset: 0x00021750
    private void NewProblem()
    {
        if (questionText.text.ToLower().Contains("reflexes"))
        {
            questionText.text = string.Empty;
        }
        side = UnityEngine.Random.Range(0, 2);
        this.problem++;
        if (this.problem <= 4)
        {
            baldiAudio.PlayOneShot(fast);
            waitingForHit = true;
            thinkFastChucklenuts = 2 / ((gc.notebooks + 3) / 4);
            if (thinkFastChucklenuts < 0.95f)
            {
                thinkFastChucklenuts = 0.95f;
            }
            if ((this.gc.mode == "story" & (this.problem <= 3 || this.gc.notebooks <= 1)) || (this.gc.mode == "endless" & (this.problem <= 3 || this.gc.notebooks != 2)))
            {
                if (this.side == 0)
                {
                    circle.GetComponent<RectTransform>().anchoredPosition = new Vector2(70, -120);
                    cross.GetComponent<RectTransform>().anchoredPosition = new Vector2(180, -120);
                    baldiFeed.SetTrigger("Left");
                }
                else if (this.side == 1)
                {
                    cross.GetComponent<RectTransform>().anchoredPosition = new Vector2(70, -120);
                    circle.GetComponent<RectTransform>().anchoredPosition = new Vector2(180, -120);
                    baldiFeed.SetTrigger("Right");
                }
            }
            else
            {
                cross.GetComponent<RectTransform>().anchoredPosition = new Vector2(70, -120);
                cross2 = Instantiate(cross, cross.transform.parent);
                cross2.GetComponent<RectTransform>().anchoredPosition = new Vector2(180, -120);
                baldiFeed.SetTrigger("uhm");
                // What the foack!
            }
        }
        else
        {
            this.endDelay = 3f;
        }
    }

    void EndQuestionText()
    {
        endTextDone = true;
        questionText.lineSpacing = 0;
        if (!this.gc.spoopMode)
        {
            this.questionText.text = "I knew you could do it! Come to me, I have a reward for you.";
        }
        else if (this.gc.mode == "endless" & this.problemsWrong <= 0)
        {
            int num = Mathf.RoundToInt(UnityEngine.Random.Range(0f, 1f));
            this.questionText.text = this.endlessHintText[num];
        }
        else if (this.gc.mode == "story" & this.problemsWrong >= 4)
        {
            this.questionText.text = "WE'RE GONNA HAVE A LOT MORE TESTS LATER, JUST YOU WAIT";
            if (this.baldiScript.isActiveAndEnabled) this.baldiScript.Hear(this.playerPosition, 7f);
            this.gc.failedNotebooks++;
        }
        else
        {
            int num2 = Mathf.RoundToInt(UnityEngine.Random.Range(0f, 1f));
            this.questionText.text = this.hintText[num2];
        }
    }

    // Token: 0x06000986 RID: 2438 RVA: 0x00023BC0 File Offset: 0x00021FC0
    public void CheckAnswer(bool correct)
    {
        learnMusic.pitch += 0.05f;
        thinkFastChucklenuts = 30;
        waitingForHit = false;
        baldiFeed.SetTrigger("idle");
        circle.GetComponent<RectTransform>().anchoredPosition = new Vector2(9999, -120);
        cross.GetComponent<RectTransform>().anchoredPosition = new Vector2(9999, -120);
        if (cross2 != null)
        {
            cross2.GetComponent<RectTransform>().anchoredPosition = new Vector2(9999, -120);
        }        
        questionText.lineSpacing = -12;
        if (correct)
        {
            GetComponent<AudioSource>().PlayOneShot(right);
            if (gc.spoopMode)
            {
                questionText.text += "\nCORRECT.";
            }
            else
            {
                questionText.text += "\nThat's correct!";
            }
        }
        else
        {
            if (learnMusic.isPlaying)
            {
                learnMusic.Stop();
            }
            baldiFeed.gameObject.GetComponent<Image>().color = Color.red;
            GetComponent<AudioSource>().PlayOneShot(wrong);
            questionText.text += "\nWRONG.";
        }
        if (this.problem <= 4)
        {
            if (correct & !this.impossibleMode)
            {
                this.results[this.problem - 1].sprite = this.correct;
                this.baldiAudio.Stop();
                StartCoroutine(ThiinkFast());
            }
            else
            {
                this.problemsWrong++;
                this.results[this.problem - 1].sprite = this.incorrect;
                if (!this.gc.spoopMode)
                {
                    this.baldiFeed.SetTrigger("angry");
                    this.gc.ActivateSpoopMode();
                }
                if (this.gc.mode != "endless")
                {
                    if (this.problem == 4)
                    {
                        this.baldiScript.GetAngry(1f);
                    }
                    else
                    {
                        this.baldiScript.GetTempAngry(0.1f);
                    }
                }
                else
                {
                    this.baldiScript.GetAngry(0.8f);
                }
                StartCoroutine(ThiinkFast());
            }
        }
    }

    // Token: 0x0600098B RID: 2443 RVA: 0x00023E28 File Offset: 0x00022228
    private void ExitGame()
    {
        if (this.problemsWrong <= 0 & this.gc.mode == "endless")
        {
            this.baldiScript.GetAngry(-1f);
            if (baldiScript.isActiveAndEnabled)
            {
                baldiScript.Stun(0.35f * gc.notebooks, 0.14f * (gc.notebooks * 4));
            }
            else
            {
                gc.queuedStuns += 0.35f * gc.notebooks;
            }
        }
        this.gc.DeactivateLearningGame(base.gameObject);
    }

    public void Boink()
    {
        GetComponent<AudioSource>().PlayOneShot(boink);
    }

    // Token: 0x04000641 RID: 1601
    public GameControllerScript gc;

    // Token: 0x04000642 RID: 1602
    public BaldiScript baldiScript;

    // Token: 0x04000643 RID: 1603
    public Vector3 playerPosition;

    // Token: 0x04000644 RID: 1604
    public GameObject mathGame;

    // Token: 0x04000645 RID: 1605
    public Image[] results = new Image[3];

    // Token: 0x04000646 RID: 1606
    public Sprite correct;

    // Token: 0x04000647 RID: 1607
    public Sprite incorrect;

    public GameObject circle, cross, cross2;

    // Token: 0x04000649 RID: 1609
    public TMP_Text questionText;

    // Token: 0x0400064C RID: 1612
    public Animator baldiFeed;

    // Token: 0x0400064D RID: 1613
    public Transform baldiFeedTransform;

    // Token: 0x0400064E RID: 1614
    public AudioClip think, fast, boink;

    // Token: 0x04000657 RID: 1623
    public AudioClip[] bal_praises = new AudioClip[5];

    public AudioClip right, wrong;

    // Token: 0x04000659 RID: 1625
    public Button firstButton;

    // Token: 0x0400065A RID: 1626
    private float endDelay, thinkFastChucklenuts;

    // Token: 0x0400065B RID: 1627
    private int problem;

    // Token: 0x04000660 RID: 1632
    private int side;

    // Token: 0x04000662 RID: 1634
    private string[] hintText = new string[]
    {
        "I GET ANGRIER FOR EVERY TEST YOU FAIL",
        "I HEAR EVERY DOOR YOU OPEN"
    };

    // Token: 0x04000663 RID: 1635
    private string[] endlessHintText = new string[]
    {
        "That's more like it...",
        "Keep up the good work or see me after testing..."
    };

    // Token: 0x04000665 RID: 1637
    private bool impossibleMode, waitingForHit, endTextDone;

    // Token: 0x04000667 RID: 1639
    private int problemsWrong;

    // Token: 0x04000669 RID: 1641
    public AudioSource baldiAudio, learnMusic;
}
