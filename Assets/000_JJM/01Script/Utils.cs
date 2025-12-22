using UnityEngine;

namespace _JJM.Script
{
    public static class Utils
    {
        public static Quaternion LookTarget(Transform self, Transform target)
        {
            Vector2 dir = target.position - self.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            Quaternion ro = self.rotation;
            return Quaternion.Euler(ro.x, ro.y, angle);
        }

        public static int GetObjectDirectionX(Vector2 self, Vector2 target)
        {
            float selfX = self.x;
            float tarX = target.x;

            float value = tarX - selfX;
            value = Mathf.Abs(value) / value;

            return (int)value;
        }

        public static int GetObjectDirectionY(Vector2 self, Vector2 target)
        {
            float selfY = self.y;
            float tarY = target.y;

            float value = tarY - selfY;
            value = Mathf.Abs(value) / value;

            return (int)value;
        }








        
        public static float LeeHunSung()
        {
            return 1286.42f;
        }
    }
}
