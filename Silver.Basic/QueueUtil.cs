using System;
using System.Collections.Generic;

namespace Silver.Basic
{
    public class QueueUtil<T> where T : class
    {
        private object lockAsync = new object();
        public Queue<T> queue = new Queue<T>();

        /// <summary>
        /// 获取数量
        /// </summary>
        public int Count
        {
            get
            {
                return queue.Count;
            }
        }

        /// <summary>
        /// 入队
        /// </summary>
        /// <param name="item"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void Enqueue(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("输入数据不能为空");
            }
            lock (lockAsync)
            {
                queue.Enqueue(item);
            }
        }

        /// <summary>
        /// 出队
        /// </summary>
        /// <returns></returns>
        public List<T> Dequeue()
        {
            List<T> list = new List<T>();
            lock (lockAsync)
            {
                if (queue.Count > 0)
                {
                    list.Add(queue.Dequeue()); 
                }   
            }
            return list;
        }
         
        /// <summary>
        /// 清空
        /// </summary>
        /// <param name="match"></param>
        public void ClearAll()
        {
            lock (lockAsync)
            {
                if (queue.Count > 0)
                {
                    queue.Clear();
                }
            }
        }

    }
}
