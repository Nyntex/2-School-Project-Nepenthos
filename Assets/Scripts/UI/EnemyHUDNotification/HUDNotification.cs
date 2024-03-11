using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HUDNotification : MonoBehaviour
{
    [SerializeField]
    private GameObject notificationArrowPrefab;

    private List<EnemyController> enemys;
    private List<NotificationDirector> directionArrows = new List<NotificationDirector>();

    private void Start()
    {
        enemys = FindAllEnemys();
        foreach(EnemyController enemy in enemys)
        {
            var arrow = Instantiate(notificationArrowPrefab,transform.position, Quaternion.identity);
            arrow.transform.SetParent(transform, true);
            var target = arrow.GetComponent<NotificationDirector>();
            directionArrows.Add(target);
        }

        for(int i = 0; i < enemys.Count; i++)
        {
            directionArrows[i].Setup(enemys[i], this);
        }
    }

    private void Update()
    {
        var tempRotation = transform.eulerAngles;
        transform.eulerAngles = new Vector3(0,transform.eulerAngles.y, transform.eulerAngles.z);
        foreach(var arrow in directionArrows)
        {
            arrow.UpdateArrow();
        }
        transform.eulerAngles = tempRotation;
    }

    private List<EnemyController> FindAllEnemys()
    {
        IEnumerable<EnemyController> saveableObjects = FindObjectsOfType<EnemyController>(true);

        return new List<EnemyController>(saveableObjects);
    }
}
