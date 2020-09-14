using System;
using CSharpFunctionalExtensions;

namespace NodeTreeTest.DataAccess.Abstractions.ValueObjects
{
    public class Name : ValueObject<Name>
    {
        private Name(string value)
        {
            Value = value;
        }

        public static Name Create(string value) =>
            value switch
            {
                _ when value is null => 
                    throw new ArgumentNullException(value),
                
                _ when value.Length < 3 => 
                    throw new ArgumentException("Name value is less than 3 letters", nameof(value)),
                
                _ => new Name(value)
            };

        public string Value { get; }
        
        protected override bool EqualsCore(Name other)
        {
            return Value == other.Value;
        }

        protected override int GetHashCodeCore()
        {
            return Value.GetHashCode();
        }
    }
}