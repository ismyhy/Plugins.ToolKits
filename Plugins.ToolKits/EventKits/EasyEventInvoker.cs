using System;

namespace Plugins.ToolKits.EventKits
{
    internal sealed class EasyEventInvoker : IDisposable
    {
        private EasyEventBase _owner;

        private object _runAction;

        internal EasyEventHandle EasyEventHandle;

        internal object EventToken;

        internal EasyEventInvoker(object action, EasyEventBase owner, object eventToken = null)
        {
            _owner = owner;
            _runAction = action;
            EventToken = eventToken;
            EasyEventHandle = new EasyEventHandle(this);
        }

        public void Dispose()
        {
            _owner.RemoveEasyEvent(this);
            _owner = null;
            _runAction = null;
        }

        public override string ToString()
        {
            return EventToken?.ToString() ?? $"{Environment.TickCount}";
        }

        public void Invoke()
        {
            InnerRunActions(delegate (Action i)
            {
                i.Invoke();
            });
        }

        public void Invoke<TType>(TType target)
        {
            InnerRunActions(delegate (Action<TType> i)
            {
                i.Invoke(target);
            });
        }

        public void Invoke<TType, TType2>(TType target, TType2 target2)
        {
            InnerRunActions(delegate (Action<TType, TType2> i)
            {
                i.Invoke(target, target2);
            });
        }

        public void Invoke<TType, TType2, TType3>(TType target, TType2 target2, TType3 target3)
        {
            InnerRunActions(delegate (Action<TType, TType2, TType3> i)
            {
                i.Invoke(target, target2, target3);
            });
        }

        public void Invoke<TType, TType2, TType3, TType4>(TType target, TType2 target2, TType3 target3, TType4 target4)
        {
            InnerRunActions(delegate (Action<TType, TType2, TType3, TType4> i)
            {
                i.Invoke(target, target2, target3, target4);
            });
        }

        public void Invoke<TType, TType2, TType3, TType4, TType5>(TType target, TType2 target2, TType3 target3, TType4 target4, TType5 target5)
        {
            InnerRunActions(delegate (Action<TType, TType2, TType3, TType4, TType5> i)
            {
                i.Invoke(target, target2, target3, target4, target5);
            });
        }

        public void Invoke<TType, TType2, TType3, TType4, TType5, TType6>(TType target, TType2 target2, TType3 target3, TType4 target4, TType5 target5, TType6 target6)
        {
            InnerRunActions(delegate (Action<TType, TType2, TType3, TType4, TType5, TType6> i)
            {
                i.Invoke(target, target2, target3, target4, target5, target6);
            });
        }

        public void Invoke<TType, TType2, TType3, TType4, TType5, TType6, TType7>(TType target, TType2 target2, TType3 target3, TType4 target4, TType5 target5, TType6 target6, TType7 target7)
        {
            InnerRunActions(delegate (Action<TType, TType2, TType3, TType4, TType5, TType6, TType7> i)
            {
                i.Invoke(target, target2, target3, target4, target5, target6, target7);
            });
        }

        public void Invoke<TType, TType2, TType3, TType4, TType5, TType6, TType7, TType8>(TType target, TType2 target2, TType3 target3, TType4 target4, TType5 target5, TType6 target6, TType7 target7, TType8 target8)
        {
            InnerRunActions(delegate (Action<TType, TType2, TType3, TType4, TType5, TType6, TType7, TType8> i)
            {
                i.Invoke(target, target2, target3, target4, target5, target6, target7, target8);
            });
        }

        public void Invoke<TType, TType2, TType3, TType4, TType5, TType6, TType7, TType8, TType9>(TType target, TType2 target2, TType3 target3, TType4 target4, TType5 target5, TType6 target6, TType7 target7, TType8 target8, TType9 target9)
        {
            InnerRunActions(delegate (Action<TType, TType2, TType3, TType4, TType5, TType6, TType7, TType8, TType9> i)
            {
                i.Invoke(target, target2, target3, target4, target5, target6, target7, target8, target9);
            });
        }

        public void Invoke<TType, TType2, TType3, TType4, TType5, TType6, TType7, TType8, TType9, TType10>(TType target, TType2 target2, TType3 target3, TType4 target4, TType5 target5, TType6 target6, TType7 target7, TType8 target8, TType9 target9, TType10 target10)
        {
            InnerRunActions(delegate (Action<TType, TType2, TType3, TType4, TType5, TType6, TType7, TType8, TType9, TType10> i)
            {
                i.Invoke(target, target2, target3, target4, target5, target6, target7, target8, target9, target10);
            });
        }

        public void Invoke<TType, TType2, TType3, TType4, TType5, TType6, TType7, TType8, TType9, TType10, TType11>(TType target, TType2 target2, TType3 target3, TType4 target4, TType5 target5, TType6 target6, TType7 target7, TType8 target8, TType9 target9, TType10 target10, TType11 target11)
        {
            InnerRunActions(delegate (Action<TType, TType2, TType3, TType4, TType5, TType6, TType7, TType8, TType9, TType10, TType11> i)
            {
                i.Invoke(target, target2, target3, target4, target5, target6, target7, target8, target9, target10, target11);
            });
        }

        private void InnerRunActions<T>(Action<T> action)
        {
            T tar = (T)_runAction;
            action.Invoke(tar);
        }
    }
}