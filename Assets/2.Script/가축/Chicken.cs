using UnityEngine;

// Animal을 상속받으므로 MonoBehaviour를 제거합니다.
public class Chicken : Animal
{
    // 이제 OnMouseDown과 Produce 함수는 Animal 스크립트에서 상속받아 사용합니다.
    // Chicken 스크립트에는 더 이상 별도의 코드가 필요하지 않습니다.
}