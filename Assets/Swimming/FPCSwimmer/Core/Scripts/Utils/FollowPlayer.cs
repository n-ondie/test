using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPCSwimmer
{

    public class FollowPlayer : MonoBehaviour
    {
        public GameObject MainPlayer;

        void Update()
        {
            this.transform.position = MainPlayer.transform.position;
        }
    }

}