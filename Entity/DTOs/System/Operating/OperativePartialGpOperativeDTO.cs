namespace Entity.DTOs.System.Operating
{
    public class OperativePartialGpOperativeDTO
    {
        public int Id { get; set; }
        public int OperativeGroupId { get; set; }

        //Envio Email
        public string GroupName { get; set; } = string.Empty;
        public DateTimeOffset DateStart { get; set; }
        public DateTimeOffset DateEnd { get; set; }
    }
}
