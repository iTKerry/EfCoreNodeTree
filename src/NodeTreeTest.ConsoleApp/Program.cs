using System;
using System.Collections.Generic;
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
            
            var rootParams = new Dictionary<string, object>
            {
                {"@root", "root-value"}
            };
            
            var documentTokens = RecAsync(root, rootParams);

            await foreach (var res in documentTokens)
            {
                Console.WriteLine($"Token key: {res.Name} | TokenValue: {res.QueryResult}");
            }
        }

        private static IAsyncEnumerable<TokenResult> RecAsync(
            Token token, 
            Dictionary<string, object> parameters) =>
            token.NodeType switch
            {
                TokenNodeType.Root => ExecuteParameterToken(token, parameters),
                TokenNodeType.Parameter => ExecuteParameterToken(token, parameters),
                TokenNodeType.Value => ExecuteValueToken(token, parameters),
                _ => throw new InvalidOperationException()
            };

        private static async IAsyncEnumerable<TokenResult> ExecuteParameterToken(
            Token token, 
            Dictionary<string, object> parameters)
        {
            if (string.IsNullOrWhiteSpace(token.QueryParameter))
                throw new ArgumentException(nameof(token.QueryParameter));

            var queryResult = await SimulateSqlExecution(token.Query, parameters);
            parameters.Add(token.QueryParameter, queryResult);
            
            foreach (var childNode in token.ChildrenTokens)
            await foreach (var recResult in RecAsync(childNode.Child, parameters))
                yield return recResult;
        }
        
        private static async IAsyncEnumerable<TokenResult> ExecuteValueToken(
            Token token, 
            Dictionary<string, object> parameters)
        {
            var queryResult = await SimulateSqlExecution(token.Query, parameters);
            yield return new TokenResult(token.Name.Value, token.DocumentType.ToString(), queryResult);
        }

        private static async Task<string> SimulateSqlExecution(
            string query, 
            Dictionary<string, object> parameters)
        {
            var paramString = string.Join(" ", parameters.Select(p => $"[param: {p.Key} | query: {p.Value as string}]"));
            Console.WriteLine(paramString);
            
            var result = query;
            
            return await Task.FromResult(result);
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
            var a = Token.CreateRoot(Name.Create("token_a"), "SELECT a", "@a", FirstApp);
            var b = Token.CreateParameter(Name.Create("token_b"), "SELECT b", "@b", FirstApp);
            var d = Token.CreateParameter(Name.Create("token_d"), "SELECT d", "@d", FirstApp);
            var e = Token.CreateParameter(Name.Create("token_e"), "SELECT e", "@e", FirstApp);

            var c = Token.CreateText(Name.Create("token_c"), "SELECT c", FirstApp);
            var f = Token.CreateText(Name.Create("token_f"), "SELECT f", FirstApp);
            var g = Token.CreateText(Name.Create("token_g"), "SELECT g", FirstApp);
            var h = Token.CreateText(Name.Create("token_h"), "SELECT h", FirstApp);
            
            a.AttachChild(b);
            b.AttachChildren(c, d, e);
            d.AttachChildren(f, g);
            e.AttachChild(h);

            return a;
        }
    }

    internal class TokenResult
    {
        public TokenResult(string name, string type, string queryResult)
        {
            Name = name;
            Type = type;
            QueryResult = queryResult;
        }

        public string Name { get; }
        public string Type { get; }
        public string QueryResult { get; }
    }
}