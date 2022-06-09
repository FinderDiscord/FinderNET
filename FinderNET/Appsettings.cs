public class Appsettings {
    public string token { get; set; } = "paste token here";
    public ulong testGuild { get; set; } = 0;
    public ConnectionStrings ConnectionStrings { get; set; } = new ConnectionStrings();
}

public class ConnectionStrings {
    public string Default { get; set; } = "";
}