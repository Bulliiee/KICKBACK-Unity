using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance;

    public GameObject objectPrefab; // 풀링 할 오브젝트 프리팹
    public int poolSize = 10;
    private Queue<GameObject> objectPool = new Queue<GameObject>();

    private void Awake()
    {
        Instance = this;
        InitializePool();
    }

    // 오브젝트 풀 초기화
    private void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(objectPrefab, transform.position, Quaternion.identity, transform);
            obj.SetActive(false);
            objectPool.Enqueue(obj);
        }
    }

    // 오브젝트 요청
    public GameObject GetObject()
    {
        if (objectPool.Count == 0)
        {
            // 필요하다면 풀 크기를 동적으로 증가시킬 수 있습니다.
            GameObject obj = Instantiate(objectPrefab);
            obj.SetActive(false);
            return obj;
        }

        GameObject pooledObj = objectPool.Dequeue();
        pooledObj.SetActive(true);
        return pooledObj;
    }

    // 오브젝트 반환
    public void ReturnObject(GameObject obj)
    {
        obj.transform.SetParent(transform);
        obj.SetActive(false);
        objectPool.Enqueue(obj);
    }
}