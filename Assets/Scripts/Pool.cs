using System;
using UnityEngine;
using System.Collections.Generic;
using Match3Core;

// ReSharper disable once CheckNamespace
public class Pool : MonoBehaviour
{
    public GameObject blockPrefab;
    public Sprite yellowBlock;
    public Sprite blueBlock;
    public Sprite orangeBlock;
    public Sprite greenBlock;
    public Sprite redBlock;
    public Sprite purpleBlock;
    public Sprite rainbowBlock;
    public Sprite bombSprite;
    public Sprite crossSprite;
    public Transform boardTransform;
    public GameObject explosionPrefab;
    public GameObject lightningPrefab;
    public GameObject laserPrefab;
    public Transform popupParentTransform;
    public GameObject popupTextPrefab;

    public int initialBlocksAmount = 81;
    public int initialExplosionAmount = 10;
    public int initialLightningAmount = 8;
    public int initialLaserAmount = 20;
    public int initialTextPopupAmount = 50;

    private List<GameObject> pooledBlockObjects;
    private List<GameObject> pooledExplosionObjects;
    private List<GameObject> pooledLightningObjects;
    private List<GameObject> pooledLaserObjects;
    private List<GameObject> pooledTextPopupObjects;
    private Dictionary<BlockColor, Sprite> blockTypeToSprite;

    public void Awake()
    {
        blockTypeToSprite = new Dictionary<BlockColor, Sprite>
        {
            {BlockColor.Yellow, yellowBlock},
            {BlockColor.Blue, blueBlock},
            {BlockColor.Orange, orangeBlock},
            {BlockColor.Green, greenBlock},
            {BlockColor.Red, redBlock},
            {BlockColor.Purple, purpleBlock},
            {BlockColor.Rainbow, rainbowBlock}
        };

        pooledBlockObjects = new List<GameObject>();

        for (int i = 0; i < initialBlocksAmount; i++)
        {
            AddBlockObject();
        }

        pooledExplosionObjects = new List<GameObject>();

        for (int i = 0; i < initialExplosionAmount; i++)
        {
            AddExplosionObject();
        }

        pooledLightningObjects = new List<GameObject>();

        for (int i = 0; i < initialLightningAmount; i++)
        {
            AddLightningObject();
        }

        pooledLaserObjects = new List<GameObject>();

        for (int i = 0; i < initialLaserAmount; i++)
        {
            AddLaserObject();
        }

        pooledTextPopupObjects = new List<GameObject>();
        for (int i = 0; i < initialTextPopupAmount; i++)
        {
            AddTextPopupObject();
        }
    }

    private GameObject AddTextPopupObject()
    {
        var obj = (GameObject) Instantiate(popupTextPrefab);
        obj.transform.SetParent(popupParentTransform, false);
        obj.SetActive(false);
        pooledTextPopupObjects.Add(obj);
        return obj;
    }

    private GameObject AddLaserObject()
    {
        var obj = (GameObject) Instantiate(laserPrefab);
        obj.transform.SetParent(boardTransform, false);
        obj.SetActive(false);
        pooledLaserObjects.Add(obj);
        return obj;
    }

    private GameObject AddLightningObject()
    {
        var obj = (GameObject) Instantiate(lightningPrefab);
        obj.transform.SetParent(boardTransform, false);
        obj.SetActive(false);
        pooledLightningObjects.Add(obj);
        return obj;
    }

    private GameObject AddExplosionObject()
    {
        var obj = (GameObject) Instantiate(explosionPrefab);
        obj.transform.SetParent(boardTransform, false);
        obj.SetActive(false);
        pooledExplosionObjects.Add(obj);
        return obj;
    }

    private GameObject AddBlockObject()
    {
        var obj = (GameObject) Instantiate(blockPrefab);
        obj.transform.SetParent(boardTransform, false);
        obj.SetActive(false);
        pooledBlockObjects.Add(obj);
        return obj;
    }

    public GameObject GetBlock(BlockColor blockColor, BlockType blockType)
    {
        for (int i = 0; i < pooledBlockObjects.Count; i++)
        {
            if (!pooledBlockObjects[i].activeInHierarchy)
            {
                var spriteRenderers = pooledBlockObjects[i].GetComponentsInChildren<SpriteRenderer>(true);
                try
                {
                    spriteRenderers[0].sprite = blockTypeToSprite[blockColor];
                }
                catch (Exception)
                {
                    Debug.LogError("Block Color: " + blockColor);
                    throw;
                }


                Sprite modificator = null;
                switch (blockType)
                {
                    case BlockType.Bomb:
                        modificator = bombSprite;
                        break;
                    case BlockType.Cross:
                        modificator = crossSprite;
                        break;
                }
                spriteRenderers[1].sprite = modificator;

                return pooledBlockObjects[i];
            }
        }

        var obj = AddBlockObject();
        obj.GetComponent<SpriteRenderer>().sprite = blockTypeToSprite[blockColor];
        return obj;
    }

    public GameObject GetExplosion()
    {
        for (int i = 0; i < pooledExplosionObjects.Count; i++)
        {
            if (!pooledExplosionObjects[i].activeInHierarchy)
            {
                return pooledExplosionObjects[i];
            }
        }

        var obj = AddExplosionObject();
        return obj;
    }

    public GameObject GetLightning()
    {
        for (int i = 0; i < pooledLightningObjects.Count; i++)
        {
            if (!pooledLightningObjects[i].activeInHierarchy)
            {
                return pooledLightningObjects[i];
            }
        }

        var obj = AddLightningObject();
        return obj;
    }

    public GameObject GetLaser()
    {
        for (int i = 0; i < pooledLaserObjects.Count; i++)
        {
            if (!pooledLaserObjects[i].activeInHierarchy)
            {
                return pooledLaserObjects[i];
            }
        }

        var obj = AddLaserObject();
        return obj;
    }


    public GameObject GetTextPopup()
    {
        for (int i = 0; i < pooledTextPopupObjects.Count; i++)
        {
            if (!pooledTextPopupObjects[i].activeInHierarchy)
            {
                return pooledTextPopupObjects[i];
            }
        }

        var obj = AddTextPopupObject();
        return obj;
    }
}