using System;
using System.Collections.Generic;
using System.Linq;
using NodeTreeTest.DataAccess.Abstractions.Base;
using NodeTreeTest.DataAccess.Abstractions.Enums;
using NodeTreeTest.DataAccess.Abstractions.ValueObjects;

#nullable enable

namespace NodeTreeTest.DataAccess.Abstractions.Entities
{
    public class Token : Entity
    {
        public Name Name { get; }
        public string Query { get; }
        public string? QueryParameter { get; }
        public TokenDocumentType? DocumentType { get; }
        public TokenNodeType NodeType { get; }
        
        public virtual Application Application { get; }
        
        private readonly List<TokenNode> _childrenTokens = new List<TokenNode>();
        public virtual IReadOnlyList<TokenNode> ChildrenTokens => _childrenTokens;
        
        private readonly List<TokenNode> _parentTokens = new List<TokenNode>();
        public IReadOnlyList<TokenNode> ParentTokens => _parentTokens;

        protected Token()
        {
        }
        
        private Token(Name name, string query, string queryParameter,
            TokenDocumentType? documentType, 
            TokenNodeType nodeType, 
            Application application)
        {
            Name = name;
            Query = query;
            QueryParameter = queryParameter;
            DocumentType = documentType;
            NodeType = nodeType;
            Application = application;
        }

        public static Token Create(Name name, string query, TokenDocumentType documentType, Application application)
        {
            if (name is null)
                throw new ArgumentNullException(nameof(name));
            if (application is null)
                throw new ArgumentNullException(nameof(application));

            return new Token(name, query, string.Empty, documentType, TokenNodeType.Value, application);
        }

        public static Token CreateText(Name name, string query, Application application)
        {
            if (name is null)
                throw new ArgumentNullException(nameof(name));
            if (application is null)
                throw new ArgumentNullException(nameof(application));

            return new Token(name, query, string.Empty, TokenDocumentType.Text, TokenNodeType.Value, application);
        }
        
        public static Token CreateParameter(Name name, string query, string queryParam, Application application)
        {
            if (name is null)
                throw new ArgumentNullException(nameof(name));
            if (application is null)
                throw new ArgumentNullException(nameof(application));
            
            return new Token(name, query, queryParam, null, TokenNodeType.Parameter, application);
        }

        public static Token CreateRoot(Name name, string query, string queryParam, Application application)
        {
            if (name is null)
                throw new ArgumentNullException(nameof(name));
            if (application is null)
                throw new ArgumentNullException(nameof(application));
            
            return new Token(name, query, queryParam, null, TokenNodeType.Root, application);
        }
        
        public void SetParent(Token token)
        {
            if (token is null)
                throw new ArgumentNullException(nameof(token));
            
            var node = TokenNode.Create(token, this);

            _parentTokens.Add(node);
        }

        public void AttachChildren(params Token[] tokens) =>
            tokens.ToList().ForEach(AttachChild);
        
        public void AttachChild(Token token)
        {
            if (token is null)
                throw new ArgumentNullException(nameof(token));
            
            var node = TokenNode.Create(this, token);

            _childrenTokens.Add(node);
        }
    }
}