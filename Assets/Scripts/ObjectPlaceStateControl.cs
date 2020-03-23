using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CLEditor
{
    public class ObjectPlaceStateControl : MonoBehaviour
    {
        public ObjectControlState State;

        private bool iscollder;
        // Use this for initialization
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            if (transform.position.x <= -0.4|| transform.position.x >= 0.4||transform.position.z <= -0.9 || transform.position.z >= 0.9) State = ObjectControlState.NotPlace;
            else if(iscollder) State = ObjectControlState.NotPlace;
            else State = ObjectControlState.CanPlace;
        }

        private void OnTriggerStay(Collider other)
        {
            iscollder = true;
        }

        private void OnTriggerExit(Collider other)
        {
            iscollder = false;
        }


        private void OnDestroy()
        {
        }
    }

}
