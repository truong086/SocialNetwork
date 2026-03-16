using Microsoft.AspNetCore.SignalR;

namespace SocialNetwork.ChatHub
{
    public class CommentHub : Hub
    {
        public async Task JoinPost(string postId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, postId);
        }

        public async Task LeavePost(string postId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, postId);
        }
    }
}
