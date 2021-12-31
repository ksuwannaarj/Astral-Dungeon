using UnityEngine;

public class Room : MonoBehaviour
{
    public Doorway[] doorways;
    public MeshCollider meshCollider;
    public int ratio;
    public Bounds RoomBounds {
        get { return meshCollider.bounds; }
    }
}
