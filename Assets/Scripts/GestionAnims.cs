using UnityEngine;


public class GestionAnims : MonoBehaviour
{
    //À un certain point dans les animations de saut (avec et sans arme), il y a un event (événement) où cette méthode est appellée
    public void MessageFin(float valeur)
    {
        //Envoi la valeur d'un float (1.2) à la méthode "FinAnim" du script "BougePerso" qui va être la valeur de la hauteur du saut du perso
        transform.parent.SendMessage("FinAnim", valeur);
    }
}
