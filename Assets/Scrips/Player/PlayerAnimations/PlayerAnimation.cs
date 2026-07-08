using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator animator;
    private MovementPlayer movement;

    void Awake()
    {
        animator = GetComponent<Animator>();
        movement = GetComponent<MovementPlayer>();
    }

    void Update()
    {
        HandleAnimatorParameters();
    }

    private void HandleAnimatorParameters()
    {
        // mengambil variabel MoveInput pada movementPlayer
        Vector2 moveInput = movement.MoveInput;

        // melakukan pengecekan kode  jika panjang magnitude MoveInput lebih dari 0.1 maka animasi akan di putar
        if (moveInput.magnitude > 0.1f)
        {
            animator.SetBool("isMoving", true);
            animator.SetFloat("moveX", moveInput.x);
            animator.SetFloat("moveY", moveInput.y);
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