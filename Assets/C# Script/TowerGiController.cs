using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerGiController : MonoBehaviour
{
    public float speed = 10f; // Gi �ӵ�
    private float lifetime = 3f;
    private float spawnTime;
    private GameObject target;

    void Start()
    {
        spawnTime = Time.time;
    }

    void Update()
    {
        if (target == null) // Ÿ���� ������ �Ҹ�
        {
            Destroy(gameObject);
            return; //Ÿ���� ������ ������� �ʵ���
        }
        else {
            // Ÿ�� �������� �̵�
            Vector3 direction = (target.transform.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;
        }

        // ���� �ð��� ������ �Ҹ�
        if (Time.time - spawnTime > lifetime)
        {
            Destroy(gameObject);
        }
    }

    // Ÿ�� ����
    public void SetTarget(GameObject target)
    {
        this.target = target;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == target)
        {
            // Ÿ�ٿ� �������� �� ������ ������
            var targetController = collision.GetComponent<MonsterController>();
            if (targetController != null)
            {
                targetController.TakeDamage(1.0f); // ������ 1 �ֱ�
            }
            Destroy(gameObject); // ����ü �Ҹ�
        }
    }
}