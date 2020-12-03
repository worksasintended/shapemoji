﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreArea : CanHoldHookableObject
{
    private List<Stone> _stones;

    private BoxCollider2D _collider;
    // Start is called before the first frame update
    void Start()
    {
        _stones = new List<Stone>();
        _collider = gameObject.GetComponent<BoxCollider2D>();
    }

    public override bool StoneToCanHoldHookableObject(Stone stone)
    {
        HookableObjectController.StoneToScoreArea(stone, this);

        return true;
    }

    public override bool IsStoneInCanHoldHookableObject(Stone stone)
    {
        return _stones.Contains(stone);
    }

    public void AddStone(Stone stone)
    {
        _stones.Add(stone);
        //lock n-1th stone
        if (_stones.Count > 1)
        {
            HookableObjectController.DisableStoneDraggable(_stones[_stones.Count - 2]);
        }
    }

    public override Vector3 GetPositionOfStoneChild(Stone stone)
    {
        Vector3 currentPosition = HookableObjectController.GetPositionOfHookableObject(stone);
        if (_stones.Contains(stone) && !_collider.bounds.Contains(currentPosition)) // bound only works with rectengular colliders 
        {
            return gameObject.transform.position;
        }
        return currentPosition;
    }

    public override void RemoveStone(Stone stone)
    {
        _stones.Remove(stone);
        // reenable draggable on last stone
        var stoneCount = _stones.Count;
        if (stoneCount > 1)
        {
            HookableObjectController.EnableStoneDraggable(_stones[stoneCount-1]);

        }
    }
    
}
