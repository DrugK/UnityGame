using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Transform[] spawnPoint; //���� ����Ʈ�� ��ġ�� ����
    public GameObject[] prefabs;
    public GameObject tower2;  // Ÿ��2 ������Ʈ
    public GameObject tower3;  // Ÿ��3 ������Ʈ
    public GameObject tower4;  // Ÿ��4 ������Ʈ

    private float spawnTime = 0f;        // ���� Ÿ�̸�
    private float elapsedTime = 0f;     // ���� �� ��� �ð�
    private float spawnDelay = 1f;      // �ʱ� ���� ������ (1��)
    private const float spawnDelayReduction = 0.2f; // 1�� ��� �� ������ ���� ��
    private const float minSpawnDelay = 0.3f;       // �ּ� ���� ������ ��

    void Awake() 
    {
        spawnPoint = GetComponentsInChildren<Transform>(); //�����ʰ� �ڽ����� �����ִ� ����Ʈ���� ��ġ�� ������
    }
   
    void Update()
    {
        spawnTime += Time.deltaTime;   // ���� Ÿ�̸� ����
        elapsedTime += Time.deltaTime; // �� ��� �ð� ����

        // 1�и��� ���� ������ ����
        if (elapsedTime >= 60f)
        {
            IncreaseSpawnSpeed();
            elapsedTime = 0f; // ��� �ð� �ʱ�ȭ
        }

        // ���� ������ �ʰ� �� ���� ����
        if (spawnTime > spawnDelay)
        {
            spawnTime = 0f; // Ÿ�̸� �ʱ�ȭ
            Spawn();        // ���� ����
        }
    }

    void IncreaseSpawnSpeed()
    {
        if (spawnDelay > minSpawnDelay) // �ּ� ���� �����̺��� ũ�� ����
        {
            spawnDelay -= spawnDelayReduction;
            Debug.Log($"Spawn speed increased! New spawn delay: {spawnDelay:F2}s");
        }
        else
        {
            Debug.Log("Spawn delay is at its minimum!");
        }
    }

    void Spawn()
    {
        GameObject monster = null;
        int prefabIndex = 0;

        // Ÿ��, Ÿ��2, Ÿ��3, Ÿ��4�� ���� ���
        if (tower2 != null && tower3 != null && tower4 != null)
        {
            prefabIndex = Random.Range(0, 5);
        }
        // Ÿ��, Ÿ��2�� ���� ���
        else if (tower2 != null)
        {
            prefabIndex = Random.Range(0, 3);
        }
        else
        {
            prefabIndex = Random.Range(0, 2);
        }

        monster = PoolManager.instance.GetPreFab(prefabs[prefabIndex]); //���� ����

        if (monster != null)
        {
            int spawnIndex = Random.Range(1, spawnPoint.Length); //���� ����Ʈ �� �������� ����(0�� �ε����� ������ �ڽ�)
            monster.transform.position = spawnPoint[spawnIndex].position; //���� ����Ʈ�� ���� �ű��
        }
        else
        {
            Debug.LogError("���� ������ �����Ͽ����ϴ�.");
        }
    }
}
