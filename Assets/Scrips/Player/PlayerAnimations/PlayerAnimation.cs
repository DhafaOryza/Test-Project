using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator animator;
    private MovementPlayer movement;

    void Start()
    {
        animator = GetComponent<Animator>();
        movement = GetComponent<MovementPlayer>();
    }

    void Update()
    {
        Vector2 moveInput = movement.MoveInput;

        if (moveInput.magnitude > 0.1f)
        {
            Vector3 localMove = transform.InverseTransformDirection(new Vector3(moveInput.x, moveInput.y, 0));

            animator.SetFloat("moveX", localMove.x);
            animator.SetFloat("moveY", localMove.y);
            animator.SetBool("isMoving", true);
        }
        else
        {
            animator.SetBool("isMoving", false);
            animator.SetFloat("moveX", 0);
            animator.SetFloat("moveY", 0);
        }
    }

    public void ChangeWeaponAnimator(RuntimeAnimatorController newController)
    {
        if (animator != null && newController != null)
        {
            animator.runtimeAnimatorController = newController;
        }
    }
}