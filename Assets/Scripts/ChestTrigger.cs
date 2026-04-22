using TMPro;
using UnityEngine;

public class ChestTrigger : MonoBehaviour
{
    public TMP_Text lockedText;

    private Animator animator;
    private bool opened = false;

    void Start()
    {
        animator = GetComponent<Animator>();

        if (lockedText != null)
            lockedText.enabled = false;
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

        if (player == null)
            return;

        if (!player.KeyPickedUp)
        {
            if (lockedText != null)
                lockedText.enabled = true;

            return;
        }

        if (lockedText != null)
            lockedText.enabled = false;

        animator.SetTrigger("OpenChest");
        opened = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (lockedText != null)
            lockedText.enabled = false;
    }
}
