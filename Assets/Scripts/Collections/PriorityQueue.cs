using System.Collections.Generic;
using System.Linq;

namespace Custom.Collections.Generic
{
    public class PriorityQueue<T> where T : class
    {
        private Dictionary<int, HashSet<T>> _items = new Dictionary<int, HashSet<T>>();
        private int _minPriority = int.MaxValue;

        public int Count { get; private set; }

        public void Enqueue(T obj, int priority)
        {
            if (!_items.ContainsKey(priority))
            {
                _items.Add(priority, new HashSet<T>());
            }
            
            _items[priority].Add(obj);

            UpdateMin();

            Count++;
        }

        public T Dequeue()
        {
            HashSet<T> objs = _items[_minPriority];
            T obj = objs.FirstOrDefault();
            if (obj != null)
            {
                objs.Remove(obj);
                if (objs.Count == 0)
                {
                    _items.Remove(_minPriority);
                }
                Count--;
                UpdateMin();

                return obj;
            }

            return null;
        }

        public void ChangePriority(T obj, int oldPriority, int newPriority)
        {
            if(newPriority == oldPriority)
            {
                //nothing to be done, it's already as it should be
                return;
            }

            HashSet<T> objs = _items[oldPriority];
            if (objs.Contains(obj))
            {
                objs.Remove(obj);
                if(objs.Count == 0)
                {
                    _items.Remove(oldPriority);
                }

                Enqueue(obj, newPriority);
            }
        }

        public void Clear()
        {
            _items.Clear();
            Count = 0;
            _minPriority = int.MaxValue;
        }

        private void UpdateMin()
        {
            _minPriority = _items.Keys.Min();
        }
    }
}