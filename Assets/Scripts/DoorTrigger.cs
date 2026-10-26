
using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    public Animator doorAnimator;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            doorAnimator.SetTrigger("OpenDoor");
            Debug.Log("El jugador ha entrat a la zona de la porta");
        }
    }
}
