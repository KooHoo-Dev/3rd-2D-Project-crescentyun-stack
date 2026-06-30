using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class UFOController : MonoBehaviour
{
    // 위,아래 움직임 기능 넣어보기
    // 기본적으로 하강을 하지만 스페이스를 눌렀을때는 상승

    public float Speed = 1.0f;
    public int CoinCount = 0;

    // Update is called once per frame
    void Update()
    {
        //기본적으로 하강
        Vector3 moveVector = new Vector3(0, -1, 0);

        // Space를 누르면 상승한다
        if (Keyboard.current.spaceKey.isPressed)
        {
            moveVector = moveVector * (-1f); // -1 * -1 = +1;
        }

        //속도 * 프레임당 이동량을 구해준다
        moveVector = moveVector * Speed * Time.deltaTime;

        transform.position += moveVector;
    }


    // 충돌중일때는 충돌되지 않게 하기위한 조건 검사용 변수
    private bool isColliding = false;

    // OnTriggerEnter2D 이벤트 함수는
    // Rigidbody를 가진 GameObject가 Collider(IsTrigger가 True)개체와
    // 접촉했을때 1회 호출됩니다.
    private void OnTriggerEnter2D(Collider2D collider)
    {
        // .CompareTag() || GameObject.CompareTag()
        // - 매개변수로 전달된 값(문자열, 태그)과 지정된 gameObject가 갖고 있는 Tag를 비교하여
        //  True/False bool 타입으로 반환합니다.
        // - 전달된 값과 동일하면 True, 아니라면 False를 반환합니다

        if (collider.gameObject.CompareTag("Coin"))
        {
            Destroy(collider.gameObject);
            CoinCount += 1;
        }
        else if((isColliding == false) && (collider.gameObject.CompareTag("Obstacle")))
        {
            // 지형과의 충돌 로직을 작성
            isColliding = true;
            StartCoroutine(CollisionEffect()); // 코루틴 함수를 호출하는 방법
        }
    }

    // # Coroutine?
    // - 시간의 흐름에 따라 순차적으로 진행되는 로직을 Update 함수 없이 깔끔하게 작성
    // 하게 해주는 유니티의 강력한 기능입니다. 
    // - 함수의 실행을 잠시 멈추고, 특정 조건 후 멈춘 지점부터 다시 시작할 수 있는
    // 함수 입니다.
    // - Coroutine으로 선언된 함수는 그냥 실행할 수 없으며, StartCoroutine()이라는 함수를
    // 통해만 실행할 수 있습니다.

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>(); 
        // 게임오브젝트에 부착된 SpriteRenderer 컴포넌트를 가져 옵니다.
    }

    private IEnumerator CollisionEffect()
    {
        Color origin = spriteRenderer.color;
        Color effect = spriteRenderer.color;
        effect.a = 0.2f; // 투명도를 20%로 낮춰줍니다.

        float term = 0.1f;

        for (int i = 0; i < 10; ++i)
        {
            spriteRenderer.color = effect;
            yield return new WaitForSeconds(term);
            spriteRenderer.color = origin;
            yield return new WaitForSeconds(term);
        }

        isColliding = false;
    }

}