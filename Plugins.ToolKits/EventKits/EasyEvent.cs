using System;
using System.Threading.Tasks;

namespace Plugins.ToolKits.EventKits
{
    public sealed class EasyEvent : EasyEventBase
    {
        public EasyEventHandle Register(Action action, object eventToken = null)
        {
            return InnerAdd(action, eventToken);
        }

        public void Invoke()
        {
            RunActions(i => i.Invoke());
        }

        public Task InvokeAsync()
        {
            return RunActionsAsync(i => i.Invoke());
        }
    }

    public sealed class EasyEvent<TType> : EasyEventBase
    {
        public EasyEventHandle Register(Action<TType> action, object eventToken = null)
        {
            return InnerAdd(action, eventToken);
        }

        public void Invoke(TType target)
        {
            RunActions(i => i.Invoke(target));
        }

        public Task InvokeAsync(TType target)
        {
            return RunActionsAsync(i => i.Invoke(target));
        }
    }

    public sealed class EasyEvent<TType, TType2> : EasyEventBase
    {
        public EasyEventHandle Register(Action<TType, TType2> action, object eventToken = null)
        {
            return InnerAdd(action, eventToken);
        }

        public void Invoke(TType target, TType2 target2)
        {
            RunActions(i => i.Invoke(target, target2));
        }

        public Task InvokeAsync(TType target, TType2 target2)
        {
            return RunActionsAsync(i => i.Invoke(target, target2));
        }
    }


    public sealed class EasyEvent<TType, TType2, TType3> : EasyEventBase
    {
        public EasyEventHandle Register(Action<TType, TType2, TType3> action, object eventToken = null)
        {
            return InnerAdd(action, eventToken);
        }

        public void Invoke(TType target, TType2 target2, TType3 target3)
        {
            RunActions(i => i.Invoke(target, target2, target3));
        }

        public Task InvokeAsync(TType target, TType2 target2, TType3 target3)
        {
            return RunActionsAsync(i => i.Invoke(target, target2, target3));
        }
    }

    public sealed class EasyEvent<TType, TType2, TType3, TType4> : EasyEventBase
    {
        public EasyEventHandle Register(Action<TType, TType2, TType3, TType4> action, object eventToken = null)
        {
            return InnerAdd(action, eventToken);
        }

        public void Invoke(TType target, TType2 target2, TType3 target3, TType4 target4)
        {
            RunActions(i => i.Invoke(target, target2, target3, target4));
        }

        public Task InvokeAsync(TType target, TType2 target2, TType3 target3, TType4 target4)
        {
            return RunActionsAsync(i => i.Invoke(target, target2, target3, target4));
        }
    }


    public sealed class EasyEvent<TType, TType2, TType3, TType4, TType5> : EasyEventBase
    {
        public EasyEventHandle Register(Action<TType, TType2, TType3, TType4, TType5> action, object eventToken = null)
        {
            return InnerAdd(action, eventToken);
        }

        public void Invoke(TType target, TType2 target2, TType3 target3, TType4 target4, TType5 target5)
        {
            RunActions(i => i.Invoke(target, target2, target3, target4, target5));
        }

        public Task InvokeAsync(TType target, TType2 target2, TType3 target3, TType4 target4, TType5 target5)
        {
            return RunActionsAsync(i => i.Invoke(target, target2, target3, target4, target5));
        }
    }

    public sealed class EasyEvent<TType, TType2, TType3, TType4, TType5, TType6> : EasyEventBase
    {
        public EasyEventHandle Register(Action<TType, TType2, TType3, TType4, TType5, TType6> action, object eventToken = null)
        {
            return InnerAdd(action, eventToken);
        }

        public void Invoke(TType target, TType2 target2, TType3 target3, TType4 target4, TType5 target5, TType6 target6)
        {
            RunActions(i => i.Invoke(target, target2, target3, target4, target5, target6));
        }

        public Task InvokeAsync(TType target, TType2 target2, TType3 target3, TType4 target4, TType5 target5, TType6 target6)
        {
            return RunActionsAsync(i => i.Invoke(target, target2, target3, target4, target5, target6));
        }
    }

    public sealed class EasyEvent<TType, TType2, TType3, TType4, TType5, TType6, TType7> : EasyEventBase
    {
        public EasyEventHandle Register(Action<TType, TType2, TType3, TType4, TType5, TType6, TType7> action, object eventToken = null)
        {
            return InnerAdd(action, eventToken);
        }

