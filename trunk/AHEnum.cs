using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace De.AHoerstemeier.Tambon
{
    internal enum EntityType
    {
        Unknown,
        Changwat,
        Amphoe, KingAmphoe,
        Tambon,
        Muban,
        Thesaban, // actually not a type, but a mistake in DOPA population of Nong Khai where they forgot the type
        ThesabanNakhon, ThesabanMueang, ThesabanTambon,
        Khet,
        Khwaeng,
        SakhaTambon, SakhaKhwaeng, Sakha,
        Monthon,
        Sukhaphiban,
        SukhaphibanTambon, SukhaphibanMueang,
        Mueang, Bangkok,
        Chumchon,
        TAO,
        TC,
        PAO,
        Phak
    };
    internal enum EntityModification
    {
        Creation,
        Abolishment,
        Rename,
        StatusChange,
        AreaChange,
        Constituency
    }
    internal enum ProtectedAreaTypes
    {
        NationalPark,
        ForestPark,
        WildlifeSanctuary,
        NonHuntingArea,
        HistoricalPark,
        HistoricalSite,
        NationalPreservedForest
    }
    internal enum EntityLeaderType
    {
        Governor,
        ViceGovernor,
        DistrictOfficer,
        MinorDistrictOfficer,
        Kamnan,
        PhuYaiBan,
        PAOChairman,
        Mayor,
        TAOChairman,
        SanitaryDistrictChairman
    }
    internal enum PersonTitle
    {
        Unknown,
        Mister,
        Miss,
        Mistress,
        General,
        LieutenantGeneral,
        MajorGeneral,
        Colonel,
        LieutenantColonel,
        Major, 
        Captain,
        FirstLieutenant,
        SecondLieutenant,
        ActingSecondLieutenant,
        SubLieutenant
    }
    internal enum OfficeType
    {
        ProvinceHall,
        PAOOffice,
        DistrictOffice,
        TAOOffice,
        MunicipalityOffice,
        VillageHeadmanOffice,
        DistrictMuseum // ???
    }
}