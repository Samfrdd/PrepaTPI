using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Pathfinding1 : MonoBehaviour
{

    [SerializeField]
    private float f;
    [SerializeField]
    private float r;
    [SerializeField]
    private float l;

    [SerializeField]
    private float _state = 0;
    [SerializeField]
    private float distance = 0;

    [SerializeField]
    private bool _blocked = false;

    [SerializeField]
    private float currentSpeed = 5f;

    [SerializeField]
    private bool isMoving = false;

    [SerializeField]
    private List<GameObject> _allChildren;

    [SerializeField]
    private GameObject _parent;
    [SerializeField]
    private GameObject _currentBloc;

    [SerializeField]
    private bool _trouve = false;

    [SerializeField]
    private List<Material> _allMaterial;

    [SerializeField]
    private GameObject _prefabParent;

    [SerializeField]
    private GameObject _dossierIA;

    [SerializeField]
    private bool _isOriginal = false;

    [SerializeField]
    private bool _noPathFound = false;

    private bool algoFinished = false;

    [SerializeField]
    private bool hasDuplicate = false;

    [SerializeField]
    private bool canDuplicate = false;



    private Vector3 lastPosition; // Position du GameObject lors de la dernière frame
    [SerializeField]
    private RayCastScript scriptLayerFrontal;
    [SerializeField]
    private RayCastScript scriptLayerLeft;
    [SerializeField]
    private RayCastScript scriptLayerRight;

    public bool Blocked { get => _blocked; private set => _blocked = value; }
    public bool Trouve { get => _trouve; private set => _trouve = value; }
    public GameObject Parent { get => _parent; private set => _parent = value; }
    public bool IsOriginal { get => _isOriginal; set => _isOriginal = value; }
    public bool IsMoving { get => isMoving; set => isMoving = value; }

    private void Start()
    {
        IsOriginal = false;
        IsMoving = true;
        lastPosition = transform.position;
        distance = 0;
        canDuplicate = false;
        hasDuplicate = false;
        _blocked = false;
        _dossierIA = GameObject.FindWithTag("IA");
        _state = 0;
        currentSpeed = 0f;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Bloc")
        {
            _currentBloc = other.gameObject;
        }
        if (other.gameObject.tag == "pathfinder")
        {
            if (other.gameObject.GetComponent<Pathfinding1>().currentSpeed == 0f)
            {
                canDuplicate = false;
                gameObject.GetComponent<Pathfinding1>().Blocked = true;
            }
        }
    }
    void FixedUpdate()
    {
        float right = scriptLayerRight.Distance;
        float _forward = scriptLayerFrontal.Distance;
        float left = scriptLayerLeft.Distance;

        f = _forward;
        l = left;
        r = right;

        // Afficher la distance parcourue
        //  Debug.Log("Distance parcourue : " + distance);

        CalculterState();
        CalculateDistance();
        CheckIfFinish();

        if (IsMoving)
        {
            Move();
        }

        if (distance > 4)
        {
            canDuplicate = true;
        }
        else
        {
            canDuplicate = false;
        }



    }

    public void CalculterState()
    {
        _state = f + l + r;
    }

    public void CalculateDistance()
    {
        float distanceFrame = Vector3.Distance(transform.position, lastPosition);

        // Mise à jour de la distance totale parcourue
        distance += distanceFrame;

        // Mettre à jour la position précédente pour la prochaine frame
        lastPosition = transform.position;
    }
    public void CheckIfFinish()
    {
        if (_isOriginal && _blocked && !_noPathFound)
        {
            _noPathFound = true;
            // GameObject.FindWithTag("gameManager").GetComponent<RandomGeneration>().NoPathFound();
        }

        if (_isOriginal && _trouve && !algoFinished)
        {
            algoFinished = true;
            // GameObject.FindWithTag("gameManager").GetComponent<RandomGeneration>().ExitFound();
        }
    }

    public void Move()
    {
        if (!_blocked) // Si on a pas atteint un sens unique
        {

            switch (_state)
            {
                case 0:  // Croisement

                    if (_currentBloc != null)
                    {
                        if (!hasDuplicate && canDuplicate && CheckIfFirstPathfinder())
                        {
                            transform.position = _currentBloc.transform.position + new Vector3(0, 1, 0);
                            StopMovement();
                            DuplicationForward();
                            DuplicationRight();
                            DuplicateLeft();
                        }
                    }

                    break;
                case 1: // Bloqué devant mais droite et gauche libre
                    if (!hasDuplicate && canDuplicate && CheckIfFirstPathfinder())
                    {
                        transform.position = _currentBloc.transform.position + new Vector3(0, 1, 0);
                        StopMovement();
                        DuplicateLeft();
                        DuplicationRight();
                    }
                    break;
                case 2: // Rien devant et a droite
                    if (!hasDuplicate && canDuplicate && CheckIfFirstPathfinder())
                    {

                        transform.position = _currentBloc.transform.position + new Vector3(0, 1, 0);
                        StopMovement();

                        DuplicationForward();
                        DuplicationRight();
                    }

                    break;
                case 3:   // Virage a droite 
                          // transform.Rotate(Vector3.up, 90f);
                          // currentSpeed = 5f;
                    if (!hasDuplicate && canDuplicate && CheckIfFirstPathfinder())
                    {
                        StopMovement();
                        DuplicationRight();
                    }
                    break;
                case 4: // Devant et gauche libre

                    if (!hasDuplicate && canDuplicate && CheckIfFirstPathfinder())
                    {

                        transform.position = _currentBloc.transform.position + new Vector3(0, 1, 0);
                        StopMovement();
                        DuplicationForward();
                        DuplicateLeft();
                    }

                    break;
                case 5: // Virage gauche
                    // currentSpeed = 5f;
                    // transform.Rotate(Vector3.up, -90f);
                    if (!hasDuplicate && canDuplicate && CheckIfFirstPathfinder())
                    {
                        StopMovement();
                        DuplicateLeft();
                    }
                    break;
                case 6:  // Rien devant 
                    currentSpeed = 5f;
                    break;
                case 7: // Bloqué

                    transform.position = _currentBloc.transform.position + new Vector3(0, 1, 0);
                    // BlockPathfinder();
                    // IsMoving = false;
                    break;
                default:
                    break;
            }
        }
        else
        {
            StopMovement();
            canDuplicate = false;
        }

        Vector3 movement = Vector3.forward * currentSpeed * Time.deltaTime;
        transform.Translate(movement);
    }

    public void StopMovement()
    {
        currentSpeed = 0f;
        isMoving = false;

    }

    public bool CheckIfFirstPathfinder()
    {
        if (_currentBloc.GetComponent<CheckAlreadyPass>().Pathfinder == this.gameObject)
        {
            return true;
        }
        else
        {
            BlockPathfinder();
            return false;
        }
    }
    public void DuplicationForward()
    {
        hasDuplicate = true;
        Debug.Log(transform.localEulerAngles.y);

        Vector3 newPos = transform.position;

        if (transform.localEulerAngles.y == 0)
        {
            newPos = transform.position + new Vector3(0, 0, 3f);

        }
        else if (transform.localEulerAngles.y == 90)
        {
            newPos = transform.position + new Vector3(3f, 0, 0f);

        }
        else if (transform.localEulerAngles.y == 180)
        {
            newPos = transform.position + new Vector3(0f, 0, -3f);
        }
        else if (transform.localEulerAngles.y == 270)
        {
            newPos = transform.position + new Vector3(-3f, 0, 0f);
        }

        GameObject pathfinderClone = Instantiate(_prefabParent, newPos, transform.rotation);

        pathfinderClone.GetComponent<Pathfinding1>().SetParent(this.gameObject);
        pathfinderClone.GetComponent<Pathfinding1>().ClearListChildren();
        pathfinderClone.GetComponent<Pathfinding1>().SetFolderParent(pathfinderClone);
        this.gameObject.GetComponent<Pathfinding1>().AddChildren(pathfinderClone);

    }

    public void DuplicationRight()
    {
        hasDuplicate = true;

        Debug.Log(transform.localEulerAngles.y);

        Vector3 newPos = transform.position;


        if (transform.localEulerAngles.y == 0)
        {
            newPos = transform.position + new Vector3(3f, 0, 0f);
        }
        else if (transform.localEulerAngles.y == 90)
        {
            newPos = transform.position + new Vector3(0, 0, -3f);

        }
        else if (transform.localEulerAngles.y == 180)
        {
            newPos = transform.position + new Vector3(-3f, 0, 0f);
        }
        else if (transform.localEulerAngles.y == 270)
        {
            newPos = transform.position + new Vector3(0f, 0, 3f);
        }

        GameObject pathfinderClone = Instantiate(_prefabParent, newPos, transform.rotation);

        pathfinderClone.GetComponent<Pathfinding1>().SetParent(this.gameObject);
        pathfinderClone.GetComponent<Pathfinding1>().ClearListChildren();
        pathfinderClone.GetComponent<Pathfinding1>().SetFolderParent(pathfinderClone);
        this.gameObject.GetComponent<Pathfinding1>().AddChildren(pathfinderClone);

        pathfinderClone.transform.Rotate(Vector3.up, 90f);

    }

    public void DuplicateLeft()
    {
        hasDuplicate = true;

        Vector3 newPos = transform.position;

        if (transform.localEulerAngles.y == 0)
        {
            newPos = transform.position + new Vector3(-3f, 0, 0f);
        }
        else if (transform.localEulerAngles.y == 90)
        {
            newPos = transform.position + new Vector3(0, 0, 3f);
        }
        else if (transform.localEulerAngles.y == 180)
        {
            newPos = transform.position + new Vector3(3f, 0, 0f);
        }
        else if (transform.localEulerAngles.y == 270)
        {
            newPos = transform.position + new Vector3(0f, 0, -3f);
        }

        GameObject pathfinderClone = Instantiate(_prefabParent, newPos, transform.rotation);

        pathfinderClone.GetComponent<Pathfinding1>().SetParent(this.gameObject);
        pathfinderClone.GetComponent<Pathfinding1>().ClearListChildren();
        pathfinderClone.GetComponent<Pathfinding1>().SetFolderParent(pathfinderClone);
        this.gameObject.GetComponent<Pathfinding1>().AddChildren(pathfinderClone);

        pathfinderClone.transform.Rotate(Vector3.up, -90f);

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
            _parent.GetComponent<Pathfinding1>().CheckIfAllChildrenBlocked();
        }

    }

    public void FindExit()
    {
        _trouve = true;
        gameObject.GetComponent<TrailRenderer>().material = _allMaterial[0];
        if (Parent != null)
        {
            Parent.GetComponent<Pathfinding1>().FindExit();

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
        pathfinder.transform.parent = _dossierIA.transform;

    }

    public void CheckIfAllChildrenBlocked()
    {
        bool oneIsBloked = true;

        for (int i = 0; i < _allChildren.Count; i++)
        {
            if (!_allChildren[i].GetComponent<Pathfinding1>().Blocked)
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
