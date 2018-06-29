using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Lisp
{
    public class AstNumber : AstAtom
    {
        private readonly int i;
        private readonly double d;
        private readonly Type t;

        public static AstNumber operator +(AstNumber left, AstNumber right)
        {
            return new AstNumber(left.Value + right.Value);
        }

        public static AstNumber operator *(AstNumber left, AstNumber right)
        {
            return new AstNumber(left.Value * right.Value);
        }

        public static AstNumber operator -(AstNumber left, AstNumber right)
        {
            return new AstNumber(left.Value - right.Value);
        }

        public static AstNumber operator -(AstNumber right)
        {
            return new AstNumber(-right.Value);
        }

        public static AstNumber operator /(AstNumber left, AstNumber right)
        {
            dynamic l;
            if (left.t is Type.Double ||
                right.t is Type.Double ||
                left.Value % right.Value == 0) // Integer division OK
                l = left.Value;
            else
                l = (double) left.Value;
            return new AstNumber(l / right.Value);
        }

        public static AstBoolean operator >(AstNumber left, AstNumber right)
        {
            return new AstBoolean(left.Value > right.Value);
        }

        public static AstBoolean operator <(AstNumber left, AstNumber right)
        {
            return new AstBoolean(left.Value < right.Value);
        }

        public AstNumber() : this(0)
        {
        }

        public AstNumber(double d)
        {
            t = Type.Double;
            this.d = d;
        }

        public AstNumber(int i)
        {
            t = Type.Integer;
            this.i = i;
        }

        private dynamic Value
        {
            get
            {
                switch (t)
                {
                    case Type.Integer:
                        return i;
                    case Type.Double:
                        return d;
                    default:
                        throw new Exception("AstNumber was not of known type!");
                }
            }
        }


        public string Represent()
        {
            return Value.ToString();
        }

        private enum Type
        {
            Integer,
            Double
        }

        protected bool Equals(AstNumber other)
        {
            switch (t)
            {
                case Type.Integer:
                    return t == other.t && i == other.i;
                case Type.Double:
                    return t == other.t && d.Equals(other.d);
                default:
                    throw new Exception("AstNumber was not of known type!");
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AstNumber) obj);
        }

        public override int GetHashCode()
        {
            switch (t)
            {
                case Type.Integer:
                    return HashCode.Combine(t, i);
                case Type.Double:
                    return HashCode.Combine(t, i);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public static class AstHelpers
    {
        public static AstNumber Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, AstNumber> selector)
        {
            if (source == null)
                throw new ArgumentNullException();
            if (selector == null)
                throw new ArgumentNullException();
            var num = new AstNumber();
            foreach (var source1 in source)
                num += selector(source1);
            return num;
        }

        public static AstNumber Multiply<TSource>(this IEnumerable<TSource> source, Func<TSource, AstNumber> selector)
        {
            if (source == null)
                throw new ArgumentNullException();
            if (selector == null)
                throw new ArgumentNullException();
            var num = new AstNumber(1);
            foreach (var source1 in source)
                num *= selector(source1);
            return num;
        }

        public static AstNumber Subtract<TSource>(this List<TSource> source, Func<TSource, AstNumber> selector)
            where TSource : AstObject
        {
            if (source == null)
                throw new ArgumentNullException();
            if (selector == null)
                throw new ArgumentNullException();
            var first = selector(source.First());
            if (source.Count == 1)
                return -first;
            return source.Skip(1).Aggregate(first, (current, next) => current - selector(next));
        }

        public static AstNumber Divide<TSource>(this List<TSource> source, Func<TSource, AstNumber> selector)
            where TSource : AstObject
        {
            if (source == null)
                throw new ArgumentNullException();
            if (selector == null)
                throw new ArgumentNullException();
            var first = selector(source.First());
            if (source.Count == 1)
                return new AstNumber(1.0) / first;
            return source.Skip(1).Aggregate(first, (current, next) => current / selector(next));
        }

        public static AstBoolean GreaterThen<TSource>(this List<TSource> source, Func<TSource, AstNumber> selector)
            where TSource : AstObject
        {
            return AggregateMinimum2(source, selector, (a, b) => a > b);
        }
        
        public static AstBoolean LessThen<TSource>(this List<TSource> source, Func<TSource, AstNumber> selector)
            where TSource : AstObject
        {
            return AggregateMinimum2(source, selector, (a, b) => a < b);
        }

        private static AstBoolean AggregateMinimum2<TSource>(this IReadOnlyCollection<TSource> source,
            Func<TSource, AstNumber> selector, Func<AstNumber, AstNumber, AstBoolean> comp)
        {
            if (source == null)
                throw new ArgumentNullException();
            if (selector == null)
                throw new ArgumentNullException();
            if (source.Count < 2)
                throw new Exception("Too few args");
            var q = new Queue<TSource>(source);

            var previous = selector(q.Dequeue());
            var ret = new AstBoolean(false);
            while (q.Count > 0)
            {
                var current = selector(q.Dequeue());
                ret = comp(previous, current);
                previous = current;
            }

            return ret;
        }
    }
}