using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Pathfinding1 : MonoBehaviour
{

    [SerializeField]
    private float speed = 5f;

    [SerializeField]
    private List<GameObject> _allChildren;

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

    [SerializeField]
    private bool hasDuplicate = false;
    [SerializeField]
    private bool isMoving = false;
    [SerializeField]
    private bool canDuplicate = false;
    [SerializeField]
    private float distance = 0;
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

    private void Start()
    {
        isMoving = true;
        lastPosition = transform.position;
        distance = 0;
        canDuplicate = false;
        hasDuplicate = false;
        _blocked = false;
    }

    void FixedUpdate()
    {
        float distanceFrame = Vector3.Distance(transform.position, lastPosition);

        // Mise à jour de la distance totale parcourue
        distance += distanceFrame;

        // Mettre à jour la position précédente pour la prochaine frame
        lastPosition = transform.position;

        // Afficher la distance parcourue
        //  Debug.Log("Distance parcourue : " + distance);

        if (isMoving)
        {
            Move();
        }

        if (distance > 3)
        {
            canDuplicate = true;
        }

        if (_isOriginal && _blocked && !_noPathFound)
        {
            _noPathFound = true;
            GameObject.FindWithTag("gameManager").GetComponent<RandomGeneration>().NoPathFound();
        }

        if (_isOriginal && _trouve && !algoFinished)
        {
            algoFinished = true;
            GameObject.FindWithTag("gameManager").GetComponent<RandomGeneration>().ExitFound();
        }

    }



    void Move()
    {
        if (!_blocked || !hasDuplicate) // Si on a pas atteint un sens unique
        {
            float right = scriptLayerRight.Distance;
            float forward = scriptLayerFrontal.Distance;
            float left = scriptLayerLeft.Distance;

            // Debug.Log("left : " + left);
            // Debug.Log("right : " + right);
            // Debug.Log("forward : " + forward);


            if (left > 1.9 && left < 2.1 && right > 1.9 && right < 2.1 && forward > 2)
            {
                // Tout droit
                speed = 5f;

            }
            else
            {
                //Tout droit good mais un coté libre
                if (left > 2 && right < 1.9 && right > 2.1 && forward > 4)
                {

                    isMoving = false;

                    Debug.Log("bloc T gauche");
                    speed = 0f;
                    transform.position += new Vector3(0, 0, 2);
                    if (canDuplicate)
                    {
                        DuplicPathfinder(2, forward, left, right);

                    }


                }
                else if (right > 2.1 && left > 1.9 && left < 2.1 && forward > 4)
                {
                    Debug.Log("bloc T droite");
                    speed = 0f;
                    transform.position += new Vector3(0, 0, 2);

                    if (canDuplicate)
                    {
                        DuplicPathfinder(2, forward, left, right);
                    }

                }
                else
                {
                    speed = 0;
                    Debug.Log("Bloqué devant");
                }

            }





            Vector3 movement = transform.forward * speed * Time.deltaTime;
            transform.Translate(movement);
        }
    }

    public void DuplicPathfinder(int nb, float forward, float left, float right)
    {
        Debug.Log("lancement duplication param : " + nb + " " + forward + " " + left + " " + right);
        _blocked = true;
        hasDuplicate = true;
        for (int i = 0; i < nb; i++)
        {
            if (left > 2)
            {
                Vector3 pos = transform.position + new Vector3(-2, 0, 0);
                Debug.Log("new pathfinder direction gauche");
                // dup
                // GameObject pathfinderClone = Instantiate(_prefabParent, pos, transform.rotation);
                // pathfinderClone.transform.Rotate(Vector3.up, -90f);

                // pathfinderClone.transform.parent = transform;

                // pathfinderClone.GetComponent<Pathfinding1>().SetParent(this.gameObject);

                // pathfinderClone.GetComponent<Pathfinding1>().ClearListChildren();

                // pathfinderClone.GetComponent<Pathfinding1>().SetFolderParent(pathfinderClone);
                // this.gameObject.GetComponent<Pathfinding1>().AddChildren(pathfinderClone);
                // left = 0;
            }
            else if (right > 2)
            {
                // dup
                Debug.Log("new pathfinder direction droite");

                Vector3 pos = transform.position + new Vector3(0, 0, 0);

                GameObject pathfinderClone = Instantiate(_prefabParent, transform.position, transform.rotation);
                pathfinderClone.transform.Rotate(Vector3.up, 90f);
                 pathfinderClone.transform.position += new Vector3(2, 0, -2);

                pathfinderClone.GetComponent<Pathfinding1>().SetParent(this.gameObject);

                pathfinderClone.GetComponent<Pathfinding1>().ClearListChildren();

                pathfinderClone.GetComponent<Pathfinding1>().SetFolderParent(pathfinderClone);
                this.gameObject.GetComponent<Pathfinding1>().AddChildren(pathfinderClone);

                right = 0;
            }
            else if (forward > 2)
            {
                Debug.Log("new pathfinder direction devant");

                Vector3 pos = transform.position + new Vector3(0, 0, 3);

                // dup
                GameObject pathfinderClone = Instantiate(_prefabParent, transform.position, transform.rotation);



                pathfinderClone.GetComponent<Pathfinding1>().SetParent(this.gameObject);

                pathfinderClone.GetComponent<Pathfinding1>().ClearListChildren();

                pathfinderClone.GetComponent<Pathfinding1>().SetFolderParent(pathfinderClone);
                this.gameObject.GetComponent<Pathfinding1>().AddChildren(pathfinderClone);

                forward = 0;
            }
        }
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
        pathfinder.transform.parent = _prefabParent.transform;

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
