namespace ServiceCenter.Models
{
    public static class EmployeePositions
    {
        public const string SeniorMechanic = "Старший механик";
        public const string Mechanic = "Механик";
        public const string Electrician = "Автоэлектрик";
        public const string MechanicDiagnostician = "Механик-диагност";

        public static List<string> GetAll()
        {
            return new List<string>
            {
                SeniorMechanic,
                Mechanic,
                Electrician,
                MechanicDiagnostician
            };
        }
    }
}