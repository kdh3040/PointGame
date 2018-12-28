using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameChar : MonoBehaviour
{
    public Animator Anim;
    public bool BlockLeftDir = true;

    public void Initialize()
    {
        // 캐릭터 초기화
        Anim.SetTrigger("idle");
    }

    public void CharJump(bool left)
    {
        // 캐릭터 점프
        Anim.Rebind();
        Anim.SetTrigger("run");
        gameObject.transform.localScale = new Vector3(left ? 1 : -1, 1, 1);
    }

    public void CharIdle()
    {
        Anim.Rebind();
        Anim.SetTrigger("idle");
    }
}
