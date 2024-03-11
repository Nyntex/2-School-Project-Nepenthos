using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(VisualEffect))]
public class PlayableSelfDeletingVFX : MonoBehaviour
{
    private float timeUntilParticleCheck = 2.0f;

    public void Play()
    {
        //GetComponent<GameObject>().SetActive(true);
        GetComponent<VisualEffect>().Play();
    }

    private void Update()
    {
        timeUntilParticleCheck -= Time.deltaTime;

        if (timeUntilParticleCheck <= 0)
        {
            if(GetComponent<VisualEffect>().aliveParticleCount <= 0) 
            { 
                Destroy(gameObject);
            }
        }
    }
}
