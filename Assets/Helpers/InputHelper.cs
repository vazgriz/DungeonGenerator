using UnityEngine;

namespace Assets.Helpers
{
    public static class InputHelper
    {
        public static float VerticalAxis => Input.GetAxisRaw("Vertical");
        public static float HorizontalAxis => Input.GetAxisRaw("Horizontal");
        public static float MouseXAxis => Input.GetAxis("Mouse X");
        public static float MouseYAxis => Input.GetAxis("Mouse Y");
        public static Vector2 MouseXY => new Vector2(MouseXAxis, MouseYAxis);
        public static bool PrimaryFire => Input.GetButtonDown("Fire1");
    }
}