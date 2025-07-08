using UnityEngine;
using Unity.Netcode;

public class PlayerController : NetworkBehaviour
{
    public Transform startPoint;
    public GameObject victoryPanel;
    public GameObject defeatPanel;

    private void Update()
    {
        if (!IsOwner) return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        transform.position = mousePos;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsOwner) return;

        if (collision.CompareTag("Wall"))
        {
            transform.position = startPoint.position;
            defeatPanel.SetActive(true);
        }

        if (collision.CompareTag("Goal"))
        {
            victoryPanel.SetActive(true);
        }
    }
}
