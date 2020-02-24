using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace GitAutoPR
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Git AutoPR");
            string user = "MaximRouiller";
            string repo = "gitautomation";
            var date = DateTime.UtcNow.ToString("yyyyMMdd-HHmmss");
            string branchName = $"whats-new-{date}";
            string filename = $"hello-world-{date}.md";
            string githubToken = Environment.GetEnvironmentVariable("GITHUB_TOKEN", EnvironmentVariableTarget.Process);

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", githubToken);
            client.DefaultRequestHeaders.UserAgent.Add(new System.Net.Http.Headers.ProductInfoHeaderValue("GitAutoPR", "1.0"));
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            client.BaseAddress = new Uri("https://api.github.com");

            //get head
            var headResponse = await client.GetAsync($"/repos/{user}/{repo}/git/ref/heads/master");
            var @ref = JsonSerializer.Deserialize<Ref>(await headResponse.Content.ReadAsStringAsync());

            // create a branch
            await CreateBranch(user, repo, branchName, client, @ref.@object.sha);


            // create a file
            byte[] fileContentAsBytes = Encoding.UTF8.GetBytes("# Hello World");
            string base64Converted = Convert.ToBase64String(fileContentAsBytes);
            await CreateFile(user, repo, client, branchName, base64Converted, filename);

            // create a PR
            var prPayload = new { title = $"Automated: Adding {filename} to repository", head = branchName, @base = "master" };
            var prResponse = await client.PostAsync($"/repos/{user}/{repo}/pulls", new StringContent(JsonSerializer.Serialize(prPayload), Encoding.UTF8, "application/json"));
            prResponse.EnsureSuccessStatusCode();
        }

        private static async Task CreateBranch(string user, string repo, string branchName, HttpClient client, string sha)
        {
            var branchPayload = JsonSerializer.Serialize(new
            {
                @ref = $"refs/heads/{branchName}",
                sha = sha
            });

            var createBranchResponse = await client.PostAsync($"/repos/{user}/{repo}/git/refs", new StringContent(branchPayload, Encoding.UTF8, "application/json"));
            createBranchResponse.EnsureSuccessStatusCode();
        }

        private static async Task CreateFile(string user, string repo, HttpClient client, string branchName, string base64Converted, string filename)
        {
            var contentPayload = JsonSerializer.Serialize(new { message = $"adding {filename}", content = base64Converted, branch = branchName });
            var createFileResponse = await client.PutAsync($"/repos/{user}/{repo}/contents/{filename}", new StringContent(contentPayload, Encoding.UTF8, "application/json"));
            createFileResponse.EnsureSuccessStatusCode();
        }
    }
}
