using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstructuraDatos__RPN
{
    public interface IStack<T>
    {
        //push, pop, peek, size, isEmpty

        int Size { get; }

        bool Empty { get; }

        void Push(T element);

        T Pop();

        T Peek();
    }
}
