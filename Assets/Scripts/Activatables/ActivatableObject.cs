using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

[RequireComponent(typeof(BoxCollider))]
public class ActivatableObject : MonoBehaviour, IActivatable, ISaveable
{
    [SerializeField]
    protected GameObject ObjectToDeactivate;

    [Space(10)]
    [Header("- - - Please Generate an ID - - -")]
    [Space(10)]
    [SerializeField] protected string id;
    #region Generate GUID Button
    [ContextMenu("Generate new GUID")]
    public void GenerateID()
    {
#if UNITY_EDITOR
        if (PrefabStageUtility.GetCurrentPrefabStage() == null)
        {
            id = System.Guid.NewGuid().ToString();
            EditorUtility.SetDirty(this);
        }
#endif
    }
    [InspectorButton("GenerateID")]
    public bool generate;
    private void OnButtonClicked()
    { GenerateID(); }
    #endregion Generate GUID Button

    protected bool hasBeenActivated = false;

    public virtual void Activate()
    {
        if(hasBeenActivated)
        {
            ObjectToDeactivate.SetActive(!ObjectToDeactivate.activeSelf);
            SaveSystem.instance.ActivateObject(id);
        }
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if(!hasBeenActivated) 
        { 
            hasBeenActivated = true;
            Activate();
        }
    }

    public virtual void LoadData(GameData data) //During Awake
    {
        if(data.activatableObjectDataDictionary.ContainsKey(id)) 
        {
            hasBeenActivated = data.activatableObjectDataDictionary[id].hasBeenActivated;
        }
    }
    private void Start() //Will call after it has been loaded to set it active or not
    {
        Activate();
    }

    public virtual void SaveData(ref GameData data)
    {
        if(data.activatableObjectDataDictionary.ContainsKey(id)) 
        {
            data.activatableObjectDataDictionary.Remove(id);
        }

        data.activatableObjectDataDictionary.Add(id, new ActivatableObjectData(hasBeenActivated));
    }
}
