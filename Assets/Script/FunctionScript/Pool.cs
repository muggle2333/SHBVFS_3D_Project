using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Singleton<T> where T : class
{
    private static T instance;
    public static T Instance
    {
        get 
        { 
            if(instance == null)
            {
                instance = (T)System.Activator.CreateInstance(typeof(T),true);
            }
            return instance;
        }
    }
}
public class Pool : Singleton<Pool>
{
    private Dictionary<string, List<GameObject>> dic;
    private Pool()
    {
        dic = new Dictionary<string, List<GameObject>>();
    }

    public GameObject GetObj(string name)
    {
        GameObject obj = null;
        //if the pool has a object
        if(dic.ContainsKey(name) && dic[name].Count >0)
        {
            //take the first object
            obj = dic[name][0];
            //remove the object in the list
            dic[name].RemoveAt(0);
            //foreach(var item in dic[name])
            //{
            //    if (item.activeSelf == false) continue;
            //    obj = item;
            //    dic[name].Remove(item);
            //    break;
            //}
            //obj = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/" + name));
            //SetObj(name, obj);
        }
        //if there is no object
        else
        {
            //instantiate the object
            obj = GameObject.Instantiate(Resources.Load<GameObject>(name));
            //Put it to the pool
            //SetObj(name, obj);
            
        }
        //Reset the name
        obj.name = name;
        obj.SetActive(true);
        return obj;
    }
    public void SetObj(string name, GameObject obj)
    {
        if(obj.GetComponent<Rigidbody>())
        {
            obj.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
        obj.SetActive(false);
        //if there is a list
        if(dic.ContainsKey(name))
        {
            //add object to the list
            dic[name].Add(obj);
        }
        else
        {
            //add a new list which contains obj
            dic.Add(name, new List<GameObject>(){ obj});
        }
    }
    //Clean the pool before restart
    public void Clear()
    {
        dic.Clear();
    }
}
