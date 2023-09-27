using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;


public class GestionAccueil : MonoBehaviour
{
    //Zone de texte du nom du perso
    [SerializeField] private TMP_Text nomJ;

    //Infos du perso (SO)
    [SerializeField] private InfosPerso infosPerso;


    //Changer le nom du perso quand on �cris qq dans la zone de texte
    public void ChangeNom(string nom)
    {
        infosPerso.nomPerso = nom;
    }


    //V�rifie si le nom du perso a bien �t� �crit dans la zone de texte
    public void VerifieNom()
    {
        //S'il est �crit
        if (infosPerso.nomPerso != "")
        {
            //Charge la sc�ne suivante
            SceneManager.LoadScene("Terrain");
        }
    }
}