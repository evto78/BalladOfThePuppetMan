using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpriteLibrary : MonoBehaviour
{
    [System.Serializable]
    public class EnemySpriteList
    {
        public List<Sprite> spriteList;
    }

    public List<EnemySpriteList> globalSpriteList;
}
