using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Hey!
/// Tarodev here. I built this controller as there was a severe lack of quality & free 2D controllers out there.
/// Right now it only contains movement and jumping, but it should be pretty easy to expand... I may even do it myself
/// if there's enough interest. You can play and compete for best times here: https://tarodev.itch.io/
/// If you hve any questions or would like to brag about your score, come to discord: https://discord.gg/GqeHHnhHpz
/// </summary>
public class PlayerController : MonoBehaviour, IPlayerController {
    // Public for external hooks
    public Vector3 Velocity { get; private set; }
    public FrameInput Inputs { get; private set; }
    public bool JumpingThisFrame { get; private set; }
    public bool LandingThisFrame { get; private set; }
    public Vector3 RawMovement { get; private set; }
    public bool Grounded => _colDown;
    public Transform visuals;
    public PlayerAnimator animator;

    private Vector3 _lastPosition;
    private float _currentHorizontalSpeed, _currentVerticalSpeed;

    private bool facingRight;
    private bool movingObject;
    private Collider2D movingCollider;
    private Collider2D myCollider;

    // This is horrible, but for some reason colliders are not fully established when update starts...
    private bool _active;
    void Awake() => Invoke(nameof(Activate), 0.5f);
    void Activate() =>  _active = true;
        
    private void Update() {
        if (Input.GetButtonDown("Cancel"))
            Application.Quit();
        if(!_active) return;
        // Calculate velocity
        Velocity = (transform.position - _lastPosition) / Time.deltaTime;
        _lastPosition = transform.position;

        GatherInputs();
        RunCollisionChecks();

        CalculateWalk(); // Horizontal movement
        CalculateJumpApex(); // Affects fall speed, so calculate before gravity
        CalculateGravity(); // Vertical movement

        movingObject = CheckMoveable();
        CalculateClimb();
        CalculateJump(); // Possibly overrides 
        CalculateSwimming();

        HandleDirections();
        HandleAnimations();


        MoveCharacter(); // Actually perform the axis movement
        if(insideGround())
        {
            if(_colDown)
            {
                moveUp();
            } else if(_colUp)
            {
                moveDown();
            }
        }

        CheckInteract();
        HandleAudio();
    }

    private void HandleDirections()
    {
        if (_currentHorizontalSpeed > 0 && facingRight)
        {
            Flip();
        }

        if (_currentHorizontalSpeed < 0 && !facingRight)
        {
            Flip();
        }
    }

    private void HandleAnimations()
    {
        if (movingObject)
        {
            animator.state = AnimationState.Push;
            animator.SetSpeed(_currentHorizontalSpeed);
        } else if(swimming)
        {
            animator.state = AnimationState.Swim;
            animator.SetSpeed(_currentVerticalSpeed / 4);
        }
        else if(climbing && !topOfLadder)
        {
            animator.state = AnimationState.Climb;
            animator.SetSpeed(_currentVerticalSpeed / 4);
        }
        else if (_currentHorizontalSpeed != 0f)
        {
            animator.state = AnimationState.Walk;
            animator.SetSpeed(_currentHorizontalSpeed);
        }
        else
        {
            animator.state = AnimationState.Idle;
        }
    }

    private void Start()
    {
        spellbook = new DictionarySpellbook(startingWord);
        objectLayer = LayerMask.GetMask("Object");
        myCollider = gameObject.GetComponent<Collider2D>();
        _defaultMinFallSpeed = _minFallSpeed;
    }

    private void Flip()
    {
        Vector3 currentScale = visuals.localScale;
        currentScale.x *= -1;
        visuals.localScale = currentScale;

        facingRight = !facingRight;
    }

    #region Gather Inputs

    private void GatherInputs() {
        Inputs = new FrameInput {
            JumpDown = Input.GetButtonDown("Jump"),
            JumpUp = Input.GetButtonUp("Jump"),
            X = Input.GetAxisRaw("Horizontal"),
            Y = Input.GetAxisRaw("Vertical"),
            Interact = Input.GetButtonDown("Fire1"),
            Reset = Input.GetKeyDown(KeyCode.R)
        };
        if (Inputs.JumpDown) {
            _lastJumpPressed = Time.time;
        }
    }

