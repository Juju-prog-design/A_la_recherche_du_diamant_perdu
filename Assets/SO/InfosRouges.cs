using UnityEngine;


//Permet de cr�er un SO dans le dossier "SO"
[CreateAssetMenu(fileName = "InfosRouges", menuName = "SO/NouveauRouge")]


public class InfosRouges : ScriptableObject
{
    //Nb de pts de la pi�ce
    public int nbPts;
}