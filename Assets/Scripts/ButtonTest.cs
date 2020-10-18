using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonTest : MonoBehaviour
{
        private Animator anim;

    
    public bool animateFood;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
 public bool animation_bool;
 void Update () {

     }

    public void PlayAnimation() {
 gameObject.GetComponent<Animation>().Play ("FoodSlideOut");
     }
}
