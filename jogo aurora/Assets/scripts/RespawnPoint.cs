using UnityEngine;

public class RespawnPoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerRespawn playerRespawn = collision.GetComponent<PlayerRespawn>();

            if (playerRespawn != null)
            {
                playerRespawn.SetRespawnPoint(transform.position);
            }
        }
    }
}