using System;

namespace API.Helpers;

public class LikeParams : PagingParams
{
    public string MemberId { get; set; } = "";
    public string Predicate { get; set; } = "liked";
}
