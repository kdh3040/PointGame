using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RGameBlockGroup : MonoBehaviour {

    public enum R_BLOCK_GROUP_TYPE
    {
        NONE,
        BLOCKS,
        START,
        CLEAR,
    }

    public List<RGameBlock> BlockList = new List<RGameBlock>();
    public GameObject SafeBlock;
    public R_BLOCK_GROUP_TYPE Type;

    public void init(R_BLOCK_GROUP_TYPE type)
    {
        Type = type;
        SafeBlock.gameObject.SetActive(false);

        for (int index = 0; index < BlockList.Count; ++index)
        {
            BlockList[index].SetData(RGameBlock.R_BLOCK_TYPE.NONE);
        }

        if (type == R_BLOCK_GROUP_TYPE.BLOCKS)
        {
            // 안전 발판을 만들어 줘야함
            bool selfBlockEnable = false;
            while (selfBlockEnable == false)
            {
                if (selfBlockEnable)
                    break;

                int blockCount = Random.Range(1, BlockList.Count);
                int enableBlockCount = 0;

                for (int index = 0; index < BlockList.Count; ++index)
                {
                    var blockType = (RGameBlock.R_BLOCK_TYPE)Random.Range((int)RGameBlock.R_BLOCK_TYPE.SAW, (int)RGameBlock.R_BLOCK_TYPE.NONE + 1);

                    if (blockType == RGameBlock.R_BLOCK_TYPE.COIN || blockType == RGameBlock.R_BLOCK_TYPE.SAFE)
                        selfBlockEnable = true;

                    if (blockType != RGameBlock.R_BLOCK_TYPE.NONE)
                        enableBlockCount++;

                    BlockList[index].SetData(blockType);

                    if (selfBlockEnable && enableBlockCount >= blockCount)
                        break;
                }
            }
        }
        else if(type == R_BLOCK_GROUP_TYPE.CLEAR ||
            type == R_BLOCK_GROUP_TYPE.START)
        {
            SafeBlock.gameObject.SetActive(true);
        }
    }

    public Transform SetCharAttach(int index, bool enable)
    {
        ResetCharAttach();
        BlockList[index].SetCharAttach(enable);

        return BlockList[index].gameObject.transform;
    }

    public int GetCharAttachBlockindex()
    {
        for (int index = 0; index < BlockList.Count; ++index)
        {
            if (BlockList[index].CharAttach)
            {
                return index;
            }
        }

        return -1;
    }

    public void ResetCharAttach()
    {
        for (int i = 0; i < BlockList.Count; ++i)
        {
            BlockList[i].CharAttach = false;
        }
    }

    public bool IsGameOverCheck()
    {
        for (int index = 0; index < BlockList.Count; ++index)
        {
            if(BlockList[index].CharAttach)
            {
                if (BlockList[index].Type == RGameBlock.R_BLOCK_TYPE.SAW)
                    return true;
            }
        }

        return false;
    }

    public bool IsGameCoinGetCheck()
    {
        for (int index = 0; index < BlockList.Count; ++index)
        {
            if (BlockList[index].CharAttach)
            {
                if (BlockList[index].Type == RGameBlock.R_BLOCK_TYPE.COIN)
                {
                    BlockList[index].GetCoin();
                    return true;
                }
            }
        }

        return false;
    }

    public RGameBlock.R_BLOCK_TYPE GetBlockType(int index)
    {
        for (int i = 0; i < BlockList.Count; ++i)
        {
            if(i == index)
            {
                return BlockList[index].Type;
            }
        }

        return RGameBlock.R_BLOCK_TYPE.NONE;
    }
}
