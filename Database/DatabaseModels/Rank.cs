using System;
using System.Collections.Generic;
using System.Linq;

namespace Database.DatabaseModels
{
    public enum Rank
    {
        // Civilians
        MR = 0,
        MS = 1,
        MRS = 2,
        OTHER = 3,

        // Chao Recruits
        REC = 4,
        PTE = 5,
        CPL = 6,
        CFC = 7,

        // Specs
        CADET_SGT = 8,
        THIRD_SGT = 9,
        SECOND_SGT = 10,
        FIRST_SGT = 11,

        STAFF_SGT = 12,
        MASTER_SGT = 13,

        // Warrant Officers
        THIRD_WO = 14,
        SECOND_WO = 15,
        FIRST_WO = 16,
        MASTER_WO = 17,
        SENIOR_WO = 18,
        CHIEF_WO = 19,

        // Officers
        OCT = 20,
        SECOND_LT = 21,
        LTA = 22,
        CPT = 23,

        // Crabs
        MAJ = 24,
        LTC = 25,
        SLTC = 26,
        COL = 27,

        // Stars
        BG = 28,
        MG = 29,
        LG = 30,
        GEN = 31,

        // Military Experts
        ME1_T = 32,
        ME1_A = 33,
        ME1 = 34,
        ME2 = 35,
        ME3 = 36,
        ME4 = 37,
        ME4_T = 38,
        ME4_A = 39,
        ME5 = 40,
        ME6 = 41,
        ME7 = 42,
        ME8 = 43,

        // DXOs
        DX1 = 44,
        DX2 = 45,
        DX3 = 46,
        DX4 = 47,
        DX5 = 48,
        DX6 = 49,
        DX7 = 50,
        DX8 = 51,
        DX9 = 52,
        DX10 = 53,
        DX11 = 54,
        DX12 = 55,

        SEPARATOR = -1,
    }

    public static class RankHelper
    {
        /// <summary>
        /// Returns a list of available ranks, with SEPARATOR = -1 inserted where there should be a separation.
        /// </summary>
        public static List<Rank> GetValuesWithSeparators()
        {
            List<Rank> ret = new List<Rank>(Enum.GetValues(typeof(Rank)).Cast<Rank>());
            // Insert from the bottom up, in order to not screw up the indexes required.
            ret.Insert(44, Rank.SEPARATOR);
            ret.Insert(32, Rank.SEPARATOR);
            ret.Insert(28, Rank.SEPARATOR);
            ret.Insert(24, Rank.SEPARATOR);
            ret.Insert(20, Rank.SEPARATOR);
            ret.Insert(14, Rank.SEPARATOR);
            ret.Insert(8, Rank.SEPARATOR);
            ret.Insert(4, Rank.SEPARATOR);
            return ret;
        }
    }
}
