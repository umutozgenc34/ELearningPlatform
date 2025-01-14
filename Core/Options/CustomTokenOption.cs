﻿namespace Core.Options;

public class CustomTokenOption
{
    public List<string> Audience { get; set; }
    public string Issuer { get; set; }
    public int AccessTokenExpiration { get; set; }
    public string SecurityKey { get; set; }
}
