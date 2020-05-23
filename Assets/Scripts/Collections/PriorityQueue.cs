using System.Collections.Generic;
using System.Linq;

namespace Custom.Collections.Generic
{
    /// <summary>
    /// Simple implementation of the priority queue using a dictionary and Min sorting.
    /// </summary>
    /// <typeparam name="T">Type of data stored in the priority queue.</typeparam>
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

        public bool Contains(T obj)
        {
            foreach (var kvp in _items)
            {
                if (kvp.Value.Contains(obj))
                    return true;
            }
            return false;
        }

        public void Clear()
        {
            _items.Clear();
            Count = 0;
            _minPriority = int.MaxValue;
        }

        private void UpdateMin()
        {
            if(_items.Count > 0)
                _minPriority = _items.Keys.Min();
        }
    }
}