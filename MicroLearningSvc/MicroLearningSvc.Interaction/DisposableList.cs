using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroLearningSvc
{
    public class DisposableList : IDisposable
    {
        readonly object _lock = new object();
        readonly LinkedList<IDisposable> _disposables = new LinkedList<IDisposable>();

        public DisposableList()
        {

        }

        public T Add<T>(T obj)
            where T : IDisposable
        {
            lock (_lock)
            {
                _disposables.AddFirst(obj);
                return obj;
            }
        }

        public void Dispose()
        {
            lock (_lock)
            {
                foreach (var item in _disposables)
                {
                    item.SafeDispose();
                }

                _disposables.Clear();
            }
        }
    }

}
