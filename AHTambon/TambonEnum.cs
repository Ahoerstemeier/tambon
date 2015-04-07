using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace De.AHoerstemeier.Tambon
{
    public enum EntityType
    {
        Unknown,
        Country,
        Changwat,
        Amphoe,
        KingAmphoe,
        Tambon,
        Muban,
        Thesaban, // actually not a type, but a mistake in DOPA population of Nong Khai where they forgot the type
        ThesabanNakhon,
        ThesabanMueang,
        ThesabanTambon,
        Khet,
        Khwaeng,
        SakhaTambon,
        SakhaKhwaeng,
        Sakha,
        Monthon,
        Sukhaphiban,
        SukhaphibanTambon,
        SukhaphibanMueang,
        Mueang,
        Bangkok,
        Chumchon,
        TAO,
        PAO,
        SaphaTambon,
        Phak,
        KlumChangwat,
        Constituency,
        ElectoralRegion,
        ProvinceCouncil
    };

    public enum EntityModification
    {
        Creation,
        Abolishment,
        Rename,
        StatusChange,
        AreaChange,
        Constituency
    }

    public enum ProtectedAreaTypes
    {
        NationalPark,
        ForestPark,
        WildlifeSanctuary,
        NonHuntingArea,
        HistoricalPark,
        HistoricalSite,
        NationalPreservedForest
    }

    public enum EntityLeaderType
    {
        Unknown,
        Governor,
        BangkokGovernor,
        ViceGovernor,
        DistrictOfficer,
        DistrictOfficerBangkok,
        MinorDistrictOfficer,
        Kamnan,
        PhuYaiBan,
        PAOChairman,
        PAOClerk,
        PAOCouncilChairman,
        Mayor,
        MunicipalClerk,
        MunicipalityCouncilChairman,
        TAOMayor,
        TAOChairman,
        TAOCouncilChairman,
        TAOClerk,
        TambonCouncilChairman,
        SanitaryDistrictChairman,
        ChumchonChairman,
        MueangGovernor,
        ProvinceCouncilChairman
    }

    public enum PersonTitle
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

    public enum OfficeType
    {
        Unknown,
        ProvinceHall,
        PAOOffice,
        DistrictOffice,
        TAOOffice,
        MunicipalityOffice,
        VillageHeadmanOffice,
        ChumchonOffice,
        SubdistrictHeadmanOffice,
        DistrictMuseum // ???
    }

    public enum GazetteSignPosition
    {
        Unknown,
        PrimeMinister,
        DeputyPrimeMinister,
        MinisterOfInterior,
        DeputyMinisterOfInterior,
        MinistryOfInteriorPermanentSecretary,
        MinisterOfAgriculture,
        DeputyMinisterOfAgriculture,
        ProvinceGovernor,
        ViceProvinceGovernor,
        DeputyProvinceGovernor,
        BangkokGovernor,
        BangkokPermanentSecretary,
        DeputyBangkokPermanentSecretary,
        MinisterOfInformationAndCommunicationTechnology,
        ElectionCommissionPresident,
        ElectionCommissionActingPresident,
        RoyalInstitutePresident,
        RoyalInstituteActingPresident,
        DepartmentOfTransportDirectorGeneral,
        DeputyDepartmentOfTransportDirectorGeneral,
        DistrictOfficerBangkok,
        DistrictOfficer,
        SpeakerOfParliament,
        DeputySpeakerOfParliament,
        Mayor,
        TAOMayor,
        TAOChairman,
        PAOChairman,
        MunicipalPermanentSecretary,
        MinistryOfInteriorDeputyPermanentSecretary,
        FineArtsDepartmentDirectorGeneral,
        MinisterOfNaturalResourcesAndEnvironment,
        RegisterOfficeDirector,
        RegisterOfficeDeputyDirector,
        DirectorGeneralDepartmentOfProvincialAdministration
    }
}