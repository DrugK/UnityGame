using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rigid2D; // 물리 이동을 위한 변수
    Vector2 moveVelocity; //정규화된 벡터값을 담기 위한 변수
    Vector3 dir; //마우스 방향벡터를 저장할 변수

    public float speed = 10.0f; // 플레이어의 스피드를 조절하는 변수
    private float currHp; //플레이어의 현재 hp
    public float maxHp = 10f; //플레이어의 최대 체력
    public float giSpeed = 30.0f; //기의 속도
    public float ShootRate = 0.5f; //다음 기를 쏘기까지 걸리는 딜레이 시간
    private float nextShootTime = 0f; //시간 계산

    public GameObject giPrefab; //쏠 기
    public GameObject hpbar; // 체력바를 보이거나 보이지 않게 하기 위해
    public RectTransform hpfront; // 체력바의 스케일을 조정해 닳게하기 위해

    private float currExp; //플레이어의 현재 Exp
    public float maxExp = 200f; // Exp 최대치
    public float minExp = 1f; // Exp 최소치
    public GameObject expbar; // 경험치바를 보이거나 보이지 않게 하기 위해
    public RectTransform expfront; // 경험치바의 스케일을 조정해 닳게하기 위해

    public float stageTimeLimit = 180f; // 스테이지 제한 시간 (초 단위)
    private float elapsedTime = 0f;    // 경과 시간

    public PoolManager pool;

    Vector2 minBounds = new Vector2(-57, -32); //맵의 크기
    Vector2 maxBounds = new Vector2(57, 32);
    Vector2 startPos = new Vector2(0, -2); //시작 위치 조정

    void Start()
    {
        rigid2D = GetComponent<Rigidbody2D>(); //rigidbody 컴포넌트 가져오기
        transform.position = startPos; //시작지점에서 시작
        currHp = maxHp; // 최대 체력만큼 현재 체력 설정
        currExp = minExp; // 최소치 Exp로 설정

        UpdateExpBar(); // 경험치바 초기화
    }

    void Update()
    {
        if (Time.timeScale > 0) // 게임이 일시정지 상태가 아닐 때만 타이머 작동
        {
            elapsedTime += Time.deltaTime;

            if (elapsedTime >= stageTimeLimit)
            {
                StageClear();
            }
        }

        Vector2 pos = transform.position; //플레이어의 위치
        pos.x = Mathf.Clamp(pos.x, minBounds.x, maxBounds.x); //플레이어의 위치를 맵 크기에 맞춰 제한
        pos.y = Mathf.Clamp(pos.y, minBounds.y, maxBounds.y);
        transform.position = pos; //제한된 위치로 변환

        float x = Input.GetAxisRaw("Horizontal"); //ad를 사용해서 이동(input 매니저 조정 필요)
        float y = Input.GetAxisRaw("Vertical"); // ws를 이용해서 이동

        Vector2 move = new Vector2(x, y);
        moveVelocity = move.normalized * speed; // 속도에 맞춰 벡터값 정규화

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); //마우스가 클릭된 위치 구하기
        dir = (mousePosition - pos).normalized; ////마우스가 클릭되었을 때 위치와 플레이어의 위치 간의 방향 구하기

        if (Input.GetMouseButtonDown(0) && Time.time > nextShootTime) // 마우스를 좌클릭 했을때, 딜레이 시간이 지나면
        {
            nextShootTime = Time.time + ShootRate; //딜레이 시간 재조정
            Shoot(); //쏘기
        }
    }

    private void StageClear()
    {
        string currentSceneName = SceneManager.GetActiveScene().name; // 현재 씬 이름 가져오기
        string nextSceneName = GetNextSceneName(currentSceneName);    // 다음 씬 이름 계산


        Debug.Log($"Stage Clear! Moving to next scene: {nextSceneName}");
        SceneManager.LoadScene(nextSceneName);
        
    }

    private string GetNextSceneName(string currentSceneName)
    {
        // 정규식을 사용해 숫자를 추출
        System.Text.RegularExpressions.Match match =
            System.Text.RegularExpressions.Regex.Match(currentSceneName, @"(\d+)$");

        if (match.Success)
        {
            int currentSceneNumber = int.Parse(match.Value); // 현재 씬 번호 추출
            string nextSceneName;

            // ClearNextScene → GameScene 전환
            if (currentSceneName.StartsWith("ClearNextScene"))
            {
                nextSceneName = $"GameScene{currentSceneNumber}"; // GameSceneX로 이동
            }
            // GameScene → ClearNextScene 전환
            else if (currentSceneName.StartsWith("GameScene"))
            {
                nextSceneName = $"ClearNextScene{currentSceneNumber + 1}"; // ClearNextSceneX로 이동
            }
            else
            {
                nextSceneName = null; // 예상치 못한 씬 이름
            }

            return nextSceneName;
        }

        return null; // 숫자가 포함되지 않은 경우
    }

    private void UpdateExpBar()
    {
        if (expfront != null)
        {
            float expRatio = currExp / maxExp; // 현재 경험치 비율 계산
            expfront.localScale = new Vector3(expRatio, 1.0f, 1.0f); // 경험치바 크기 조정
        }
    }

    public void GainExperience(float exp)
    {

        currExp += exp; // 경험치 증가
        UpdateExpBar(); // 경험치바 업데이트

        // 경험치가 최대치에 도달하면 레벨업 처리
        if (currExp >= maxExp)
        {
            currExp -= maxExp; // 초과된 경험치 유지
            LevelUp();         // 레벨업 메서드 호출
        }

        // 경험치 UI 업데이트 (필요하다면 구현)
        Debug.Log($"Current Experience: {currExp}");
    }

    private void LevelUp()
    {
        // 레벨업 로직
        Debug.Log("Level Up!");
    }

    void FixedUpdate() //Update함수는 프레임이 일정하지 않기 때문에 rigidbody를 다루는 코드를 설정하는 함수
    {

        rigid2D.MovePosition(rigid2D.position + moveVelocity * Time.fixedDeltaTime); //플레이어를 움직이게 하는 코드, 플레이어의 포지션에 벡터값과 프레임 사이의 간격을 곱해 플레이어를 움직임

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("eat")) //eat 태그를 가진 오브젝트와 부딪히면
        {
            if (currHp > 0)
            { //현재 체력이 남아있다면
                currHp -= 1.0f; //체력 1갂기
                hpfront.localScale = new Vector3(currHp / maxHp, 1.0f, 1.0f); // 현재 체력을 최대 체력으로 나누어서 hp조절

                Destroy(collision.gameObject); //부딪힌 몬스터 삭제 

                //풀링을 사용한다면....
                //PoolManager.instance.ReturnMonster(collision.gameObject); 충돌한 몬스터 비활성화(풀링)

            }

            else
            {
                GameOver();
            }
        }
        if (collision.gameObject.CompareTag("gohome")) //gohome 태그를 가진 오브젝트와 부딪히면
        {
            if (currHp > 0)
            { //현재 체력이 남아있다면
                currHp -= 1.0f; //체력 1갂기
                hpfront.localScale = new Vector3(currHp / maxHp, 1.0f, 1.0f); // 현재 체력을 최대 체력으로 나누어서 hp조절

                Destroy(collision.gameObject); //부딪힌 몬스터 삭제 

                //풀링을 사용한다면....
                //PoolManager.instance.ReturnMonster(collision.gameObject); 충돌한 몬스터 비활성화(풀링)

            }

            else
            {
                GameOver();
            }
        }
        if (collision.gameObject.CompareTag("what")) //what 태그를 가진 오브젝트와 부딪히면
        {
            if (currHp > 0)
            { //현재 체력이 남아있다면
                currHp -= 2.0f; //체력 2갂기
                hpfront.localScale = new Vector3(currHp / maxHp, 1.0f, 1.0f); // 현재 체력을 최대 체력으로 나누어서 hp조절

                Destroy(collision.gameObject); //부딪힌 몬스터 삭제 

                //풀링을 사용한다면....
                //PoolManager.instance.ReturnMonster(collision.gameObject); 충돌한 몬스터 비활성화(풀링)

            }

            else
            {
                GameOver();
            }
        }
        if (collision.gameObject.CompareTag("no")) //no 태그를 가진 오브젝트와 부딪히면
        {
            if (currHp > 0)
            { //현재 체력이 남아있다면
                currHp -= 3.0f; //현재 체력 갂기
                hpfront.localScale = new Vector3(currHp / maxHp, 1.0f, 1.0f); // 현재 체력을 최대 체력으로 나누어서 hp조절

                Destroy(collision.gameObject); //부딪힌 몬스터 삭제 

                //풀링을 사용한다면....
                //PoolManager.instance.ReturnMonster(collision.gameObject); 충돌한 몬스터 비활성화(풀링)

            }

            else
            {
                GameOver();
            }
        }
        if (collision.gameObject.CompareTag("pressure")) //pressure 태그를 가진 오브젝트와 부딪히면
        {
            if (currHp > 0)
            { //현재 체력이 남아있다면
                currHp -= 5.0f; //현재 체력 갂기
                hpfront.localScale = new Vector3(currHp / maxHp, 1.0f, 1.0f); // 현재 체력을 최대 체력으로 나누어서 hp조절
                Destroy(collision.gameObject); //부딪힌 몬스터 삭제 

                //풀링을 사용한다면....
                //PoolManager.instance.ReturnMonster(collision.gameObject); 충돌한 몬스터 비활성화(풀링)

            }

            else
            {
                GameOver();
            }
        }
        if (collision.gameObject.CompareTag("exp")) {
            GainExperience(10); // 경험치 획득
            Destroy(collision.gameObject);
        }

    }

    public void GameOver()
    {
        SceneManager.LoadScene("GameOver");
    }

    void Shoot() { // 기를 쏘는 함수
        GameObject gi = Instantiate(giPrefab); //플레이어 위치에 기 스폰
        gi.transform.position = transform.position; //기의 위치를 플레이어의 위치로 이동
        gi.GetComponent<Rigidbody2D>().velocity = dir * giSpeed; //설정한 기의 속도만큼 마우스 위치로 발사
    }

}


