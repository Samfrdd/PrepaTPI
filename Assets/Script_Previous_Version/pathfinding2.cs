using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pathfinding2 : MonoBehaviour
{
    [SerializeField]
    private List<Transform> _waypoints;
    [SerializeField]
    private float speed = 5f;

    [SerializeField]
    private List<GameObject> _allChildren;

    [SerializeField]
    private Transform _to;

    [SerializeField]
    private GameObject _parent;

    [SerializeField]
    private bool _blocked = false;

    [SerializeField]
    private bool _trouve = false;

    [SerializeField]
    private List<Material> _allMaterial;

    [SerializeField]
    private GameObject _prefabParent;

    private bool _isOriginal = false;

    private bool _noPathFound = false;

    private bool algoFinished = false;




    public List<Transform> Waypoints { get => _waypoints; private set => _waypoints = value; }
    public Transform To { get => _to; private set => _to = value; }
    public bool Blocked { get => _blocked; private set => _blocked = value; }
    public bool Trouve { get => _trouve; private set => _trouve = value; }
    public GameObject Parent { get => _parent; private set => _parent = value; }
    public bool IsOriginal { get => _isOriginal; set => _isOriginal = value; }

    private void Start()
    {
        _prefabParent = GameObject.FindWithTag("IA");
        GameObject.FindWithTag("gameManager").GetComponent<ManagerUI>().NbPathfinder++;
        GameObject.FindWithTag("gameManager").GetComponent<ManagerUI>().UpdateView();


    }

    void Update()
    {

        MoveTowardsWaypoint();

        if (_isOriginal && _blocked && !_noPathFound)
        {
            _noPathFound = true;
          //  GameObject.FindWithTag("gameManager").GetComponent<RandomGeneration>().NoPathFound();
        }

        if (_isOriginal && _trouve && !algoFinished)
        {
            algoFinished = true;
          //  GameObject.FindWithTag("gameManager").GetComponent<RandomGeneration>().ExitFound();
        }

    }



    void MoveTowardsWaypoint()
    {
        if (!_blocked) // Si on a pas atteint un sens unique
        {
            if (Waypoints.Count > 0)
            {
                Transform targetWaypoint = Waypoints[Waypoints.Count - 1];
                float step = speed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, step);
            }
        }
    }

    public void AddWaypoint(Transform newWaypoint)
    {
        // Ajoutez le nouveau waypoint � la liste
        _waypoints.Add(newWaypoint);
        _to = newWaypoint;


    }

    public void ClearWaypointList()
    {

        _waypoints.Clear();

    }

    public void SetParent(GameObject parent)
    {
        this.Parent = parent;
    }

    public void BlockPathfinder()
    {
        gameObject.GetComponent<TrailRenderer>().material = _allMaterial[2];
        gameObject.GetComponent<MeshRenderer>().enabled = false;


        _blocked = true;

        if (!IsOriginal)
        {
            _parent.GetComponent<pathfinding2>().CheckIfAllChildrenBlocked();
        }

    }

    public void FindExit()
    {
        _trouve = true;
        gameObject.GetComponent<TrailRenderer>().material = _allMaterial[0];
        if (Parent != null)
        {
            Parent.GetComponent<pathfinding2>().FindExit();

        }
    }

    public void AddChildren(GameObject child)
    {
        _allChildren.Add(child);
    }

    public void ClearListChildren()
    {
        _allChildren.Clear();
    }

    public void SetFolderParent(GameObject pathfinder)
    {
        pathfinder.transform.parent = _prefabParent.transform;

    }

    public void CheckIfAllChildrenBlocked()
    {
        bool oneIsBloked = true;

        for (int i = 0; i < _allChildren.Count; i++)
        {
            if (!_allChildren[i].GetComponent<pathfinding2>().Blocked)
            {
                oneIsBloked = false;
            }
        }

        if (oneIsBloked)
        {
            BlockPathfinder();
        }
    }
}
