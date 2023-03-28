namespace ConnectFour.Api.Models;

public class ChallengeModel
{
    /// <summary>
    /// Id of the user that should be challenged
    /// </summary>
    public Guid TargetUserId { get; set; }
}