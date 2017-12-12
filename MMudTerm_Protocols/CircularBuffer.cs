using System;
using System.Collections.Generic;
using System.Collections;

namespace MudTermProtocols
{
    /// <summary>
    /// Generic Circular Buffer
    /// Do not use non-nullable types... yet
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CircularBuffer<T>
    {
        int maxCount;
        CircularBufferCounter _tail, _head;
        T[] buffer;
        public T this[int idx] 
        { 
            get{
                int val = this._head.Value + idx;
                if(val > maxCount)
                {
                    return buffer[val - maxCount - 1];
                }
                return buffer[val]; 
                }
            set
            {
                int val = this._head.Value + idx;
                if(val > maxCount)
                {
                    this.buffer[val - maxCount - 1] = value; ;
                }
                this.buffer[val] = value;
            }
        }

        public int Count
        {
            get
            {
                if (this._head.Value > this._tail.Value)
                {
                    return (maxCount - this._head.Value + 1) + this._tail.Value;
                }
                return this._tail.Value - this._head.Value;
            }
        }

        public int Head
        { get { return this._head.Value; } }

        public int Tail
        { get { return this._tail.Value; } }

        public CircularBuffer(int maxCount)
        {
            this.maxCount = maxCount;
            this.Clear();
        }

        public void Add(T item)
        {
            //need to find a better way to handle non-nullables
            if (buffer[this._tail.Next()] == null)
            {
                buffer[this._tail.Value] = item;
                this._tail += 1;
            }
            else
            {
                buffer[this._head.Value] = default(T);
                this._head += 1;
                buffer[this._tail.Value] = item;
                this._tail += 1;
            }
        }

        public void Clear()
        {
            this.buffer = new T[this.maxCount+1];
            this._tail = new CircularBuffer<T>.CircularBufferCounter(maxCount, 0);
            this._head = new CircularBuffer<T>.CircularBufferCounter(maxCount, 0);
        }

        public void ReplaceTailItem(T item)
        {
            buffer[this._tail.Value] = item;
        }

        public T GetTailItem()
        {
            return buffer[this._tail.Value];
        }

        public void DeleteHead()
        {
            buffer[this._head.Value] = default(T);
            this._head += 1;
        }

        //public void Resize(int newMax)
        //{
        //    this.maxCount = newMax+1;
        //    this._head.Resize(newMax);
        //    this._tail.Resize(newMax);
        //
        //    shuffle the buffers
        //}

        internal class CircularBufferCounter
        {
            int _val, _max;
            public int Value
            { get { return this._val; } }

            internal CircularBufferCounter(int max, int value)
            {
                this._max = max;
                this._val = value;
            }

            internal void Resize(int newMax)
            {
                this._max = newMax;
            }

            public static CircularBufferCounter operator +(CircularBufferCounter a, int b)
            {
                int newVal = ((a.Value + b) > a._max) ? 0 : (a.Value + b);
                return new CircularBufferCounter(a._max, newVal);
            }

            internal int Next()
            {
                return ((this._val + 1) > this._max) ? 0 : (this._val + 1);
            }
        }
    }
}
