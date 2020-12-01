﻿using Lean.Touch;
using UnityEngine;

//TODO: at critical number of lines cut into multiple .cs files

/**
 * abstract class HookableObject is parent class of all GameObject which can be hooked by projectile
 *
 *  */
public abstract class HookableObject : MonoBehaviour
{
    protected GameObject Parent;


    /**
     * on Start() set layer to PlayingFieldLayer
     */
    protected virtual void Start()
    {
        SetLayerToPlayingFieldLayer();
    }

    /**
     * Calls for action at controller on collision with
     *
     * @param Collider other
     */
    public abstract void OnTriggerEnter2D(Collider2D other);

    /**
         * change collision layer to PlayingFieldLayer
         */
    public void SetLayerToPlayingFieldLayer()
    {
        gameObject.layer = LayerMask.NameToLayer("PlayingFieldLayer");
    }

    /**
         * change collision layer to DraggableLayer
         */
    public void SetLayerToDraggableLayer()
    {
        gameObject.layer = LayerMask.NameToLayer("DraggableLayer");
    }

    /**
     * sets transform parent so movement is joined
     */
    public void SetTransformParent(Transform parentTransform)
    {
        transform.parent = parentTransform;
    }

    /**
     * SetParent sets the parent of this gameobject
     *
     * @param GameObject parent: parent to set
     */
    public void SetParent(GameObject parent)
    {
        Parent = parent;
    }

    /**
     * set new parent and return old parent
     *
     * @returns parent before changing
     * @param newParent is the parent given to SetParent
     */
    public GameObject ChangeParent(GameObject newParent)
    {
        var oldParent = Parent;
        SetParent(newParent);
        return oldParent;
    }

    /**
     * SetPosition sets the position of the hookable object
     *
     * @param position The position to set the hookable object to
     */
    public void SetPosition(Vector3 position)
    {
        gameObject.transform.position = position;
    }

    /**
     * DestroyHookableObject destroys the hookable object
     */
    public void DestroyHookableObject()
    {
        Destroy(gameObject);
    }

    /**
     * OnWoundIn is called when the Harpoon is wound in
     */
    public abstract void OnWoundIn(Inventory inventory);
}

/**
 * class Stone inherited from HookableObject
 *
 * OnStart set to layer Playing field
 */
public class Stone : HookableObject
{
    private bool _draggable;

    // Parent that changes when the stone collides with a CanHoldHookableObject
    private CanHoldHookableObject _onDeselectParent;

    /**
     * gets called by controller when WoundIn event is triggered. calls StoneToInventory
     *
     * @param inventory where stone is put to
     */
    public override void OnWoundIn(Inventory inventory)
    {
        HookableObjectController.StoneToInventory(this, inventory);
    }


    /**
     * Calls for action at controller on collision with
     *
     * @param Collider other
     */
    public override void OnTriggerEnter2D(Collider2D other)
    {
        HookableObjectController.OnHookableObjectCollision(this, other.gameObject);
    }

    /**
     * OnTriggerExit2D is called when the Collider other has stopped touching the trigger.
     *
     * @param Collider2D other
     */
    private void OnTriggerExit2D(Collider2D other)
    {
        _onDeselectParent = null;
    }

    /**
     * SetOnDeselectParent sets the parent of the current frame
     *
     * @param canHoldHookableObject the parent to set
     */
    public void SetOnDeselectParent(CanHoldHookableObject canHoldHookableObject)
    {
        _onDeselectParent = canHoldHookableObject;
    }

    /**
     * sets property _draggable
     *
     * @param bool state: bool _draggable is set to
     */
    public void SetDraggable(bool state)
    {
        _draggable = state;
        if (state)
        {
            MakeDraggable();
        }
    }

    /**
     * gets called by lean event, when releasing stone from drag and puts it back to parent position or first sets the new parent
     */
    public void OnDeselectOnUp()
    {
        if (_onDeselectParent != null && _onDeselectParent != Parent.GetComponent<CanHoldHookableObject>())
        {
            HookableObjectController.RemoveStoneFromCanHoldHookableObject(this, Parent);
            HookableObjectController.OnDeselectOnCanHoldHookableObject(this, _onDeselectParent);
        }

        transform.position =
            HookableObjectController.GetParentPositionOfChildStone(Parent.GetComponent<CanHoldHookableObject>(), this);
    }

    /**
     * MakeDraggable adds the Lean Touch Scripts and functionality to the stone to make it draggable
     */
    private void MakeDraggable()
    {
        //set selectable
        gameObject.AddComponent<LeanSelectable>();
        //move on drag
        gameObject.AddComponent<LeanDragTranslate>();
        //set deselectable
        var leanSelectable = gameObject.GetComponent<LeanSelectable>();
        leanSelectable.DeselectOnUp = true;
        leanSelectable.IsolateSelectingFingers = true;
        leanSelectable.OnDeselect.AddListener(OnDeselectOnUp);
    }

    /**
     * adds or enables Lean Touch Scripts for Rotating and Scaling Stone with two fingers
     */
    public void MakeScalableAndRotatable()
    {
        var scalable = gameObject.GetComponent<LeanPinchBoundedScale>();
        var rotatable = gameObject.GetComponent<LeanTwistRotate>();
        if (scalable == null) scalable = gameObject.AddComponent<LeanPinchBoundedScale>();
        if (rotatable == null) rotatable = gameObject.AddComponent<LeanTwistRotate>();
        scalable.enabled = true;
        rotatable.enabled = true;
    }
    
    /**
     * Disables Lean Touch Scripts for Rotating and Scaling Stone with two fingers
     */
    public void DisableScalableAndRotatable()
    {
        var scalable = gameObject.GetComponent<LeanPinchBoundedScale>();
        var rotatable = gameObject.GetComponent<LeanTwistRotate>();
        if (scalable != null) scalable.enabled = false; 
        if (rotatable != null) rotatable.enabled = false;
    }
}

/**
 * class Item inherited from HookableObject
 *
 * future: will hold some 
 */
public class Item : HookableObject
{
    /**
     *  gets called by controller on wound in event
     *
     * @param inventory belonging to the player base
     */
    public override void OnWoundIn(Inventory inventory)
    {
        throw new System.NotImplementedException();
    }

    /**
     * Calls for action at controller on collision with
     *
     * @param Collider other
     */
    public override void OnTriggerEnter2D(Collider2D other)
    {
        HookableObjectController.OnHookableObjectCollision(this, other.gameObject);
    }
}