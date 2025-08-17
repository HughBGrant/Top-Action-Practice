using UnityEngine;

public class GrenadeOrbitGroup : MonoBehaviour
{
    [SerializeField] private Transform[] grenades;   // 4개
    private float radius = 2f;    // 캐릭터로부터 거리
    [SerializeField] private float orbitSpeed = 90f; // 도/초
    [SerializeField] private bool useWorldUp = false; // true면 월드 Y축 기준 회전

    void Start()
    {
        ArrangeEvenly();
    }

    void LateUpdate()
    {
        // Player의 자식이라면 항상 중심 유지
        if (transform.parent != null) transform.localPosition = Vector3.zero;

        // 그룹 자체를 회전시켜서 공전
        if (useWorldUp)
            transform.Rotate(Vector3.up, orbitSpeed * Time.deltaTime, Space.World);
        else
            transform.Rotate(0f, orbitSpeed * Time.deltaTime, 0f, Space.Self);
    }

    // 네 개를 90도 간격으로 배치 (갯수 바뀌어도 균등 배치)
    public void ArrangeEvenly()
    {
        if (grenades == null || grenades.Length == 0) return;

        for (int i = 0; i < grenades.Length; i++)
        {
            if (grenades[i] == null) continue;

            float t = (float)i / grenades.Length;
            float angle = t * Mathf.PI * 2f;
            Vector3 localPos = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * radius;
            grenades[i].localPosition = localPos;
        }
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (Application.isPlaying) return;
        ArrangeEvenly();
    }
#endif
}