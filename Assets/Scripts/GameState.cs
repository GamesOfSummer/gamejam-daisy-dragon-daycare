using System.Collections;
using System.Collections.Generic;
using MonsterLove.StateMachine;
using UnityEngine; //Remember the using statement before the class declaration

public class GameState : MonoBehaviour {

    static public GameState Instance { get; private set; }

    public enum States {
        TitleScreen,
        Credits,
        Play,
        End,

    }

    StateMachine<States> fsm;

    void Awake () {
        Instance = this;
        fsm = StateMachine<States>.Initialize (this);
        fsm.ChangeState (States.TitleScreen);
    }

    public bool IsCurrentStateTitle () {
        return fsm.State == States.TitleScreen;
    }

    public bool IsCurrentStatePlay () {
        return fsm.State == States.Play;
    }

    public bool IsCurrentStateEnd () {
        return fsm.State == States.End;
    }

    public void ChangeState_Start () {
        fsm.ChangeState (States.TitleScreen);
    }

    public void ChangeState_Play () {
        fsm.ChangeState (States.Play);
    }

    public void ChangeState_End () {
        fsm.ChangeState (States.End);
    }

    void Play_Enter () {
        Debug.Log ("Entering play state - We are now ready");
    }

    void Play_Update () {
        //Debug.Log ("Update INIT state - We are now ready");
    }

    void End_Enter () {
        Debug.Log ("Enterinlg End state - We are now ready");
    }
}