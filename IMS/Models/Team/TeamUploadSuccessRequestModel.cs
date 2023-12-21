namespace IMS.Models.Team;

public class TeamUploadSuccessRequestModel
{
    public int Tid { get; set; }
    public int Uid { get; set; }
    public string FileName { get; set; } = "";
    public string FilePath { get; set; } = "";
}