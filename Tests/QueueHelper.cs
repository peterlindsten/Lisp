using System.Collections.Generic;

namespace Tests
{
    public static class QueueHelper
    {
        public static void EnqueueAll<T>(this Queue<T> me, params T[] tokens)
        {
            foreach (var t in tokens)
            {
                me.Enqueue(t);
            }
        }
    }
}