using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;


public class GestionAccueil : MonoBehaviour
{
    //Zone de texte du nom du perso
    [SerializeField] private TMP_Text nomJ;

    //Infos du perso (SO)
    [SerializeField] private InfosPerso infosPerso;


    //Changer le nom du perso quand on écris qq dans la zone de texte
    public void ChangeNom(string nom)
    {
        infosPerso.nomPerso = nom;
    }


    //Vérifie si le nom du perso a bien été écrit dans la zone de texte
    public void VerifieNom()
    {
        //S'il est écrit
        if (infosPerso.nomPerso != "")
        {
            //Charge la scène suivante
            SceneManager.LoadScene("Terrain");
        }
    }
}