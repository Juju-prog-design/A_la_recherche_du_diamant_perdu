using UnityEngine;
using UnityEngine.InputSystem;


public class StarterAssetsInputs : MonoBehaviour
{
    [Header("Valeurs des déplacements du perso")]
    public Vector2 bouge;
    public Vector2 regarder;
    public bool saute;
    public bool cours;
    public bool tire;

    //Bouger
    public void OnBouge(InputValue value)
    {
        MoveInput(value.Get<Vector2>());
    }


    //Regarder
    public void OnRegarder(InputValue value)
    {
        LookInput(value.Get<Vector2>());
    }


    //Sauter
    public void OnSaute(InputValue value)
    {
        saute = value.isPressed;
    }


    //Courir
    public void OnCours(InputValue value)
    {
        cours = value.isPressed;
    }


    //Valeurs quand le perso bouge
    public void MoveInput(Vector2 newMoveDirection)
    {
        bouge = newMoveDirection;
    }


    //Valeurs quand le perso regarde
    public void LookInput(Vector2 newLookDirection)
    {
        regarder = newLookDirection;
    }


    //Valeurs quand le perso saute
    public void JumpInput(bool newJumpState)
    {
        saute = newJumpState;
    }


    //Valeurs quand le perso cours
    public void SprintInput(bool newSprintState)
    {
        cours = newSprintState;
    }


    //Valeurs quand le perso tire avec son arme
    public void OnTire(InputValue value)
    {
        tire = value.isPressed;
    }
}
