using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GestionAudio : MonoBehaviour
{
    // Audio Mixer
    [SerializeField] private AudioMixer audioMixer;

    // Son des effets sonores (pour vérifier si ceux-ci sont assez forts)
    [SerializeField] private AudioClip sonFxTest;

    // Sliders de la musique et des effets sonores
    [SerializeField] private Slider SliderMusique;
    [SerializeField] private Slider SliderEffets;


    // Audio source
    private AudioSource audioSource;


    // Est appelée au lancement de la scène
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // Vérifie si on se trouve dans la scène Accueil
        if (SceneManager.GetActiveScene().name == "Accueil")
        {
            // Applique les valeurs initiales des sliders au AudioMixer
            audioMixer.SetFloat("VolumeMusique", SliderMusique.value);
            audioMixer.SetFloat("VolumeEffets", SliderEffets.value);
        }

        // Sinon
        else
        {
            // Récupère les volumes actuels du AudioMixer
            audioMixer.GetFloat("VolumeMusique", out float volumeMusique);
            audioMixer.GetFloat("VolumeEffets", out float volumeEffets);

            // Met à jour les sliders
            SliderMusique.value = volumeMusique;
            SliderEffets.value = volumeEffets;
        }
    }

    // Fait en sorte que le slider de la musique contrôle le niveau audio de celle-ci
    public void AjusterVolumeMusique(float volume)
    {
        audioMixer.SetFloat("VolumeMusique", volume);
    }


    // Fait en sorte que le slider des effets sonores contrôle le niveau audio de ceux-ci
    public void AjusterVolumeEffets(float volume)
    {
        audioMixer.SetFloat("VolumeEffets", volume);
    }


    // Fait en sorte de jouer un effet sonore de Test
    public void TestVolumeEffets()
    {
        audioSource.PlayOneShot(sonFxTest);
    }
}