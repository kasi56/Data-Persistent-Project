using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;
using UnityEngine.SceneManagement;

public class ManPage : MonoBehaviour
{
    public static ManPage Instance;
    public TMP_InputField InputName;
    public string PlayerName;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }


    public void StartGame()
    {
        if (InputName.text.Length != 0)
        {
            PlayerName = InputName.text;
            SceneManager.LoadScene("main");
        }
        else
        {
            Debug.Log("Please input name");
        }
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
}
