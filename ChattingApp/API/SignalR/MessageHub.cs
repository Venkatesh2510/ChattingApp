using System;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR;

public class MessageHub(IMessageRepository messageRepository, IMemberRepository memberRepository): Hub
{
    override public async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var otherUser = httpContext?.Request.Query["userId"].ToString() ?? throw new Exception("Other user not found");

        var groupName = GetGroupName(GetUserId(), otherUser);
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

        var messages = await messageRepository.GetMessageThread(GetUserId(), otherUser);

        await Clients.Group(groupName).SendAsync("ReceiveMessageThread", messages);
    }

    public async Task SendMessage(CreateMessageDto createMessageDto)
    {
        var sender = await memberRepository.GetMemberByIdAsync(GetUserId());
        var recipient = await memberRepository.GetMemberByIdAsync(createMessageDto.RecipientId);

        if (recipient == null || sender == null || sender.Id == createMessageDto.RecipientId)
        {
            throw new HubException("Cannot send this message");
        }

        var message = new Message
        {
            SenderId = sender.Id,
            RecipientId = recipient.Id,
            Content = createMessageDto.Content
        };

        messageRepository.AddMessage(message);

        if (await messageRepository.SaveAllAsync())
        {
            var group = GetGroupName(sender.Id, recipient.Id);
            await Clients.Group(group).SendAsync("NewMessage", message.ToDto());
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var httpContext = Context.GetHttpContext();
        var otherUser = httpContext?.Request.Query["userId"].ToString() ?? throw new Exception("Other user not found");

        var groupName = GetGroupName(GetUserId(), otherUser);
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

        await base.OnDisconnectedAsync(exception);
    }

    private string GetUserId()
    {
        return Context.User?.GetMemberId()
            ?? throw new Exception("User not found");
    }

    private static string GetGroupName(string? caller, string? other)
    {
        var stringCompare = string.CompareOrdinal(caller, other) < 0;
        return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
    }
}
