using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField] protected Animator _animator;

    public void Walk(bool enabled)
    {
        _animator.SetBool("IsWalking", enabled);
    }

    public void Attack()
    {
        _animator.SetTrigger("Attack");
    }

    public void TakeDamage()
    {
        _animator.SetTrigger("TakeDamage");
    }

    public void TriggerAnimataion(string name)
    {
        _animator.SetTrigger(name);
    }
}
