using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Node : INode
{
    public Bounds bound { get; set; }
    
    private int depth;
    private Tree belongTree;
    private Node[] childList;
    private List<ObjData> objList;//属于这个节点的游戏对象
    
    public Node(Bounds bound, int depth, Tree belongTree)
    {
        this.belongTree = belongTree;
        this.bound = bound;
        this.depth = depth;
        //childList = new Node[belongTree.maxChildCount];
        objList = new List<ObjData>();//对象列表
      
    }
    
    public void InsertObj(ObjData obj)
    {
        Node node = null;
        bool bChild = false;
        
        if(depth < belongTree.maxDepth && childList == null)
        {
            //如果还没到叶子节点，可以拥有儿子且儿子未创建，则创建儿子
            CreateChild();
        }
        //
        if(childList != null)
        {
            for (int i = 0; i < childList.Length; ++i)
            {
                Node item = childList[i];
                if (item == null)
                {
                    break;
                }
                // 这里如果换成包围盒相交，那么就有意义了，否则无意义
                // 不可能 出现一个点在两个节点的包围盒中
                if (item.bound.Contains(obj.pos))
                {
                    if (node != null)
                    {
                        bChild = false;
                        break;
                    }
                    node = item;//当前节点
                    bChild = true;
                }
            }
        }
        
        if (bChild)
        {
            //只有一个儿子可以包含该物体，则该物体
            node.InsertObj(obj);
        }
        else //代表孩子没法处理这个对象
        {
            // 如果不止一个孩子包含它，那这个物体归父节点拥有
            objList.Add(obj);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="camera"></param>
    public void TriggerMove(Camera camera)
    {
        //刷新当前节点
        for(int i = 0; i < objList.Count; ++i)
        {
            //进入该节点中意味着该节点在摄像机内，把该节点保存的物体全部创建出来
            // 未激活的激活
            // 未实例化的实例化
            ResourcesManager.Instance.LoadAsync(objList[i]);
        }

        if(depth == 0)
        {
            // 每次刷新所有节点前，要把所有的对象的标记，清0
            ResourcesManager.Instance.RefreshStatus();
        }

        //刷新子节点
        if (childList != null)
        {
            for(int i = 0; i < childList.Length; ++i)
            {
                if (childList[i].bound.CheckBoundIsInCamera(camera))
                {
                    childList[i].TriggerMove(camera);
                }
            }
        }
    }
 
    private void CreateChild()
    {
        childList = new Node[belongTree.maxChildCount];
        int index = 0;
        for(int i = -1; i <= 1; i+=2)
        {
            for(int j = -1; j <= 1; j+=2)
            {
                Vector3 centerOffset = new Vector3(bound.size.x / 4 * i, 0, bound.size.z / 4 * j);
                Vector3 cSize = new Vector3(bound.size.x / 2, bound.size.y, bound.size.z / 2);
                Bounds cBound = new Bounds(bound.center + centerOffset, cSize);
                childList[index++] = new Node(cBound, depth + 1, belongTree);
            }
        }
    }
    
    public void DrawBound()
    {
        if(objList.Count != 0)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(bound.center, bound.size - Vector3.one*0.1f);
        }
        else
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(bound.center, bound.size - Vector3.one * 0.1f);
        }
        
        if(childList != null)
        {
            for(int i = 0; i < childList.Length; ++i)
            {
                childList[i].DrawBound();
            }
        }
        
    }
}
