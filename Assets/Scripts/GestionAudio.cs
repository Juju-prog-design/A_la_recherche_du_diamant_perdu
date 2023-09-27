using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GestionAudio : MonoBehaviour
{
    //Audio Mixer
    [SerializeField] private AudioMixer audioMixer;

    //Son des effets sonores (pour v�rifier si ceux-ci sont assez forts)
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

        //Si on est dans la sc�ne "Accueil"
        if (SceneManager.GetActiveScene().name == "Accueil")
        {
            //Fait en sorte que le slider de la musique soit � un niveau acceptable (pas trop fort)
            audioMixer.SetFloat("VolumeMusique", SliderMusique.value);
        }

        //Sinon
        else
        {
            //Met la valeur du slider � celle par d�faut
            float volume;
            audioMixer.GetFloat("VolumeMusique", out volume);
            SliderMusique.value = volume;
        }
    }

    //Fait en sorte que le slider de la musique contr�le le niveau audio de celle-ci
    public void AjusterVolumeMusique(float volume)
    {
        audioMixer.SetFloat("VolumeMusique", volume);
    }


    //Fait en sorte que le slider des effets sonores contr�le le niveau audio de ceux-ci
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