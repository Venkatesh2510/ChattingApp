using System;
using API.Entities;
using API.Helpers;

namespace API.Interfaces;

public interface ILikeRepository
{
    Task<MemberLike?> GetMemberLike(string sourceMemberId, string ttargetMemberId);
    Task<PaginatedResult<Member>> GetMemberLikes(LikeParams likeParams);
    Task<IReadOnlyList<string>> GetCurrentMemberLikeIds(string memberId);
    void DeleteLike(MemberLike like);
    void AddLike(MemberLike like);
    Task<bool> SaveAlChanges();
}
