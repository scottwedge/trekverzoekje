using System;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System.Timers;
using System.Threading.Tasks;

namespace trekverzoekje
{
    class Program
    {
        static int counter = 0;

        static async Task Main(string[] args)
        {
            const String collectionUri = "https://dev.azure.com/mmguide";

            // Get credentials from user
            VssCredentials creds = GetCreds();

            // Timer for refresh
            Timer timer = new Timer(5000);

            // Get connection object
            VssConnection connection = GetConnection(collectionUri, creds);

            // Get GitHttpClient object in order to communicate with Git endpoints
            GitHttpClient gitClient = connection.GetClient<GitHttpClient>();

            try
            {
                // First get of PR's
                await printPullRequests(gitClient);
            }
            catch (Exception e)
            {
                Console.WriteLine("probably entered wrong credentials, restart app pls");
            }

            // Automatic refresh of PR's
            timer.Elapsed += async (sender, e ) => await printPullRequests(gitClient);
            timer.Start();
            timer.AutoReset = true;

            // Exit app on key press
            Console.ReadKey();
        }

        static async Task printPullRequests(GitHttpClient gitClient)
        {
            // Get all PR's related to 'GeoSense' 
            var prs = await gitClient.GetPullRequestsByProjectAsync("GeoSense", null);
            var prCounter = 0;

            Console.WriteLine("Pull Requests: (refresh #)" + counter);
            Console.WriteLine("");
            foreach (var pr in prs)
            {
                Console.WriteLine("PR #" + prCounter + " " + pr.Title);
                Console.WriteLine(pr.Url);
                Console.WriteLine("");
                prCounter++;
            }

            Console.WriteLine("Automatic refresh every 5 sec. Press any key to exit... ");
            Console.WriteLine("");

            counter++;
        }

        static VssCredentials GetCreds()
        {
            // ask for credentials
            Console.WriteLine("Enter username, e.g., m.sitohang@mmguide.nl)");
            string user = Console.ReadLine();
            Console.WriteLine("Enter password, e.g, BestPasswordEver)");
            string pass = Console.ReadLine();

            // create creds objects with credential
            VssCredentials creds = new VssBasicCredential(user, pass);

            return creds;
        }

        static VssConnection GetConnection(string collectionUri, VssCredentials creds)
        {
            return new VssConnection(new Uri(collectionUri), creds);
        }
    }
}
