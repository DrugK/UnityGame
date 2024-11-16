using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressureController : MonoBehaviour
{
    float speed = 1f; // ������ �ӵ�
    public float maxHp = 30f; //������ �ִ� ü��
    private float currHp; //������ ���� hp

    GameObject player; //�÷��̾� ������Ʈ
    GameObject tower; //Ÿ�� ������Ʈ
    GameObject tower2; //Ÿ��2 ������Ʈ
    GameObject tower3; //Ÿ��3 ������Ʈ
    GameObject tower4; //Ÿ��4 ������Ʈ
    public GameObject hpbar; // ü�¹ٸ� ���̰ų� ������ �ʰ� �ϱ� ����
    public RectTransform hpfront; // ü�¹��� �������� ������ ����ϱ� ����

    void Start()
    {
        this.player = GameObject.Find("player"); //�÷��̾��� ã��
        this.tower = GameObject.Find("tower"); //Ÿ�� ã��
        this.tower2 = GameObject.Find("tower2"); //Ÿ��2 ã�� (�������� ����)
        this.tower3 = GameObject.Find("tower3"); //Ÿ��3 ã�� (�������� ����)
        this.tower4 = GameObject.Find("tower4"); //Ÿ��4 ã�� (�������� ����)

        currHp = maxHp; // ���� ���۽� �ִ�ü�¿� ���� ���� ü�� ����
        hpbar.SetActive(false); // ü�¹� �����
    }
    void Update()
    {
        if (currHp <= 0)
        { 
            Destroy(gameObject);
            PlayerController playerController = player.GetComponent<PlayerController>(); // �÷��̾��� ��ũ��Ʈ ����
            playerController.GainExperience(10f); // ����ġ 10 ���� (������ ������ ����)
            //ü���� 0�̵Ǹ� ���� ��Ȱ��ȭ(Ǯ�� ��� ����)
            //PoolManager.instance.ReturnMonster(gameObject);
        }
    }
    void FixedUpdate()
    {
        Vector3 monsterPosition = transform.position; // ������ ��ġ
        Vector3 playerPosition = player.GetComponent<Transform>().position; // �÷��̾��� ��ġ
        Vector3 towerPosition = tower.GetComponent<Transform>().position; // Ÿ���� ��ġ

        float playerToMonster = Vector3.Distance(monsterPosition, playerPosition); // ���Ϳ� �÷��̾�� �Ÿ�
        float towerToMonster = Vector3.Distance(monsterPosition, towerPosition); // ���Ϳ� Ÿ������ �Ÿ�

        // Ÿ��, Ÿ��2, Ÿ��3, Ÿ��4�� ���� ���
        if (tower2 != null && tower3 != null && tower4 != null)
        {
            Vector3 tower2Position = tower2.GetComponent<Transform>().position; // Ÿ��2�� ��ġ
            Vector3 tower3Position = tower3.GetComponent<Transform>().position; // Ÿ��3�� ��ġ
            Vector3 tower4Position = tower4.GetComponent<Transform>().position; // Ÿ��4�� ��ġ

            float tower2ToMonster = Vector3.Distance(monsterPosition, tower2Position); // ���Ϳ� Ÿ��2���� �Ÿ�
            float tower3ToMonster = Vector3.Distance(monsterPosition, tower3Position); // ���Ϳ� Ÿ��3���� �Ÿ�
            float tower4ToMonster = Vector3.Distance(monsterPosition, tower4Position); // ���Ϳ� Ÿ��4���� �Ÿ�

            // Ÿ����� �÷��̾��� �Ÿ� ��
            if (towerToMonster > playerToMonster && tower2ToMonster > playerToMonster && tower3ToMonster > playerToMonster && tower4ToMonster > playerToMonster)
            {
                transform.position = Vector3.MoveTowards(monsterPosition, playerPosition, speed * Time.deltaTime); // �÷��̾ ���󰡱�
            }
            else if (tower2ToMonster < towerToMonster && tower2ToMonster < tower3ToMonster && tower2ToMonster < tower4ToMonster)
            {
                transform.position = Vector3.MoveTowards(monsterPosition, tower2Position, speed * Time.deltaTime); // Ÿ��2�� ����
            }
            else if (tower3ToMonster < towerToMonster && tower3ToMonster < tower2ToMonster && tower3ToMonster < tower4ToMonster)
            {
                transform.position = Vector3.MoveTowards(monsterPosition, tower3Position, speed * Time.deltaTime); // Ÿ��3�� ����
            }
            else if (tower4ToMonster < towerToMonster && tower4ToMonster < tower2ToMonster && tower4ToMonster < tower3ToMonster)
            {
                transform.position = Vector3.MoveTowards(monsterPosition, tower4Position, speed * Time.deltaTime); // Ÿ��4�� ����
            }
            else
            {
                transform.position = Vector3.MoveTowards(monsterPosition, towerPosition, speed * Time.deltaTime); // �⺻ Ÿ���� ����
            }
        }
        // Ÿ��, Ÿ��2�� ���� ���
        else if (tower2 != null)
        {
            Vector3 tower2Position = tower2.GetComponent<Transform>().position; // Ÿ��2�� ��ġ
            float tower2ToMonster = Vector3.Distance(monsterPosition, tower2Position); // ���Ϳ� Ÿ��2���� �Ÿ�

            if (towerToMonster > playerToMonster)
            {
                transform.position = Vector3.MoveTowards(monsterPosition, playerPosition, speed * Time.deltaTime); // �÷��̾ ���󰡱�
            }
            else
            {
                transform.position = Vector3.MoveTowards(monsterPosition, tower2Position, speed * Time.deltaTime); // Ÿ��2�� ����
            }
        }

        // Ÿ���� ���� ���
        else
        {
            if (towerToMonster > playerToMonster)
            {
                transform.position = Vector3.MoveTowards(monsterPosition, playerPosition, speed * Time.deltaTime); // �÷��̾ ���󰡱�
            }
            else
            {
                transform.position = Vector3.MoveTowards(monsterPosition, towerPosition, speed * Time.deltaTime); // Ÿ���� ����
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("gi")) //gi �±׸� ���� ������Ʈ�� �ε�����
        {
            hpbar.SetActive(true); //ü�¹� ���̱�
            if (currHp > 0)
            { //���� ü���� �����ִٸ�
                currHp -= 1.0f; //���� ü�� �A��
                hpfront.localScale = new Vector3(currHp / maxHp, 1.0f, 1.0f); // ���� ü���� �ִ� ü������ ����� hp����

                Destroy(collision.gameObject); //�浹�� ��� �ı�

            }
            else
            {
                PlayerController playerController = player.GetComponent<PlayerController>(); // �÷��̾��� ��ũ��Ʈ ����
                playerController.GainExperience(10f); // ����ġ 10 ���� (������ ������ ����)
            }
        }
    }
}
