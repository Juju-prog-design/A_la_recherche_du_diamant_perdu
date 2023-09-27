using UnityEngine;
using TMPro;


public class Resultats : MonoBehaviour
{
    //Infos du perso (SO)
    [SerializeField] private InfosPerso infosPerso;

    //Zones de texte qui auront les valeurs des infos du perso
    [SerializeField] private TMP_Text nomPerso;
    [SerializeField] private TMP_Text nbPts;
    [SerializeField] private TMP_Text nbEnnemis;
    [SerializeField] private TMP_Text temps;


    //Est appelée au lancement de la scène
    void Start()
    {
        //Met les valeurs du SO du perso dans les zones de texte appropriées      
        nomPerso.text = infosPerso.nomPerso;
        nbPts.text = infosPerso.nbPts.ToString();
        nbEnnemis.text = infosPerso.nbEnnemis.ToString();
        temps.text = infosPerso.temps.ToString();
    }
}