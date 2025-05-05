using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public string interactionMessage = "Interagiu com o objeto!";

    public virtual void Interact()
    {
        Debug.Log(interactionMessage);

    }

}
