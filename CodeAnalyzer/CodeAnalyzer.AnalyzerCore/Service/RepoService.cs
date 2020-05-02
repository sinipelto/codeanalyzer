using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security;
using System.Xml;
using CodeAnalyzer.AnalyzerCore.Interface;
using CodeAnalyzer.AnalyzerCore.Model;
using LibGit2Sharp;

namespace CodeAnalyzer.AnalyzerCore.Service
{
    public class RepoService : IRepoService
    {
        private readonly string _basePath;

        public RepoService(string basePath)
        {
            // Ensure path ending slash
            _basePath = basePath ?? throw new ArgumentNullException(nameof(basePath));
        }

        private const string ZipName = "master.zip";
        private const string ArchiveUrl = "/archive/" + ZipName;

        public List<GitRepository> EnumerateRepositories(List<string> repoUrls)
        {
            var repos = new List<GitRepository>();

            foreach (var repoUrl in repoUrls)
            {
                var segm = new Uri(repoUrl).Segments;
                var name = segm[^1];

                var repo = new GitRepository
                {
                    Name = name,
                    Url = repoUrl,
                };

                repos.Add(repo);
            }

            return repos;
        }

        public PomFile FetchPomDetails(string projectPath)
        {
            var xmlPath = Path.Combine(_basePath, projectPath, "pom.xml");
            
            var pom = new PomFile();

            var doc = new XmlDocument();

            doc.Load(xmlPath);

            foreach (var node in doc.GetElementsByTagName("project").Item(0).Cast<XmlNode>())
            {
                switch (node.Name)
                {
                    case "artifactId":
                        pom.ArtifactId = node.InnerText;
                        break;
                    case "groupId":
                        pom.GroupId = node.InnerText;
                        break;
                    case "name":
                        pom.Name = node.InnerText;
                        break;
                }
            }
            
            return pom;
        }

        public void DownloadRepository(GitRepository repo)
        {
            var url = repo.Url + ArchiveUrl;
            
            var archPath = Path.Combine(_basePath, repo.Name + ".zip");

            using var client = new WebClient();

            // Download the zip file from the git repo url
            client.DownloadFile(url, archPath);

            // Extract the master zip file to the target directory
            ZipFile.ExtractToDirectory(archPath, _basePath);
        }

        public void CloneRepository(string repoUrl, string repoName)
        {
            var path = Path.Combine(_basePath, repoName);

            if (new Uri(repoUrl).Host.Contains("gitlab"))
            {
                // Gitlab repo needs credentials. This function also clones.
                GetGitlabCredentials(repoUrl, path);
            }
            else
            {
                // Not Gitlab (so hopefully Github).
                Repository.Clone(repoUrl, path);
            }

            if (!Repository.IsValid(path))
                throw new DirectoryNotFoundException("Cloned repository not valid.");
        }

        private void GetGitlabCredentials(string repoUrl, string path)
        {
            var tryAgain = true;
            while (tryAgain)
            {
                Console.Write("Enter Gitlab username: ");
                var gitUsername = Console.ReadLine();
                var gitPassword = GetPasswordInput();
                var cloneOptions = GetCloneOptions(gitUsername, gitPassword);

                try
                {
                    Repository.Clone(repoUrl, path, cloneOptions);
                    tryAgain = false;
                }
                catch (LibGit2SharpException e)
                {
                    Console.WriteLine("Error: " + e.Message);
                    Console.WriteLine("Error cloning repo. Check username and password.");
                }
                finally
                {
                    gitPassword.Dispose();
                }
            }
        }

        private CloneOptions GetCloneOptions(string username, SecureString password)
        {
            return new CloneOptions
            {
                CredentialsProvider = (url, user, cred) =>
                    new UsernamePasswordCredentials
                    {
                        Username = username,
                        Password = new NetworkCredential(string.Empty, password).Password
                    }
            };
            //return options;
        }

        private SecureString GetPasswordInput()
        {
            var password = new SecureString();
            
            Console.Write("Enter Gitlab password: ");
            ConsoleKeyInfo key = Console.ReadKey(true);

            while (key.Key != ConsoleKey.Enter)
            {
                if (key.Key != ConsoleKey.Backspace)
                {
                    password.AppendChar(key.KeyChar);
                    Console.Write("*");
                }
                else
                {
                    if (password.Length != 0)
                    {
                        password.RemoveAt(password.Length - 1);
                        Console.Write("\b \b");
                    }
                }
                key = Console.ReadKey(true);
            }
            Console.WriteLine("");
            return password;
        }

        private static void EmptyFolderRecursively(string path)
        {
            var dir = new DirectoryInfo(path);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException("Directory does not exist.");
            }

            IEnumerable<DirectoryInfo> dirList;
            IEnumerable<FileInfo> fileList;

            try
            {
                dirList = dir.EnumerateDirectories();
                fileList = dir.EnumerateFiles();
            }
            catch (Exception)
            {
                dir.Delete();
                return;
            }

            foreach (var directory in dirList)
            {
                EmptyFolderRecursively(directory.FullName);
                try
                {
                    directory.Delete();
                }
                catch (Exception)
                {
                    // ignore
                }
            }

            foreach (var file in fileList)
            {
                File.SetAttributes(file.FullName, RemoveAttribute(file.Attributes, FileAttributes.ReadOnly));
                file.Delete();
            }
        }

        private static FileAttributes RemoveAttribute(FileAttributes att, FileAttributes removeAtt)
        {
            return att & ~removeAtt;
        }

        public void CleanAllRepositories()
        {
            // Remove any old extracted zip files
            EmptyFolderRecursively(_basePath);
        }
    }
}