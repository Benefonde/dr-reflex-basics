using UnityEngine;
using UnityEngine.SceneManagement;
public class BenefondCratesSoCoolBro : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (Random.Range(1, 21) == 2)
        {
            benefondCreatesAud.clip = benefondCratesEasterEggs[Random.Range(0, benefondCratesEasterEggs.Length)];
        }
        benefondCreatesAud.Play();
        Invoke(nameof(SendToGame), 3.333f);
    }
    void SendToGame()
    {
        SceneManager.LoadScene("Warning");
    }
    public AudioSource benefondCreatesAud;
    public AudioClip[] benefondCratesEasterEggs;
}
