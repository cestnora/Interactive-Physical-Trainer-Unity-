using System.Collections;
using UnityEngine;

public class Next : MonoBehaviour
{
    private Animator mAnimator;
    private bool isPaused = false;
    private int rightArrowPressCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        mAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (mAnimator != null)
        {
            // Trigger the next transition with the Right Arrow key (single press)
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                rightArrowPressCount++;

                // Call Next2 when the right arrow is pressed twice
                if (rightArrowPressCount == 2)
                {
                    mAnimator.SetTrigger("Next2");
                    StartCoroutine(ResetNext2Trigger());
                    rightArrowPressCount = 0; // Reset the counter
                }
                else
                {
                    // Call the original Next transition if only pressed once
                    mAnimator.SetTrigger("Next");
                    StartCoroutine(ResetNextTrigger());
                }
            }
            // Toggle pause with the Space key
            if (Input.GetKeyDown(KeyCode.Space))
            {
                isPaused = !isPaused;
                mAnimator.SetBool("isPaused", isPaused);

                if (isPaused)
                {
                    mAnimator.SetFloat("Speed", 0); // Pause animation
                }
                else
                {
                    mAnimator.SetFloat("Speed", 1); // Resume animation
                }
            }
        }
    }

    // Coroutine to reset the "Next" trigger after a short delay
    private IEnumerator ResetNextTrigger()
    {
        yield return null; // Wait for one frame
        mAnimator.ResetTrigger("Next");
    }
    // Coroutine to reset the "Next2" trigger after a short delay
    private IEnumerator ResetNext2Trigger()
    {
        yield return null; // Wait for one frame
        mAnimator.ResetTrigger("Next2");
    }
}
