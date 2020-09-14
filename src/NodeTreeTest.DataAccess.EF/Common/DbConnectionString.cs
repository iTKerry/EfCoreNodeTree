using System;

namespace NodeTreeTest.DataAccess.EF.Common
{
    public sealed class DbConnectionString
    {
        public DbConnectionString()
        {
            Value = "Server=localhost,1433;Database=NodeTreeTest;User=SA;Password=Your_password123;";
        }
        
        public DbConnectionString(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException(nameof(value));
            
            Value = value;
        }

        public string Value { get; }
    }
}