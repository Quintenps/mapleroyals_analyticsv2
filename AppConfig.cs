using System.ComponentModel.DataAnnotations;

namespace MapleRoyalsPlayerCount;

public class AppConfig
{
    [Required]
    public string ServiceUrl { get; set; }
    [Required]
    public string Endpoint { get; set; }
    [Required]
    public string Cron { get; set; }
    [Required]
    public string UserAgent { get; set; }

    public string DisplayConfig()
    {
        return $"Configured application:\n Service URL {ServiceUrl},\n EndPoint {Endpoint},\n Cron {Cron},\n UserAgent {UserAgent}";
    }
}