    #endregion

    #region Collisions

    [Header("COLLISION")] [SerializeField] private Bounds _characterBounds;
    [SerializeField] private LayerMask _groundLayer, platformLayer;
    [SerializeField] private int _detectorCount = 3;
    [SerializeField] private float _detectionRayLength = 0.1f;
    [SerializeField] [Range(0.1f, 0.3f)] private float _rayBuffer = 0.1f; // Prevents side detectors hitting the ground
    [SerializeField] private float moveableCheckWidth, moveableCheckHeight;
    [SerializeField] private Transform rightBound, leftBound, feet;
    [SerializeField] private float antiClipYAmount;

    private RayRange _raysUp, _raysRight, _raysDown, _raysLeft;
    private bool _colUp, _colRight, _colDown, _colLeft;

    private float _timeLeftGrounded;

    // We use these raycast checks for pre-collision information
    private void RunCollisionChecks() {
        // Generate ray ranges. 
        CalculateRayRanged();

        // Ground
        LandingThisFrame = false;
        var groundedCheck = RunDetection(_raysDown, _groundLayer) || RunDetection(_raysDown, platformLayer);
        if (_colDown && !groundedCheck) _timeLeftGrounded = Time.time; // Only trigger when first leaving
        else if (!_colDown && groundedCheck) {
            _coyoteUsable = true; // Only trigger when first touching
            LandingThisFrame = true;
        }

        _colDown = groundedCheck;

        // The rest
        _colUp = RunDetection(_raysUp, _groundLayer) ;
        _colLeft = RunDetection(_raysLeft, _groundLayer);
        _colRight = RunDetection(_raysRight, _groundLayer);
        
        bool RunDetection(RayRange range, LayerMask checkLayer) {
            return EvaluateRayPositions(range).Any(point => Physics2D.Raycast(point, range.Dir, _detectionRayLength, checkLayer));
        }
    }

    //Check if colliding with a Moveable object
    private bool CheckMoveable()
    {
        if(Inputs.X > 0)
        {
            //Check Right
            movingCollider = Physics2D.OverlapBox(rightBound.position, new Vector2(moveableCheckWidth, moveableCheckHeight), 0f, objectLayer);
            if (movingCollider != null)
            {
                return movingCollider.tag.Equals("Moveable");
            }
        }
        if(Inputs.X < 0)
        {
            //Check Left
            movingCollider = Physics2D.OverlapBox(leftBound.position, new Vector2(moveableCheckWidth, moveableCheckHeight), 0f, objectLayer);
            if (movingCollider != null)
            {
                return movingCollider.tag.Equals("Moveable");
            }
        }
        return false;
    }

    private void CalculateRayRanged() {
        // This is crying out for some kind of refactor. 
        var b = new Bounds(transform.position + _characterBounds.center, _characterBounds.size);

        _raysDown = new RayRange(b.min.x + _rayBuffer, b.min.y, b.max.x - _rayBuffer, b.min.y, Vector2.down);
        _raysUp = new RayRange(b.min.x + _rayBuffer, b.max.y, b.max.x - _rayBuffer, b.max.y, Vector2.up);
        _raysLeft = new RayRange(b.min.x, b.min.y + _rayBuffer, b.min.x, b.max.y - _rayBuffer, Vector2.left);
        _raysRight = new RayRange(b.max.x, b.min.y + _rayBuffer, b.max.x, b.max.y - _rayBuffer, Vector2.right);
    }


    private IEnumerable<Vector2> EvaluateRayPositions(RayRange range) {
        for (var i = 0; i < _detectorCount; i++) {
            var t = (float)i / (_detectorCount - 1);
            yield return Vector2.Lerp(range.Start, range.End, t);
        }
    }

    private bool insideGround()
    {
        Collider2D contact = Physics2D.OverlapBox(transform.position + _characterBounds.center, _characterBounds.size, 0f, _groundLayer);
        return contact != null;
    }

    private void moveUp()
    {
        Vector3 upVector = new Vector3(0f, antiClipYAmount, 0f);
        transform.position += upVector;
    }

    private void moveDown()
    {
        Vector3 downVector = new Vector3(0f, -antiClipYAmount, 0f);
        transform.position += downVector;
    }

