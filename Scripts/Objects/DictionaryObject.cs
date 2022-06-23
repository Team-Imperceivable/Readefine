using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DictionaryObject : MonoBehaviour
{
    [Header("Initialization Variables")]
    [SerializeField] public string swappable;
    

    public string sentence;
    public Definition definition;
    public ActiveKeyword keyword = ActiveKeyword.None;
    private Collider2D myCollider;
    private Rigidbody2D rb;
    private float normalGravity;
    private LayerMask normalLayer;

    // Start is called before the first frame update
    void Start()
    {
        //Creates the definition, has to be inside a method so it's here
        definition = new Definition(sentence, swappable);
        myCollider = gameObject.GetComponent<Collider2D>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        normalGravity = rb.gravityScale;
        normalLayer = gameObject.layer;
        UpdateText();
    }

    // Update is called once per frame
    void Update()
    {
        AlterProperties();
        switch (keyword)
        {
            case ActiveKeyword.None:
                break;
            case ActiveKeyword.Deadly:
                Collider2D playerCollider = GetPlayerInContactCollider();
                if (playerCollider != null)
                {
                    playerCollider.gameObject.GetComponent<PlayerController>().Kill();
                }
                break;
            case ActiveKeyword.Moveable:
                gameObject.tag = "Moveable";
                rb.constraints = RigidbodyConstraints2D.None;
                break;
            case ActiveKeyword.Floating:
                rb.gravityScale = 0f;
                Vector3 floatMove = new Vector3(0f, floatSpeed * Time.deltaTime, 0f);
                foreach(Collider2D collider in GetOnTop())
                {
                    collider.transform.position += floatMove;
                }
                break;
            case ActiveKeyword.Passable:
                ContactFilter2D filter = new ContactFilter2D
                {
                    useLayerMask = true,
                    layerMask = interactableLayers
                };
                Collider2D[] contacts = new Collider2D[2];
                if(Physics2D.OverlapBox(transform.position, myCollider.bounds.size, 0f, filter, contacts) > 1)
                {
                    gameObject.GetComponent<Renderer>().material.color = new Color(1.0f, 1.0f, 1.0f, passingOpacity);
                }
                gameObject.layer = LayerMask.NameToLayer("Passable");
                break;
            case ActiveKeyword.Controllable:
                gameObject.tag = "Controllable";
                rb.constraints = RigidbodyConstraints2D.None;
                FrameInput playerInputs = new FrameInput
                {
                    X = Input.GetAxisRaw("Horizontal"),
                    Y = Input.GetAxisRaw("Vertical"),
                    JumpDown = Input.GetButtonDown("Jump"),
                    JumpUp = Input.GetButtonUp("Jump")
                };

                RunCollisionChecks();

                CalculateWalk(playerInputs);
                CalculateJumpApex();
                CalculateGravity(); // Vertical movement

                movingObject = CheckMoveable(playerInputs);
                CalculateClimb(playerInputs);
                CalculateJump(playerInputs);

                MoveCharacter();
                Debug.Log(RawMovement);
                break;
        }
    }

    #region PROPERTIES
    [Header("Properties")]
    [SerializeField] private string[] solutions;
    [SerializeField] private Text definitionTextBox;
    /// <summary>
    /// Checks if the definition will alter the properties of the object
    /// </summary>
    /// <returns>
    /// true if the current combination of keywords can change the object's properties
    /// </returns>
    public bool Solved()
    {
        foreach(string solution in solutions)
        {
            if(solution.Equals(swappable))
            {
                return true;
            }
        }
        return false;
    }

    public void UpdateText()
    {
        definition.SetSwappable(swappable);
        definitionTextBox.text = definition.GetDefinition();
    }

    /// <summary>
    /// Swaps a word in the definition, throws an error if the target or new value aren't in the defintiion.
    /// </summary>
    /// <param name="target">
    /// Word that is getting swapped out
    /// </param>
    /// <param name="newValue">
    /// Word that is getting swapped in
    /// </param>
    /// <returns>
    /// The word that is getting swapped out
    /// </returns>
    public string SwapWord(string newWord)
    {
        swappable = newWord;
        return definition.Swap(swappable);
    }
    

    private void AlterProperties()
    {
        if(Solved())
        {

            //Template for setting the keyword
            //if (swappable.Equals(Keyword Name))
            //    keyword = ActiveKeyword.Keyword Name
            if (swappable.Equals("deadly"))
                keyword = ActiveKeyword.Deadly;
            if (swappable.Equals("moveable"))
                keyword = ActiveKeyword.Moveable;
            if (swappable.Equals("controllable"))
                keyword = ActiveKeyword.Controllable;
            if (swappable.Equals("floating"))
                keyword = ActiveKeyword.Floating;
            else
                rb.gravityScale = normalGravity;
            if (swappable.Equals("passable"))
                keyword = ActiveKeyword.Passable;
            else
                gameObject.layer = normalLayer;
        } else
        {
            gameObject.tag = "Untagged";
            keyword = ActiveKeyword.None;
            gameObject.layer = normalLayer;
            rb.constraints = RigidbodyConstraints2D.FreezePositionX;
            rb.gravityScale = normalGravity;
        }
    }
    #endregion

    private bool PlayerInContact()
    {
        List<Collider2D> contacts = new List<Collider2D>();

        myCollider.GetContacts(contacts);

        foreach(Collider2D collider in contacts)
        {
            if(collider.tag.Equals("Player"))
            {
                return true;
            }
        }
        return false;
    }
    private Collider2D GetPlayerInContactCollider()
    {
        List<Collider2D> contacts = new List<Collider2D>();

        myCollider.GetContacts(contacts);

        foreach (Collider2D collider in contacts)
        {
            if (collider.tag.Equals("Player"))
            {
                return collider;
            }
        }
        return null;
    }

    [Header("Floating")]
    [SerializeField] private float floatSpeed;
    [SerializeField] private LayerMask interactableLayers;
    private List<Collider2D> GetOnTop()
    {
        List<Collider2D> contacts = new List<Collider2D>();
        ContactFilter2D filter = new ContactFilter2D
        {
            useLayerMask = true,
            layerMask = interactableLayers
        };
        Physics2D.OverlapBox(head.position, new Vector2(onTopCheckWidth, onTopCheckHeight), 0f, filter, contacts);
        return contacts;
    }

    [Header("Passable")]
    [SerializeField] private float passingOpacity;

    #region Controllable Movement

    public bool JumpingThisFrame { get; private set; }
    public bool LandingThisFrame { get; private set; }
    public Vector3 Velocity { get; private set; }
    public Vector3 RawMovement { get; private set; }
    public bool Grounded => _colDown;
    private Vector3 _lastPosition;
    private float _currentHorizontalSpeed, _currentVerticalSpeed;
    private Collider2D movingCollider;
    private bool movingObject;

    [Header("COLLISION")][SerializeField] private Bounds _characterBounds;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private int _detectorCount = 3;
    [SerializeField] private float _detectionRayLength = 0.1f;
    [SerializeField][Range(0.1f, 0.3f)] private float _rayBuffer = 0.1f; // Prevents side detectors hitting the ground
    [SerializeField] private float moveableCheckWidth, moveableCheckHeight;
    [SerializeField] private float onTopCheckWidth, onTopCheckHeight;
    [SerializeField] private Transform rightBound, leftBound, feet, head;


    private RayRange _raysUp, _raysRight, _raysDown, _raysLeft;
    private bool _colUp, _colRight, _colDown, _colLeft;

    private float _timeLeftGrounded;

    // We use these raycast checks for pre-collision information
    private void RunCollisionChecks()
    {
        // Generate ray ranges. 
        CalculateRayRanged();

        // Ground
        LandingThisFrame = false;
        var groundedCheck = RunDetection(_raysDown, _groundLayer) || RunDetection(_raysDown, gameObject.layer);
        if (_colDown && !groundedCheck) _timeLeftGrounded = Time.time; // Only trigger when first leaving
        else if (!_colDown && groundedCheck)
        {
            _coyoteUsable = true; // Only trigger when first touching
            LandingThisFrame = true;
        }

        _colDown = groundedCheck;

        // The rest
        _colUp = RunDetection(_raysUp, _groundLayer);
        _colLeft = RunDetection(_raysLeft, _groundLayer);
        _colRight = RunDetection(_raysRight, _groundLayer);

        bool RunDetection(RayRange range, LayerMask checkLayer)
        {
            return EvaluateRayPositions(range).Any(point => Physics2D.Raycast(point, range.Dir, _detectionRayLength, checkLayer));
        }
    }

    //Check if colliding with a Moveable object
    private bool CheckMoveable(FrameInput Inputs)
    {
        if (Inputs.X > 0)
        {
            //Check Right
            movingCollider = Physics2D.OverlapBox(rightBound.position, new Vector2(moveableCheckWidth, moveableCheckHeight), 0f, gameObject.layer);
            if (movingCollider != null)
            {
                return movingCollider.tag.Equals("Moveable");
            }
        }
        if (Inputs.X < 0)
        {
            //Check Left
            movingCollider = Physics2D.OverlapBox(leftBound.position, new Vector2(moveableCheckWidth, moveableCheckHeight), 0f, gameObject.layer);
            if (movingCollider != null)
            {
                return movingCollider.tag.Equals("Moveable");
            }
        }
        return false;
    }

    private void CalculateRayRanged()
    {
        // This is crying out for some kind of refactor. 
        var b = new Bounds(transform.position + _characterBounds.center, _characterBounds.size);

        _raysDown = new RayRange(b.min.x + _rayBuffer, b.min.y, b.max.x - _rayBuffer, b.min.y, Vector2.down);
        _raysUp = new RayRange(b.min.x + _rayBuffer, b.max.y, b.max.x - _rayBuffer, b.max.y, Vector2.up);
        _raysLeft = new RayRange(b.min.x, b.min.y + _rayBuffer, b.min.x, b.max.y - _rayBuffer, Vector2.left);
        _raysRight = new RayRange(b.max.x, b.min.y + _rayBuffer, b.max.x, b.max.y - _rayBuffer, Vector2.right);
    }


    private IEnumerable<Vector2> EvaluateRayPositions(RayRange range)
    {
        for (var i = 0; i < _detectorCount; i++)
        {
            var t = (float)i / (_detectorCount - 1);
            yield return Vector2.Lerp(range.Start, range.End, t);
        }
    }

    [Header("WALKING")][SerializeField] private float _acceleration = 90;
    [SerializeField] private float _moveClamp = 13;
    [SerializeField] private float _deAcceleration = 60f;
    [SerializeField] private float _apexBonus = 2;
    [SerializeField] private float _pushSpeedModifier = 0.95f;

    private void CalculateWalk(FrameInput Inputs)
    {
        if (Inputs.X != 0)
        {
            // Set horizontal move speed
            _currentHorizontalSpeed += Inputs.X * _acceleration * Time.deltaTime;

            // clamped by max frame movement
            _currentHorizontalSpeed = Mathf.Clamp(_currentHorizontalSpeed, -_moveClamp, _moveClamp);

            // Apply bonus at the apex of a jump
            var apexBonus = Mathf.Sign(Inputs.X) * _apexBonus * _apexPoint;
            _currentHorizontalSpeed += apexBonus * Time.deltaTime;
        }
        else
        {
            // No input. Let's slow the character down
            _currentHorizontalSpeed = Mathf.MoveTowards(_currentHorizontalSpeed, 0, _deAcceleration * Time.deltaTime);
        }

        if (_currentHorizontalSpeed > 0 && _colRight || _currentHorizontalSpeed < 0 && _colLeft)
        {
            // Don't walk through walls
            _currentHorizontalSpeed = 0;
        }

        if (movingObject)
        {
            Collider2D[] results = new Collider2D[2];
            ContactFilter2D filter = new ContactFilter2D
            {
                useLayerMask = true,
                layerMask = _groundLayer
            };
            if (movingCollider.OverlapCollider(filter, results) == 2)
            {
                Vector3 relativePos = movingCollider.transform.position - myCollider.transform.position;
                if (relativePos.x > 0 && _currentHorizontalSpeed > 0 || relativePos.x < 0 && _currentHorizontalSpeed < 0)
                {
                    _currentHorizontalSpeed = 0;
                }
            }
        }

        if (movingObject && _currentHorizontalSpeed != 0)
        {
            _currentHorizontalSpeed *= _pushSpeedModifier;
        }
    }

    [Header("JUMPING")][SerializeField] private float _jumpHeight = 30;
    [SerializeField] private float _jumpApexThreshold = 10f;
    [SerializeField] private float _coyoteTimeThreshold = 0.1f;
    [SerializeField] private float _jumpBuffer = 0.1f;
    [SerializeField] private float _jumpEndEarlyGravityModifier = 3;
    private bool _coyoteUsable;
    private bool _endedJumpEarly = true;
    private float _apexPoint; // Becomes 1 at the apex of a jump
    private float _lastJumpPressed;
    private bool CanUseCoyote => _coyoteUsable && !_colDown && _timeLeftGrounded + _coyoteTimeThreshold > Time.time;
    private bool HasBufferedJump => _colDown && _lastJumpPressed + _jumpBuffer > Time.time;

    private void CalculateJumpApex()
    {
        if (!_colDown)
        {
            // Gets stronger the closer to the top of the jump
            _apexPoint = Mathf.InverseLerp(_jumpApexThreshold, 0, Mathf.Abs(Velocity.y));
            _fallSpeed = Mathf.Lerp(_minFallSpeed, _maxFallSpeed, _apexPoint);
        }
        else
        {
            _apexPoint = 0;
        }
    }

    private void CalculateJump(FrameInput Inputs)
    {
        // Jump if: grounded or within coyote threshold || sufficient jump buffer
        if (Inputs.JumpDown && !movingObject)
        {
            if (CanUseCoyote || HasBufferedJump || topOfLadder)
            {
                _currentVerticalSpeed = _jumpHeight;
                _endedJumpEarly = false;
                _coyoteUsable = false;
                _timeLeftGrounded = float.MinValue;
                JumpingThisFrame = true;
            }
        }
        else
        {
            JumpingThisFrame = false;
        }

        // End the jump early if button released
        if (!_colDown && Inputs.JumpUp && !_endedJumpEarly && Velocity.y > 0)
        {
            // _currentVerticalSpeed = 0;
            _endedJumpEarly = true;
        }

        if (_colUp)
        {
            if (_currentVerticalSpeed > 0) _currentVerticalSpeed = 0;
        }
    }

    [Header("GRAVITY")][SerializeField] private float _fallClamp = -40f;
    [SerializeField] private float _minFallSpeed = 80f;
    [SerializeField] private float _maxFallSpeed = 120f;
    private float _fallSpeed;

    private void CalculateGravity()
    {
        if (_colDown)
        {
            // Move out of the ground
            if (_currentVerticalSpeed < 0) _currentVerticalSpeed = 0;
        }
        else
        {
            // Add downward force while ascending if we ended the jump early
            var fallSpeed = _endedJumpEarly && _currentVerticalSpeed > 0 ? _fallSpeed * _jumpEndEarlyGravityModifier : _fallSpeed;

            // Fall
            _currentVerticalSpeed -= fallSpeed * Time.deltaTime;

            // Clamp
            if (_currentVerticalSpeed < _fallClamp) _currentVerticalSpeed = _fallClamp;
        }
    }

    [Header("Climbing")]
    [SerializeField] private float climbSpeed = 10f;
    public bool canClimb;
    public bool topOfLadder;

    private void CalculateClimb(FrameInput Inputs)
    {
        if (canClimb)
        {
            if (!(topOfLadder && Inputs.Y > 0f) && !(_colDown && Inputs.Y < 0f))
            {
                _currentVerticalSpeed = climbSpeed * Inputs.Y;
            }
            else
            {

            }
        }
        else
        {
            
        }
    }
    public bool FeetTouching(Bounds bounds)
    {
        return bounds.Contains(feet.position);
    }

    [Header("MOVE")]
    [SerializeField, Tooltip("Raising this value increases collision accuracy at the cost of performance.")]
    private int _freeColliderIterations = 10;

    // We cast our bounds before moving to avoid future collisions
    private void MoveCharacter()
    {
        var pos = transform.position + _characterBounds.center;
        RawMovement = new Vector3(_currentHorizontalSpeed, _currentVerticalSpeed); // Used externally
        var move = RawMovement * Time.deltaTime;
        var furthestPoint = pos + move;

        // check furthest movement. If nothing hit, move and don't do extra checks
        var hit = Physics2D.OverlapBox(furthestPoint, _characterBounds.size, 0, _groundLayer);
        if (!hit)
        {
            transform.position += move;
            return;
        }

        // otherwise increment away from current pos; see what closest position we can move to
        var positionToMoveTo = transform.position;
        for (int i = 1; i < _freeColliderIterations; i++)
        {
            // increment to check all but furthestPoint - we did that already
            var t = (float)i / _freeColliderIterations;
            var posToTry = Vector2.Lerp(pos, furthestPoint, t);

            if (Physics2D.OverlapBox(posToTry, _characterBounds.size, 0, _groundLayer))
            {
                transform.position = positionToMoveTo;

                // We've landed on a corner or hit our head on a ledge. Nudge the player gently
                if (i == 1)
                {
                    if (_currentVerticalSpeed < 0) _currentVerticalSpeed = 0;
                    var dir = transform.position - hit.transform.position;
                    transform.position += dir.normalized * move.magnitude;
                }

                return;
            }

            positionToMoveTo = posToTry;
        }
    }

    private void OnDrawGizmos()
    {
        // Bounds
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position + _characterBounds.center, _characterBounds.size);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(leftBound.position, new Vector2(moveableCheckWidth, moveableCheckHeight));
        Gizmos.DrawWireCube(rightBound.position, new Vector2(moveableCheckWidth, moveableCheckHeight));
        Gizmos.DrawWireCube(head.position, new Vector2(onTopCheckWidth, onTopCheckHeight));

        // Rays
        if (!Application.isPlaying)
        {
            CalculateRayRanged();
            Gizmos.color = Color.blue;
            foreach (var range in new List<RayRange> { _raysUp, _raysRight, _raysDown, _raysLeft })
            {
                foreach (var point in EvaluateRayPositions(range))
                {
                    Gizmos.DrawRay(point, range.Dir * _detectionRayLength);
                }
            }
        }

        if (!Application.isPlaying) return;

        // Draw the future position. Handy for visualizing gravity
        Gizmos.color = Color.red;
        var move = new Vector3(_currentHorizontalSpeed, _currentVerticalSpeed) * Time.deltaTime;
        Gizmos.DrawWireCube(transform.position + _characterBounds.center + move, _characterBounds.size);
    }
    #endregion
}

public enum ActiveKeyword
{
    None,
    Deadly,
    Moveable,
    Floating,
    Swimmable,
    Controllable,
    Passable,
    Climbable
}
