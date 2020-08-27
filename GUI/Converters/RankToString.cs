using Database.DatabaseModels;
using System;
using System.Globalization;
using System.Windows.Data;

namespace GUI.Converters
{
    class RankToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool full = (parameter as string) == "Full";
            return value switch
            {
                null => "",
                Rank rank => rank switch
                {
                    #region Rank -> string
                    Rank.MR => full ? "Mister" : "MR",
                    Rank.MS => full ? "Miss" : "MS",
                    Rank.MRS => full ? "Missus" : "MRS",
                    Rank.OTHER => full ? "Other" : "OTHER",
                    Rank.REC => full ? "Recruit" : "REC",
                    Rank.PTE => full ? "Private" : "PTE",
                    Rank.CPL => full ? "Corporal" : "CPL",
                    Rank.CFC => full ? "Corporal (First Class)" : "CFC",
                    Rank.CADET_SGT => full ? "Sergeant Cadet" : "SCT",
                    Rank.THIRD_SGT => full ? "Third Sergeant" : "3SG",
                    Rank.SECOND_SGT => full ? "Second Sergeant" : "2SG",
                    Rank.FIRST_SGT => full ? "First Sergeant" : "1SG",
                    Rank.STAFF_SGT => full ? "Staff Sergeant" : "SSG",
                    Rank.MASTER_SGT => full ? "Master Sergeant" : "MSG",
                    Rank.THIRD_WO => full ? "Third Warrant Officer" : "3WO",
                    Rank.SECOND_WO => full ? "Second Warrant Officer" : "2WO",
                    Rank.FIRST_WO => full ? "First Warrant Officer" : "1WO",
                    Rank.MASTER_WO => full ? "Master Warrant Officer" : "MWO",
                    Rank.SENIOR_WO => full ? "Senior Warrant Officer" : "SWO",
                    Rank.CHIEF_WO => full ? "Chief Warrant Officer" : "CWO",
                    Rank.OCT => full ? "Officer Cadet" : "OCT",
                    Rank.SECOND_LT => full ? "Second Lieutenant" : "2LT",
                    Rank.LTA => full ? "Lieutenant" : "LTA",
                    Rank.CPT => full ? "Captain" : "CPT",
                    Rank.MAJ => full ? "Major" : "MAJ",
                    Rank.LTC => full ? "Lieutenant-Colonel" : "LTC",
                    Rank.SLTC => full ? "Senior Lieutenant-Colonel" : "SLTC",
                    Rank.COL => full ? "Colonel" : "COL",
                    Rank.BG => full ? "Brigadier-General" : "BG",
                    Rank.MG => full ? "Major-General" : "MG",
                    Rank.LG => full ? "Lieutenant-General" : "LG",
                    Rank.GEN => full ? "General" : "GEN",
                    Rank.ME1_T => full ? "Military Expert 1 (Trainee)" : "ME1(T)",
                    Rank.ME1_A => full ? "Military Expert 1 (Apprentice)" : "ME1(A)",
                    Rank.ME1 => full ? "Military Expert 1" : "ME1",
                    Rank.ME2 => full ? "Military Expert 2" : "ME2",
                    Rank.ME3 => full ? "Military Expert 3" : "ME3",
                    Rank.ME4 => full ? "Military Expert 4" : "ME4",
                    Rank.ME4_T => full ? "Military Expert 4 (Trainee)" : "ME4(T)",
                    Rank.ME4_A => full ? "Military Expert 4 (Apprentice)" : "ME4(A)",
                    Rank.ME5 => full ? "Military Expert 5" : "ME5",
                    Rank.ME6 => full ? "Military Expert 6" : "ME6",
                    Rank.ME7 => full ? "Military Expert 7" : "ME7",
                    Rank.ME8 => full ? "Military Expert 8" : "ME8",
                    Rank.DX1 => full ? "Defence Executive Officer 1" : "DX1",
                    Rank.DX2 => full ? "Defence Executive Officer 2" : "DX2",
                    Rank.DX3 => full ? "Defence Executive Officer 3" : "DX3",
                    Rank.DX4 => full ? "Defence Executive Officer 4" : "DX4",
                    Rank.DX5 => full ? "Defence Executive Officer 5" : "DX5",
                    Rank.DX6 => full ? "Defence Executive Officer 6" : "DX6",
                    Rank.DX7 => full ? "Defence Executive Officer 7" : "DX7",
                    Rank.DX8 => full ? "Defence Executive Officer 8" : "DX8",
                    Rank.DX9 => full ? "Defence Executive Officer 9" : "DX9",
                    Rank.DX10 => full ? "Defence Executive Officer 10" : "DX10",
                    Rank.DX11 => full ? "Defence Executive Officer 11" : "DX11",
                    Rank.DX12 => full ? "Defence Executive Officer 12" : "DX12",
                    _ => full ? "Other" : "OTHER"
                    #endregion
                },
                _ => throw new ArgumentException("value was not a Rank")
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
