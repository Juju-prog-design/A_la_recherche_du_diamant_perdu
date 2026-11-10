using UnityEngine;
using UnityEngine.EventSystems;


public class CurseurBTN : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Texture du curseur
    [SerializeField] private Texture2D curseurMain;


    // Est appelée au lancement de la scène
    void Start()
    {
        // Verrouille le curseur dans la fenêtre de jeu
        Cursor.lockState = CursorLockMode.Confined;
    }


    // Quand la souris entre sur le BTN, change le curseur
    public void OnPointerEnter(PointerEventData eventData)
    {
        Cursor.SetCursor(curseurMain, Vector2.zero, CursorMode.Auto);
    }


    // Quand la souris quitte le BTN, remet le curseur par défaut
    public void OnPointerExit(PointerEventData eventData)
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }


    // Réinitialise le curseur si le BTN est désactivé
    private void OnDisable()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}