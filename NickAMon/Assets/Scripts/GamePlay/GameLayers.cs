using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLayers : MonoBehaviour
{
    [SerializeField] private LayerMask solidObjectsLayer;
    [SerializeField] private LayerMask grassLayer;
    [SerializeField] private LayerMask ledgeLayer;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask fieldOfViewLayer;
    [SerializeField] private LayerMask portalLayer;

    public static GameLayers Instance { get; set; }

    private void Awake()
    {
        Instance = this;
    }

    public LayerMask SolidObjectsLayer
    {
        get => solidObjectsLayer;
    }

    public LayerMask GrassLayer
    {
        get => grassLayer;
    }
    public LayerMask InteractableLayer
    {
        get => interactableLayer;
    }
    public LayerMask LedgeLayer
    {
        get => ledgeLayer;
    }
    public LayerMask PlayerLayer
    {
        get => playerLayer;
    }
    public LayerMask FieldOfViewLayer
    {
        get => fieldOfViewLayer;
    }
    public LayerMask PortalLayer
    {
        get => portalLayer;
    }
    public LayerMask TriggerableLayers
    {
        get => grassLayer | fieldOfViewLayer | portalLayer | ledgeLayer;
    }

}