    private void OnDrawGizmos() {
        // Bounds
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position + _characterBounds.center, _characterBounds.size);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(leftBound.position, new Vector2(moveableCheckWidth, moveableCheckHeight));
        Gizmos.DrawWireCube(rightBound.position, new Vector2(moveableCheckWidth, moveableCheckHeight));
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + checkBounds.center, checkBounds.size);

        // Rays
        if (!Application.isPlaying) {
            CalculateRayRanged();
            Gizmos.color = Color.blue;
            foreach (var range in new List<RayRange> { _raysUp, _raysRight, _raysDown, _raysLeft }) {
                foreach (var point in EvaluateRayPositions(range)) {
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


    #region Walk

    [Header("WALKING")] [SerializeField] private float _acceleration = 90;
    [SerializeField] private float _moveClamp = 13;
    [SerializeField] private float _deAcceleration = 60f;
    [SerializeField] private float _apexBonus = 2;
    [SerializeField] private float _pushSpeedModifier = 0.95f;

    private void CalculateWalk() {
        if (Inputs.X != 0) {
            // Set horizontal move speed
            _currentHorizontalSpeed += Inputs.X * _acceleration * Time.deltaTime;

            // clamped by max frame movement
            _currentHorizontalSpeed = Mathf.Clamp(_currentHorizontalSpeed, -_moveClamp, _moveClamp);

            // Apply bonus at the apex of a jump
            var apexBonus = Mathf.Sign(Inputs.X) * _apexBonus * _apexPoint;
            _currentHorizontalSpeed += apexBonus * Time.deltaTime;
        }
        else {
            // No input. Let's slow the character down
            _currentHorizontalSpeed = Mathf.MoveTowards(_currentHorizontalSpeed, 0, _deAcceleration * Time.deltaTime);
        }

        if (_currentHorizontalSpeed > 0 && _colRight || _currentHorizontalSpeed < 0 && _colLeft) {
            // Don't walk through walls
            _currentHorizontalSpeed = 0;
        }

        if(movingObject)
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

        if(movingObject && _currentHorizontalSpeed != 0)
        {
            _currentHorizontalSpeed *= _pushSpeedModifier;
        }
    }

    #endregion

    #region Gravity

    [Header("GRAVITY")] [SerializeField] private float _fallClamp = -40f;
    [SerializeField] private float _minFallSpeed = 80f;
    [SerializeField] private float _maxFallSpeed = 120f;
    private float _defaultMinFallSpeed;
    private float _fallSpeed;

    private void CalculateGravity() {
        if (_colDown) {
            // Move out of the ground
            if (_currentVerticalSpeed < 0) _currentVerticalSpeed = 0;
        }
        else {
            if (swimming)
                _minFallSpeed = 0f;
            else
                _minFallSpeed = _defaultMinFallSpeed;

            // Add downward force while ascending if we ended the jump early
            var fallSpeed = _endedJumpEarly && _currentVerticalSpeed > 0 ? _fallSpeed * _jumpEndEarlyGravityModifier : _fallSpeed;

            // Fall
            _currentVerticalSpeed -= fallSpeed * Time.deltaTime;

            // Clamp
            if (_currentVerticalSpeed < _fallClamp ) _currentVerticalSpeed = _fallClamp;
        }
    }

    #endregion

    #region Jump

    [Header("JUMPING")] [SerializeField] private float _jumpHeight = 30;
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

    private void CalculateJumpApex() {
        if (!_colDown) {
            // Gets stronger the closer to the top of the jump
            _apexPoint = Mathf.InverseLerp(_jumpApexThreshold, 0, Mathf.Abs(Velocity.y));
            _fallSpeed = Mathf.Lerp(_minFallSpeed, _maxFallSpeed, _apexPoint);
        }
        else {
            _apexPoint = 0;
        }
    }

    private void CalculateJump() {
        // Jump if: grounded or within coyote threshold || sufficient jump buffer
        if (Inputs.JumpDown && !movingObject)
        {
            if(CanUseCoyote || HasBufferedJump || topOfLadder || swimming)
            {
                _currentVerticalSpeed = _jumpHeight;
                _endedJumpEarly = false;
                _coyoteUsable = false;
                _timeLeftGrounded = float.MinValue;
                JumpingThisFrame = true;
            }
        }
        else {
            JumpingThisFrame = false;
        }

        // End the jump early if button released
        if (!_colDown && Inputs.JumpUp && !_endedJumpEarly && Velocity.y > 0) {
            // _currentVerticalSpeed = 0;
            _endedJumpEarly = true;
        }

        if (_colUp) {
            if (_currentVerticalSpeed > 0) _currentVerticalSpeed = 0;
        }
    }

    #endregion

    #region Climb
    [Header("CLIMBING")]
    [SerializeField] private float climbSpeed;
    [SerializeField] private LayerMask climbableLayer;
    public bool canClimb;
    private bool topOfLadder;
    private bool climbing;

    private void CalculateClimb()
    {
        if(canClimb)
        {
            Collider2D hitCollider = Physics2D.OverlapBox(transform.position, myCollider.bounds.size, 0f, climbableLayer);
            if (hitCollider != null)
            {
                DictionaryObject dictObj = hitCollider.gameObject.GetComponent<DictionaryObject>();
                if (dictObj.swappable.Equals("climbable"))
                {
                    topOfLadder = FeetTouching(dictObj.onTopBounds);
                    if (!(topOfLadder && Inputs.Y > 0f) && !(_colDown && Inputs.Y < 0f))
                    {
                        _currentVerticalSpeed = climbSpeed * Inputs.Y;
                        climbing = true;
                    }
                    else
                    {
                        climbing = false;
                    }
                }
                else
                {
                    climbing = false;
                    topOfLadder = false;
                }
            }
            else
            {
                topOfLadder = false;
                climbing = false;
            }
        }
    }
    public bool FeetTouching(Bounds bounds)
    {
        return bounds.Contains(feet.position);
    }
    #endregion

    #region Swimming
    [Header("SWIMMING")]
    [SerializeField] private float swimmingVerticalModifier;
    [SerializeField] private LayerMask waterLayer;
    public bool swimming;
    private void CalculateSwimming()
    {
        if (swimming)
        {
            
            if(Physics2D.OverlapPoint(transform.position, waterLayer) != null)
            {
                _currentVerticalSpeed *= swimmingVerticalModifier;
            } else
            {
                swimming = false;
            }
            
        }
    }
    #endregion

    #region Move

    [Header("MOVE")] [SerializeField, Tooltip("Raising this value increases collision accuracy at the cost of performance.")]
    private int _freeColliderIterations = 10;

    // We cast our bounds before moving to avoid future collisions
    private void MoveCharacter() {
        var pos = transform.position + _characterBounds.center;
        RawMovement = new Vector3(_currentHorizontalSpeed, _currentVerticalSpeed); // Used externally
        var move = RawMovement * Time.deltaTime;
        var furthestPoint = pos + move;

        // check furthest movement. If nothing hit, move and don't do extra checks
        var hit = Physics2D.OverlapBox(furthestPoint, _characterBounds.size, 0, _groundLayer);
        if (!hit) {
            transform.position += move;
            return;
        }

        // otherwise increment away from current pos; see what closest position we can move to
        var positionToMoveTo = transform.position;
        for (int i = 1; i < _freeColliderIterations; i++) {
            // increment to check all but furthestPoint - we did that already
            var t = (float)i / _freeColliderIterations;
            var posToTry = Vector2.Lerp(pos, furthestPoint, t);

            if (Physics2D.OverlapBox(posToTry, _characterBounds.size, 0, _groundLayer)) {
                transform.position = positionToMoveTo;

                // We've landed on a corner or hit our head on a ledge. Nudge the player gently
                if (i == 1) {
                    if (_currentVerticalSpeed < 0) _currentVerticalSpeed = 0;
                    var dir = transform.position - hit.transform.position;
                    //transform.position += dir.normalized * move.magnitude;
                }

                return;
            }

            positionToMoveTo = posToTry;
        }
    }

    #endregion

    #region Interact
    [Header("INTERACT")]
    [SerializeField] private LayerMask interactableLayers;
    [SerializeField] private float maxDistance;
    private LayerMask objectLayer;
    private Canvas spellbookWindow => gameObject.GetComponentInChildren(typeof(Canvas), true) as Canvas;
    private Canvas definitionWindow;
    private Collider2D mostRecentCollider;

    private void CheckInteract()
    {
        if (Inputs.Reset)
            Kill();

        if(mostRecentCollider != null && definitionWindow != null && definitionWindow.enabled)
        {
            if(Vector3.Distance(mostRecentCollider.ClosestPoint(transform.position), myCollider.ClosestPoint(mostRecentCollider.transform.position)) > maxDistance)
            {
                definitionWindow.enabled = false;
            }
        }

        if (Inputs.Interact)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Collider2D hitCollider = Physics2D.OverlapPoint(mousePos, interactableLayers);

            if(hitCollider != null)
            {
                if (Vector3.Distance(hitCollider.ClosestPoint(transform.position), myCollider.ClosestPoint(hitCollider.transform.position)) > maxDistance)
                    return;

                //Open Spellbook
                if (hitCollider.gameObject.tag.Equals("Player"))
                {
                    if (spellbookWindow.enabled)
                    {
                        spellbookWindow.enabled = false;
                    }
                    else
                    {
                        //Update window and enable it
                        Text spellbookText = gameObject.GetComponentInChildren(typeof(Text), true) as Text;
                        spellbookText.text = spellbook.GetWord();
                        spellbookWindow.enabled = true;
                    }
                    return;
                }

                GameObject hitObject = hitCollider.gameObject;
                mostRecentCollider = hitCollider;
                //Open object definition
                if(definitionWindow == null)
                {
                    //If no window stored, store it and enable it
                    definitionWindow = hitObject.GetComponentInChildren(typeof(Canvas), true) as Canvas;
                    definitionWindow.enabled = true;
                    DictionaryObject dictObj = hitObject.GetComponent(typeof(DictionaryObject)) as DictionaryObject;
                    dictObj.UpdateText();
                }
                else
                {
                    //Disable stored window and open new one
                    Canvas newWindow = hitObject.GetComponentInChildren(typeof(Canvas), true) as Canvas;
                    if(newWindow != definitionWindow)
                    {
                        definitionWindow.enabled = false;
                        newWindow.enabled = true;
                        definitionWindow = newWindow;
                        DictionaryObject dictObj = hitObject.GetComponent(typeof(DictionaryObject)) as DictionaryObject;
                        dictObj.UpdateText();
                    } else
                    {
                        definitionWindow.enabled = !definitionWindow.enabled;
                    }
                }
            }
        }
    }
    #endregion

    #region Spellbook
    [SerializeField] private string startingWord;
    public DictionarySpellbook spellbook;
    
    public void UpdateSpellbook()
    {
        Text spellbookText = gameObject.GetComponentInChildren(typeof(Text)) as Text;
        spellbookText.text = spellbook.GetWord();
    }
    #endregion

    #region Respawn
    [Header("RESPAWN")]
    [SerializeField] private Respawn respawnScript;

    public void Kill()
    {
        respawnScript.Reset();
    }
    #endregion

    #region Audio
    [Header("AUDIO")]
    [SerializeField] private PlayerSFX SFX;
    [SerializeField] private LayerMask SFXLayers;
    [SerializeField] private Bounds checkBounds;

    private string GetUnderneath()
    {
        Collider2D underneath = Physics2D.OverlapBox(transform.position + checkBounds.center, checkBounds.size, 0f, SFXLayers);
        if (underneath != null)
        {
            if (LayerMask.LayerToName(underneath.gameObject.layer).Equals("Ground"))
                return "Ground";
            if (LayerMask.LayerToName(underneath.gameObject.layer).Equals("Water"))
                return "Water";
            if (LayerMask.LayerToName(underneath.gameObject.layer).Equals("Platform"))
                return "Platform";
            if (LayerMask.LayerToName(underneath.gameObject.layer).Equals("Object"))
                return "Object";
        }
        return "None";
    }
    private void HandleAudio()
    {
        if(_colDown && _currentHorizontalSpeed != 0)
            SFX.PlayAudio(GetUnderneath());
    }
    #endregion
}