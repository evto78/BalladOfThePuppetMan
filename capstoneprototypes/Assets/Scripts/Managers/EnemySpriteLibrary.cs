using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpriteLibrary : MonoBehaviour
{
    [System.Serializable]
    public class EnemySpriteList
    {
        public List<AnimSheet> anims;
    }

    [System.Serializable]
    public class AnimSheet
    {
        public List<Sprite> animFrames;
    }

    public List<EnemySpriteList> globalSpriteList;
}
