using UnityEngine;

public class BossHeadTracker : MonoBehaviour
{
    public HeadHealth[] heads;

    private bool victoryTriggered = false;

    void Update()
    {
        if (victoryTriggered) return;

        bool allDead = true;

        foreach (var h in heads)
        {
            if (h != null && !h.IsDead())
            {
                allDead = false;
                break;
            }
        }

        if (allDead)
        {
            victoryTriggered = true;

            GameManager gm = FindFirstObjectByType<GameManager>();

            if (gm != null)
            {
                gm.Victory();
            }
        }
    }
}