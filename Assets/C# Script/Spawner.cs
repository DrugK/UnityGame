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

    float spawnTime = 0f;

    void Awake() 
    {
        spawnPoint = GetComponentsInChildren<Transform>(); //�����ʰ� �ڽ����� �����ִ� ����Ʈ���� ��ġ�� ������
    }
   
    void Update()
    {
        spawnTime += Time.deltaTime; //������ �������� �Ϸ�� �� �ð��� ��� ������

        if (spawnTime > 0.7f) // 1�ʰ� �����ٸ�
        {
            spawnTime = 0f; //�ٽ� 0
            Spawn();//���� ����
        }
    }

    void Spawn()
    {
        // Ÿ��, Ÿ��2, Ÿ��3, Ÿ��4�� ���� ���
        if (tower2 != null && tower3 != null && tower4 != null)
        {
            GameObject monster2 = Instantiate(prefabs[Random.Range(0, 5)], transform);
            monster2.transform.position = spawnPoint[Random.Range(1, spawnPoint.Length)].position;
        }

        // Ÿ��, Ÿ��2�� ���� ���
        else if (tower2 != null)
        {
            GameObject monster2 = Instantiate(prefabs[Random.Range(0, 3)], transform);
            monster2.transform.position = spawnPoint[Random.Range(1, spawnPoint.Length)].position;
        }
        else
        {
            GameObject monster = Instantiate(prefabs[Random.Range(0, 2)], transform); //���� �����ϱ�(�����տ� �ִ� ������ �����ϰ� 0��° �ε����� ���ͺ��� 1��° ����)
            monster.transform.position = spawnPoint[Random.Range(1, spawnPoint.Length)].position;// ���� ��ġ�� spawnPoint�� 1������ ����������(0���� ������ ��ġ(0,0))
        }
    }
}
