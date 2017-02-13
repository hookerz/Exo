using UnityEngine;

public interface ITriggerHandler {
    void OnTriggerEnter(Collider collider);
    void OnTriggerExit(Collider collider);
}
