using Sirenix.OdinInspector;
using UnityEngine;
using EventChannel;
using System.Collections;

namespace Managers
{
    public class TimeManager : MonoBehaviour
    {
        [Title("Invoked Events")]
        [SerializeField] private IntEventSO _timeChangedEvent;
        [SerializeField] private VoidUniTaskEventSO _dayTimeoutEvent;

        [Title("Listening to Events")]
        [SerializeField] private GameStateDataEventSO _loadGameDataEvent;
        //[SerializeField] private VoidEventSO _startGameEvent;
        [SerializeField] private VoidIntRequestEventSO _changeDaySO;
        [SerializeField] private VoidUniTaskEventSO _respawnNextDayEvent;

        // format : {00:00} (hours, minutes)
        // total number of in game minutes

        /// <summary>
        /// Total number of in game minutes that have passed.
        /// Conversion: 5 seconds realTime = 5 minutes in game
        /// 
        /// </summary>
        [ShowInInspector] public int CurrentTime {  get; private set; }

        private int _dayStartTime = 60;
        [ShowInInspector] private int _dayEndTime = 1240;
        private bool _isRunning = false;

        #region Singleton
        // singleton reference
        private static TimeManager _instance;
        public static TimeManager Instance { get { return _instance; } }

        private void Awake()
        {
            // singleton pattern
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
            }
        }
        #endregion


        private void OnEnable()
        {
            // if not loading from save file
            CurrentTime = _dayStartTime;
            _loadGameDataEvent.OnInvokeEvent += OnLoadGameData;
            //_startGameEvent.OnInvokeEvent += StartTime;
            _changeDaySO.OnResultEvent += EndDay;
            _respawnNextDayEvent.OnStartEvent += EndDay;
        }
        private void OnDisable()
        {
            _loadGameDataEvent.OnInvokeEvent -= OnLoadGameData;
            //_startGameEvent.OnInvokeEvent -= StartTime;
            _changeDaySO.OnResultEvent -= EndDay;
            _respawnNextDayEvent.OnStartEvent -= EndDay;
        }
        private void Start()
        {
            StartTime();
        }

        #region Load Data
        private void OnLoadGameData(SaveData.GameStateData data)
        {
            CurrentTime = data.CurrentGameTime;
        }
        #endregion

        #region Utility
        public static string GetTimeString(int time)
        {
            int hours = time / 60;
            int minutes = time % 60;
            return string.Format("{0:00}:{1:00}", hours, minutes);
        }
        #endregion

        // called when time first starts in game
        private void StartTime()
        {
            // broadcast starting time (could be different based on save data)
            _timeChangedEvent.InvokeEvent(CurrentTime);

            StartCoroutine(GameTimeCoroutine());
        }

        private void EndDay()
        {
            // reset currrent time
            CurrentTime = _dayStartTime;

            // start the coroutine again if it is stopped
            if (!_isRunning) { StartCoroutine(GameTimeCoroutine()); }

            _timeChangedEvent.InvokeEvent(CurrentTime);
        }

        // called when a day ends
        private void EndDay(int num)
        {
            // reset currrent time
            CurrentTime = _dayStartTime;
            
            // start the coroutine again if it is stopped
            if (!_isRunning) { StartCoroutine(GameTimeCoroutine()); }

            _timeChangedEvent.InvokeEvent(CurrentTime);
        }

        private void ForceEndDay()
        {
            // day finished due to running out of time
            _dayTimeoutEvent.StartEvent();
        }

        IEnumerator GameTimeCoroutine()
        {
            _isRunning = true;
            while (CurrentTime < _dayEndTime)
            {
                yield return new WaitForSeconds(5);
                CurrentTime += 5;
                _timeChangedEvent.InvokeEvent(CurrentTime);
            }

            // end day when max time is reached
            _isRunning = false;
            ForceEndDay();
        }
    }
}