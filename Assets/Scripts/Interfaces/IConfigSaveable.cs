using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IConfigSaveable
{
    void LoadData(ConfigData data);
    void SaveData(ref ConfigData data);
}
