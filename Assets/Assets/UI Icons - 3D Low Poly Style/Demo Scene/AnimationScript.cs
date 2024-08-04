using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FWC
{

    public class AnimationScript : MonoBehaviour
    {
        [SerializeField] float max, min;

        [SerializeField] RectTransform rectTransform;

        public float velocity = 0;

        public float smoothTime = 1;

        [SerializeField] bool isVertical;


        void OnEnable()
        {
            if (!isVertical)
            {
                rectTransform.anchoredPosition = new Vector2(min, 0);

            }
            else
            {
                rectTransform.anchoredPosition = new Vector2(0, min);

            }


            StartCoroutine(Begin());
        }

        IEnumerator Begin()
        {
            while (true)
            {
                yield return new WaitForSeconds(smoothTime * 4);

                if (!isVertical)
                {
                    rectTransform.anchoredPosition = new Vector2(max, 0);

                }
                else
                {
                    rectTransform.anchoredPosition = new Vector2(0, max);

                }


                float temp = max;
                float temp2 = min;


                max = temp2;
                min = temp;




                yield return null;

            }

        }



        private void Update()
        {

            if (!isVertical)
            {

                rectTransform.anchoredPosition = new Vector2(Mathf.SmoothDamp(rectTransform.anchoredPosition.x, max, ref velocity, smoothTime), 0);

                return;
            }

            rectTransform.anchoredPosition = new Vector2(0, Mathf.SmoothDamp(rectTransform.anchoredPosition.y, max, ref velocity, smoothTime));


        }

    }

}


