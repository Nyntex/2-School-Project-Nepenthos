using UnityEngine;

public class NotificationDirector : MonoBehaviour
{
    [SerializeField]
    private float distanceToCenter = 0.25f;

    [SerializeField]
    private SpriteRenderer sprite;

    private EnemyController target;
    private HUDNotification rotator;

    public void Setup(EnemyController target, HUDNotification rotator)
    {
        this.target = target;
        this.rotator = rotator;
        SetColorToAlertnessStrengthOfTarget();
        this.target.enemyDied += SetColorToAlertnessStrengthOfTarget;
    }

    public void UpdateArrow()
    {
        if(target == null) return;
        SetColorToAlertnessStrengthOfTarget();
        LetSpriteLookAtTarget();
    }

    private void SetColorToAlertnessStrengthOfTarget()
    {
        if (target.Destroyed || !target.isActiveAndEnabled)
        {
            sprite.color = new Color(1, 1, 1, 0);
            return;
        }
        var alphaColor = target.currentAlertness / target.MaxAlertness;
        sprite.color = new Color(1, 1, 1, alphaColor);
    }

    private void LetSpriteLookAtTarget()
    {
        Vector3 direction = (target.transform.position - transform.position).normalized;
        transform.LookAt(new Vector3(direction.x,0f,direction.z) + transform.position + transform.forward);
        sprite.transform.position = transform.position + transform.forward * distanceToCenter;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, distanceToCenter);
    }
}
