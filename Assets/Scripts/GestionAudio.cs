using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GestionAudio : MonoBehaviour
{
    //Audio Mixer
    [SerializeField] private AudioMixer audioMixer;

    //Son des effets sonores (pour vérifier si ceux-ci sont assez forts)
    [SerializeField] private AudioClip sonFxTest;

    //Sliders de la musique et des effets sonores
    [SerializeField] private Slider SliderMusique;
    [SerializeField] private Slider SliderEffets;


    //Audio source
    private AudioSource audioSource;


    void Start()
    {
        //Audio source ayant le son Test
        audioSource = GetComponent<AudioSource>();

        //Si on est dans la scène "Accueil"
        if (SceneManager.GetActiveScene().name == "Accueil")
        {
            //Fait en sorte que le slider de la musique soit à un niveau acceptable (pas trop fort)
            audioMixer.SetFloat("VolumeMusique", SliderMusique.value);
        }

        //Sinon
        else
        {
            //Met la valeur du slider à celle par défaut
            float volume;
            audioMixer.GetFloat("VolumeMusique", out volume);
            SliderMusique.value = volume;
        }
    }

    //Fait en sorte que le slider de la musique contrôle le niveau audio de celle-ci
    public void AjusterVolumeMusique(float volume)
    {
        audioMixer.SetFloat("VolumeMusique", volume);
    }


    //Fait en sorte que le slider des effets sonores contrôle le niveau audio de ceux-ci
    public void AjusterVolumeEffets(float volume)
    {
        audioMixer.SetFloat("VolumeEffets", volume);
    }


    //Fait en sorte de jouer un effet sonore de Test
    public void TestVolumeEffets()
    {
        audioSource.PlayOneShot(sonFxTest);
    }
}