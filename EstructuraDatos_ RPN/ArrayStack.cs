using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstructuraDatos__RPN
{
    public class ArrayStack<T> : IStack<T>
    {
        //Variables privadas, Propiedades, Metodos
        //Constantes con MAYUSCULAS
        private const int INITIAL_CAPACITY = 5;

        private T[] data;

        public int Capacity { get; private set; }
        //Se puede privatizar set para escribir el valor sin necesidad de darle acceso publico.
        public int Size { get; private set; }

        public bool Empty => Size == 0;

        public ArrayStack()
        {
            data = new T[INITIAL_CAPACITY];
            Capacity = INITIAL_CAPACITY;
            Size = 0;
        }

        public ArrayStack(int capacity)
        {
            data = new T[capacity];
            Capacity = capacity;
            Size = 0;
        }

        public ArrayStack(ArrayStack<T> stack)
        {
            //Para evitar que se referencie el stack anterior, creamos una copia utilizando arreglos internos de la misma capacidad del stack anterior, Copiando el array con el Copy(arregloOriginal, arregloDestino, el tamaño del arreglo).
            data = new T[stack.Capacity];
            Array.Copy(stack.data, data, stack.Size);
            Capacity = stack.Capacity;
            Size = stack.Size;
        }

        public void Push(T element)
        {
            //Validar si está lleno
            if (Size == Capacity)
            {
                Capacity *= 2;
                Array.Resize(ref data, Capacity);
                //throw new IndexOutOfRangeException("Full stack");
            }

            data[Size++] = element;
        }

        public T Pop()
        {
            if (Empty)
            {
                throw new IndexOutOfRangeException("No elements in stack");
            }

            if (Size <= Capacity / 5)
            {
                Capacity /= 2;
                Array.Resize(ref data, Capacity);
            }

            // --Size Primero decrementa, luego lo usa
            // Size-- Primero lo usa y luego le quita
            return data[--Size];
        }

        public T Peek()
        {
            if (Empty)
            {
                throw new IndexOutOfRangeException("No elements in stack");
            }

            return data[Size - 1];
        }

        public void Clear()
        {
            Array.Clear(data, 0, Capacity);
            Size = 0;
        }

        public string GetDataText()
        {

            return $"data = [{String.Join(", ", data.Take(Size))}]";
        }
#if DEBUG

#endif
    }
}
