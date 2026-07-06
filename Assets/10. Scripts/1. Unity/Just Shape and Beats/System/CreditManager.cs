using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Just_Shape_And_Beats
{
    public class CreditManager : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if(Keyboard.current.enterKey.wasPressedThisFrame)
            {
                SceneManager.LoadScene("00. Main");
            }
        }
    }
}

