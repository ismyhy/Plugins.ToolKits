using System;
using System.Collections.Generic;
using System.Linq;

namespace Plugins.ToolKits.MVVM
{
    internal class Mapping<TToken, TRecipient, THandler> : List<Tuple<TToken, TRecipient, THandler>>
    {
        private readonly object _syncRoot = new object();

        public bool Remove(TToken token, TRecipient recipient)
        {
            //if (Count(token) == 0)
            //{
            //    return false;
            //}

            Tuple<TToken, TRecipient, THandler> tup = this.FirstOrDefault(i => Equals(i.Item1, token));
            if (tup is null)
            {
                return false;
            }

            lock (_syncRoot)
            {
                if (Equals(tup.Item2, recipient))
                {
                    Remove(tup);
                    return true;
                }

                return false;
            }
        }


        public bool Remove(TRecipient recipient)
        {
            Tuple<TToken, TRecipient, THandler>[] array = this.Where(i => Equals(i.Item2, recipient)).ToArray();
            if (array.Length == 0)
            {
                return false;
            }

            lock (_syncRoot)
            {
                for (int i = 0, j = array.Length; i < j; i++)
                {
                    Tuple<TToken, TRecipient, THandler> item = array[i];
                    Remove(item);
                }

                return true;
            }
        }


        public bool Add(TToken token, TRecipient recipient, THandler handler)
        {
            lock (_syncRoot)
            {
                base.Add(Tuple.Create(token, recipient, handler));
                return true;
            }
        }


        public bool TryGet(TToken token, out TRecipient recipient, out THandler handler)
        {
            Tuple<TToken, TRecipient, THandler> tup = this.FirstOrDefault(i => Equals(i.Item1, token));
            if (tup is null)
            {
                recipient = default;
                handler = default;
                return false;
            }

            recipient = tup.Item2;
            handler = tup.Item3;
            return true;
        }


        public bool TryGets(TToken token, out TRecipient[] recipient, out THandler[] handler)
        {
            List<Tuple<TToken, TRecipient, THandler>> tup = this.Where(i => Equals(i.Item1, token)).ToList();
            if (tup.Count == 0)
            {
                recipient = new TRecipient[0];
                handler = new THandler[0];
                return false;
            }

            recipient = tup.Select(i => i.Item2).ToArray();
            handler = tup.Select(i => i.Item3).ToArray();
            return true;
        }

        public new int Count()
        {
            return base.Count;
        }

        public new int Count(TToken token)
        {
            return this.Count(i => Equals(i.Item1, token));
        }
    }
}