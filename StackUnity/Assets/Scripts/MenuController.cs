using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public Text scoreText;
 
	private void Start ()
    {
        scoreText.text = PlayerPrefs.GetInt("score").ToString();
	}

    public void InitGame(string name)
    {
        SceneManager.LoadScene(name);
    }
}
