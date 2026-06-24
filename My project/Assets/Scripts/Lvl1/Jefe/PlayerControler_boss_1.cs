using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController_boss_1 : MonoBehaviour
{
    public float velocidad_2 = 5f;

    void Update()
    {
        float movimientoX = 0;
        float movimientoY = 0;

        // ARRIBA
        if (Keyboard.current.wKey.isPressed)
        {
            movimientoY = 1;
        }

        // ABAJO
        if (Keyboard.current.sKey.isPressed)
        {
            movimientoY = -1;
        }

        // IZQUIERDA
        if (Keyboard.current.aKey.isPressed)
        {
            movimientoX = -1;
        }

        // DERECHA
        if (Keyboard.current.dKey.isPressed)
        {
            movimientoX = 1;
        }

        transform.Translate(
            movimientoX * velocidad_2 * Time.deltaTime,
            movimientoY * velocidad_2 * Time.deltaTime,
            0
        );

        // TELETRANSPORTE
        if (transform.position.x >= 80.69f)
        {
            transform.position = new Vector3(
                -7.59f,
                transform.position.y,
                transform.position.z
            );
        }
    }
}