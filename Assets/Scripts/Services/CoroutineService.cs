using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Services
{
    // ToDo Remove if redundant
    public class CoroutineService
    {
        public IEnumerator CoroutineWithDelay(Func<IEnumerator> func, float seconds)
        {
            yield return new WaitForSeconds(seconds);
            yield return func();
        }

        public IEnumerator CoroutineWaitForAll(Func<IEnumerator> func, float seconds)
        {
            yield return new WaitForSeconds(seconds);
            yield return func();
        }
    }
}
