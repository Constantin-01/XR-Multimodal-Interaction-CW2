using UnityEngine;

public class TargetReset : MonoBehaviour
{
    public GameObject targetPrefab;

    public Transform[] spawnPoints;

    public void ResetTargets()
    {
        GameObject[] oldTargets = GameObject.FindGameObjectsWithTag("Target");

        foreach (GameObject target in oldTargets)
        {
            Destroy(target);
        }

        foreach (Transform spawn in spawnPoints)
        {
            Instantiate(targetPrefab, spawn.position, spawn.rotation);
        }
    }
}