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
    private float speed = 5f;

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
        IsMoving = true;
        lastPosition = transform.position;
        distance = 0;
        canDuplicate = false;
        hasDuplicate = false;
        _blocked = false;
        _dossierIA = GameObject.FindWithTag("IA");
        _state = 0;
        speed = 0f;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Bloc")
        {
            _currentBloc = other.gameObject;
        }
        if (other.gameObject.tag == "pathfinder")
        {
            other.gameObject.GetComponent<Pathfinding1>().Blocked = true;
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
            Move(right, _forward, left);
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

    public void Move(float right, float _forward, float left)
    {
        if (!_blocked) // Si on a pas atteint un sens unique
        {

            switch (_state)
            {
                case 0:  // Croisement

                    if (_currentBloc != null)
                    {
                        if (!hasDuplicate && canDuplicate)
                        {


                            hasDuplicate = true;
                            transform.position = _currentBloc.transform.position + new Vector3(0, 1, 0);
                            speed = 0f;
                            DuplicationForward();
                            DuplicationRight();
                            DuplicateLeft();

                        }
                    }

                    break;
                case 1: // Bloqué devant mais droite et gauche libre
                    if (!hasDuplicate && canDuplicate)
                    {
                        hasDuplicate = true;
                        transform.position = _currentBloc.transform.position + new Vector3(0, 1, 0);
                        speed = 0f;
                        DuplicateLeft();
                        DuplicationRight();
                    }
                    break;
                case 2: // Rien devant et a droite

                    if (_currentBloc.name == "Bloc_Virage")
                    {
                        speed = 5f;
                    }
                    else
                    {


                        if (!hasDuplicate && canDuplicate)
                        {


                            hasDuplicate = true;
                            transform.position = _currentBloc.transform.position + new Vector3(0, 1, 0);
                            speed = 0f;
                            DuplicationForward();
                            DuplicationRight();
                        }
                    }
                    break;
                case 3:   // Virage a droite 

                    speed = 0f;
                    // transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.y + 90, transform.rotation.z, 0);

                    transform.Rotate(Vector3.up, 90f);


                    break;
                case 4: // Devant et gauche libre

                    if (_currentBloc.name == "Bloc_Virage")
                    {
                        speed = 5f;
                    }
                    else
                    {


                        if (!hasDuplicate && canDuplicate)
                        {
                            hasDuplicate = true;
                            transform.position = _currentBloc.transform.position + new Vector3(0, 1, 0);
                            speed = 0f;
                            DuplicationForward();
                            DuplicateLeft();
                        }
                    }
                    break;
                case 5: // Virage gauche
                    speed = 0f;
                    break;
                case 6:  // Rien devant 
                    speed = 5f;
                    break;
                case 7: // Bloqué
                    speed = 0f;
                    // BlockPathfinder(); Marche :)
                    break;
                default:
                    break;
            }


            // if (_forward > 2.5)
            // {
            //     if (right > 2.5 && _forward > 5)
            //     {
            //         speed = 0f;
            //         Debug.Log("Duplication droite et tout droit");

            //         if (!hasDuplicate && canDuplicate)
            //         {
            //             transform.position = _currentBloc.transform.position;

            //             transform.position = transform.position + new Vector3(0, 1, 0);
            //             hasDuplicate = true;
            //             DuplicationForward();

            //         }

            //     }
            //     else if (left > 2.5 && _forward > 5)
            //     {
            //         speed = 0f;
            //         Debug.Log("Duplication gauche et tout droit");
            //         transform.position = _currentBloc.transform.position;
            //         transform.position = transform.position + new Vector3(0, 1, 0);
            //     }
            //     else
            //     {
            //         speed = 5f;
            //     }
            // }
            // else
            // {
            //     // Mur en face
            //     if (left > 2.5)
            //     {
            //         transform.Rotate(Vector3.up, -90f);
            //         speed = 0f;
            //     }
            //     else if (right > 2.5)
            //     {
            //         transform.Rotate(Vector3.up, 90f);
            //         speed = 0f;
            //     }
            //     else
            //     {
            //         // Bloqué 
            //         speed = 0f;
            //     }

            // }
        }








        Vector3 movement = Vector3.forward * speed * Time.deltaTime;
        transform.Translate(movement);
    }

    public void DuplicationForward()
    {

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

        Debug.Log(transform.localEulerAngles.y);

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

    // public void DuplicPathfinder(int nb, float forward, float left, float right)
    // {
    //     Debug.Log("lancement duplication param : " + nb + " " + forward + " " + left + " " + right);
    //     _blocked = true;
    //     for (int i = 0; i < nb; i++)
    //     {
    //         if (left > 2)
    //         {
    //             Vector3 pos = transform.position + new Vector3(-2, 0, 0);
    //             Debug.Log("new pathfinder direction gauche");
    //             // dup
    //             // GameObject pathfinderClone = Instantiate(_prefabParent, pos, transform.rotation);
    //             // pathfinderClone.transform.Rotate(Vector3.up, -90f);

    //             // pathfinderClone.transform.parent = transform;

    //             // pathfinderClone.GetComponent<Pathfinding1>().SetParent(this.gameObject);

    //             // pathfinderClone.GetComponent<Pathfinding1>().ClearListChildren();

    //             // pathfinderClone.GetComponent<Pathfinding1>().SetFolderParent(pathfinderClone);
    //             // this.gameObject.GetComponent<Pathfinding1>().AddChildren(pathfinderClone);
    //             // left = 0;
    //         }
    //         else if (right > 2)
    //         {
    //             // dup
    //             Debug.Log("new pathfinder direction droite");
    //             Vector3 pos = transform.position + new Vector3(0, 0, 0);
    //             GameObject pathfinderClone = Instantiate(_prefabParent, pos, transform.rotation);
    //             pathfinderClone.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionZ;
    //             pathfinderClone.transform.Rotate(Vector3.up, 90f);
    //             pathfinderClone.name = transform.name + i;
    //             Debug.Log("rot" + pathfinderClone.transform.rotation);
    //             pathfinderClone.transform.parent = _dossierIA.transform;
    //             pathfinderClone.GetComponent<Pathfinding1>().SetParent(this.gameObject);
    //             pathfinderClone.GetComponent<Pathfinding1>().IsMoving = false;

    //             pathfinderClone.GetComponent<Pathfinding1>().ClearListChildren();

    //             pathfinderClone.GetComponent<Pathfinding1>().SetFolderParent(pathfinderClone);
    //             pathfinderClone.transform.position += new Vector3(3, 0, 0);

    //             right = 0;
    //         }
    //         else if (forward > 2)
    //         {
    //             Debug.Log("new pathfinder direction devant");

    //             Vector3 pos = transform.position + new Vector3(0, 0, 3);

    //             // dup
    //             GameObject pathfinderClone = Instantiate(_prefabParent, transform.position, transform.rotation);


    //             pathfinderClone.transform.parent = _dossierIA.transform;

    //             pathfinderClone.GetComponent<Pathfinding1>().SetParent(this.gameObject);

    //             pathfinderClone.GetComponent<Pathfinding1>().ClearListChildren();

    //             pathfinderClone.GetComponent<Pathfinding1>().SetFolderParent(pathfinderClone);
    //             this.gameObject.GetComponent<Pathfinding1>().AddChildren(pathfinderClone);


    //             forward = 0;
    //         }
    //     }
    // }
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
