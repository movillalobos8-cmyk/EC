using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float velocidad = 5f;
    public float velocidadAdelante = 3f;

    void Update()
    {
        float movimiento = 0;

        if (Keyboard.current.wKey.isPressed)
        {
            movimiento = 1;
        }

        if (Keyboard.current.sKey.isPressed)
        {
            movimiento = -1;
        }

        transform.Translate(
            velocidadAdelante * Time.deltaTime,
            movimiento * velocidad * Time.deltaTime,
            0
        );
    }
}