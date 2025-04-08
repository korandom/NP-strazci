namespace App.Server.DTOs
{
    public class GenerateResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public IEnumerable<PlanDto> Plans { get; set; } = [];
    }
}
