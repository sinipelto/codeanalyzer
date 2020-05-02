using System;

namespace CodeAnalyzer.LocalTester
{
    public class CommitTester
    {
        public void TestCommitListing()
        {
            Console.WriteLine("STARTING COMMIT LISTING TEST...");
            Console.WriteLine("EXITING COMMIT LISTING TEST...");
        }

        private const string TestRepoUrl = "/repos/jenkins-docs/simple-java-maven-app";
    }
}
