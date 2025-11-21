using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    private Vector2 currentRespawnPoint;

    void Start()
    {
        // Respawn inicial (posição inicial do player)
        currentRespawnPoint = transform.position;
    }

    public void SetRespawnPoint(Vector2 newPoint)
    {
        currentRespawnPoint = newPoint;
        Debug.Log("Novo Respawn Registrado em: " + newPoint);
    }

    public void Respawn()
    {
        transform.position = currentRespawnPoint;
    }
}