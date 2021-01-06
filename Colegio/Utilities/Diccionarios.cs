using System.Collections.Generic;

namespace Colegio.Utilities
{
    public static class Diccionarios
    {
        public static Dictionary<string, string> MostrarHoras()
        {
            Dictionary<string, string> horas = new Dictionary<string, string>
            {
                ["6_30-AM"] = "6:30 AM",
                ["7_00-AM"] = "7:00 AM",
                ["7_30-AM"] = "7:30 AM",
                ["8_00-AM"] = "8:00 AM",
                ["8_30-AM"] = "8:30 AM",
                ["9_00-AM"] = "9:00 AM",
                ["9_30-AM"] = "9:30 AM",
                ["10_00-AM"] = "10:00 AM",
                ["10_30-AM"] = "10:30 AM",
                ["11_00-AM"] = "11:00 AM",
                ["11_30-AM"] = "11:30 AM",
                ["12_00-PM"] = "12:00 PM",
                ["12_30-PM"] = "12:30 PM",
                ["1_00-PM"] = "1:00 PM",
                ["1_30-PM"] = "1:30 PM",
                ["2_00-PM"] = "2:00 PM",
                ["2_30-PM"] = "2:30 PM",
                ["3_00-PM"] = "3:00 PM",
                ["3_30-PM"] = "3:30 PM",
                ["4_00-PM"] = "4:00 PM",
                ["4_30-PM"] = "4:30 PM",
                ["5_00-PM"] = "5:00 PM",
                ["5_30-PM"] = "5:30 PM",
                ["6_00-PM"] = "6:00 PM",
            };
            return horas;
        }
        public static Dictionary<string, string> MostrarDias()
        {
            Dictionary<string, string> diasSemana = new Dictionary<string, string>
            {
                ["l"] = "Lunes",
                ["m"] = "Martes",
                ["x"] = "Miércoles",
                ["j"] = "Juéves",
                ["v"] = "Viernes",
                ["s"] = "Sábado",
            };
            return diasSemana;
        }

        public static Dictionary<string, string> MostrarMilitar()
        {
            Dictionary<string, string> militar = new Dictionary<string, string>
            {
                ["6_30-AM"] = "630",
                ["7_00-AM"] = "700",
                ["7_30-AM"] = "730",
                ["8_00-AM"] = "800",
                ["8_30-AM"] = "830",
                ["9_00-AM"] = "900",
                ["9_30-AM"] = "930",
                ["10_00-AM"] = "1000",
                ["10_30-AM"] = "1030",
                ["11_00-AM"] = "1100",
                ["11_30-AM"] = "1130",
                ["12_00-PM"] = "1200",
                ["12_30-PM"] = "1230",
                ["1_00-PM"] = "1300",
                ["1_30-PM"] = "1330",
                ["2_00-PM"] = "1400",
                ["2_30-PM"] = "1430",
                ["3_00-PM"] = "1500",
                ["3_30-PM"] = "1530",
                ["4_00-PM"] = "1600",
                ["4_30-PM"] = "1630",
                ["5_00-PM"] = "1700",
                ["5_30-PM"] = "1730",
                ["6_00-PM"] = "1800",
            };
            return militar;
        }
    }
}
