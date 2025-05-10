using System;
using Newtonsoft.Json;

[Serializable]
public class User
{
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public int? Id { get; set; }

    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Login { get; set; }
    public string Password { get; set; }
}

[Serializable]
public class Category
{
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public int? Id { get; set; }

    public string Name { get; set; }
}

[Serializable]
public class Lot
{
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public int? Id { get; set; }

    public string Name { get; set; }
    public string Description { get; set; }
    public int? Bet { get; set; }
    public string Status { get; set; }
    public DateTime? EndData { get; set; }
    public int CreatorUserId { get; set; }
    public int? BidderUserId { get; set; }
    public int CategoryId { get; set; }
}
