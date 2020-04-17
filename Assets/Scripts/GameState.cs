using System.Collections;
using System.Collections.Generic;
using MonsterLove.StateMachine;
using UnityEngine; //Remember the using statement before the class declaration

public class GameState : MonoBehaviour {

    static public GameController Instance { get; private set; }

    public enum States {
        TitleScreen,
        Credits,
        Play,
        End,

    }

    StateMachine<States> fsm;

    // Start is called before the first frame update
    void Start () {
        fsm = StateMachine<States>.Initialize (this);
        fsm.ChangeState (States.Play);

    }

    // Update is called once per frame
    void Update () {

    }
}