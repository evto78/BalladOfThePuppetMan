using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadoutOnHover : MonoBehaviour
{
    SkillSelectionManager selectionManager;

    public string readoutText;

    private void Start()
    {
        selectionManager = GameObject.Find("UI - Turnbased").GetComponent<SkillSelectionManager>();

        if (selectionManager.readoutObjects.Contains(this)) { return; }

        selectionManager.readoutObjects.Add(this);
    }

    private void OnEnable()
    {
        if (!Application.isPlaying) { return; }

        selectionManager = GameObject.Find("UI - Turnbased").GetComponent<SkillSelectionManager>();

        if (selectionManager.readoutObjects.Contains(this)) { return; }

        selectionManager.readoutObjects.Add(this);
    }

    private void OnDestroy()
    {
        if (selectionManager != null && selectionManager.readoutObjects.Contains(this))
        {
            selectionManager.readoutObjects.Remove(this);
        }
    }

    private void OnDisable()
    {
        if (selectionManager != null && selectionManager.readoutObjects.Contains(this))
        {
            selectionManager.readoutObjects.Remove(this);
        }
    }
}
