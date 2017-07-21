using UnityEngine;

public class PortalTeleportation : MonoBehaviour {

    [SerializeField]
    private Transform destination;
    [SerializeField]
    private float transportDelay;
    [SerializeField]
    private float closingDelay;
    [SerializeField]
    private GameObject activeParticles;
    [SerializeField]
    private Transform particlesSpawner;
    private GameObject particles;
    private GameObject transitingPlayer;

    void OnTriggerEnter (Collider player)
    {
        if (!particles) { 
            particles = Instantiate(activeParticles, gameObject.transform.position, gameObject.transform.rotation);
            transitingPlayer = player.gameObject;
            Invoke("TransportPlayer", transportDelay);
        }
    }

    void TransportPlayer()
    {
        transitingPlayer.transform.position = destination.position;
        Invoke("ClosePortal", closingDelay);
    }

    void ClosePortal()
    {
        Destroy(particles);
        particles = null;
    }
}
