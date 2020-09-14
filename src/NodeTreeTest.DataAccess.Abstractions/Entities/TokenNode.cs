using System;
using NodeTreeTest.DataAccess.Abstractions.Base;

namespace NodeTreeTest.DataAccess.Abstractions.Entities
{
    public class TokenNode : Entity
    {
        public virtual Token Parent { get; }
        public virtual Token Child { get; }

        protected TokenNode()
        {
        }

        private TokenNode(Token parent, Token child)
        {
            Parent = parent;
            Child = child;
        }
        
        public static TokenNode Create(Token parent, Token child)
        {
            if (parent is null)
                throw new ArgumentNullException(nameof(parent));
            
            if (child is null)
                throw new ArgumentNullException(nameof(child));

            if (parent.Id != 0 && child.Id != 0 && parent == child)
                throw new ArgumentException("Token cannot be related to self"); 
            
            return new TokenNode(parent, child);
        }
    }
}