using UnityEngine;

public class ChestTrigger : MonoBehaviour
{
    private Animator animator;
    private bool opened = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (opened)
            return;

        if (!other.CompareTag("Player"))
            return;

        PlayerMovement player = other.GetComponent<PlayerMovement>();
        if (player == null)
            player = other.GetComponentInParent<PlayerMovement>();

        if (player == null || !player.KeyPickedUp)
            return;

        animator.SetTrigger("OpenChest");
        opened = true;
    }
}
