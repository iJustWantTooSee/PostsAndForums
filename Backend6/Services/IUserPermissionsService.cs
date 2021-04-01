using System;
using Backend6.Models;

namespace Backend6.Services
{
    public interface IUserPermissionsService
    {
        Boolean CanEditPost(Post post);

        Boolean CanEditForumTopic(ForumTopic post);

        Boolean CanEditForumMessage(ForumMessage post);

        Boolean CanEditPostComment(PostComment postComment);
    }
}