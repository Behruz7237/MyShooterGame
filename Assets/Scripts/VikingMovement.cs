using UnityEngine;

public class VikingEnemy : MonoBehaviour
{
    public Transform player;
    public float speed = 2f;
    public float attackDistance = 2f;

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > attackDistance)
        {
            // MOVE
            transform.position = Vector3.MoveTowards(transform.position, player.position, speed * Time.deltaTime);

            animator.SetFloat("Speed", 1f);
        }
        else
        {
            // ATTACK
            animator.SetFloat("Speed", 0f);
            animator.SetTrigger("Attack");
        }
    }
}