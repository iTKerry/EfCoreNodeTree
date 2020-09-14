using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NodeTreeTest.DataAccess.Abstractions.Entities;
using NodeTreeTest.DataAccess.Abstractions.Enums;
using NodeTreeTest.DataAccess.Abstractions.ValueObjects;
using NodeTreeTest.DataAccess.EF;
using static NodeTreeTest.DataAccess.Abstractions.Entities.Application;

namespace NodeTreeTest.ConsoleApp
{
    internal static class Program
    {
        private static async Task Main()
        {
            var db = await GetDatabase();

            // seed database
            var initialRoot = InitialData();
            db.Attach(initialRoot);
            await db.SaveChangesAsync();

            // get entity from db
            var root = await db.Tokens
                .FirstOrDefaultAsync(x =>
                    x.NodeType == TokenNodeType.Root &&
                    x.Application == FirstApp);

            PrintTree(root, "root");
        }

        private static async Task<AppDbContext> GetDatabase()
        {
            var dbContext = new Startup(new ServiceCollection())
                .BuildProvider()
                .GetRequiredService<AppDbContext>();

            await dbContext.Database.EnsureDeletedAsync();
            await dbContext.Database.MigrateAsync();

            await dbContext.Applications.AddRangeAsync(AllApplications);
            await dbContext.SaveChangesAsync(true);
            
            return dbContext;
        }
        
        private static Token InitialData()
        {
            var a = Token.CreateRoot(Name.Create("aaa"), "a", "@a", FirstApp);
            var b = Token.CreateParameter(Name.Create("bbb"), "b", "@b", FirstApp);
            var d = Token.CreateParameter(Name.Create("ddd"), "d", "@d", FirstApp);
            var e = Token.CreateParameter(Name.Create("eee"), "e", "@e", FirstApp);

            var c = Token.CreateText(Name.Create("ccc"), "c", FirstApp);
            var f = Token.CreateText(Name.Create("fff"), "f", FirstApp);
            var g = Token.CreateText(Name.Create("ggg"), "g", FirstApp);
            var h = Token.CreateText(Name.Create("hhh"), "h", FirstApp);
            
            a.AttachChild(b);
            b.AttachChildren(c, d, e);
            d.AttachChildren(f, g);
            e.AttachChild(h);

            return a;
        }

        private static void PrintTree(Token token, string parameters, int depth = 1)
        {
            switch (token.NodeType)
            {
                case TokenNodeType.Root:
                case TokenNodeType.Parameter:
                    ProcessParameter(token, parameters, depth);
                    break;

                case TokenNodeType.Value:
                    ProcessValue(token, parameters, depth);
                    break;
                
                default:
                    throw new InvalidOperationException();
            }
        }

        private static void ProcessParameter(Token token, string parameters, int depth)
        {
            if (depth != 1 && token.NodeType == TokenNodeType.Root)
                throw new InvalidOperationException($"This is second root in a tree: {token.Name}");

            if (string.IsNullOrWhiteSpace(token.QueryParameter))
                throw new InvalidOperationException(
                    $"Root or Parameter node should contain parameter for: {token.Name.Value}");

            var newParams = $"{parameters}/{token.QueryParameter}";
            var result = $"params: {parameters} | name: {token.Name.Value} | query: {token.Query}";
            var depthChars = Enumerable.Range(0, depth - 1).Select(i => '-').ToArray();
            var path = token.ChildrenTokens.Any() ? depth != 1 ? "┐" : "-" : "-";
            var depthString = new string(depthChars.Skip(0).ToArray());

            Console.WriteLine($"└{depthString}{path} {result}");
            
            foreach (var childToken in token.ChildrenTokens)
            {
                PrintTree(childToken.Child, newParams, depth + 1);
            }
        }

        private static void ProcessValue(Token token, string parameters, int depth)
        {
            var result = $"params: {parameters} | name: {token.Name.Value} | query: {token.Query}";
            var depthChars = Enumerable.Range(0, depth - 1).Select(i => ' ').ToArray();
            var depthString = new string(depthChars.Take(depthChars.Length - 1).Append('└').ToArray());
            
            Console.WriteLine($" {depthString} {result}");
        }
    }
}