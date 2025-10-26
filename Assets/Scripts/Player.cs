using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.XInput;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
        // VARIAVEIS ESTÁTICAS
        public static event Action<int, string> OnAmmoChanged;
        //public static event Action<Collider2D, PolygonCollider2D, bool> PlayerPassed;
        //public static event Action<Collider2D, PolygonCollider2D> PlayerWantedPass;
        public static event Action<int, Transform, KeyCode, KeyCode> PlayerInteracted;
        public static bool isFlipped, playerStats;
        public static string ammo;

        // CAMPOS VÍSIVEIS NO INSPECTOR
        [Header("Defina o Player")]
        public Jogador jogador; // cria o seletor
        public enum Jogador // cria o dropdown
        {
                PlayerOne,
                PlayerTwo
        }

        [Header("Especificações")]
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private int _speed, _ammoCapacity;
        [SerializeField] private float jumpForce;
        [SerializeField] private GameObject _shootSound;
        [SerializeField] private GameObject _rechargeSound;
        [SerializeField] private GameObject _rechargeFullSound;


        [Header("Defina as Teclas")]
        public Commands commands; // cria o seletor
        public enum Commands // cria o dropdown
        {
                Keyboard,
                Controller
        }

        [SerializeField] private float _deadZone;
        private KeyCode _jumpKey;
        private KeyCode _shootKey;
        private KeyCode _reloadKey;
        private KeyCode _interactKey;

        //[SerializeField] private KeyCode _attackKey;

        // CAMPOS PRIVADOS;
        private int _ammo, _id, _gamepad;
        private float _axisX;
        private string _animName, _input;
        private bool _isGrounded, _inInteractionArea, _isWalking,
            _isReloading, _isFlipped,
            _playerOne, _isAiming;

        private Rigidbody2D _rb2d;
        private Transform _gunL, _gunR, _armaUsada, _heavygun;
        private Animator _animator;
        private GameObject _spawn;

        static public KeyCode shootK, interactionK;

        void Start() {
                _isAiming = false;
                _ammo = _ammoCapacity;

                _playerOne = (this.jogador == 0);
                if (_playerOne) {
                        Spawn("playerSpawn");
                        _id = 0;
                } else {
                        Spawn("playerSpawn2");
                        _id = 1;
                }

                _animator = GetComponent<Animator>();
                _rb2d = GetComponent<Rigidbody2D>();

                _gunL = GetComponentInChildren<Transform>().Find("gunL");
                _gunR = GetComponentInChildren<Transform>().Find("gunR");

                _armaUsada = _gunR;

                DefineCommands();

                shootK = _shootKey;
                interactionK = _interactKey;
        }

        private void OnEnable() {
                ControllerDetector.OnInputChange += SetController;
        }

        private void OnDisable() {
                ControllerDetector.OnInputChange -= SetController;
        }

        void SetController(int id, bool controller) {
                if (id == _id + 1 ) {
                        if (controller) {
                                this.commands = (Commands)1;
                        } else {
                                this.commands = (Commands)0;
                        }
                }
        
                DefineCommands();
        }

        void DefineCommands() {
                if (_id == 0) {
                        if (this.commands == 0) {
                                _jumpKey = KeyCode.W;
                                _shootKey = KeyCode.LeftShift;
                                _reloadKey = KeyCode.LeftControl;
                                _interactKey = KeyCode.E;
                                _input = "Horizontal-P1B";
                        } else {
                                if (Gamepad.current is DualSenseGamepadHID) {
                                        _jumpKey = KeyCode.Joystick1Button1;
                                        _shootKey = KeyCode.Joystick1Button0;
                                        _reloadKey = KeyCode.Joystick1Button2;
                                        _interactKey = KeyCode.Joystick1Button3;
                                } else {
                                        _jumpKey = KeyCode.Joystick1Button0;
                                        _shootKey = KeyCode.Joystick1Button2;
                                        _reloadKey = KeyCode.Joystick1Button1;
                                        _interactKey = KeyCode.Joystick1Button3;
                                }                            
                                _input = "Horizontal-P1";
                        }
                } else {
                        if (this.commands == 0) {
                                _jumpKey = KeyCode.UpArrow;
                                _shootKey = KeyCode.RightShift;
                                _reloadKey = KeyCode.RightControl;
                                _interactKey = KeyCode.PageDown;
                                _input = "Horizontal-P2B";
                        } else {
                                if (Gamepad.current is DualSenseGamepadHID) {
                                        _jumpKey = KeyCode.Joystick2Button1;
                                        _shootKey = KeyCode.Joystick2Button0;
                                        _reloadKey = KeyCode.Joystick2Button2;
                                        _interactKey = KeyCode.Joystick2Button3;
                                } else {
                                        _jumpKey = KeyCode.Joystick2Button0;
                                        _shootKey = KeyCode.Joystick2Button2;
                                        _reloadKey = KeyCode.Joystick2Button1;
                                        _interactKey = KeyCode.Joystick2Button3;
                                }
                                _input = "Horizontal-P2";
                        }
                }

                Debug.Log("controles definidos!");
        }

        void Update() {
                if (!_isAiming) {
                        SetAmmo();
                        Shoot();
                } else {
                        _animator.SetBool("isWalking", false);
                }

                Interacted();
        }

        void FixedUpdate() {
                if (!_isAiming) {
                        Jump();
                        SetMoviment();
                } else {
                        _animator.SetBool("isWalking", false);
                }
        }

        private void Spawn(string spawn) {
                _spawn = GameObject.FindGameObjectWithTag(spawn);
                gameObject.transform.position = _spawn.transform.position;
        }

        // CHAMA EVENTOS
        private void CallAmmoChange() {
                ammo = (_ammo + "/" + _ammoCapacity);
                OnAmmoChanged?.Invoke(_id, ammo);
        }

        private void CallInteract() {
                PlayerInteracted?.Invoke(_id, _heavygun, _shootKey, _reloadKey);
        }

        // METODOS

        void Moviment(float axis) {
                _rb2d.velocity = new Vector2(axis * _speed, _rb2d.velocity.y); // em Y ele está recebendo a velocidade atribuida (ou seja, gravidade);_isWalking = (_rb2d.velocity.x != 0f); 
                _isWalking = (_rb2d.velocity.x != 0);
                if (_isWalking) {
                        SetFlip(axis);
                }
                _armaUsada = _isFlipped ? _gunL.transform : _gunR.transform;
        }

        void Jump() {
                if (Input.GetKey(_jumpKey) && _isGrounded) { // qualquer variavel bool é entendida como TRUE dentro de verificações, exceto se for !bool
                        Vector2 _jump = new Vector2(0f, jumpForce);
                        _rb2d.AddForce(_jump, ForceMode2D.Impulse);
                }
        }

        void Interacted() {
                if (_inInteractionArea && Input.GetKeyDown(_interactKey)) {
                        _isAiming = !_isAiming;
                        playerStats = _isAiming;
                        if (_isAiming) {
                                CallInteract();
                        }
                }
        }

        void Shoot() {
                if ((_ammo > 0) && Input.GetKeyDown(_shootKey) && !_isReloading) {
                        var _som = Instantiate(_shootSound, gameObject.transform.position, Quaternion.identity);
                        Destroy(_som, 2f);
                        SetShoot(_armaUsada, _isFlipped);
                        --_ammo;
                        CallAmmoChange();
                } else if (Input.GetKeyDown(_reloadKey)) {
                        StartCoroutine(Reload());
                }
        }

        // SETs
        void SetAmmo() {
                ammo = (_ammo + "/" + _ammoCapacity);
        }

        void SetFlip(float a) {
                _isFlipped = a < 0;
                _animator.SetBool("isFlipped", _isFlipped);
        }

        void SetMoviment() {
                _axisX = Input.GetAxis(_input);

                Debug.Log(_axisX);

                if (Mathf.Abs(_axisX) > _deadZone) {
                        Moviment(_axisX);
                } else {
                        Moviment(0);
                }

                _animator.SetBool("isWalking", _isWalking);
                _animator.SetBool("isIdle", !_isWalking);

                //Debug.LogWarning("player está: \n " +
                //    "idle? = " + _animator.GetBool("isIdle") +
                //    " || andando? = " + _animator.GetBool("isWalking") +
                //    " || virado? " + _animator.GetBool("isFlipped"));
        }

        void SetShoot(Transform _pos, bool _flip) {
                if (!_flip) {
                        _animName = "Shooting";
                        StartCoroutine(Anim("isShooting", _animName));
                        Instantiate(bulletPrefab, _pos.transform.position, transform.rotation);
                } else {
                        _animName = "Shooting e";
                        StartCoroutine(Anim("isShooting", _animName));
                        Instantiate(bulletPrefab, _pos.transform.position, new Quaternion(0, -180, 0, 0));
                }
        }

        void Knife() {
                //if (!_anim.isPlaying && Input.GetKeyUp(_attackKey)) {
                //    _anim.Play("attack");
                //}
        }

        // RECARGA
        IEnumerator Reload() {
                _isReloading = true;

                for (int i = _ammo; i <= _ammoCapacity; i++) {
                        _ammo = i;
                        int lastNum = _ammo;
                        yield return new WaitForSeconds(0.5f);
                        CallAmmoChange();
                        var _som = Instantiate(_rechargeSound, gameObject.transform.position, Quaternion.identity);
                        Destroy(_som, 1f);
                        //Debug.Log(_ammo);
                }

                var _som2 = Instantiate(_rechargeFullSound, gameObject.transform.position, Quaternion.identity);
                Destroy(_som2, 1f);
                //yield return new WaitForSeconds(2f);
                //_ammo = _ammoCapacity;
                //CallAmmoChange();

                _isReloading = false;

                yield break;
        }

        // ANIMAÇÃO
        IEnumerator Anim(string boolName, string animName) {
                _animator.SetBool(boolName, true);

                yield return null;

                while (_animator.GetCurrentAnimatorStateInfo(0).IsName(animName) &&
                    _animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f) {
                        yield return null;
                }

                _animator.SetBool(boolName, false);

                yield break;
        }

        // COLISSÕES
        private void OnCollisionStay2D(Collision2D other) {
                if (other.gameObject.CompareTag("ground")) {
                        _isGrounded = true;
                }

                //if (other.gameObject.CompareTag("sensor")){
                //    if (Input.GetAxis(_downKey) < 0) {
                //        _collider = other.gameObject.GetComponent<PolygonCollider2D>();
                //        PlayerWantedPass?.Invoke(_self, _collider);
                //    } 
                //}
        }

        private void OnCollisionExit2D(Collision2D other) {
                if (other.gameObject.CompareTag("ground")) {
                        _isGrounded = false;
                }
        }

        private void OnCollisionEnter2D(Collision2D other) {
                if (other.gameObject.CompareTag("enemy")) {
                        Destroy(gameObject);
                }
        }

        //private void OnTriggerEnter2D(Collider2D other)
        //{
        //    if (other.gameObject.CompareTag("sensor")) {
        //        _collider = other.gameObject.GetComponent<PolygonCollider2D>();
        //        _isRight = (_axisX > 0);
        //        PlayerPassed?.Invoke(_self, _collider, !_isRight);
        //    }
        //}

        private void OnTriggerStay2D(Collider2D other) {
                if (other.gameObject.CompareTag("arma") || other.gameObject.CompareTag("arma2")) {
                        _inInteractionArea = true;
                        _heavygun = other.GetComponentInChildren<Transform>().Find("pos");
                }
        }

        private void OnTriggerExit2D(Collider2D other) {
                if (other.gameObject.CompareTag("arma") || other.gameObject.CompareTag("arma2")) {
                        _inInteractionArea = false;
                }
        }

}



