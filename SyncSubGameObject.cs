using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SyncSubGameObject
{
    public static SimpleTree simpleTree;

    private static void CoppyCurrentSelection()
    {
        Transform selection = Selection.activeTransform;

        if (simpleTree == null)
            simpleTree = new SimpleTree(selection);

        if (selection)
        {
            simpleTree.root = selection;

            Mapping(simpleTree);
        }
    }

    private static void Mapping(SimpleTree _simpleTree)
    {
        foreach (Transform t in _simpleTree.root)
        {
            SimpleTree st = new SimpleTree(t);
            _simpleTree.AddChild(st);
            Mapping(st);
        }
    }

    private static void PastToCurrentSelection()
    {
        Transform selection = Selection.activeTransform;

        if (simpleTree == null)
            return;

        if (selection)
        {
            PastChild(selection, simpleTree);
        }
    }

    private static void PastChild(Transform parent, SimpleTree _simpleTree)
    {
        foreach (SimpleTree st in _simpleTree.childs)
        {
            if (parent.Find(st.root.name))
            {
                PastChild(parent.Find(st.root.name), st);
                continue;
            }

            GameObject obj = new GameObject(st.root.name);
            obj.transform.parent = parent;
            obj.transform.localPosition = st.root.transform.localPosition;
            obj.transform.localRotation = st.root.transform.localRotation;
            obj.transform.localScale = st.root.transform.localScale;
            if (!st.root.gameObject.activeSelf)
                obj.SetActive(false);
            PastChild(obj.transform, st);
        }
    }

    private static void ClearRoot()
    {
        if (simpleTree == null)
            return;

        simpleTree.childs.Clear();
    }


    [MenuItem("GameObject/Copy Select GameObject #&E")]
    public static void CopySelectGameObject()
    {
        CoppyCurrentSelection();

        Debug.Log("拷贝完成");
    }

    [MenuItem("GameObject/Paste Select GameObject #&R")]
    public static void PastSelectGameObject()
    {
        PastToCurrentSelection();

        ClearRoot();

        Debug.Log("创建完成");
    }
}

public class SimpleTree
{
    public Transform root;
    public List<SimpleTree> childs;

    public SimpleTree(Transform _root)
    {
        root = _root;
        childs = new List<SimpleTree>();
    }

    public void AddChild(SimpleTree _child)
    {
        if (childs.Contains(_child))
            return;
        childs.Add(_child);
    }
}