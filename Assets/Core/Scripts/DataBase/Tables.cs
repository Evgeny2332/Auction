using System;
using Newtonsoft.Json;

[Serializable]
public class User
{
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public int? Id { get; set; }

    public string FirstName { get; set; }
    public string LastName { get; set; }
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
    public string Status { get; set; }

    public int CreatorUserId { get; set; }
    public int BidderUserId { get; set; }
    public int CategoryId { get; set; }

    public int IconIndex { get; set; } // если используешь иконки
}
