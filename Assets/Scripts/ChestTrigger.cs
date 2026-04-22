using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChestTrigger : MonoBehaviour
{
    public TMP_Text lockedText;
    public string nextSceneName = "level2";

    private Animator animator;
    private bool opened = false;
    private bool loadingNextScene = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        Debug.Log($"[ChestTrigger] Start on '{name}'. Animator found: {animator != null}. Locked text assigned: {lockedText != null}.", this);

        if (lockedText != null)
        {
            Canvas parentCanvas = lockedText.GetComponentInParent<Canvas>(true);
            if (parentCanvas != null && !parentCanvas.gameObject.activeSelf)
            {
                parentCanvas.gameObject.SetActive(true);
                Debug.Log("[ChestTrigger] Activated parent canvas once at startup so the TMP text can be shown without toggling GameObjects later.", this);
            }

            if (!lockedText.gameObject.activeSelf)
            {
                lockedText.gameObject.SetActive(true);
                Debug.Log("[ChestTrigger] Activated the TMP text GameObject once at startup.", this);
            }

            SetLockedTextVisible(false, "Start");
        }
        else
        {
            Debug.LogWarning("[ChestTrigger] No lockedText is assigned in the inspector.", this);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"[ChestTrigger] Trigger entered by '{other.name}' (tag: {other.tag}). Opened: {opened}.", this);

        if (opened)
        {
            Debug.Log("[ChestTrigger] Ignoring trigger because the chest is already open.", this);
            return;
        }

        if (!other.CompareTag("Player"))
        {
            Debug.Log("[ChestTrigger] Ignoring trigger because the collider is not tagged Player.", this);
            return;
        }

        PlayerMovement player = other.GetComponent<PlayerMovement>();
        if (player == null)
            player = other.GetComponentInParent<PlayerMovement>();

        if (player == null)
        {
            Debug.LogWarning("[ChestTrigger] Player entered trigger, but no PlayerMovement component was found.", this);
            return;
        }

        Debug.Log($"[ChestTrigger] Player entered chest radius. KeyPickedUp: {player.KeyPickedUp}.", this);

        if (!player.KeyPickedUp)
        {
            if (lockedText != null)
                SetLockedTextVisible(true, "Player entered without key");
            else
                Debug.LogWarning("[ChestTrigger] Player has no key, but lockedText is not assigned.", this);

            return;
        }

        if (lockedText != null)
            SetLockedTextVisible(false, "Player entered with key");

        Debug.Log("[ChestTrigger] Opening chest because the player has the key.", this);

        opened = true;
        animator.SetTrigger("OpenChest");
        StartCoroutine(LoadNextSceneAfterChestOpens());
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (lockedText != null)
            SetLockedTextVisible(false, "Player exited trigger");
    }

    void SetLockedTextVisible(bool visible, string context)
    {
        lockedText.enabled = true;
        lockedText.alpha = visible ? 1f : 0f;
        lockedText.raycastTarget = visible;

        Debug.Log(
            $"[ChestTrigger] {context}. lockedText.enabled={lockedText.enabled}, lockedText.alpha={lockedText.alpha}, textObjectActiveSelf={lockedText.gameObject.activeSelf}, textObjectActiveInHierarchy={lockedText.gameObject.activeInHierarchy}, text='{lockedText.text}'.",
            this
        );

        if (visible && !lockedText.gameObject.activeInHierarchy)
        {
            Debug.LogWarning(
                "[ChestTrigger] lockedText was set visible, but it is still inactive in the hierarchy. A parent object is probably disabled.",
                this
            );
        }
    }

    IEnumerator LoadNextSceneAfterChestOpens()
    {
        if (loadingNextScene)
            yield break;

        loadingNextScene = true;

        if (animator == null)
        {
            Debug.LogWarning("[ChestTrigger] No Animator found. Loading next scene immediately.", this);
            SceneManager.LoadScene(nextSceneName);
            yield break;
        }

        yield return null;

        while (animator.IsInTransition(0))
            yield return null;

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        while (!stateInfo.IsName("Chest"))
        {
            yield return null;
            stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        }

        while (animator.IsInTransition(0) || stateInfo.normalizedTime < 1f)
        {
            yield return null;
            stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        }

        Debug.Log($"[ChestTrigger] Chest animation finished. Loading scene '{nextSceneName}'.", this);
        SceneManager.LoadScene(nextSceneName);
    }
}
