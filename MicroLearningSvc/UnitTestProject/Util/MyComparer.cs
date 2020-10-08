using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestProject.Util
{
    static class MyComparer
    {
        class EquilityComparerImpl<A, B> : IComparer
        {
            private Func<A, B, bool> _comparison;

            public EquilityComparerImpl(Func<A, B, bool> comparison)
            {
                _comparison = comparison;
            }

            int IComparer.Compare(object x, object y)
            {
                if (x is A a1 && y is B b1)
                    return _comparison(a1, b1) ? 0 : 1;
                else if (y is A a2 && x is B b2)
                    return _comparison(a2, b2) ? 0 : 1;
                else
                    return -1;
            }
        }

        public static IComparer EquilityFor<A, B>(Func<A, B, bool> comparison)
        {
            return new EquilityComparerImpl<A, B>(comparison);
        }
    }
}
