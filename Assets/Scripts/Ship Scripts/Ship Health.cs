using UnityEngine;

public class ShipHealth : Health
{
    protected override void Die()
    {
        Time.timeScale = 0f;
        //Если дойдут руки, сделать полноценную остановку игру(с очками и перезапуском)
    }
}
