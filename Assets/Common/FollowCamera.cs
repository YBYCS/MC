using UnityEngine;

namespace LYcommon {
	/// <summary>
	/// This script allows the camera to follow the character
	/// </summary>
	public class FollowCamera : MonoBehaviour
    {
        /// <summary>
        /// the boundary of the map
        /// </summary>
        public float l, r, u, d;
        public GameObject character;//¸úËæµÄ½ÇÉ«

        void CameraMove()
        {

            if (character.transform.position.x < r && character.transform.position.x > l)
            {

                transform.position = new Vector3(character.transform.position.x, transform.position.y, transform.position.z);
            }
            if (character.transform.position.y < u && character.transform.position.y > d)
            {
                transform.position = new Vector3(transform.position.x, character.transform.position.y, transform.position.z);
            }
        }
    }
}

