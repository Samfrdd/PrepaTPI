using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class middleBloc : MonoBehaviour
{
    [SerializeField]
    private List<Transform> _lstExit;

    private bool _alreadyPass;

    private bool canDuplicate = false;

    // ????
    public List<Transform> _lstClone;

    private GameObject _cloneParent;
       
    public bool AlreadyPass { get => _alreadyPass;private set => _alreadyPass = value; }

    private GameObject _firstPathfinder;
   

    // Start is called before the first frame update
    void Start()
    {
        _alreadyPass = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (canDuplicate)
        {
            Duplicate();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("pathfinder"))
        {
            if (!_alreadyPass)
            {
                _alreadyPass = true;
                _firstPathfinder = other.gameObject;

                if (_lstExit.Count == 2)
                {
                    if (other.gameObject.GetComponent<pathfinding2>().Waypoints.Contains(_lstExit[0].transform))
                    {
                        other.gameObject.GetComponent<pathfinding2>().AddWaypoint(_lstExit[1].transform);
                       
                    }
                    else
                    {
                       
                         other.gameObject.GetComponent<pathfinding2>().AddWaypoint(_lstExit[0].transform);
                       

                    }
                }
                else
                {
                    _alreadyPass = true;
                   

                    _cloneParent = other.gameObject;
                        canDuplicate = false;
                        // Clonner et les envoy� dans les 3 sorties
                        for (int i = 0; i < _lstExit.Count; i++)
                        {
                            if (!other.gameObject.GetComponent<pathfinding2>().Waypoints.Contains(_lstExit[i].transform))
                            {

                            _lstClone.Add(_lstExit[i]);
                               
                            }
                        }
                       canDuplicate = true;        
                }
            }
            else
            {
               
                // Si le partent n'est pas le premier a �tre venu sur le bloc
                if(other.gameObject.GetComponent<pathfinding2>().Parent != _firstPathfinder)
                {
                    other.gameObject.GetComponent<pathfinding2>().BlockPathfinder();

                }


            }
        }
    }


    private void Duplicate()
    {
        canDuplicate = false;
        for (int i = 0; i < _lstClone.Count; i++)
        {
            GameObject pathfinderClone = Instantiate(_cloneParent, _cloneParent.transform.position, _cloneParent.transform.rotation);

           // pathfinderClone.transform.parent = _cloneParent.transform;

            pathfinderClone.GetComponent<pathfinding2>().AddWaypoint(_lstClone[i]);

            pathfinderClone.GetComponent<pathfinding2>().SetParent(_cloneParent);

            pathfinderClone.GetComponent<pathfinding2>().ClearListChildren();

            pathfinderClone.GetComponent<pathfinding2>().SetFolderParent(pathfinderClone);
            _cloneParent.GetComponent<pathfinding2>().AddChildren(pathfinderClone);

        }


    }
}
