using System.Collections.Generic;

namespace Lisp
{
    public class AstObject
    {
    }

    public class AstList : AstObject
    {
        public readonly List<AstObject> Objects = new List<AstObject>();

        public void Add(AstObject ao)
        {
            Objects.Add(ao);
        }
    }

    public class AstAtom : AstObject
    {
        public readonly string Text;

        public AstAtom(string text)
        {
            Text = text;
        }
    }
}