using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Linq;

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

    public Button HpUp, Recovery, DamageUp, SpeedUp; // ������ ������ ��ư(�÷��̾�)
    public Button TowerAttackSpeed, TowerDamage, TowerGiSpeed, TowerHpRecovery; // ������ ������ ��ư(Ÿ��)
    public GameObject levelUpPanel; // ������ �������� ���� �г�

    Vector2 minBounds = new Vector2(-57, -32); //���� ũ��
    Vector2 maxBounds = new Vector2(57, 32);
    Vector2 startPos = new Vector2(0, -2); //���� ��ġ ����

    private bool isLevelingUp = false;

    void Start()
    {

        rigid2D = GetComponent<Rigidbody2D>(); //rigidbody ������Ʈ ��������
        transform.position = startPos; //������������ ����
        currHp = maxHp; // �ִ� ü�¸�ŭ ���� ü�� ����
        currExp = minExp; // �ּ�ġ Exp�� ����

        UpdateExpBar(); // ����ġ�� �ʱ�ȭ

        HideLevelUpPanel(); // �ʱ⿡�� ������ �г� �����
    }

    void Update()
    {
        if (isLevelingUp) return; // ������ �߿��� ���� ����

        if (Time.timeScale > 0) // ������ �Ͻ����� ���°� �ƴ� ���� Ÿ�̸� �۵�
        {
            elapsedTime += Time.deltaTime;

            if (elapsedTime >= stageTimeLimit)
            {
                StageClear();
            }
        }

        Vector2 pos = transform.position; //�÷��̾��� ��ġ
        pos.x = Mathf.Clamp(pos.x, minBounds.x, maxBounds.x); //�÷��̾��� ��ġ�� �� ũ�⿡ ���� ����
        pos.y = Mathf.Clamp(pos.y, minBounds.y, maxBounds.y);
        transform.position = pos; //���ѵ� ��ġ�� ��ȯ

        float x = Input.GetAxisRaw("Horizontal"); //ad�� ����ؼ� �̵�(input �Ŵ��� ���� �ʿ�)
        float y = Input.GetAxisRaw("Vertical"); // ws�� �̿��ؼ� �̵�

        Vector2 move = new Vector2(x, y);
        moveVelocity = move.normalized * speed; // �ӵ��� ���� ���Ͱ� ����ȭ

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); //���콺�� Ŭ���� ��ġ ���ϱ�
        dir = (mousePosition - pos).normalized; ////���콺�� Ŭ���Ǿ��� �� ��ġ�� �÷��̾��� ��ġ ���� ���� ���ϱ�

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

        // ���� �Ͻ�����
        Time.timeScale = 0;

        ShowLevelUpPanel();
    }

    private void ShowLevelUpPanel()
    {
        // 8���� �ɼ� �迭
        string[] options = new string[]
        {
        "Ÿ�� ����ü �ӵ� up", "�÷��̾� �̵� �ӵ� up", "�÷��̾� �ִ� ü�� up",
        "Ÿ�� ����ü �߻� �ӵ� up", "�÷��̾� HP ȸ��", "HP ���� Ÿ�� ȸ��",
        "�÷��̾� ���ݷ� up", "Ÿ�� ���ݷ� up", "Ÿ�� ��ӵ� up", "Ÿ�� HP up"
        };

        int[] randomIndexes = GetRandomIndexes(options.Length, 8); // ��ư 8�� ���� ����

        HpUp.GetComponentInChildren<TextMeshProUGUI>().text = options[randomIndexes[0]];
        Recovery.GetComponentInChildren<TextMeshProUGUI>().text = options[randomIndexes[1]];
        DamageUp.GetComponentInChildren<TextMeshProUGUI>().text = options[randomIndexes[2]];
        SpeedUp.GetComponentInChildren<TextMeshProUGUI>().text = options[randomIndexes[3]];
        TowerAttackSpeed.GetComponentInChildren<TextMeshProUGUI>().text = options[randomIndexes[4]];
        TowerDamage.GetComponentInChildren<TextMeshProUGUI>().text = options[randomIndexes[5]];
        TowerGiSpeed.GetComponentInChildren<TextMeshProUGUI>().text = options[randomIndexes[6]];
        TowerHpRecovery.GetComponentInChildren<TextMeshProUGUI>().text = options[randomIndexes[7]];

        
        HpUp.onClick.AddListener(() => ApplyLevelUpEffect(randomIndexes[0]));
        Recovery.onClick.AddListener(() => ApplyLevelUpEffect(randomIndexes[1]));
        DamageUp.onClick.AddListener(() => ApplyLevelUpEffect(randomIndexes[2]));
        SpeedUp.onClick.AddListener(() => ApplyLevelUpEffect(randomIndexes[3]));
        TowerAttackSpeed.onClick.AddListener(() => ApplyLevelUpEffect(randomIndexes[4]));
        TowerDamage.onClick.AddListener(() => ApplyLevelUpEffect(randomIndexes[5]));
        TowerGiSpeed.onClick.AddListener(() => ApplyLevelUpEffect(randomIndexes[6]));
        TowerHpRecovery.onClick.AddListener(() => ApplyLevelUpEffect(randomIndexes[7]));
        

        PositionButtonsRandomly(); // ��ư ��ġ �������� ��ġ
        levelUpPanel.SetActive(true);
    }

    private void HideLevelUpPanel()
    {
        // ������ �гο��� ��ư �̺�Ʈ ������ ����
        HpUp.onClick.RemoveAllListeners();
        Recovery.onClick.RemoveAllListeners();
        DamageUp.onClick.RemoveAllListeners();

        // ������ �г� �����
        levelUpPanel.SetActive(false);

        // ���� �簳
        Time.timeScale = 1;
        isLevelingUp = false; // ������ ���� �ƴ�
    }

    private void ApplyLevelUpEffect(int optionIndex)
    {
        switch (optionIndex)
        {
            case 0: // Ÿ�� ����ü �ӵ� up
                Debug.Log("Ÿ�� ����ü �ӵ� ����!");
                break;
            case 1: // �÷��̾� �̵� �ӵ� up
                //speed += 2.0f;
                Debug.Log("�÷��̾� �̵� �ӵ� ����!");
                break;
            case 2: // �÷��̾� �ִ� ü�� up
                //maxHp += 5.0f;
                Debug.Log("�÷��̾� �ִ� ü�� ����!");
                break;
            case 3: // Ÿ�� ����ü �߻� �ӵ� up
                ShootRate -= 0.1f;
                Debug.Log("Ÿ�� ����ü �߻� �ӵ� ����!");
                break;
            case 4: // �÷��̾� HP ȸ��
                //currHp = maxHp;
                Debug.Log("�÷��̾� HP ȸ��!");
                break;
            case 5: // HP ���� Ÿ�� ȸ��
                Debug.Log("HP ���� Ÿ�� ȸ��!");
                break;
            case 6: // �÷��̾� ���ݷ� up
                Debug.Log("�÷��̾� ���ݷ� ����!");
                break;
            case 7: // Ÿ�� ���ݷ� up
                Debug.Log("Ÿ�� ���ݷ� ����!");
                break;
        }

        // ������ �г� ����� ���� �簳
        HideLevelUpPanel();
    }
    
    private int[] GetRandomIndexes(int range, int count)
    {
        System.Random rand = new System.Random();
        HashSet<int> indexes = new HashSet<int>();
        while (indexes.Count < count)
        {
            indexes.Add(rand.Next(range));
        }
        return new List<int>(indexes).ToArray();
    }

    private void PositionButtonsRandomly()
    {
        // ī�޶��� �߽� ��ġ�� �����ɴϴ�.
        Vector3 cameraCenter = Camera.main.transform.position;

        // ��ư�� ��ġ�� ��ġ��
        Vector3[] positions = new Vector3[]
        {
            new Vector3(cameraCenter.x - 15, cameraCenter.y, 5),
            new Vector3(cameraCenter.x, cameraCenter.y, 5),
            new Vector3(cameraCenter.x + 15, cameraCenter.y, 5)
        };

        // ��ư �迭
        Button[] buttons = new Button[]
        {
            HpUp,
            Recovery,
            DamageUp,
            SpeedUp,
            TowerAttackSpeed,
            TowerDamage,
            TowerGiSpeed,
            TowerHpRecovery
        };

        // ��ư �迭 ũ�⿡ ���缭 �������� 3���� ����
        System.Random rand = new System.Random();
        List<int> selectedIndexes = new List<int>();
        while (selectedIndexes.Count < 3)
        {
            int randomIndex = rand.Next(buttons.Length);
            if (!selectedIndexes.Contains(randomIndex)) // �ߺ� ����
            {
                selectedIndexes.Add(randomIndex);
            }
        }

        // ���õ� ��ư �ؽ�Ʈ ������Ʈ
        string[] options = new string[]
        {
        "Ÿ�� ����ü �ӵ� up", "�÷��̾� �̵� �ӵ� up", "�÷��̾� �ִ� ü�� up",
        "Ÿ�� ����ü �߻� �ӵ� up", "�÷��̾� HP ȸ��", "HP ���� Ÿ�� ȸ��",
        "�÷��̾� ���ݷ� up", "Ÿ�� ���ݷ� up"
        };

        // �� ��ư�� �ش��ϴ� �ؽ�Ʈ ����
        for (int i = 0; i < selectedIndexes.Count; i++)
        {
            buttons[selectedIndexes[i]].GetComponentInChildren<TextMeshProUGUI>().text = options[i];
        }

        // ��ġ �迭�� �����ϰ� ���� ���� List�� ��ȯ
        List<Vector3> positionList = new List<Vector3>(positions);
        positionList = positionList.OrderBy(x => rand.Next()).ToList(); // ���� ����

        // ���õ� ��ư�鿡 ���� ��ġ �Ҵ�
        for (int i = 0; i < selectedIndexes.Count; i++)
        {
            buttons[selectedIndexes[i]].transform.position = positionList[i];
        }
    }

    void FixedUpdate() //Update�Լ��� �������� �������� �ʱ� ������ rigidbody�� �ٷ�� �ڵ带 �����ϴ� �Լ�
    {
        if (!isLevelingUp)
        {
            rigid2D.MovePosition(rigid2D.position + moveVelocity * Time.fixedDeltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("eat")) //eat �±׸� ���� ������Ʈ�� �ε�����
        {
            if (currHp > 0)
            { //���� ü���� �����ִٸ�
                currHp -= 1.0f; //ü�� 1�A��
                hpfront.localScale = new Vector3(currHp / maxHp, 1.0f, 1.0f); // ���� ü���� �ִ� ü������ ����� hp����

                PoolManager.instance.ReturnPreFab(collision.gameObject); //�浹�� ���� ��Ȱ��ȭ(Ǯ��)

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

                PoolManager.instance.ReturnPreFab(collision.gameObject);//�浹�� ���� ��Ȱ��ȭ(Ǯ��)

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

                PoolManager.instance.ReturnPreFab(collision.gameObject); //�浹�� ���� ��Ȱ��ȭ(Ǯ��)
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

                PoolManager.instance.ReturnPreFab(collision.gameObject); //�浹�� ���� ��Ȱ��ȭ(Ǯ��)

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

                PoolManager.instance.ReturnPreFab(collision.gameObject); //�浹�� ���� ��Ȱ��ȭ(Ǯ��)

            }

            else
            {
                GameOver();
            }
        }
        if (collision.gameObject.CompareTag("exp")) {
            GainExperience(10); // ����ġ ȹ��
            PoolManager.instance.ReturnPreFab(collision.gameObject); //����ġ ��ȯ
        }
    }

    public void GameOver()
    {
        SceneManager.LoadScene("GameOver");
    }

    void Shoot() { // �⸦ ��� �Լ�
        GameObject gi = PoolManager.instance.GetPreFab(giPrefab); //Ǯ���� �⸦ ��������
        gi.transform.position = transform.position; //���� ��ġ�� �÷��̾��� ��ġ�� �̵�
        gi.GetComponent<Rigidbody2D>().velocity = dir * giSpeed; //������ ���� �ӵ���ŭ ���콺 ��ġ�� �߻�
    }

}


