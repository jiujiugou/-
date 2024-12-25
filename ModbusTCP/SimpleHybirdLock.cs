using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModbusTCP
{
    public class SimpleHybirdLock : IDisposable
    {
        private bool disposedValue = false;
        /// <summary>
        /// 基于用户模式构造同步锁
        /// </summary>
        private Int32 m_waiter = 0;
        /// <summary>
        /// 基于内核模式构造同步锁
        /// </summary>
        private AutoResetEvent m_waiterLock = new AutoResetEvent(false);
        /// <summary>
        /// 进入锁
        /// </summary>
        public void Enter()
        {
            if (Interlocked.Increment(ref m_waiter) == 1)
            {
                return;
            }
            m_waiterLock.WaitOne();
        }
        /// <summary>
        /// 释放锁
        /// </summary>
        public void Leave()
        {
            if (Interlocked.Decrement(ref m_waiter) == 0)
            {
                return;
            }
            m_waiterLock.Set();
        }
        public bool IsWaitting => m_waiter == 0;
        public void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    m_waiterLock.Dispose(); // 释放内核资源
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
