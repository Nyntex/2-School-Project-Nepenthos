using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Script_VFX_Teleport : MonoBehaviour
{

    public VisualEffect VFX_Teleport;

    public float rate = 0.02f;

    private bool teleportActive;

    // Start is called before the first frame update
    void Start()
    {
        VFX_Teleport.Stop();
        VFX_Teleport.SetFloat("WarpAmount", 0);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input .GetKeyDown(KeyCode.Space)) 
        {
            teleportActive = true;
            StartCoroutine(ActivateParticles());
        }
        if(Input .GetKeyUp(KeyCode.Space))
        {
            teleportActive= false;
            StartCoroutine(ActivateParticles());
        }

      
    }

    IEnumerator ActivateParticles() 
    {
        if(teleportActive)
        {
            VFX_Teleport.Play();

            float amount = VFX_Teleport.GetFloat("WarpAmount");
            while(amount < 1 & teleportActive)
            {
                amount = +rate;
                VFX_Teleport.SetFloat("WarpAmount", amount);
                yield return new WaitForSeconds(0.1f);
            }
        }
        else
        {
            float amount = VFX_Teleport.GetFloat("WarpAmount");
            while (amount > 0 & !teleportActive)
            {
                amount = -rate;
                VFX_Teleport.SetFloat("WarpAmount", amount);
                yield return new WaitForSeconds(0.1f);

                if (amount <= 0 + rate) 
                {
                    amount = 0;
                    VFX_Teleport.SetFloat("WarpAmount", amount);

                    VFX_Teleport.Stop();
                }
            }
           
        }

           
    }

}
