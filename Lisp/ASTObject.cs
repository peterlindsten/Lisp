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


    public interface AstAtom : AstObject, IRepresentable
    {
    }

    public interface IRepresentable
    {
        string Represent();
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