        public void Invoke(TType target, TType2 target2, TType3 target3, TType4 target4, TType5 target5, TType6 target6, TType7 target7)
        {
            RunActions(i => i.Invoke(target, target2, target3, target4, target5, target6, target7));
        }

        public Task InvokeAsync(TType target, TType2 target2, TType3 target3, TType4 target4, TType5 target5, TType6 target6, TType7 target7)
        {
            return RunActionsAsync(i => i.Invoke(target, target2, target3, target4, target5, target6, target7));
        }
    }

    public sealed class EasyEvent<TType, TType2, TType3, TType4, TType5, TType6, TType7, TType8> : EasyEventBase
    {
        public EasyEventHandle Register(Action<TType, TType2, TType3, TType4, TType5, TType6, TType7, TType8> action, object eventToken = null)
        {
            return InnerAdd(action, eventToken);
        }

        public void Invoke(TType target, TType2 target2, TType3 target3, TType4 target4, TType5 target5, TType6 target6, TType7 target7, TType8 target8)
        {
            RunActions(i => i.Invoke(target, target2, target3, target4, target5, target6, target7, target8));
        }

        public Task InvokeAsync(TType target, TType2 target2, TType3 target3, TType4 target4, TType5 target5, TType6 target6, TType7 target7, TType8 target8)
        {
            return RunActionsAsync(i => i.Invoke(target, target2, target3, target4, target5, target6, target7, target8));
        }
    }

    public sealed class EasyEvent<TType, TType2, TType3, TType4, TType5, TType6, TType7, TType8, TType9> : EasyEventBase
    {
        public EasyEventHandle Register(Action<TType, TType2, TType3, TType4, TType5, TType6, TType7, TType8, TType9> action, object eventToken = null)
        {
            return InnerAdd(action, eventToken);
        }

        public void Invoke(TType target, TType2 target2, TType3 target3, TType4 target4, TType5 target5, TType6 target6, TType7 target7, TType8 target8, TType9 target9)
        {
            RunActions(i => i.Invoke(target, target2, target3, target4, target5, target6, target7, target8, target9));
        }

        public Task InvokeAsync(TType target, TType2 target2, TType3 target3, TType4 target4, TType5 target5, TType6 target6, TType7 target7, TType8 target8, TType9 target9)
        {
            return RunActionsAsync(i => i.Invoke(target, target2, target3, target4, target5, target6, target7, target8, target9));
        }
    }


    public sealed class EasyEvent<TType, TType2, TType3, TType4, TType5, TType6, TType7, TType8, TType9, TType10> : EasyEventBase
    {
        public EasyEventHandle Register(Action<TType, TType2, TType3, TType4, TType5, TType6, TType7, TType8, TType9, TType10> action, object eventToken = null)
        {
            return InnerAdd(action, eventToken);
        }

        public void Invoke(TType target, TType2 target2, TType3 target3, TType4 target4, TType5 target5, TType6 target6, TType7 target7, TType8 target8, TType9 target9, TType10 target10)
        {
            RunActions(i => i.Invoke(target, target2, target3, target4, target5, target6, target7, target8, target9, target10));
        }

        public Task InvokeAsync(TType target, TType2 target2, TType3 target3, TType4 target4, TType5 target5, TType6 target6, TType7 target7, TType8 target8, TType9 target9, TType10 target10)
        {
            return RunActionsAsync(i => i.Invoke(target, target2, target3, target4, target5, target6, target7, target8, target9, target10));
        }
    }

    public sealed class EasyEvent<TType, TType2, TType3, TType4, TType5, TType6, TType7, TType8, TType9, TType10, TType11> : EasyEventBase
    {
        public EasyEventHandle Register(Action<TType, TType2, TType3, TType4, TType5, TType6, TType7, TType8, TType9, TType10, TType11> action, object eventToken = null)
        {
            return InnerAdd(action, eventToken);
        }

        public void Invoke(TType target, TType2 target2, TType3 target3, TType4 target4, TType5 target5, TType6 target6, TType7 target7, TType8 target8, TType9 target9, TType10 target10, TType11 target11)
        {
            RunActions(i => i.Invoke(target, target2, target3, target4, target5, target6, target7, target8, target9, target10, target11));
        }

        public Task InvokeAsync(TType target, TType2 target2, TType3 target3, TType4 target4, TType5 target5, TType6 target6, TType7 target7, TType8 target8, TType9 target9, TType10 target10, TType11 target11)
        {
            return RunActionsAsync(i => i.Invoke(target, target2, target3, target4, target5, target6, target7, target8, target9, target10, target11));
        }
    }
}