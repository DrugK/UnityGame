using UnityEngine;

public class GameManager : MonoBehaviour
{
    public void ResumeGame()
    {
        // ������ ��� ������ �� �ְ� ����
        Time.timeScale = 1;  // ���� �ӵ� �簳
    }

    public void PauseGame()
    {
        // ������ ���� �� ���� �Ͻ� ����
        Time.timeScale = 0;  // ���� �Ͻ� ����
    }
}
