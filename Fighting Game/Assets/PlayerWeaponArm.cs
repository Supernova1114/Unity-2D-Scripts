using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class PlayerEntity : Entity
{
    [Header("WeaponArm")]
    [SerializeField] private GameObject m_arm;
    [SerializeField] private GameObject m_hand;
    [SerializeField] private LayerMask m_pickupItem;

    private PickupItem m_currentPickupItem = null;

    private float m_shootingRotation;

    // Sets the input limtis in degrees for the directions to where the player can fire the weapon
    readonly float[] INPUT_ANGLE_LIMITS = {22.5f, 67.5f, 112.5f, 157.5f, 202.5f, 247.5f, 292.5f, 337.5f};
    // Sets the angles in degrees for where the weapon can fire
    readonly int[] FIRING_ANGLES = {0, 45, 90, 135, 180, 225, 270, 315};


    private void HandlePlayerShooting()
    {
        if (GetPlayerInput().sqrMagnitude > 0)
        {
            float shootRot = GetShootingRotation();
            if (!m_isSliding || shootRot < FIRING_ANGLES[5])
            {
                m_shootingRotation = shootRot;
            }
            else
            {
                ResetShootingRotation();
            }

        }
        else
        {
            ResetShootingRotation();
        }

        m_arm.transform.rotation = Quaternion.AngleAxis(m_shootingRotation, transform.forward);
    }


    private void ResetShootingRotation()
    {
        if (m_facingDirection >= 0)
        {
            m_shootingRotation = FIRING_ANGLES[0];
        }
        else
        {
            m_shootingRotation = FIRING_ANGLES[4];
        }
    }


    private float GetShootingRotation()
    {
        float shootingRot = 0;
        float inputRotation = Mathf.Rad2Deg * Mathf.Atan2(m_movementInput.y, m_movementInput.x);
        
        if (inputRotation < 0)
        {
            inputRotation += 360;
        }

        // converting input limits to shooting directions 
        if (inputRotation >= INPUT_ANGLE_LIMITS[6] && inputRotation <= INPUT_ANGLE_LIMITS[7])
        {
            shootingRot = FIRING_ANGLES[7]; // aim down&right
        }
        else if (inputRotation <= INPUT_ANGLE_LIMITS[0] || inputRotation >= INPUT_ANGLE_LIMITS[7])
        {
            shootingRot = FIRING_ANGLES[0]; // aim right
        }
        else if (inputRotation >= INPUT_ANGLE_LIMITS[0] && inputRotation <= INPUT_ANGLE_LIMITS[1])
        {
            shootingRot = FIRING_ANGLES[1]; // aim right&up
        }
        else if (inputRotation >= INPUT_ANGLE_LIMITS[1] && inputRotation <= INPUT_ANGLE_LIMITS[2])
        {
            shootingRot = FIRING_ANGLES[2]; // aim up
        }
        else if (inputRotation >= INPUT_ANGLE_LIMITS[2] && inputRotation <= INPUT_ANGLE_LIMITS[3])
        {
            shootingRot = FIRING_ANGLES[3]; // aim left&up
        }
        else if (inputRotation >= INPUT_ANGLE_LIMITS[3] && inputRotation <= INPUT_ANGLE_LIMITS[4])
        {
            shootingRot = FIRING_ANGLES[4]; // aim left
        }
        else if (inputRotation >= INPUT_ANGLE_LIMITS[4] && inputRotation <= INPUT_ANGLE_LIMITS[5])
        {
            shootingRot = FIRING_ANGLES[5]; // aim leftt&down
        }
        else if (inputRotation >= INPUT_ANGLE_LIMITS[5] && inputRotation <= INPUT_ANGLE_LIMITS[6])
        {
            shootingRot = FIRING_ANGLES[6]; // aim down
        }

        return shootingRot;
    }

    public override void Consume()
    {
        if (m_currentPickupItem != null)
        {
            m_currentPickupItem.Consume();
        }
    }

    private void TryPickupDropItem()
    {
        Collider2D itemCollider = Physics2D.OverlapBox(transform.position, m_collider.bounds.size, 0, m_pickupItem.value);

        DropItem();
        if (itemCollider != null)
        {
            GrabItem(itemCollider.GetComponent<PickupItem>());
        }
    }

    private void GrabItem(PickupItem item)
    {
        if (m_currentPickupItem == null)
        {
            item.Collect(this);
            item.transform.parent = m_hand.transform;
            item.transform.localRotation = Quaternion.identity;
            item.transform.localPosition = Vector3.zero;

            m_currentPickupItem = item;
        }
    }

    private void DropItem()
    {
        if (m_currentPickupItem != null)
        {
            m_currentPickupItem.transform.parent = null;
            m_currentPickupItem.Drop();
            m_currentPickupItem = null;
        }
    }

}