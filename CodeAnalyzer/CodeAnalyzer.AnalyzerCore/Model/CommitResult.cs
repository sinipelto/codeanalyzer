using System;

namespace CodeAnalyzer.AnalyzerCore.Model
{
    public class CommitResult
    {
        public string Sha { get; set; }
        public string NodeId { get; set; }
        public Commit Commit { get; set; }
        public string Url { get; set; }
        public string HtmlUrl { get; set; }
        public string CommentsUrl { get; set; }
        public AuthorBig Author { get; set; }
        public CommitterBig Committer { get; set; }
        public Parent[] Parents { get; set; }
    }

    public class Commit
    {
        public Author Author { get; set; }
        public Committer Committer { get; set; }
        public string Message { get; set; }
        public Tree Tree { get; set; }
        public string Url { get; set; }
        public int CommentCount { get; set; }
        public Verification Verification { get; set; }
    }

    public class Author
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime Date { get; set; }
    }

    public class Committer
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime Date { get; set; }
    }

    public class Tree
    {
        public string Sha { get; set; }
        public string Url { get; set; }
    }

    public class Verification
    {
        public bool Verified { get; set; }
        public string Reason { get; set; }
        public string Signature { get; set; }
        public string Payload { get; set; }
    }

    public class AuthorBig
    {
        public string Login { get; set; }
        public int Id { get; set; }
        public string NodeId { get; set; }
        public string AvatarUrl { get; set; }
        public string GravatarId { get; set; }
        public string Url { get; set; }
        public string HtmlUrl { get; set; }
        public string FollowersUrl { get; set; }
        public string FollowingUrl { get; set; }
        public string GistsUrl { get; set; }
        public string StarredUrl { get; set; }
        public string SubscriptionsUrl { get; set; }
        public string OrganizationsUrl { get; set; }
        public string ReposUrl { get; set; }
        public string EventsUrl { get; set; }
        public string ReceivedEventsUrl { get; set; }
        public string Type { get; set; }
        public bool SiteAdmin { get; set; }
    }

    public class CommitterBig
    {
        public string Login { get; set; }
        public int Id { get; set; }
        public string NodeId { get; set; }
        public string AvatarUrl { get; set; }
        public string GravatarId { get; set; }
        public string Url { get; set; }
        public string HtmlUrl { get; set; }
        public string FollowersUrl { get; set; }
        public string FollowingUrl { get; set; }
        public string GistsUrl { get; set; }
        public string StarredUrl { get; set; }
        public string SubscriptionsUrl { get; set; }
        public string OrganizationsUrl { get; set; }
        public string ReposUrl { get; set; }
        public string EventsUrl { get; set; }
        public string ReceivedEventsUrl { get; set; }
        public string Type { get; set; }
        public bool SiteAdmin { get; set; }
    }

    public class Parent
    {
        public string Sha { get; set; }
        public string Url { get; set; }
        public string HtmlUrl { get; set; }
    }
}