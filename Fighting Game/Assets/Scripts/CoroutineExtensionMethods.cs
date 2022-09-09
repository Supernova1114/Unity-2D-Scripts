using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace MichaelWolfGames
{
    ///-///////////////////////////////////////////////////////////
    ///
    /// Extension methods for running coroutines and tweens.
    /// 
    public static class CoroutineExtensionMethods
    {

        ///-///////////////////////////////////////////////////////////
        /// 
        public static void InvokeAction(this MonoBehaviour invokedOn, Action action, float time, bool useRealTime = false)
        {
            if (time <= 0f)
            {
                action();
            }
            else
            {
                invokedOn.StartCoroutine(CoWaitForSeconds(action, time, useRealTime));
            }
        }
        
        ///-///////////////////////////////////////////////////////////
        /// 
        public static void InvokeAction(this MonoBehaviour invokedOn, Action action, YieldInstruction yieldInstruction)
        {
            if (yieldInstruction == null)
            {
                Debug.LogError("[CoroutineExtensionMethods]: Invoked Enumerator is Null!");
                action();
            }
            else
            {
                invokedOn.StartCoroutine(CoWaitForYieldInstruction(action, yieldInstruction));
            }
        }

        ///-///////////////////////////////////////////////////////////
        /// 
        public static void InvokeActionAtEndOfFrame(this MonoBehaviour invokedOn, Action action)
        {
            invokedOn.InvokeActionAtEndOfFrames(action, 1);
        }

        ///-///////////////////////////////////////////////////////////
        /// 
        public static void InvokeActionAtEndOfFrames(this MonoBehaviour invokedOn, Action action, int numFrames)
        {
            if (numFrames <= 0)
            {
                action();
                return;
            }
            List<YieldInstruction> yieldInstructions = new List<YieldInstruction>();
            for (int i = 0; i < numFrames; i++)
            {
                yieldInstructions.Add(new WaitForEndOfFrame());
            }
            invokedOn.StartCoroutine(CoWaitForYieldInstructions(action, yieldInstructions));
        }
        
        ///-///////////////////////////////////////////////////////////
        /// 
        public static void InvokeActionAfterFixedUpdate(this MonoBehaviour invokedOn, Action action)
        {
            invokedOn.InvokeActionAfterFixedUpdateFrames(action, 1);
        }

        ///-///////////////////////////////////////////////////////////
        /// 
        public static void InvokeActionAfterFixedUpdateFrames(this MonoBehaviour invokedOn, Action action, int numFrames)
        {
            if (numFrames <= 0)
            {
                action();
                return;
            }
            List<YieldInstruction> yieldInstructions = new List<YieldInstruction>();
            for (int i = 0; i < numFrames; i++)
            {
                yieldInstructions.Add(new WaitForFixedUpdate());
            }
            invokedOn.StartCoroutine(CoWaitForYieldInstructions(action, yieldInstructions));
        }

        ///-///////////////////////////////////////////////////////////
        /// 
        private static IEnumerator CoWaitForSeconds(Action actionCallback, float time, bool useRealTime = false)
        {
            if (useRealTime)
            {
                yield return new WaitForSecondsRealtime(time);
            }
            else
            {
                yield return new WaitForSeconds(time);
            }
            actionCallback();
        }

        ///-///////////////////////////////////////////////////////////
        /// 
        private static IEnumerator CoWaitForYieldInstruction(Action actionCallback, YieldInstruction yieldInstruction)
        {
            yield return yieldInstruction;
            actionCallback();
        }

        ///-///////////////////////////////////////////////////////////
        /// 
        private static IEnumerator CoWaitForYieldInstructions(Action actionCallback, IEnumerable<YieldInstruction> yieldInstructions)
        {
            foreach (YieldInstruction yieldInstruction in yieldInstructions)
            {
                yield return yieldInstruction;
            }
            actionCallback();
        }

        ///-///////////////////////////////////////////////////////////
        /// 
        public static void WaitForBool(this MonoBehaviour invokedOn, Func<bool> checkValue, Action onDoneCallback)
        {
            WaitForBool(invokedOn, checkValue, true, onDoneCallback);
        }

        ///-///////////////////////////////////////////////////////////
        /// 
        public static void WaitForBool(this MonoBehaviour invokedOn, Func<bool> checkValue, bool targetValue,
            Action onDoneCallback)
        {
            invokedOn.StartCoroutine(CoWaitForBool(checkValue, targetValue, onDoneCallback));
        }

        ///-///////////////////////////////////////////////////////////
        /// 
        private static IEnumerator CoWaitForBool(Func<bool> checkValue, bool targetValue, Action onDoneCallback)
        {
            while (checkValue() != targetValue)
            {
                yield return null;
            }
            onDoneCallback();
        }

        ///-///////////////////////////////////////////////////////////
        /// 
        public static Coroutine DoWhile(this MonoBehaviour invokedOn, Action doAction, Func<bool> getWhileValue, Action onDoneCallback = null)
        {
            return invokedOn.StartCoroutine(CoDoWhile(doAction, getWhileValue, onDoneCallback));
        }

        ///-///////////////////////////////////////////////////////////
        /// 
        private static IEnumerator CoDoWhile(Action doAction, Func<bool> getWhileValue, Action onDoneCallback = null)
        {
            while (getWhileValue())
            {
                doAction();
                yield return null;
            }
            if (onDoneCallback != null)
                onDoneCallback();
        }
        
        ///-///////////////////////////////////////////////////////////
        /// 
        public static Coroutine StartTimer(this MonoBehaviour invokedOn, float duration, Action onDoneCallback = null)
        {
            return invokedOn.StartCoroutine(CoDoTimer(duration, onDoneCallback));
        }
        
        ///-///////////////////////////////////////////////////////////
        /// 
        private static IEnumerator CoDoTimer(float duration, Action onDoneCallback = null)
        {
            yield return new WaitForSeconds(duration);
            
            if (onDoneCallback != null)
                onDoneCallback();
        }
        
        ///-///////////////////////////////////////////////////////////
        /// 
        public static Coroutine DoTween(this MonoBehaviour invokedOn, Action<float> tweenAction, Action onCompleteCallback = null, float duration = 0f, float delay = 0f, EaseType easeType = EaseType.linear, bool useUnscaledTime = false)
        {
            return invokedOn.StartCoroutine(CoDoTween( tweenAction, onCompleteCallback, duration, delay, easeType, useUnscaledTime));
        }

        ///-///////////////////////////////////////////////////////////
        /// 
        public static IEnumerator CoDoTween(Action<float> tweenAction, Action onCompleteCallback = null, float duration = 0f, float delay = 0f, EaseType easeType = EaseType.linear, bool useUnscaledTime = false)
        {
            if (delay > 0f)
            {
                if (useUnscaledTime)
                {
                    yield return new WaitForSecondsRealtime(delay);
                }
                else
                {
                    yield return new WaitForSeconds(delay);
                }
            }
            
            // Get the easing function for this EaseType.
            TweenEasing.EasingFunction easeFunc = TweenEasing.GetEasingFunction(easeType);
            if (easeFunc == null)
            {
                // Default on linear easing.
                easeFunc = TweenEasing.GetEasingFunction(EaseType.linear);
            }
            
            // Run the tween over the duration.
            float timer = 0f;
            while (timer < duration)
            {
                timer += (useUnscaledTime)? Time.unscaledDeltaTime :Time.deltaTime;
                
                // Run the tween function for 
                tweenAction(easeFunc(0,1, timer / duration));
                
                yield return null;
            }
            
            // Call the callback, if assigned.
            onCompleteCallback?.Invoke();
        }
    }
}
