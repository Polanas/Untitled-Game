using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game;

class StateMachine //Based on Celeste's state machine (thank you very much :>)
{
    private int _state;

    private Action[] _begins;

    private Action[] _ends;

    private Func<int>[] _updates;

    public int PreviousState { get; private set; }

    public StateMachine(int maxStates)
    {
        _begins = new Action[maxStates];
        _ends = new Action[maxStates];
        _updates = new Func<int>[maxStates];

        PreviousState = -1;
    }

    public void SetCallBacks(int state, Func<int> onUpdate, Action begin = null, Action end = null)
    {
        _begins[state] = begin;
        _ends[state] = end;
        _updates[state] = onUpdate;
    }

    public void ForceState(int state)
    {
        if (_state == state)
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

        if (PreviousState != _state)
            _updates[_state].Invoke();
    }
}
