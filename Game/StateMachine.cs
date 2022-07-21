using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game;

/// <summary>
/// Based on Celeste's state machine (thank you very much :>)
/// </summary>
/// 
class StateMachine<T> 
{
    private T _state;

    private int _maxStates;

    private Dictionary<T, Action> _begins;

    private Dictionary<T, Action> _ends;

    private Dictionary<T, Func<T>> _updates;

    public T PreviousState { get; private set; }

    public T State => _state;

    public StateMachine(int maxStates)
    {
        _maxStates = maxStates;

        _begins = new();
        _ends = new();
        _updates = new();
    }

    public void SetCallBacks(T state, Func<T> onUpdate, Action begin = null, Action end = null)
    {
        if (_updates.Count == _maxStates)
#if DEBUG
            throw new ArgumentException("Too much states created!");
#else
        return;
#endif

        _begins[state] = begin;
        _ends[state] = end;
        _updates[state] = onUpdate;
    }

    public void ForceState(T state)
    {
        if (EqualityComparer<T>.Default.Equals(_state, state))
            return;

        PreviousState = _state;
        _state = state;

        _ends[PreviousState]?.Invoke();
        _begins[_state]?.Invoke();

    }

    public void Update()
    {
        if (_updates[_state] != null)
            ForceState(_updates[_state].Invoke());

        if (!EqualityComparer<T>.Default.Equals(PreviousState, _state))
            _updates[_state].Invoke();
    }
}
