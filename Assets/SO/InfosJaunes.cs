using UnityEngine;


//Permet de cr�er un SO dans le dossier "SO"
[CreateAssetMenu(fileName = "InfosJaunes", menuName = "SO/NouveauJaune")]


public class InfosJaunes : ScriptableObject
{
    //Nb de pts de la pi�ce
    public int nbPts;
}