using UnityEngine;


namespace Just_Shape_And_Beats
{
    public class PlayerHP : MonoBehaviour
    {
        public int maxHp = 4;
        private int currentHP;

        public float knockbackPower = 10f;
        public float invincibilityDuration = 1.5f;

        public void Start()
        {
            currentHP = maxHp;

        }


        public void TakeDamage()
        {
           
        }


    }

}
