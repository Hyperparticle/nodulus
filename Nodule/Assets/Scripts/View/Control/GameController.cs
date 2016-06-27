using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour
{
    public GameObject PointPrefab;

    public void DebugPoint(Vector3 pos)
    {
        Instantiate(PointPrefab, pos, Quaternion.identity);
    }
}
