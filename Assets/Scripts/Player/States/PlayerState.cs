using UnityEngine;

public class PlayerState
{
    protected PlayerController playerController;
    protected Controls inputActions;

    public void Initialize(PlayerController playerController)
    {
        this.playerController = playerController;
        inputActions = playerController.InputActions;
    }
    public virtual void Enter()
    {
        Debug.Log(this.GetType().Name + " ENTER");
    }
    public virtual void UpdateState()
    {

    }
    public virtual void Exit()
    {
        //Debug.Log(this.GetType().Name + "EXIT");
    }

    public virtual void OnSneak() { }
    public virtual void OnSprint() { }
    public virtual void OnJump() { }
    public virtual void Snapshot() { }
    public virtual void OnDistractionObjectPickup(DistractionObject target) 
    {
        playerController.distractionObject = target;
        playerController.HandAnimator.SetBool("GrabbedDistractable", true);
    } 
    public virtual void ThrowDistractionObject()
    {
        Debug.Log("Distraction Object: " + playerController.distractionObject);
        playerController.HandAnimator.SetBool("GrabbedDistractable", false);
        if(playerController.distractionObject == null) { return; }
        Vector3 throwDir = Camera.main.transform.forward + new Vector3(0f, playerController.ThrowForce/50f, 0f);
        playerController.distractionObject.gameObject.SetActive(true);
        playerController.distractionObject.transform.SetParent(null);
        playerController.distractionObject.Throw(throwDir, playerController.ThrowForce);
        playerController.distractionObject = null;
        playerController.itemInHand.SetActive(false);
    }

    public virtual bool TryPlaceTrap()
    {
        Ray ray = Camera.main.ScreenPointToRay(inputActions.Mouse.Position.ReadValue<Vector2>());
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if(hit.collider.gameObject.layer == 6)
            {
                if(Vector3.Distance(playerController.gameObject.transform.position, hit.point) <= playerController.trapPlacementRange)
                {
                    GameObject go = GameObject.Instantiate(playerController.trapPrefab, new Vector3(hit.point.x, 0.05f, hit.point.z), Quaternion.identity) as GameObject;
                    return true;
                }
            }
        }
        return false;
    }

    public virtual void Blink(Vector3 targetPosition)
    {
        Debug.Log(targetPosition.ToString());
    }

    public virtual void Hide()
    {
        playerController.TransitionToState(playerController.HideState);
    }
}