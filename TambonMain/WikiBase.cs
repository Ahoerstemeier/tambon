﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace De.AHoerstemeier.Tambon
{
    public partial class WikiBase
    {
        /// <summary>
        /// Translation table from <see cref="EntityType"/> to the item at WikiData.
        /// </summary>
        public static Dictionary<EntityType, String> WikiDataItems = new Dictionary<EntityType, String>()
        {
            {EntityType.Changwat, "Q50198"},
            {EntityType.Amphoe, "Q475061"},
            {EntityType.KingAmphoe, "Q6519277"},
            {EntityType.Tambon, "Q1077097"},
            {EntityType.Muban, "Q1368879"},
            {EntityType.TAO, "Q15140073"},
            {EntityType.PAO, "Q13023500"},
            {EntityType.Thesaban, "Q1155688"},
            {EntityType.ThesabanTambon, "Q15141625"},
            {EntityType.ThesabanMueang, "Q13025342"},
            {EntityType.ThesabanNakhon, "Q15141632"},
            {EntityType.Sukhaphiban, "Q7635776"},
            {EntityType.Monthon, "Q1936511"},
            {EntityType.Country, "Q869"},
            {EntityType.Khet, "Q15634531"},
            {EntityType.Khwaeng, "Q456876"},
            {EntityType.Chumchon,"Q15253857"},
            {EntityType.SaphaTambon,"Q15695218"},
            {EntityType.KlumChangwat,"Q13012657"}
        };

        // public const String PropertyIdEntityType = "P132";
        public const String PropertyIdCountry = "P17";

        public const String PropertyIdIsInAdministrativeUnit = "P131";
        public const String PropertyIdCoordinate = "P625";
        public const String PropertyIdCapital = "P36";
        public const String PropertyIdContainsAdministrativeDivisions = "P150";
        public const String PropertyIdWebsite = "P856";
        public const String PropertyIdSharesBorderWith = "P47";
        public const String PropertyIdHeadOfGovernment = "P6";
        public const String PropertyIdPostalCode = "P281";
        public const String PropertyIdTwinCity = "P190";
        public const String PropertyIdOpenStreetMap = "P402";
        public const String PropertyIdLocationMap = "P242";
        public const String PropertyIdFreebaseIdentifier = "P646";
        public const String PropertyIdISO3166 = "P300";
        public const String PropertyIdGND = "P227";
        public const String PropertyIdWOEID = "P1281";
        public const String PropertyIdGeonames = "P1566";
        public const String PropertyIdGNSUFI = "P2326";
        public const String PropertyIdFIPS10 = "P901";
        public const String PropertyIdDmoz = "P998";
        public const String PropertyIdThaiGeocode = "P1067";
        public const String PropertyIdInstanceOf = "P31";
        public const String PropertyIdInception = "P571";
        public const String PropertyIdPopulation = "P1082";
        public const String PropertyIdSocialMedia = "P553";
        public const String PropertyIdSocialMediaAddress = "P554";
        public const String PropertyIdFoundationalText = "P457";
        public const String PropertyIdMotto = "P1451";
        public const String PropertyIdNativeLabel = "P1705";
        public const String PropertyIdOfficialName = "P1448";
        public const String PropertyIdDeterminationMethod = "P459";
        public const String PropertyIdFoursquareId = "P1968";
        public const String PropertyIdFacebookPage = "P1997";  // https://www.facebook.com/pages/-/$1
        public const String PropertyIdGooglePlusUserName = "P2847";
        public const String PropertyIdShortName = "P1813";  // wrong data type, wait for it to become multilingual!
        public const String PropertyIdIpa = "P898";
        public const String PropertyIdLanguageOfWork = "P407";  // qualifier for IPA, to be set to ItemIdThaiLanguage
        public const String PropertyIdCommonsCategory = "P373";
        public const String PropertyIdMainRegulatoryText = "P92";  // qualifier for inception to link to Gazette item

        public const String PropertyIdArea = "P2046";
        public const String PropertyIdCategoryCombinesTopic = "P971";

        public const String PropertyIdCategoryForTopic = "P910";
        public const String PropertyIdTopicForCategory = "P301";

        public const String PropertyIdIsListOf = "P360";
        public const String PropertyIdDescribedByUrl = "P973";
        public const String PropertyIdNamedAfter = "P138";

        // for qualifiers
        public const String PropertyIdPointInTime = "P585";

        public const String PropertyIdStartDate = "P580";
        public const String PropertyIdEndDate = "P582";
        public const String PropertyAppliesToPart = "P518";

        // for sources
        public const String PropertyIdStatedIn = "P248";

        public const String PropertyIdReferenceUrl = "P854";
        public const String PropertyIdPublisher = "P123";

        // for Gazette entries
        public const String PropertyIdSignatory = "P1891";

        public const String PropertyFullTextAvailableAt = "P953";

        public const String PropertyIdPublishedIn = "P1433";
        public const String ItemIdRoyalGazette = "Q869928";
        public const String PropertyIdPublicationDate = "P577";

        // public const String PropertyIdOriginalLanguage = "P364";  // deprecated for written work, use PropertyIdLanguageOfWork
        public const String ItemIdThaiLanguage = "Q9217";

        public const String PropertyIdVolume = "P478";
        public const String PropertyIdIssue = "P433";
        public const String PropertyIdPage = "P304";
        public const String PropertyIdTitle = "P1476";
        public const String ItemIdStatute = "Q820655";
        public const String ItemIdRoyalDecree = "Q13017629";
        public const String ItemIdMinisterialRegulation = "Q6406128";

        // source statements for TIS 1099
        public const String ItemSourceTIS1099BE2535 = "Q15477441";

        public const String ItemSourceTIS1099BE2548 = "Q15477531";
        public const String ItemSourceCCAATT = "Q15477767";
        public const String ItemDopa = "Q13012489";

        public const String ItemCensuses = "Q39825";
        public const String ItemRegistration = "Q15194024";

        public const String ItemWikimediaCategory = "Q4167836";

        public const String ItemSquareKilometer = "Q712226";

        // for symbols
        public const String PropertyOfficialSymbol = "P2238";

        // public const String PropertyGenericAs = "P794";
        public const String PropertyObjectHasRole = "P3831";

        public const String ItemTree = "Q10884";
        public const String ItemFlower = "Q506";
        public const String ItemAquaticAnimal = "Q1756633";
        public const String ItemColor = "Q1075";

        // source statements for population
        public static Dictionary<Int16, String> ItemCensus = new Dictionary<Int16, String>()
        {
        {2010,ItemCensus2010},
        {2000,ItemCensus2000},
        {1990,ItemCensus1990},
        {1980,ItemCensus1980},
        {1970,ItemCensus1970},
        {1960,ItemCensus1960},
        {1947,ItemCensus1947},
        {1937,ItemCensus1937},
        {1929,ItemCensus1929},
        {1919,ItemCensus1919},
        {1909,ItemCensus1909},
        };

        public const String ItemCensus2010 = "Q15637207";

        public const String ItemCensus2000 = "Q15637213";
        public const String ItemCensus1990 = "Q15637229";
        public const String ItemCensus1980 = "Q15637237";
        public const String ItemCensus1970 = "Q15639176";
        public const String ItemCensus1960 = "Q15639232";
        public const String ItemCensus1947 = "Q15639300";
        public const String ItemCensus1937 = "Q15639324";
        public const String ItemCensus1929 = "Q15639341";
        public const String ItemCensus1919 = "Q15639367";
        public const String ItemCensus1909 = "Q15639395";

        public const String ItemSeatOfLocalGovernment = "Q543654";
        public const String ItemDistrictOffice = "Q41769254";
        public const String ItemProvinceHall = "Q41769446";

        public const String ItemSocialMediaTwitter = "Q918";
        public const String ItemSocialMediaFacebook = "Q335";
        public const String ItemSocialMediaGooglePlus = "Q356";
        public const String ItemSocialMediaFoursquare = "Q51709";

        public const String MubanBookVolume1Title = "ทำเนียบท้องที่ พุทธศักราช 2546 เล่ม 1";
        public const String MubanBookVolume2Title = "ทำเนียบท้องที่ พุทธศักราช 2546 เล่ม 2";
        public const String ItemMubanBookVolume1 = "Q23793867";
        public const String ItemMubanBookVolume2 = "Q23793856";

        public const String ItemFlickrShapeFile = "Q24010939";  // Reference for PropertyIdWOEID

        public const String ItemSandbox = "Q4115189";

        public const String SiteLinkCommons = "commonswiki";
        public const String SiteLinkThaiWikipedia = "thwiki";
        public const String SiteLinkEnglishWikipedia = "enwiki";
        public const String SiteLinkGermanWikipedia = "dewiki";
    }
}