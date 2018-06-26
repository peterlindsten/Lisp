using System;
using System.Collections.Generic;
using System.Linq;

namespace Lisp
{
    public interface AstObject
    {
    }

    public class AstList : AstObject
    {
        public readonly List<AstObject> Objects = new List<AstObject>();

        public AstList(params AstObject[] aos) =>
            Objects.AddRange(aos);

        public AstList(IEnumerable<AstObject> aos) =>
            Objects.AddRange(aos);

        public void Add(AstObject ao)
        {
            Objects.Add(ao);
        }

        public override bool Equals(object obj) =>
            obj is AstList al && Objects.SequenceEqual(al.Objects);

        public override int GetHashCode()
        {
            return HashCode.Combine(Objects);
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

        public static AstBoolean LargerThen<TSource>(this List<TSource> source, Func<TSource, AstNumber> selector)
            where TSource : AstObject
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
                ret = previous > current;
                previous = current;
            }

            return ret;
        }
    }

    public interface AstAtom : AstObject, IRepresentable
    {
    }

    public interface IRepresentable
    {
        string Represent();
    }

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


    public class AstSymbol : AstAtom
    {
        public readonly string Text;

        public AstSymbol(string text)
        {
            Text = text;
        }

        public override bool Equals(object obj) =>
            obj is AstSymbol a && Text.Equals(a.Text);

        public override int GetHashCode()
        {
            return HashCode.Combine(Text);
        }

        public string Represent()
        {
            return Text;
        }
    }

    public class AstBoolean : AstAtom
    {
        public readonly bool Value;

        public AstBoolean(bool b)
        {
            Value = b;
        }


        public static bool operator ==(AstBoolean ab, AstObject ao)
        {
            if (ao is AstBoolean other)
                return ab != null && ab.Value == other.Value;
            return false;
        }

        public static bool operator !=(AstBoolean ab, AstObject ao)
        {
            return !(ab == ao);
        }

        protected bool Equals(AstBoolean other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((AstBoolean) obj);
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        public string Represent()
        {
            return Value ? "#t" : "#f";
        }
    }

    public class AstFunc : AstObject
    {
        public readonly Func<List<AstObject>, AstObject> f;

        public AstFunc(Func<List<AstObject>, AstObject> f)
        {
            this.f = f;
        }
    }
}