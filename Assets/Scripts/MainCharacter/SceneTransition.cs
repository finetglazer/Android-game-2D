using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public string sceneName; // Tên màn chơi cần chuyển đến


    // Hàm này được gọi khi một va chạm xảy ra với vật thể có Is Trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra nếu người chơi va chạm với vật thể này
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene(sceneName); // Chuyển đến màn chơi mới
        }
    }
}