using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rigid2D; // ���� �̵��� ���� ����
    Vector2 moveVelocity; //����ȭ�� ���Ͱ��� ��� ���� ����
    Vector3 dir; //���콺 ���⺤�͸� ������ ����

    public float speed = 10.0f; // �÷��̾��� ���ǵ带 �����ϴ� ����
    private float currHp; //�÷��̾��� ���� hp
    public float maxHp = 10f; //�÷��̾��� �ִ� ü��
    public float giSpeed = 30.0f; //���� �ӵ�
    public float ShootRate = 0.5f; //���� �⸦ ������ �ɸ��� ������ �ð�
    private float nextShootTime = 0f; //�ð� ���

    public GameObject giPrefab; //�� ��
    public GameObject hpbar; // ü�¹ٸ� ���̰ų� ������ �ʰ� �ϱ� ����
    public RectTransform hpfront; // ü�¹��� �������� ������ ����ϱ� ����

    private float currExp; //�÷��̾��� ���� Exp
    public float maxExp = 200f; // Exp �ִ�ġ
    public float minExp = 1f; // Exp �ּ�ġ
    public GameObject expbar; // ����ġ�ٸ� ���̰ų� ������ �ʰ� �ϱ� ����
    public RectTransform expfront; // ����ġ���� �������� ������ ����ϱ� ����

    public float stageTimeLimit = 180f; // �������� ���� �ð� (�� ����)
    private float elapsedTime = 0f;    // ��� �ð�

    public PoolManager pool;

    Vector2 minBounds = new Vector2(-57, -32); //���� ũ��
    Vector2 maxBounds = new Vector2(57, 32);
    Vector2 startPos = new Vector2(0, -2); //���� ��ġ ����

    void Start()
    {
        rigid2D = GetComponent<Rigidbody2D>(); //rigidbody ������Ʈ ��������
        transform.position = startPos; //������������ ����
        currHp = maxHp; // �ִ� ü�¸�ŭ ���� ü�� ����
        currExp = minExp; // �ּ�ġ Exp�� ����

        UpdateExpBar(); // ����ġ�� �ʱ�ȭ
    }

    void Update()
    {
        if (Time.timeScale > 0) // ������ �Ͻ����� ���°� �ƴ� ���� Ÿ�̸� �۵�
        {
            elapsedTime += Time.deltaTime;

            if (elapsedTime >= stageTimeLimit)
            {
                StageClear();
            }
        }

        Vector3 pos = transform.position; //�÷��̾��� ��ġ
        pos.x = Mathf.Clamp(pos.x, minBounds.x, maxBounds.x); //�÷��̾��� ��ġ�� �� ũ�⿡ ���� ����
        pos.y = Mathf.Clamp(pos.y, minBounds.y, maxBounds.y);
        transform.position = pos; //���ѵ� ��ġ�� ��ȯ

        float x = Input.GetAxisRaw("Horizontal"); //ad�� ����ؼ� �̵�(input �Ŵ��� ���� �ʿ�)
        float y = Input.GetAxisRaw("Vertical"); // ws�� �̿��ؼ� �̵�

        Vector2 move = new Vector2(x, y);
        moveVelocity = move.normalized * speed; // �ӵ��� ���� ���Ͱ� ����ȭ

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); //���콺�� Ŭ���� ��ġ ���ϱ�
        dir = (mousePosition - transform.position).normalized; ////���콺�� Ŭ���Ǿ��� �� ��ġ�� �÷��̾��� ��ġ ���� ���� ���ϱ�

        if (Input.GetMouseButtonDown(0) && Time.time > nextShootTime) // ���콺�� ��Ŭ�� ������, ������ �ð��� ������
        {
            nextShootTime = Time.time + ShootRate; //������ �ð� ������
            Shoot(); //���
        }
    }

    private void StageClear()
    {
        string currentSceneName = SceneManager.GetActiveScene().name; // ���� �� �̸� ��������
        string nextSceneName = GetNextSceneName(currentSceneName);    // ���� �� �̸� ���


        Debug.Log($"Stage Clear! Moving to next scene: {nextSceneName}");
        SceneManager.LoadScene(nextSceneName);
        
    }

    private string GetNextSceneName(string currentSceneName)
    {
        // ���Խ��� ����� ���ڸ� ����
        System.Text.RegularExpressions.Match match =
            System.Text.RegularExpressions.Regex.Match(currentSceneName, @"(\d+)$");

        if (match.Success)
        {
            int currentSceneNumber = int.Parse(match.Value); // ���� �� ��ȣ ����
            string nextSceneName;

            // ClearNextScene �� GameScene ��ȯ
            if (currentSceneName.StartsWith("ClearNextScene"))
            {
                nextSceneName = $"GameScene{currentSceneNumber}"; // GameSceneX�� �̵�
            }
            // GameScene �� ClearNextScene ��ȯ
            else if (currentSceneName.StartsWith("GameScene"))
            {
                nextSceneName = $"ClearNextScene{currentSceneNumber + 1}"; // ClearNextSceneX�� �̵�
            }
            else
            {
                nextSceneName = null; // ����ġ ���� �� �̸�
            }

            return nextSceneName;
        }

        return null; // ���ڰ� ���Ե��� ���� ���
    }

    private void UpdateExpBar()
    {
        if (expfront != null)
        {
            float expRatio = currExp / maxExp; // ���� ����ġ ���� ���
            expfront.localScale = new Vector3(expRatio, 1.0f, 1.0f); // ����ġ�� ũ�� ����
        }
    }

    public void GainExperience(float exp)
    {

        currExp += exp; // ����ġ ����
        UpdateExpBar(); // ����ġ�� ������Ʈ

        // ����ġ�� �ִ�ġ�� �����ϸ� ������ ó��
        if (currExp >= maxExp)
        {
            currExp -= maxExp; // �ʰ��� ����ġ ����
            LevelUp();         // ������ �޼��� ȣ��
        }

        // ����ġ UI ������Ʈ (�ʿ��ϴٸ� ����)
        Debug.Log($"Current Experience: {currExp}");
    }

    private void LevelUp()
    {
        // ������ ����
        Debug.Log("Level Up!");
    }

    void FixedUpdate() //Update�Լ��� �������� �������� �ʱ� ������ rigidbody�� �ٷ�� �ڵ带 �����ϴ� �Լ�
    {

        rigid2D.MovePosition(rigid2D.position + moveVelocity * Time.fixedDeltaTime); //�÷��̾ �����̰� �ϴ� �ڵ�, �÷��̾��� �����ǿ� ���Ͱ��� ������ ������ ������ ���� �÷��̾ ������

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("eat")) //eat �±׸� ���� ������Ʈ�� �ε�����
        {
            if (currHp > 0)
            { //���� ü���� �����ִٸ�
                currHp -= 1.0f; //ü�� 1�A��
                hpfront.localScale = new Vector3(currHp / maxHp, 1.0f, 1.0f); // ���� ü���� �ִ� ü������ ����� hp����

                Destroy(collision.gameObject); //�ε��� ���� ���� 

                //Ǯ���� ����Ѵٸ�....
                //PoolManager.instance.ReturnMonster(collision.gameObject); �浹�� ���� ��Ȱ��ȭ(Ǯ��)

            }

            else
            {
                GameOver();
            }
        }
        if (collision.gameObject.CompareTag("gohome")) //gohome �±׸� ���� ������Ʈ�� �ε�����
        {
            if (currHp > 0)
            { //���� ü���� �����ִٸ�
                currHp -= 1.0f; //ü�� 1�A��
                hpfront.localScale = new Vector3(currHp / maxHp, 1.0f, 1.0f); // ���� ü���� �ִ� ü������ ����� hp����

                Destroy(collision.gameObject); //�ε��� ���� ���� 

                //Ǯ���� ����Ѵٸ�....
                //PoolManager.instance.ReturnMonster(collision.gameObject); �浹�� ���� ��Ȱ��ȭ(Ǯ��)

            }

            else
            {
                GameOver();
            }
        }
        if (collision.gameObject.CompareTag("what")) //what �±׸� ���� ������Ʈ�� �ε�����
        {
            if (currHp > 0)
            { //���� ü���� �����ִٸ�
                currHp -= 2.0f; //ü�� 2�A��
                hpfront.localScale = new Vector3(currHp / maxHp, 1.0f, 1.0f); // ���� ü���� �ִ� ü������ ����� hp����

                Destroy(collision.gameObject); //�ε��� ���� ���� 

                //Ǯ���� ����Ѵٸ�....
                //PoolManager.instance.ReturnMonster(collision.gameObject); �浹�� ���� ��Ȱ��ȭ(Ǯ��)

            }

            else
            {
                GameOver();
            }
        }
        if (collision.gameObject.CompareTag("no")) //no �±׸� ���� ������Ʈ�� �ε�����
        {
            if (currHp > 0)
            { //���� ü���� �����ִٸ�
                currHp -= 3.0f; //���� ü�� �A��
                hpfront.localScale = new Vector3(currHp / maxHp, 1.0f, 1.0f); // ���� ü���� �ִ� ü������ ����� hp����

                Destroy(collision.gameObject); //�ε��� ���� ���� 

                //Ǯ���� ����Ѵٸ�....
                //PoolManager.instance.ReturnMonster(collision.gameObject); �浹�� ���� ��Ȱ��ȭ(Ǯ��)

            }

            else
            {
                GameOver();
            }
        }
        if (collision.gameObject.CompareTag("pressure")) //pressure �±׸� ���� ������Ʈ�� �ε�����
        {
            if (currHp > 0)
            { //���� ü���� �����ִٸ�
                currHp -= 5.0f; //���� ü�� �A��
                hpfront.localScale = new Vector3(currHp / maxHp, 1.0f, 1.0f); // ���� ü���� �ִ� ü������ ����� hp����
                Destroy(collision.gameObject); //�ε��� ���� ���� 

                //Ǯ���� ����Ѵٸ�....
                //PoolManager.instance.ReturnMonster(collision.gameObject); �浹�� ���� ��Ȱ��ȭ(Ǯ��)

            }

            else
            {
                GameOver();
            }
        }

    }

    public void GameOver()
    {
        SceneManager.LoadScene("GameOver");
    }

    void Shoot() { // �⸦ ��� �Լ�
        GameObject gi = Instantiate(giPrefab); //�÷��̾� ��ġ�� �� ����
        gi.transform.position = transform.position; //���� ��ġ�� �÷��̾��� ��ġ�� �̵�
        gi.GetComponent<Rigidbody2D>().velocity = dir * giSpeed; //������ ���� �ӵ���ŭ ���콺 ��ġ�� �߻�
    }

}


