using NodeTreeTest.DataAccess.Abstractions.Base;

namespace NodeTreeTest.DataAccess.Abstractions.Entities
{
    public class Application : Entity
    {
        public static readonly Application FirstApp = 
            new Application(1, "first-app");
        public static readonly Application SecondApp = 
            new Application(2, "second-app");

        public static readonly Application[] AllApplications = 
            {FirstApp, SecondApp};
        
        public string Name { get; }
        
        protected Application() {}
        
        private Application(int id, string name) 
            : base(id) => 
            Name = name;
    }
}