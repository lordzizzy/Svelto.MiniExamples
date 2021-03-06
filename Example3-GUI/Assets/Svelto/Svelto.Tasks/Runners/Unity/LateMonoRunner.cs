#if UNITY_5 || UNITY_5_3_OR_NEWER
using System.Collections;
using System.Collections.Generic;
using Svelto.Common;
using Svelto.Tasks.Internal;
using Svelto.Tasks.Unity.Internal;

namespace Svelto.Tasks
{
    namespace Lean.Unity
    {
        public class LateMonoRunner:LateMonoRunner<IEnumerator<TaskContract>>
        {
            public LateMonoRunner(string name) : base(name)
            {
            }
        }
        
        public class LateMonoRunner<T> : Svelto.Tasks.Unity.LateMonoRunner<SveltoTask<T>> where T : IEnumerator<TaskContract>
        {
            public LateMonoRunner(string name) : base(name)
            {
            }
        }
    }
    
    namespace ExtraLean.Unity
    {
        public class LateMonoRunner: LateMonoRunner<IEnumerator>
        {
            public LateMonoRunner(string name) : base(name)
            {
            }
        }
        
        public class LateMonoRunner<T> : Svelto.Tasks.Unity.LateMonoRunner<SveltoTask<T>> where T : IEnumerator
        {
            public LateMonoRunner(string name) : base(name)
            {
            }
        }
    }

    namespace Unity
    {
        public class LateMonoRunner<T> : LateMonoRunner<T, StandardRunningTasksInfo> where T : ISveltoTask
        {
            public LateMonoRunner(string name) : base(name, new StandardRunningTasksInfo())
            {
            }
        }

        public class LateMonoRunner<T, TFlowModifier> : BaseRunner<T> where T : ISveltoTask
                                                                        where TFlowModifier : IRunningTasksInfo
        {
            public LateMonoRunner(string name, TFlowModifier modifier) : base(name)
            {
                modifier.runnerName = name;

                _processEnumerator =
                    new CoroutineRunner<T>.Process<TFlowModifier>
                        (_newTaskRoutines, _coroutines, _flushingOperation, modifier);

                UnityCoroutineRunner.StartLateCoroutine(_processEnumerator);
            }
        }
    }
}
#endif
