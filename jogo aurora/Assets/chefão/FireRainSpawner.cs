using UnityEngine;

public class FireRainSpawner : MonoBehaviour
{
    public GameObject fireBallPrefab;
    public int quantidade = 5;
    public float area = 3f;
    public float delayEntreQuedas = 0.3f;

    void Start()
    {
        StartCoroutine(ChuvaDeFogo());
    }

    System.Collections.IEnumerator ChuvaDeFogo()
    {
        for (int i = 0; i < quantidade; i++)
        {
            Vector3 pos = new Vector3(
                transform.position.x + Random.Range(-area, area),
                transform.position.y + 4f,
                transform.position.z
            );

            Instantiate(fireBallPrefab, pos, Quaternion.identity);
            yield return new WaitForSeconds(delayEntreQuedas);
        }

        Destroy(gameObject);
    }
}