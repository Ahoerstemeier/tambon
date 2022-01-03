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
        ProvinceCouncil,
        OccupiedArea
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
        /// <summary>
        /// National park (อุทยานแห่งชาติ).
        /// </summary>
        NationalPark,

        /// <summary>
        /// Forest park (วนอุทยาน).
        /// </summary>
        ForestPark,

        /// <summary>
        /// Historical park (อุทยานประวัติศาสตร์).
        /// </summary>
        HistoricalPark,

        /// <summary>
        /// Wildlife sanctuary (เขตรักษาพันธุ์สัตว์ป่า).
        /// </summary>
        WildlifeSanctuary,

        /// <summary>
        /// Non-hunting area (เขตห้ามล่าสัตว์ป่า).
        /// </summary>
        NonHuntingArea,

        /// <summary>
        /// Historical site (เขตที่ดินโบราณสถาน).
        /// </summary>
        HistoricalSite,

        /// <summary>
        /// National protected forest (ป่าสงวนแห่งชาติ).
        /// </summary>
        NationalForest,

        /// <summary>
        /// UNESCO world heritage (มรดกโลก).
        /// </summary>
        WorldHeritage,

        /// <summary>
        /// UNESCO biosphere reserve (พื้นที่สงวนชีวมณฑล).
        /// </summary>
        BiosphereReserve,

        /// <summary>
        /// Botanical garden (สวนพฤกษศาสตร์).
        /// </summary>
        BotanicalGarden,

        /// <summary>
        /// Arboretum or forest garden (สวนรุกขชาติ).
        /// </summary>
        Arboretum,

        /// <summary>
        /// Paleontological survey area (เขตสำรวจและศึกษาวิจัยเกี่ยวกับแหล่งซากดึกดำบรรพ์หรือซากดึกดำบรรพ์).
        /// </summary>
        Paleontological,

        /// <summary>
        /// Environment protection area (เขตพื้นที่คุ้มครองสิ่งแวดล้อม).
        /// </summary>
        Environment,

        /// <summary>
        /// Buddhist temple (วัด).
        /// </summary>
        Wat,
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
        ProvinceCouncilChairman,
        ViceRoyal,
        RegionGovernor,
        RegionViceGovernor,
        MunicipalityCouncilor,
        TAOCouncilor,
        Other,
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
        DirectorGeneralDepartmentOfProvincialAdministration,
        MinisterOfEnvironment,
        DepartmentOfMineralResourceDirectorGeneral,
        DirectorOfNationalBureauOfBuddhism
    }
}