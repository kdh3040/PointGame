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

    public enum R_BLOCK_STEP
    {
        TWO,
        THREE,
    }

    private List<RGameBlock> BlockList = new List<RGameBlock>();
    public List<RGameBlock> BlockList_2_Step = new List<RGameBlock>();
    public List<RGameBlock> BlockList_3_Step = new List<RGameBlock>();
    public GameObject SafeBlock;
    public R_BLOCK_GROUP_TYPE Type;
    public R_BLOCK_STEP StepType = R_BLOCK_STEP.TWO;

    public void init(R_BLOCK_GROUP_TYPE type)
    {
        for (int i = 0; i < BlockList_2_Step.Count; i++)
        {
            BlockList_2_Step[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < BlockList_3_Step.Count; i++)
        {
            BlockList_3_Step[i].gameObject.SetActive(false);
        }

        BlockList.Clear();
        if (GamePlayManager.Instance.StageCount <= 5)
        {
            StepType = R_BLOCK_STEP.TWO;
            BlockList.AddRange(BlockList_2_Step);
        }
        else
        {
            StepType = R_BLOCK_STEP.THREE;
            BlockList.AddRange(BlockList_3_Step);
        }

        Type = type;
        SafeBlock.gameObject.SetActive(false);

        List<KeyValuePair<RGameBlock.R_BLOCK_TYPE, int>> blockTypePercent = new List<KeyValuePair<RGameBlock.R_BLOCK_TYPE, int>> ();
        blockTypePercent.Add(new KeyValuePair<RGameBlock.R_BLOCK_TYPE, int>(RGameBlock.R_BLOCK_TYPE.SAW, 40));
        blockTypePercent.Add(new KeyValuePair<RGameBlock.R_BLOCK_TYPE, int>(RGameBlock.R_BLOCK_TYPE.COIN, 60));
        blockTypePercent.Add(new KeyValuePair<RGameBlock.R_BLOCK_TYPE, int>(RGameBlock.R_BLOCK_TYPE.SAFE, 80));
        blockTypePercent.Add(new KeyValuePair<RGameBlock.R_BLOCK_TYPE, int>(RGameBlock.R_BLOCK_TYPE.NONE, 100));

        for (int index = 0; index < BlockList.Count; ++index)
        {
            BlockList[index].gameObject.SetActive(true);
            BlockList[index].SetData(RGameBlock.R_BLOCK_TYPE.NONE, StepType);
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

                int blockCheckReverse = Random.Range(0, 2);

                if (blockCheckReverse == 0)
                {
                    for (int index = 0; index < BlockList.Count; ++index)
                    {
                        var blockTypePercentValue = Random.Range(0, 101);
                        var blockType = RGameBlock.R_BLOCK_TYPE.NONE;

                        for (int i = 0; i < blockTypePercent.Count; ++i)
                        {
                            if (i == 0)
                            {
                                if (blockTypePercent[i].Value >= blockTypePercentValue)
                                {
                                    blockType = blockTypePercent[i].Key;
                                    break;
                                }

                            }
                            else if (blockTypePercent[i - 1].Value < blockTypePercentValue &&
                                blockTypePercent[i].Value >= blockTypePercentValue)
                            {
                                blockType = blockTypePercent[i].Key;
                                break;
                            }
                        }




                        if (blockType == RGameBlock.R_BLOCK_TYPE.COIN || blockType == RGameBlock.R_BLOCK_TYPE.SAFE)
                            selfBlockEnable = true;

                        if (blockType != RGameBlock.R_BLOCK_TYPE.NONE)
                            enableBlockCount++;

                        BlockList[index].SetData(blockType, StepType);

                        if (selfBlockEnable && enableBlockCount >= blockCount)
                            break;
                    }
                }
                else
                {
                    for (int index = BlockList.Count - 1; index >= 0; --index)
                    {
                        var blockTypePercentValue = Random.Range(0, 101);
                        var blockType = RGameBlock.R_BLOCK_TYPE.NONE;

                        for (int i = 0; i < blockTypePercent.Count; ++i)
                        {
                            if (i == 0)
                            {
                                if (blockTypePercent[i].Value >= blockTypePercentValue)
                                {
                                    blockType = blockTypePercent[i].Key;
                                    break;
                                }

                            }
                            else if (blockTypePercent[i - 1].Value < blockTypePercentValue &&
                                blockTypePercent[i].Value >= blockTypePercentValue)
                            {
                                blockType = blockTypePercent[i].Key;
                                break;
                            }
                        }


                        if (blockType == RGameBlock.R_BLOCK_TYPE.COIN || blockType == RGameBlock.R_BLOCK_TYPE.SAFE)
                            selfBlockEnable = true;

                        if (blockType != RGameBlock.R_BLOCK_TYPE.NONE)
                            enableBlockCount++;

                        BlockList[index].SetData(blockType, StepType);

                        if (selfBlockEnable && enableBlockCount >= blockCount)
                            break;
                    }
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
