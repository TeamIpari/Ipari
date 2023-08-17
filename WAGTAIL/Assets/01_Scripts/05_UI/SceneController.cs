using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneController : MonoBehaviour
{
    public void SceneChange()
    {
        SceneManager.LoadScene("Title");
    }
}
